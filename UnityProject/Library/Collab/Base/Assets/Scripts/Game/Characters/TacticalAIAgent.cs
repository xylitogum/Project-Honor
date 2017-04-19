using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using FSM;
using FSM.StateBehaviors;

public class TacticalAIAgent : Character
{

    public bool UseStrategyTeam = true;

    private TacticalStateMachine fsm;
    private Vector3 previousDestination;
    //private int destTimer = 0;

    private Commander_FSM commander;

    // Use this for initialization
    void Start () {
        base.Character_Start();

        // Transitions out of retreating
        var foundCover = new Transition(State.Tactical_Resting, (manager, character) => {
            return character.InSafeSpot();
        });
        var retreatingTransitions = new List<Transition>();
        retreatingTransitions.Add(foundCover);

        // Transitions out of resting
        var healthyAndFullAmmo = new Transition(State.Tactical_Moving, (manager, character) => {
            return character.HasHighHealth() && character.HasAmmo();
        });
        var healthLow = new Transition(State.Tactical_Retreating, (manager, character) => {
            return character.HasLowHealth();
        });
        var restingTransitions = new List<Transition>();
        restingTransitions.Add(healthyAndFullAmmo);
        restingTransitions.Add(healthLow);

        // Transitions out of moving
        var hasLineOfSight = new Transition(State.Tactical_Aiming, Aim);
        var movingTransitions = new List<Transition>();
        movingTransitions.Add(hasLineOfSight);
        movingTransitions.Add(healthLow);

        // Transitions out of aiming
        var noLineOfSight = new Transition(State.Tactical_Moving, (manager, character) => {
            return !character.CanSeeEnemy();
        });
        
        var aimingTransitions = new List<Transition>();
        aimingTransitions.Add(noLineOfSight);
        aimingTransitions.Add(healthLow);

        var transitions = new Dictionary<State, IEnumerable<Transition>>();
        transitions.Add(State.Tactical_Retreating, retreatingTransitions);
        transitions.Add(State.Tactical_Resting, restingTransitions);
        transitions.Add(State.Tactical_Moving, movingTransitions);
        transitions.Add(State.Tactical_Aiming, aimingTransitions);
        
        fsm = new TacticalStateMachine(State.Tactical_Resting, transitions);
        fsm.AddStateBehavior(State.Tactical_Retreating, new RetreatingStateBehavior());
        fsm.AddStateBehavior(State.Tactical_Resting, new RestingStateBehavior());
        fsm.AddStateBehavior(State.Tactical_Moving, new MovingState());
        fsm.AddStateBehavior(State.Tactical_Aiming, new AimingStateBehavior());

        previousDestination = transform.position;

        commander = GameManager.instance.commander;
    }

    // Update is called once per frame
    void Update () {
        base.Character_Update();

		if (GameManager.instance.gameState != GameManager.GameState.Running) return;

        orders = this.GetOrders();
        fsm.MoveToNextState(GameManager.instance, this);
        //!this.HasMediumHealth() || !orders
        if (!this.HasMediumHealth() || !orders || DisobeyOrders() || !UseStrategyTeam)  //"if" the agent does not have any orders or it does not have at least 50% health - execute Tactical State Machine
        {
            var command = fsm.GetCommand(this, GameManager.instance);
            var navMeshAgent = GetComponent<NavMeshAgent>();

            Debug.DrawLine(transform.position + Vector3.up, command.MoveDest + Vector3.up, team.color);
            if ((command.MoveDest - navMeshAgent.destination).magnitude > 0.05f && this.GetDestTimer() == 0) {
                previousDestination = navMeshAgent.destination;
                navMeshAgent.destination = command.MoveDest;
            }
            turn(command.TurnDir);

            if (command.ShouldFire) {
                fire();
            }

            if (command.ShouldReload) {
                reload();
            }
        }
        else //Execute Strategic_FSM Commands
        {
            //Debug.Log("Startegic FSM Executed");
            var navMeshAgent = GetComponent<NavMeshAgent>();
            var command = commander.strategyFSM.GetCommand(this, GameManager.instance);
            Debug.DrawLine(transform.position + Vector3.up, command.MoveDest + Vector3.up, team.color);
            if(strategicOrders.TargetCharacter != null)
                Debug.DrawLine(transform.position + Vector3.up, strategicOrders.TargetCharacter.transform.position + Vector3.up, Color.green);
            if (command.ShouldMove)
            {
                if ((command.MoveDest - navMeshAgent.destination).magnitude > 0.05f && this.GetDestTimer() == 0)
                {
                    previousDestination = navMeshAgent.destination;
                    navMeshAgent.destination = command.MoveDest;
                }
            }
            
			Character targetEnemy = GetEnemyToShoot();
			if (targetEnemy) // command.ShouldFire || 
            {
				turn((targetEnemy.transform.position - this.transform.position).ToVec2());
                fire();
            }

            if (command.ShouldReload)
            {
                reload();
            }
        }

        // It seems to help agents move more smoothly to only set their destination on a delay
        this.IncrementTimer();
        if (this.GetDestTimer() > 50)
        {
            this.SetDestTimer(0);
        }
    }

    /// <summary>
    /// Returns TRUE if the Character has a Line Of Sight to an opponent.
    /// </summary>
    /// <param name="agent">The agent whose Line Of Sight is being determined</param>
    /// <param name="gameManager">A reference to the Game State</param>
    /// <returns>Returns TRUE is the Character has a Line Of Sight to an opponent.</returns>
    public static bool Aim(GameManager gameManager, Character agent)
    {
        List<Character> opponents = agent.GetOpponents();

        foreach (var opponent in opponents)
        {
            if (gameManager.tileManager.PositionCanSeePosition(opponent.transform.position, agent.transform.position, agent.GetTeammates()))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Decides whether an agent should disobey its strategic orders. For example,
    /// if it is incapable of executing its orders it is best to ignore them.
    /// </summary>
    /// <returns>True if strategy will be ignored, else false</returns>
    public bool DisobeyOrders()
    {
        // Conditions for disobeying strategy
        // - Strategy is to focus fire on a target I can't see
        return strategicOrders == null ||
               (strategicOrders.StrategicState == State.Strategic_FocusFire &&
                strategicOrders.TargetCharacter && !HasLineOfSight(strategicOrders.TargetCharacter));
    }

	public Character GetEnemyToShoot()
	{
        if (strategicOrders.TargetCharacter != null && HasLineOfSight(strategicOrders.TargetCharacter))
        {
            return strategicOrders.TargetCharacter;
        }

        var agentTile = new Tile(transform.position);
		List<Character> opponents = this.GetOpponents();
        opponents.Sort((opp1, opp2) => Tile.ManhattanDistance(new Tile(opp1.transform.position), agentTile)
                 - Tile.ManhattanDistance(new Tile(opp2.transform.position), agentTile));
        foreach (var opponent in opponents)
		{
			if (GameManager.instance.tileManager.PositionCanSeePosition(opponent.transform.position, this.transform.position, this.GetTeammates()))
			{
				return opponent;
			}
		}
		return null;
	}
}

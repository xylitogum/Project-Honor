using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// MoveDestination.cs
public class Navigation_Test : MonoBehaviour
{

    public Transform goal;
    public GameObject[] coverPoints;
    public UnityEngine.AI.NavMeshAgent agent;
    public int currentCoverPoint = 0;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        coverPoints = GameObject.FindGameObjectsWithTag("Cover_Point");
        goal.position = coverPoints[currentCoverPoint].transform.position;

    }//END: Start()

    // Update is called once per frame
    void Update()
    {
        if (agent.remainingDistance < 0.5f)
        {
            currentCoverPoint = (currentCoverPoint + 1) % coverPoints.Length;
        }
        else {  }

        goal.position = coverPoints[currentCoverPoint].transform.position;
        agent.destination = goal.position;

    }//END: Update()

}//END: Navigation_Test Class



/*
 * ****************************************************************************************
 * Previous Commander_FSM Class Implementation fro Reference
 * ****************************************************************************************
 * 
 * using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using FSM.StateBehaviors;

public class Commander_FSM 
{
    public StrategicStateMachine strategyFSM;
    //public List<Character> team;
    //public List<Character> enemys;

    public Character teamTarget;    //The enemy to be focused on by all teammates
    public Vector3 teamCenterPoint;  //A general location determined by "averaging" all teammate locations
    public int teamCount = 0;           //Used to keep track of the total amount of teammates
    public int teamCountDelta = 0;      //Used to track if teammate died
    public float teamDistanceToTarget = 100.0f;   //Tracks the distance from the teamCenterPoint to the teamTarget

    //=======================================================================================================================================
    
    // Use this for initialization
    public void Start ()
    {
        Team teamRED = GameManager.instance.teams[0];

        foreach(var teammate in teamRED.members)
        {
            teammate.SetOrders(true);   //Notifies agents to execute StrategicFSM Commands - rather than TacticalFSM Commands
            
            //Debug.LogWarning("Set Orders RED");
        }//END: foreach

        teamRED.members[0].orders = false;
        teamRED.members[1].orders = false;
        
        teamCount = teamRED.members.Count;
        teamTarget = GameManager.instance.teams[1].members[0];

			
        //Define Strategic Transitions
        var teammateDeath = new Transition(State.Strategic_TakeCover, (manager) => { return teamCountDelta > 0; });
        var targetEliminated = new Transition(State.Strategic_FocusFire, (manager) => { return teamTarget.getHealth() <= 0.0f; });
        var teamFannedOut = new Transition(State.Strategic_HoldPosition, (manager) => { return GetTeamAvgDistanceFromTeamCenter() > 5.0f; });
        var teamGrouped = new Transition(State.Strategic_Move, (manager) => {  return GetTeamAvgDistanceFromTeamCenter() < 10.0f; });

        var teamLocationReached = new Transition(State.Strategic_Aim, (manager) => 
        { var TeamRED = GameManager.instance.teams[0].members;
            bool inPosition = false;
            int numberOfTeamInPosition = 0;
            foreach (var teammate in TeamRED)
            {
                if (Mathf.Abs(Vector3.Distance(teammate.transform.position, teammate.destination)) < 7.0f)
                { numberOfTeamInPosition += 1; }
            }
            if (numberOfTeamInPosition >= TeamRED.Count - 1) { inPosition = true; }
            return inPosition; });

        var teamNoLineOfSight = new Transition(State.Strategic_Move, (manager) =>
        {
            var TeamRED = GameManager.instance.teams[0].members;
            bool NLOS = true;
            int numberOfTeamWithLOS = 0;
            foreach (var teammate in TeamRED)
            {
                if (Aim(GameManager.instance, teammate))
                { numberOfTeamWithLOS += 1; }
            }
            if (numberOfTeamWithLOS >= TeamRED.Count - 1) { NLOS = false; }
            return NLOS;
        });

        var teamAmbush = new Transition(State.Strategic_Move, (manager) => { return teamDistanceToTarget < 13.0f; });
        var targetOfOpportunity = new Transition(State.Strategic_FocusFire, TeamAim);
        var retreat = new Transition(State.Strategic_TakeCover, AnyBodyHurt);
        var coverFound = new Transition(State.Strategic_FocusFire, InCover);

        //Debug.Log("FannedOut fired");Debug.Log("TeamGrouped Fired");Debug.Log("TeammateDeath Fired");Debug.Log("Team Target is " + teamTarget);Debug.Log("Ambush Launched"); 

        //Define Move Transistions
        var moveTransitions = new List<Transition>();
        moveTransitions.Add(teamLocationReached);
        moveTransitions.Add(retreat);
        
        //Define Aim Transitions
        var aimTransitions = new List<Transition>();
        //aimTransitions.Add(teamNoLineOfSight);
        aimTransitions.Add(targetOfOpportunity);

        //Define Regroup Transitions
        var regroupTransitions = new List<Transition>();
        regroupTransitions.Add(teamGrouped);

        //Define FanOut Transitions
        var fanOutTransitions = new List<Transition>();
        //fanOutTransitions.Add(teammateDeath);
        fanOutTransitions.Add(teamFannedOut);

        //Define FocusFire Transitions
        var focusFireTransitions = new List<Transition>();
        focusFireTransitions.Add(targetEliminated);
        focusFireTransitions.Add(teammateDeath);
        focusFireTransitions.Add(teamNoLineOfSight);
        focusFireTransitions.Add(retreat);

        //Define MoveToFiringPosition Transitions
        var moveToFiringPositionTransitions = new List<Transition>();
        moveToFiringPositionTransitions.Add(teamLocationReached);

        //Define HoldPosition Transitions
        var holdPositionTransitions = new List<Transition>();
        holdPositionTransitions.Add(teamAmbush);
        //holdPositionTransitions.Add(teammateDeath);

        //Define TakeCover Transitions
        var takeCoverTransitions = new List<Transition>();
        takeCoverTransitions.Add(coverFound);

        var transitions = new Dictionary<State, IEnumerable<Transition>>();
        transitions.Add(State.Strategic_FanOut, fanOutTransitions);
        transitions.Add(State.Strategic_FocusFire, focusFireTransitions);
        transitions.Add(State.Strategic_Regroup, regroupTransitions);
        transitions.Add(State.Strategic_HoldPosition, holdPositionTransitions);
        transitions.Add(State.Strategic_MoveToFiringPosition, moveToFiringPositionTransitions);
        transitions.Add(State.Strategic_Aim, aimTransitions);
        transitions.Add(State.Strategic_Move, moveTransitions);
        transitions.Add(State.Strategic_TakeCover, takeCoverTransitions);

        strategyFSM = new StrategicStateMachine(State.Strategic_Move, transitions);
        strategyFSM.AddStateBehavior(State.Strategic_FanOut, new FanOutState());
        strategyFSM.AddStateBehavior(State.Strategic_FocusFire, new FocusFireState());
        strategyFSM.AddStateBehavior(State.Strategic_Regroup, new RegroupState());
        strategyFSM.AddStateBehavior(State.Strategic_MoveToFiringPosition, new MoveToFiringPosition());
        strategyFSM.AddStateBehavior(State.Strategic_HoldPosition, new HoldPositionState());
        strategyFSM.AddStateBehavior(State.Strategic_Move, new Move());
        strategyFSM.AddStateBehavior(State.Strategic_Aim, new Aim());
        strategyFSM.AddStateBehavior(State.Strategic_TakeCover, new TakeCover());
        
    }//END: Start() Function

    //=======================================================================================================================================

    // Update is called once per frame
    // Sets the "teamTarget" to an enemy
    public void Update ()
    {
        //Determine if a Transition needs to Occur
        strategyFSM.MoveToNextStrategicState(GameManager.instance);

        Debug.Log("The current Strategic State is " + strategyFSM.CurrentState);
           
        //Determine if a soldier on the team died
        int currentTeamCount = GameManager.instance.teams[0].members.Count;
        teamCountDelta = teamCount - currentTeamCount;
        teamCount = currentTeamCount;

        //Set the TeamCenterPoint
        teamCenterPoint = DetermineTeamCenterPoint();

        //Locate Targets Of Opportunity
        Character targetOO = IDTargetOfOpportunity(GameManager.instance);

        if (targetOO != null)
        {
            teamTarget = targetOO;
        }
        else { SetTeamTargetFromTeamCenter(); }

        if (teamTarget != null)
        {
            //Determine the team's distance from their teamCenterPoint to the teamTarget
            teamDistanceToTarget = Mathf.Abs(Vector3.Distance(teamTarget.transform.position, teamCenterPoint));
            //Debug.Log("The teamDistanceTotarget = " + teamDistanceToTarget);
            Debug.DrawLine(teamTarget.transform.position + Vector3.up, teamCenterPoint + Vector3.up, Color.yellow);
            //Debug.DrawLine(IDTargetOfOpportunity(GameManager.instance).transform.position + Vector3.up, teamCenterPoint + Vector3.up, Color.yellow);
        }
    }//END: Update() Function

    //=======================================================================================================================================

    public Character GetTeamTarget()
    {
        return teamTarget;
    }//END: GetTeamTarget()

    //=======================================================================================================================================

    public Vector3 GetTeamCenter()
    {
        return teamCenterPoint;
    }//END: GetTeamCenter()

    //=======================================================================================================================================

    //Calcualtes the Center Point of all Teammates on the field by
    //  averaging all Teammate Vector3 Positions
    public Vector3 DetermineTeamCenterPoint()
    {
        //Determine the ceter point of all team members ... maybe
        Vector3 teamCenterLocation = Vector3.zero;
        Vector3 vectorSum = Vector3.one;
        List<Character> teamRED = GameManager.instance.teams[0].members;
        
        foreach (var teammate in teamRED)
        {
            vectorSum += teammate.transform.position;
        }
        teamCenterLocation = vectorSum / GameManager.instance.teams[0].members.Count;
        //Debug.Log("Team Center Point in DetermineTeamCenterPoint = " + teamCenterLocation);
        return teamCenterLocation;
    }//END: DetemineTeamCenterPoint

    //=======================================================================================================================================

    public float GetTeamAvgDistanceFromTeamCenter()
    {
        Team teamRED = GameManager.instance.teams[0];
        float distanceSum = 0.0f;
        float distanceAvg = 0.0f;
        foreach (var teammate in teamRED.members)
        {
            distanceSum += Vector3.Distance(teammate.transform.position, teamCenterPoint);
        }//END: Calcualte the SUM of all teammate distances

        distanceAvg = distanceSum / teamRED.Count();
        //Debug.Log("The Team's Average Distance from TeamCenterPoint = " + distanceAvg);
        return Mathf.Abs(distanceAvg);

    }//END: GetTeamAvgDistanceFromCenter()

    //=======================================================================================================================================

    /// <summary>
    /// Returns TRUE if the Character has a Line Of Sight to an opponent.
    /// </summary>
    /// <param name="agent">The agent whose Line Of Sight is being determined</param>
    /// <param name="gameManager">A reference to the Game State</param>
    /// <returns>Returns TRUE is the Character has a Line Of Sight to an opponent.</returns>
    private bool Aim(GameManager gameManager, Character agent)
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
    }//END: Aim()

    //=======================================================================================================================================

    private bool TeamAim(GameManager gameManager, Character agent)
    {
        List<Character> opponents = GameManager.instance.teams[1].members;
        List<Character> teammates = GameManager.instance.teams[0].members;
        int teammatesWithLOStoEnemy = 0;

        foreach (var opponent in opponents)
        {
            foreach (var teammate in teammates)
            {
                if (teammate.HasLineOfSight(opponent))
                {
                    teammatesWithLOStoEnemy += 1;
                }
            }
        }//END: foreach

        if(teammatesWithLOStoEnemy > 2)
        {       
            return true;
        }
        else
        {
            return false;
        }

    }//END: TeamAim()

    //=======================================================================================================================================

    //Returns an Enemy Character that at least 2 teammates have a Line Of Sight to,
    // or null.
    private Character IDTargetOfOpportunity(GameManager gameManager)
    {
        List<Character> opponents = GameManager.instance.teams[1].members;
        List<Character> teammates = GameManager.instance.teams[0].members;
        int teammatesWithLOStoEnemy = 0;

        foreach (var opponent in opponents)
        {
            foreach (var teammate in teammates)
            {
                if (teammate.HasLineOfSight(opponent))
                {
                    teammatesWithLOStoEnemy += 1;
                }
                if (teammatesWithLOStoEnemy > 1)
                {
                    return opponent;
                }
            }//END: inner foreach
                
        }//END: foreach

        return null;

    }//END: TeamAim()

    //=======================================================================================================================================

    private void SetTeamTargetFromTeamCenter()
    {
        //Determine what enemy the agents should focus on
        //Uses the center location of the team as a communal point of reference
        Character closestOpp = teamTarget;
        float closestDistance = float.PositiveInfinity;
        Team teamBLUE = GameManager.instance.teams[1];
        Team teamRED = GameManager.instance.teams[0];

        for (int i = 0; i < teamBLUE.members.Count; i++)
        {
            Character enemy = teamBLUE.members[i];

            for (int j = 0; j < teamRED.members.Count; j++)
            {
                Character teammate = teamRED.members[j];

                if (enemy != null && teammate != null)
                {
                    if (GameManager.instance.tileManager.PositionCanSeePosition(enemy.transform.position, teammate.transform.position))
                    {
                        float distance = Vector3.Distance(enemy.transform.position, teamCenterPoint);
                        //Debug.Log("Enemy Health is " + enemy.getHealth());
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestOpp = enemy;
                        }//END: if
                    }//END: if
                }//END: if
            }//END: for
        }//END: for

        if (closestOpp != null)
        {
            teamTarget = closestOpp;
            //Debug.Log("Team Target = " + teamTarget);
        }
    }//END: SetTeamTargetFromTeamCenter()

    //=======================================================================================================================================


    private bool AnyBodyHurt(GameManager manager)
    {
        List<Character> teamRED = GameManager.instance.teams[0].members;
        int injuredList = 0;

        foreach(var teammate in teamRED)
        {
            if(teammate.getHealth() < teammate.getMaxHealth() * 0.7f)
            {
                injuredList += 1;
            }
        }//End: foreach teammate

        if (injuredList > 2)
        {
            return true;
        }
        else { return false; }

    }//END: AnyBodyHurt()

    //=======================================================================================================================================

    private bool InCover(GameManager manager)
    {
        List<Character> teamRED = GameManager.instance.teams[0].members;
        int inCover = 0;

        foreach (var teammate in teamRED)
        {
            if (teammate.InSafeSpot())
            {
                inCover += 1;
            }
        }//End: foreach teammate

        if (inCover == teamRED.Count)
        {
            return true;
        }
        else { return false; }

    }//END: InCover()

    //=======================================================================================================================================
}//END: Commander_FSM Class
 * 
 */

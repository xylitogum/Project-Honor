using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using FSM.StateBehaviors;

/// <summary>    
/// Object that collects Game State data and processes it 
/// to direct Team Members to possible targets and destinations
/// NOTE: Please See - Navigation_Test.cs Reference Section for Previous Implementation of Commander_FSM
/// </summary>
public class Commander_FSM 
{
    public StrategicStateMachine strategyFSM;   // The FSM that maintains the strategic state of the team
    public Character teamTarget;    //The enemy to be focused on by all teammates
    public Vector3 teamCenterPoint;  //A general location determined by "averaging" all teammate locations
    public int teamCount = 0;           //Tracks the total amount of teammates
    public int teamCountDelta = 0;      //Tracks if a teammate died
    public float teamDistanceToTarget = 100.0f;   //Tracks the distance from the teamCenterPoint to the teamTarget
    
    //=======================================================================================================================================

    /// <summary>    
    /// Initializes a Commander_FSM object. Builds the strategic finite state machine.
    /// </summary>
    public void Start ()
    {
        Team teamRED = GameManager.instance.teams[0];

        // The team will be triggered to use orders based on the current strategic state
        SetTeamUseOrders(false);

        teamCount = teamRED.members.Count;
        teamTarget = GameManager.instance.teams[1].members[0];

        //Define Strategic Transitions
        var teammateDeath = new Transition(State.Strategic_Regroup, (manager) => { return teamCountDelta > 0; });
        var targetEliminated = new Transition(State.Strategic_FocusFire, (manager) => { return teamTarget.getHealth() <= 0.0f; });
        var teamFannedOut = new Transition(State.Strategic_FocusFire, (manager) => { return GetTeamAvgDistanceFromTeamCenter() > 6.0f; });
        var teamGrouped = new Transition(State.Strategic_HoldPosition, (manager) => {  return GetTeamAvgDistanceFromTeamCenter() < 5.0f; });
        var teamLocationReached = new Transition(State.Strategic_FocusFire, (manager) =>
        {
            var TeamRED = GameManager.instance.teams[0].members;
            bool inPosition = false;
            int numberOfTeamInPosition = 0;
            foreach (var teammate in TeamRED)
            {
                if (Mathf.Abs(Vector3.Distance(teammate.transform.position, teammate.destination)) < 3.0f)
                { numberOfTeamInPosition += 1; }
            }
            if (numberOfTeamInPosition == TeamRED.Count - 1) { inPosition = true; }
            return inPosition;
        });

        var teamAmbush = new Transition(State.Strategic_MoveToFiringPosition, (manager) => { return teamDistanceToTarget < 12.0f; });
		var targetHidden = new Transition(State.Strategic_MoveToFiringPosition, (manager) => {
			var TeamRED = GameManager.instance.teams[0].members;
			int numberOfVisible = 0;
			foreach (var teammate in TeamRED)
			{
				if (manager.tileManager.PositionCanSeePosition(teammate.transform.position, teamTarget.transform.position))
				{ numberOfVisible += 1; }
			}
			return numberOfVisible > (TeamRED.Count / 2);});

        // Focus fire on an enemy when it becomes weak
        var enemyWeak = new Transition(State.Strategic_MoveToFiringPosition, (manager) =>
        {
            var teamBlue = manager.teams[1];
            foreach (var enemy in teamBlue.members)
            {
                if (enemy.HasLowHealth())
                {
                    return true;
                }
            }
            return false;
        });

        // Define null strategy transitions
        var nullTransitions = new List<Transition>();
        nullTransitions.Add(enemyWeak);

        //Define Regroup Transitions
        var regroupTransitions = new List<Transition>();
		regroupTransitions.Add(teamGrouped);

        //Define FanOut Transitions
        var fanOutTransitions = new List<Transition>();
        fanOutTransitions.Add(teamFannedOut);

        //Define FocusFire Transitions
        var focusFireTransitions = new List<Transition>();
        focusFireTransitions.Add(targetEliminated);
		focusFireTransitions.Add(targetHidden);
        focusFireTransitions.Add(teammateDeath);

        //Define MoveToFiringPosition Transitions
        var moveToFiringPositionTransitions = new List<Transition>();
        moveToFiringPositionTransitions.Add(teamLocationReached);

        //Define HoldPosition Transitions
        var holdPositionTransitions = new List<Transition>();
        holdPositionTransitions.Add(teamAmbush);
        holdPositionTransitions.Add(teammateDeath);

        //Define Dictionary of State/Transition Pairs - to be used by the Strategic State Machine
        var transitions = new Dictionary<State, IEnumerable<Transition>>();
        transitions.Add(State.Strategic_FanOut, fanOutTransitions);
        transitions.Add(State.Strategic_FocusFire, focusFireTransitions);
        transitions.Add(State.Strategic_Regroup, regroupTransitions);
        transitions.Add(State.Strategic_HoldPosition, holdPositionTransitions);
        transitions.Add(State.Strategic_MoveToFiringPosition, moveToFiringPositionTransitions);
        transitions.Add(State.Strategic_Null, nullTransitions);

        //Define and Create the Strategic State Machine
        strategyFSM = new StrategicStateMachine(State.Strategic_HoldPosition, transitions);
        strategyFSM.AddStateBehavior(State.Strategic_FanOut, new FanOutState());
        strategyFSM.AddStateBehavior(State.Strategic_FocusFire, new FocusFireState());
        strategyFSM.AddStateBehavior(State.Strategic_Regroup, new RegroupState());
        strategyFSM.AddStateBehavior(State.Strategic_MoveToFiringPosition, new MoveToFiringPosition());
        strategyFSM.AddStateBehavior(State.Strategic_HoldPosition, new HoldPositionState());
    }//END: Start() Function

    //=======================================================================================================================================

    /// <summary>    
    /// Collects and Processes Game State information to 
    /// determine the next Strategic State of the team's StrategicFSM.
    /// Transitions the Strategic State Machine to its next State.
    /// </summary>
    /// <returns></returns>
    public void Update ()
    {
        //Determine if a Transition needs to Occur
        strategyFSM.MoveToNextStrategicState(GameManager.instance);

        // Disable the use of strategy when we are in the "null" strategic state
        SetTeamUseOrders(strategyFSM.CurrentState != State.Strategic_Null);     //NOTE: The "null" state is not being used currently


        //Determine if a soldier on the team died
        int currentTeamCount = GameManager.instance.teams[0].members.Count;
        teamCountDelta = teamCount - currentTeamCount;
        teamCount = currentTeamCount;

        //Set the TeamCenterPoint
        teamCenterPoint = DetermineTeamCenterPoint();
        
        //Determine what enemy the agents should focus on
        //Uses the center location of the team as a communal point of reference
        Character closestOpp = null;
        float closestDistance = float.PositiveInfinity;
        Team teamBLUE = GameManager.instance.teams[1];

		foreach (var enemy in teamBLUE.members)
        {
            float distance = Vector3.Distance(enemy.transform.position, teamCenterPoint);
            if (distance < closestDistance || closestOpp == null)
			{
				closestDistance = distance;
                closestOpp = enemy;
            }
        }

		teamTarget = closestOpp;
        
        //Determine the team's distance from their teamCenterPoint to the teamTarget
		if (teamTarget) {
			teamDistanceToTarget = Mathf.Abs( Vector3.Distance(teamTarget.transform.position, teamCenterPoint));
			Debug.DrawLine(teamTarget.transform.position + Vector3.up, teamCenterPoint + Vector3.up, Color.yellow);
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

    /// <summary>    
    /// Calcualtes the Center Point of all Teammates on the field by
    /// averaging all Teammate Vector3 Positions
    /// </summary>
    /// <returns>A Vector3; The General Center of a Team's Position</returns>
    public Vector3 DetermineTeamCenterPoint()
    {
        //Determine the ceter point of all team members
        Vector3 teamCenterLocation = Vector3.zero;
        Vector3 vectorSum = Vector3.one;
        Team teamRED = GameManager.instance.teams[0];
        
        //Average all team member's positions
        foreach (var teammate in teamRED.members)
        {
            vectorSum += teammate.transform.position;
        }
        teamCenterLocation = vectorSum / GameManager.instance.teams[0].members.Count;
        return teamCenterLocation;
    }//END: DetemineTeamCenterPoint

    //=======================================================================================================================================

    /// <summary>
    /// Gets the average distance of each team member from the team
    /// center point.
    /// </summary>
    /// <returns>A float; The average distance from the center point</returns>
    public float GetTeamAvgDistanceFromTeamCenter()
    {
        Team teamRED = GameManager.instance.teams[0];
        float distanceSum = 0.0f;
        float distanceAvg = 0.0f;
        foreach (var teammate in teamRED.members)
        {
            distanceSum += Vector3.Distance(teammate.transform.position, teamCenterPoint);
        }

        distanceAvg = distanceSum / teamRED.Count();
        return Mathf.Abs(distanceAvg);

    }

    //=======================================================================================================================================

    /// <summary>
    /// Sets the entire red team to obey/ignore orders
    /// </summary>
    /// <param name="useOrders"></param>
    public void SetTeamUseOrders(bool useOrders)
    {
        Team teamRED = GameManager.instance.teams[0];

        foreach (var teammate in teamRED.members)
        {
            teammate.SetOrders(useOrders);
        }
    }

    //=======================================================================================================================================

}//END: Commander_FSM

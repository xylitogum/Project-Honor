using System.Collections;
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
            // Notifies agents to execute StrategicFSM Commands - rather than TacticalFSM Commands
            teammate.SetOrders(true);
        }

        teamCount = teamRED.members.Count;
        teamTarget = GameManager.instance.teams[1].members[0];

			
        //Define Strategic Transitions
        var teammateDeath = new Transition(State.Strategic_Regroup, (manager) => { return teamCountDelta > 0; });
        var targetEliminated = new Transition(State.Strategic_FocusFire, (manager) => { return teamTarget.getHealth() <= 0.0f; });
        var teamFannedOut = new Transition(State.Strategic_FocusFire, (manager) => { return GetTeamAvgDistanceFromTeamCenter() > 6.0f; });
        var teamGrouped = new Transition(State.Strategic_HoldPosition, (manager) => {  return GetTeamAvgDistanceFromTeamCenter() < 5.0f; });
        var teamLocationReached = new Transition(State.Strategic_FocusFire, (manager) => 
        { var TeamRED = GameManager.instance.teams[0].members;
            bool inPosition = false;
            int numberOfTeamInPosition = 0;
            foreach (var teammate in TeamRED)
            {
                if (Mathf.Abs(Vector3.Distance(teammate.transform.position, teammate.destination)) < 3.0f)
                { numberOfTeamInPosition += 1; }
            }
            if (numberOfTeamInPosition == TeamRED.Count - 1) { inPosition = true; }
            return inPosition; });
        var teamAmbush = new Transition(State.Strategic_MoveToFiringPosition, (manager) => { return teamDistanceToTarget < 7.0f; });
		var targetHidden = new Transition(State.Strategic_MoveToFiringPosition, (manager) => {
			var TeamRED = GameManager.instance.teams[0].members;
			int numberOfVisible = 0;
			foreach (var teammate in TeamRED)
			{
				if (manager.tileManager.PositionCanSeePosition(teammate.transform.position, teamTarget.transform.position))
				{ numberOfVisible += 1; }
			}
			return numberOfVisible > (TeamRED.Count / 2);});

        //Debug.Log("FannedOut fired");Debug.Log("TeamGrouped Fired");Debug.Log("TeammateDeath Fired");Debug.Log("Team Target is " + teamTarget);Debug.Log("Ambush Launched"); 

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
		focusFireTransitions.Add(targetHidden);
        focusFireTransitions.Add(teammateDeath);

        //Define MoveToFiringPosition Transitions
        var moveToFiringPositionTransitions = new List<Transition>();
        moveToFiringPositionTransitions.Add(teamLocationReached);

        //Define HoldPosition Transitions
        var holdPositionTransitions = new List<Transition>();
        holdPositionTransitions.Add(teamAmbush);
        holdPositionTransitions.Add(teammateDeath);


        var transitions = new Dictionary<State, IEnumerable<Transition>>();
        transitions.Add(State.Strategic_FanOut, fanOutTransitions);
        transitions.Add(State.Strategic_FocusFire, focusFireTransitions);
        transitions.Add(State.Strategic_Regroup, regroupTransitions);
        transitions.Add(State.Strategic_HoldPosition, holdPositionTransitions);
        transitions.Add(State.Strategic_MoveToFiringPosition, moveToFiringPositionTransitions);

        strategyFSM = new StrategicStateMachine(State.Strategic_MoveToFiringPosition, transitions);
        strategyFSM.AddStateBehavior(State.Strategic_FanOut, new FanOutState());
        strategyFSM.AddStateBehavior(State.Strategic_FocusFire, new FocusFireState());
        strategyFSM.AddStateBehavior(State.Strategic_Regroup, new RegroupState());
        strategyFSM.AddStateBehavior(State.Strategic_MoveToFiringPosition, new MoveToFiringPosition());
        strategyFSM.AddStateBehavior(State.Strategic_HoldPosition, new HoldPositionState());
        
    }//END: Start() Function

    //=======================================================================================================================================

    // Update is called once per frame
    // Sets the "teamTarget" to the enemy that is the closest to the "center point" of the team
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
        //Debug.Log("Team Avg Distance From Center = " + GetTeamAvgDistanceFromTeamCenter());
        //Debug.Log("Team Center Position = " + GetTeamCenter());
        
        //Determine what enemy the agents should focus on
        //Uses the center location of the team as a communal point of reference
        Character closestOpp = teamTarget;
        float closestDistance = float.PositiveInfinity;
        Team teamBLUE = GameManager.instance.teams[1];

		for (int i = 0; i < teamBLUE.members.Count; i++)
        {
			Character enemy = teamBLUE.members[i];
            float distance = Vector3.Distance(enemy.transform.position, teamCenterPoint);
            //Debug.Log("Enemy Health is " + enemy.getHealth());
            if (distance < closestDistance)
			{
				closestDistance = distance;
                closestOpp = enemy;
            }//END: if

        }//END: foreach

		teamTarget = closestOpp;
        //Debug.Log("Team Target = " + teamTarget);
        
        //Determine the team's distance from their teamCenterPoint to the teamTarget
		if (teamTarget) {
			teamDistanceToTarget = Mathf.Abs( Vector3.Distance(teamTarget.transform.position, teamCenterPoint));
			//Debug.Log("The teamDistanceTotarget = " + teamDistanceToTarget);
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

    //Calcualtes the Center Point of all Teammates on the field by
    //  averaging all Teammate Vector3 Positions
    public Vector3 DetermineTeamCenterPoint()
    {
        //Determine the ceter point of all team members ... maybe
        Vector3 teamCenterLocation = Vector3.zero;
        Vector3 vectorSum = Vector3.one;
        Team teamRED = GameManager.instance.teams[0];
        
        foreach (var teammate in teamRED.members)
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
     
    //=======================================================================================================================================

}//END: Commander_FSM Class
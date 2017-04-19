using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM.StateBehaviors;
using FSM;

/// <summary>
/// Provides the strategic behavior for agents to regroup to a central point.
/// </summary>
public class RegroupState : IStateBehavior
{
    /// <summary>
    /// Sets the agent's Destination to the Team's Spawn Point 0.
    /// </summary>
    /// <param name="agent">The agent whose command should be determined</param>
    /// <param name="gameManager">A copy of the GameManager</param>
    /// <returns>A command representing the set of actions that should be taken</returns>
    public Command GetCommand(Character agent, GameManager gameManager)
    {
        Vector3 regroupPoint = GameManager.instance.teams[0].spawnPoints[0].transform.position;

        Character threat = agent.GetClosestOpponent();
        Vector3 turnVector = threat.transform.position - agent.transform.position;
        
        return new Command(regroupPoint, turnVector.ToVec2(), false, true, true);
    }
}

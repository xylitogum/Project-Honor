using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM.StateBehaviors;
using FSM;

/// <summary>
/// The strategic behavior for agents to hold their current position
/// </summary>
public class HoldPositionState : IStateBehavior
{
    //=================================================================================================================

    /// <summary>
    /// Directs the agent to hold their current position and reload.
    /// </summary>
    /// <param name="agent">The agent whose command should be determined</param>
    /// <param name="gameManager">A copy of the GameManager</param>
    /// <returns>A command representing the set of actions that should be taken</returns>
    public Command GetCommand(Character agent, GameManager gameManager)
    {
        //Aquire TeamTarget from the Commander_FSM instanced object in GameManager
        Character target = GameManager.instance.commander.teamTarget;

        //Set the Target of the Agent to the TeamTarget
        Vector3 turnVec = target.transform.position - agent.transform.position;
        return new Command(new Vector3(agent.transform.position.x, agent.transform.position.y, agent.transform.position.z), turnVec.ToVec2(), false, true, false);
        
    }//END: GetCommand() Function
}//END: HoldPositionState Class
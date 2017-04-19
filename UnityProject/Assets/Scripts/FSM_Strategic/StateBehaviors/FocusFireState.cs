using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM.StateBehaviors;
using FSM;

/// <summary>
/// Defines agent behavior for the FocusFireState.
/// Directs agents to fire on the same target.
/// </summary>
public class FocusFireState : IStateBehavior
{
    
    /// <summary>
    /// Directs the agent to fire on the same Enemy Target that other team members are firing on
    /// </summary>
    /// <param name="agent">The agent whose command should be determined</param>
    /// <param name="gameManager">A copy of the GameManager</param>
    /// <returns>A command representing the set of actions that should be taken</returns>
    public Command GetCommand(Character agent, GameManager gameManager)
    {
        // Orders are passed by the CommandPassingManager to each agent
        Character target = agent.strategicOrders != null ? agent.strategicOrders.TargetCharacter : GameManager.instance.commander.teamTarget;

        if (target != null)
        {
            // Produces the direction that the agent sould face to fire on the enemy
            Vector3 turnVector = target.transform.position - agent.transform.position;
            return new Command(agent.transform.position, turnVector.ToVec2(), true, false, false);
        }

        Debug.LogWarning("Target null. Locating New Target.");
        return new Command(Vector2.zero, Vector2.zero, false, false, false);

    }//END: GetCommand() Function

}//END: FocusFireState Class

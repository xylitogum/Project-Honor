using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM.StateBehaviors;
using FSM;

public class FocusFireState : IStateBehavior
{
    //=================================================================================================================
    
    /// <summary>
    /// Directs the agent to fire on the same Enemy Target that all other teammembers are firing on
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
            //Produces the direction that the agent sould face to fire on the enemy
            Vector3 turnVector = target.transform.position - agent.transform.position;
            //Debug.Log("turnVector = " + turnVector);
            return new Command(agent.transform.position, turnVector.ToVec2(), true, false, false);
        }

        Debug.LogWarning("Target null. Locating New Target.");
        return new Command(Vector2.zero, Vector2.zero, false, false, false);
    }//END: GetCommand() Function

    //=================================================================================================================

    /// <summary>
    /// Gets the manhattan distance between two tiles
    /// </summary>
    /// <param name="t1">The first tile</param>
    /// <param name="t2">The second tile</param>
    /// <returns>The manhattan distance between the given tiles</returns>
    private int ManhattanDistance(Tile t1, Tile t2)
    {
        return Mathf.Abs(t1.x - t2.x) + Mathf.Abs(t1.y - t2.y);
    }//END: ManhattanDistance() Function


    //=================================================================================================================

}//END: FocusFireState Class

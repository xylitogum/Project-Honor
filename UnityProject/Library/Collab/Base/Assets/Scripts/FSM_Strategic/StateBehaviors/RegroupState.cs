using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM.StateBehaviors;
using FSM;

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
        
        //Debug.LogWarning("Ummmm ... Something went WRONG with GetCommand for the RegroupState behavior. .... damn");
        //return new Command(Vector2.zero, Vector2.zero, false, false, true);
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

}//END: RegroupState Class

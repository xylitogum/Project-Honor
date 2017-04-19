using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM.StateBehaviors;
using FSM;

public class FanOutState : IStateBehavior
{

    /// <summary>
    /// Sets the agent's Destination a coverpoint that is some distance away from the agent's closest teammate.
    /// </summary>
    /// <param name="agent">The agent whose command should be determined</param>
    /// <param name="gameManager">A copy of the GameManager</param>
    /// <returns>A command representing the set of actions that should be taken</returns>
    public Command GetCommand(Character agent, GameManager gameManager)
    {
        List<Character> opponents = agent.GetOpponents();
        Character closestAlly = agent.GetClosestTeammate();
        var agentTile = new Tile(agent.transform.position);
        var closestAllyTile = new Tile(closestAlly.transform.position);

        // Get the list of all cover points sorted by distance from the agent
        List<Tile> coverPoints = gameManager.tileManager.coverSpots;
        coverPoints.Sort((point1, point2) => Tile.ManhattanDistance(point1, agentTile) - Tile.ManhattanDistance(point2, agentTile));

        // Check each cover location
        foreach (var coverPoint in coverPoints)
        {
            var safe = true;

            if (Tile.ManhattanDistance(coverPoint, closestAllyTile) > 5)
            {
                // See if this spot has a line of sight to any opponent
                foreach (var opponent in opponents)
                {
                    if (gameManager.tileManager.PositionCanSeePosition(coverPoint.ToVector3(), opponent.transform.position))
                    {
                        safe = false;
                        break;
                    }
                }
                if (!safe)
                {
                    // Choose this cover spot to MOVE to ... because, we want a cover point that provides a line of sight to a target opponent
                    var closestOpp = agent.GetClosestOpponent();
                    var turnVector = new Vector2(closestOpp.transform.position.x, closestOpp.transform.position.z) - new Vector2(agent.transform.position.x, agent.transform.position.z);
                    return new Command(new Vector3(coverPoint.x, agent.transform.position.y, coverPoint.y), turnVector, false, false, true);
                }
            }
        }//END: "foreach" loop
        
        Debug.LogWarning("Ummmm ... Something went WRONG with GetCommand for the FanOutState behavior.");
        return new Command(Vector2.zero, Vector2.zero, false, false, true);
    }//END: GetCommand() Function

}//END: FanOutState Class

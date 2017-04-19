using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM.StateBehaviors;
using FSM;

public class MoveToFiringPosition : IStateBehavior
{
    //=================================================================================================================

    /// <summary>
    /// Directs the agent to Move to a location where they can obtain a Line of Sight on the Enemy Target
    /// </summary>
    /// <param name="agent">The agent whose command should be determined</param>
    /// <param name="gameManager">A copy of the GameManager</param>
    /// <returns>A command representing the set of actions that should be taken</returns>
    public Command GetCommand(Character agent, GameManager gameManager)
    {
        // Orders are passed by the CommandPassingManager to each agent
        Character target = agent.strategicOrders != null ? agent.strategicOrders.TargetCharacter : GameManager.instance.commander.teamTarget;

        var agentTile = new Tile(agent.transform.position);

        // Get the list of all cover points sorted by distance from the agent
        List<Tile> coverPoints = gameManager.tileManager.coverSpots;
        coverPoints.Sort((point1, point2) => ManhattanDistance(point1, agentTile) - ManhattanDistance(point2, agentTile));

        var safe = true;
        // See if this spot is hidden from the target

        // Check each cover location
        foreach (var coverPoint in coverPoints)
        {
            if (target != null)
            {
                if (gameManager.tileManager.PositionCanSeePosition(new Vector3(coverPoint.x, 0, coverPoint.y), target.transform.position, agent.GetTeammates()))
                {
                    safe = false;
                }

                if (!safe)
                {
                    // Choose this cover spot to MOVE to ... because, we want a cover point that provides a line of sight to the target opponent
                    Vector3 turnVec = target.transform.position - agent.transform.position;
                    return new Command(new Vector3(coverPoint.x, agent.transform.position.y, coverPoint.y), turnVec.ToVec2(), false, false, true);
                }
            }
        }//END: "foreach" loop

        // If no cover locations will work, check all other non-cover
        // tiles on the map
        foreach (var spot in gameManager.tileManager.allSpots)
        {
            if (target != null)
            {
                if (gameManager.tileManager.PositionCanSeePosition(new Vector3(spot.x, 0, spot.y), target.transform.position, agent.GetTeammates()))
                {
                    safe = false;
                }

                if (!safe)
                {
                    // Choose this cover spot to MOVE to ... because, we want a cover point that provides a line of sight to the target opponent
                    Vector3 turnVec = target.transform.position - agent.transform.position;
                    return new Command(new Vector3(spot.x, agent.transform.position.y, spot.y), turnVec.ToVec2(), false, false, true);
                }
            }
        }

        //Produces the direction that the agent should face to fire on the enemy
        Vector3 turnVector = agent.GetClosestOpponent().transform.position - agent.transform.position;
        //Debug.Log("turnVector = " + turnVector);
        return new Command(agent.transform.position, turnVector.ToVec2(), true, false, false);

        //Debug.LogWarning("Ummmm ... Something went WRONG with GetCommand for the MoveToFiringPosition behavior. .... damn");
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

}//END: MoveToFiringPosition Class

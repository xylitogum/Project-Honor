using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM.StateBehaviors;
using FSM;

/// <summary>
/// Defines the behavior of the agent when it is in the 
/// MoveToFiringPosition state of the Strategic State Machine.
/// Agents move towards a location where they will have line of sight on a target
///  determined via the Commander_FSM class, or by their own Tactical FSM - as the situation dictates.
/// </summary>
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
        coverPoints.Sort((point1, point2) => Tile.ManhattanDistance(point1, agentTile) - Tile.ManhattanDistance(point2, agentTile));

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
        }//END: foreach

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
                    Vector3 turnVec = target.transform.position - agent.transform.position;
                    return new Command(new Vector3(spot.x, agent.transform.position.y, spot.y), turnVec.ToVec2(), false, false, true);
                }
            }
        }//END: foreach

        // Presents the direction that the agent should face to fire on the enemy
        Vector3 turnVector = agent.GetClosestOpponent().transform.position - agent.transform.position;
        return new Command(agent.transform.position, turnVector.ToVec2(), true, false, false);

    }//END: GetCommand() Function

}//END: MoveToFiringPosition Class

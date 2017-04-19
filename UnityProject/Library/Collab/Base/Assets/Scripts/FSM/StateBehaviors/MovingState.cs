using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace FSM.StateBehaviors
{

    public class MovingState : IStateBehavior
    {
        /// <summary>
        /// Gets the command for an agent in the "Moving" state
        /// Identifies the closest location to the agent that also has a line of sight to an Opponent.
        /// </summary>
        /// <param name="agent">The agent whose command should be determined</param>
        /// <param name="gameManager">A copy of the GameManager</param>
        /// <returns>A command representing the set of actions that should be taken</returns>
        public Command GetCommand(Character agent, GameManager gameManager)
        {
            List<Character> opponents = agent.GetOpponents();
            var agentTile = new Tile(agent.transform.position);

            // Get the list of all cover points sorted by distance from the agent
            List<Tile> coverPoints = gameManager.tileManager.coverSpots;
            coverPoints.Sort((point1, point2) => Tile.ManhattanDistance(point1, agentTile) - Tile.ManhattanDistance(point2, agentTile));
            
            // Check each cover location
            foreach (var coverPoint in coverPoints)
            {
                var safe = true;
                // See if this spot is hidden from all opponents
                foreach (var opponent in opponents)
                {
                    if (gameManager.tileManager.PositionCanSeePosition(new Vector3(coverPoint.x, 0, coverPoint.y), opponent.transform.position, agent.GetTeammates()))
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
                    return new Command(new Vector3(coverPoint.x, agent.transform.position.y ,coverPoint.y), turnVector, false, false, true);
                }
            }

            Debug.LogWarning("No cover points with line of sight to an enemy could be found");
            return new Command(Vector2.zero, Vector2.zero, false, false, true);
        }

    }
}
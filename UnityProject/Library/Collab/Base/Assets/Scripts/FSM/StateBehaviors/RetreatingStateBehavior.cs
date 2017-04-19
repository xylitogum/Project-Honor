using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace FSM.StateBehaviors
{
    /// <summary>
    /// Defines the behavior of a character when it is in the
    /// retreating state of the state machine.
    /// </summary>
    public class RetreatingStateBehavior : IStateBehavior {

        /// <summary>
        /// Gets the command dictating what the next move for this agent should be.
        /// </summary>
        /// <param name="agent">The agent whose command should be determined</param>
        /// <param name="gameManager">A copy of the GameManager</param>
        /// <returns>A command representing the set of actions that should be taken</returns>
        public Command GetCommand(Character agent, GameManager gameManager) {
            List<Character> opponents = agent.GetOpponents();
            var agentTile = new Tile(agent.transform.position);
            
            // Get the list of all cover points sorted by distance from the agent
            List<Tile> coverPoints = gameManager.tileManager.coverSpots;
            coverPoints.Sort((point1, point2) => Tile.ManhattanDistance(point1, agentTile) - Tile.ManhattanDistance(point2, agentTile));
            
            // Skip the first few closest points to create more distance
            for(var i = 0; i  < 3; i++)
            {
                coverPoints.Add(coverPoints[0]);
                coverPoints.RemoveAt(0);
            }

            // Check each cover location
            foreach (var coverPoint in coverPoints)
            {
                var safe = true;
                // See if this spot is hidden from all opponents
                foreach (var opponent in opponents)
                {
                    if (gameManager.tileManager.PositionCanSeePosition(new Vector3(coverPoint.x, 0, coverPoint.y), opponent.transform.position)) { 
                        safe = false;
                        break;
                    }
                }
                if(safe) {
                    // Choose this cover spot to retreat to. Turn to face the closest enemy
                    var closestOpp = agent.GetClosestOpponent();
                    var turnVector = new Vector2(closestOpp.transform.position.x, closestOpp.transform.position.z) - new Vector2(agent.transform.position.x, agent.transform.position.z);
                    return new Command(new Vector3(coverPoint.x, agent.transform.position.y ,coverPoint.y), turnVector, false, false, true);
                }
            }

            Debug.LogWarning("No safe cover point could be found");
            return new Command(Vector3.zero, Vector2.zero, false, false, true);
        }
    }
}

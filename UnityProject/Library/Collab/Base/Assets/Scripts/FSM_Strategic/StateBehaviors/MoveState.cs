using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace FSM.StateBehaviors
{

    public class Move : IStateBehavior
    {
        /// <summary>
        /// Identifies the closest location to the agent that also has a line of sight to an Opponent.
        /// Sets the NAVAgent's Destination, for the agent, to that location.
        /// </summary>
        /// <param name="agent">The agent whose command should be determined</param>
        /// <param name="gameManager">A copy of the GameManager</param>
        /// <returns>A command representing the set of actions that should be taken</returns>
        public Command GetCommand(Character agent, GameManager gameManager)
        {
            List<Character> opponents = agent.GetOpponents();
            var teamTile = new Tile(GameManager.instance.commander.teamCenterPoint);

            // Get the list of all cover points sorted by distance from the agent
            List<Tile> coverPoints = gameManager.tileManager.coverSpots;
            coverPoints.Sort((point1, point2) => ManhattanDistance(point1, teamTile) - ManhattanDistance(point2, teamTile));

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
                    agent.destination = coverPoint.ToVector3();
                    var closestOpp = agent.GetClosestOpponent();
                    var turnVector = new Vector2(closestOpp.transform.position.x, closestOpp.transform.position.z) - new Vector2(agent.transform.position.x, agent.transform.position.z);
                    return new Command(new Vector3(coverPoint.x, agent.transform.position.y, coverPoint.y), turnVector, false, false, true);
                }
            }//END: "foreach" loop

            Debug.LogWarning("No safe cover point could be found");
            return new Command(Vector2.zero, Vector2.zero, false, false, true);
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

    }//END: Move Class
}//END: FSM.StateBehaviors Namespace
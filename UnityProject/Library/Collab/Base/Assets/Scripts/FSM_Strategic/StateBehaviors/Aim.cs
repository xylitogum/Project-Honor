using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace FSM.StateBehaviors
{
    /// <summary>
    /// Defines the behavior of a character when it is in the
    /// aiming state of the tactical state machine.
    /// </summary>
    public class Aim : IStateBehavior
    {
        /// <summary>
        /// Gets the command dictating what the next move for this agent should be.
        /// </summary>
        /// <param name="agent">The agent whose command should be determined</param>
        /// <param name="gameManager">A copy of the GameManager</param>
        /// <returns>A command representing the set of actions that should be taken</returns>
        public Command GetCommand(Character agent, GameManager gameManager)
        {
            List<Character> opponents = agent.GetOpponents();
            var agentTile = new Tile(agent.transform.position);
            opponents.Sort((opp1, opp2) => ManhattanDistance(new Tile(opp1.transform.position), agentTile)
                 - ManhattanDistance(new Tile(opp2.transform.position), agentTile));

            foreach (var enemy in opponents)
            {
                if (agent.HasLineOfSight(enemy))
                {
                    var turnVec = new Vector2(enemy.transform.position.x, enemy.transform.position.z) -
                            new Vector2(agent.transform.position.x, agent.transform.position.z);
                    return new Command(agent.transform.position, turnVec, true, false, false);
                }
            }

            Debug.LogWarning("No enemy sighted in aiming state");
            return new Command(agent.transform.position, Vector2.zero, true, false, false);
        }

        /// <summary>
        /// Gets the manhattan distance between two tiles
        /// </summary>
        /// <param name="t1">The first tile</param>
        /// <param name="t2">The second tile</param>
        /// <returns>The manhattan distance between the given tiles</returns>
        private int ManhattanDistance(Tile t1, Tile t2)
        {
            return Mathf.Abs(t1.x - t2.x) + Mathf.Abs(t1.y - t2.y);
        }
    }
}
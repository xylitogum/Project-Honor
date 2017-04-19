using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace FSM.StateBehaviors
{
    /// <summary>
    /// Defines the behavior of a character when it is in the
    /// resting state of the tactical state machine.
    /// </summary>
    public class RestingStateBehavior : IStateBehavior {

        /// <summary>
        /// Gets the command dictating what the next move for this agent should be.
        /// </summary>
        /// <param name="agent">The agent whose command should be determined</param>
        /// <param name="gameManager">A copy of the GameManager</param>
        /// <returns>A command representing the set of actions that should be taken</returns>
        public Command GetCommand(Character agent, GameManager gameManager) {
            // The agent should remain in place and reload
            return new Command(agent.transform.position, Vector2.zero, false, true, false);
        }
    }
}
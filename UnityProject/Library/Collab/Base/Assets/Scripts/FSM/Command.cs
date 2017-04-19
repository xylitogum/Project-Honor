using UnityEngine;

namespace FSM
{
    /// <summary>
    /// A object containing all the information necessary to describe
    /// an agent's behavior during a single update step.
    /// </summary>
    public class Command
    {
        /// <summary>
        /// The movement destination of the agent
        /// </summary>
        public readonly Vector3 MoveDest;
        /// <summary>
        /// The direction the agent should turn to face
        /// </summary>
        public readonly Vector2 TurnDir;
        /// <summary>
        /// Whether or not the agent should be firing
        /// </summary>
        public readonly bool ShouldFire;
        /// <summary>
        /// Whether or not the agent should be reloading
        /// </summary>
        public readonly bool ShouldReload;
        /// <summary>
        /// Whether or not the agent should be moving
        /// </summary>
        public readonly bool ShouldMove;

        public Command(Vector3 moveDest, Vector2 turnDir, bool shouldFire, bool shouldReload, bool shouldMove)
        {
            MoveDest = moveDest;
            TurnDir = turnDir;
            ShouldFire = shouldFire;
            ShouldReload = shouldReload;
            ShouldMove = shouldMove;
        }
    }
}
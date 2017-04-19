using UnityEngine;

namespace FSM.StateBehaviors
{
    /// <summary>
    /// A sample state behavior for demonstration purposes only.
    /// </summary>
    public class TestStateBehavior : IStateBehavior
    {
        public Command GetCommand(Character agent, GameManager gameManager) {
            return new Command(Vector3.zero, Vector2.zero, false, false, false);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSM;

namespace Assets.Scripts.FSM_Strategic
{
    /// <summary>
    /// Represents the current strategy that a strategically
    /// savvy agent should respond to
    /// </summary>
    public class StrategicCommand
    {
        /// <summary>
        /// The current strategy
        /// </summary>
        public readonly State StrategicState;
        /// <summary>
        /// The target character of the strategy, if relevant.
        /// For example, the enemy target to focus fire on.
        /// </summary>
        public readonly Character TargetCharacter;

        public StrategicCommand(State strategicState, Character targetCharacter)
        {
            StrategicState = strategicState;
            TargetCharacter = targetCharacter;
        }
    }
}

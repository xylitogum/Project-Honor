using System;

namespace FSM
{
    /// <summary>
    /// A representation of a state transition event and its target state.
    /// </summary>
    public class Transition
    {
        /// <summary>
        /// The state to move to if the transition occurs
        /// </summary>
        public State DestinationState;

        /// <summary>
        /// A delegate function that returns a boolean true or false
        /// based on whether or not the transition should occur
        /// </summary>
        public Func<GameManager, Character, bool> EventTriggered;

        /// <summary>
        /// A delegate function that returns a boolean true or false
        /// based on whether or not the transition should occur
        /// in a strategic setting
        /// </summary>
        public Func<GameManager, bool> StrategyEventTriggered;

        public Transition(State destinationState, Func<GameManager, Character, bool> eventTriggered)
        {
            DestinationState = destinationState;
            EventTriggered = eventTriggered;
        }

        public Transition(State destinationState, Func<GameManager, bool> strategyEventTriggered)
        {
            DestinationState = destinationState;
            StrategyEventTriggered = strategyEventTriggered;
        }
    }
}
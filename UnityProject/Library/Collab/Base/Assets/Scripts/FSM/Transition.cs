using System;

namespace FSM
{
    /// <summary>
    /// A representation of a state transition event and its target state.
    /// </summary>
    public class Transition
    {
        public State DestinationState;
        public Func<GameManager, Character, bool> EventTriggered;
        public Func<GameManager, bool> StrategyEventTriggered;

        public Transition(State destinationState, Func<GameManager, Character, bool> eventTriggered)
        {
            DestinationState = destinationState;
            EventTriggered = eventTriggered;
        }

        //==================================================================================================================

        public Transition(State destinationState, Func<GameManager, bool> strategyEventTriggered)
        {
            DestinationState = destinationState;
            StrategyEventTriggered = strategyEventTriggered;

        }//END: StrategyTransition Constructor

        //==================================================================================================================

    }//END: Transition Class
}
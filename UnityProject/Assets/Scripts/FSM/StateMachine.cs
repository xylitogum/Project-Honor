using System;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    /// <summary>
    /// A generic finite-state machine that can be used to model agent
    /// behavior
    /// </summary>
    public class StateMachine
    {
        public State CurrentState;
        public State PreviousState;
        public Dictionary<State, IEnumerable<Transition>> Transitions;

        public StateMachine(State initialState, Dictionary<State, IEnumerable<Transition>> transitions) {
            CurrentState = initialState;
            PreviousState = CurrentState;
            Transitions = transitions;
        }

        /// <summary>
        /// Gets the next state that the machine should move to based
        /// on the current state of the game.
        /// </summary>
        /// <returns>
        /// The next state that should be moved to after examining
        /// the current state of the world.
        /// </returns>
        public State GetNextState(GameManager manager, Character ch)
        {
            // See if any transitions out of the current state are triggerable
            IEnumerable<Transition> currentStateTransitions;
            if(!Transitions.TryGetValue(CurrentState, out currentStateTransitions)) {
                Debug.LogWarning("No transitions defined for state " + CurrentState);
                return CurrentState;
            }
            foreach (var item in currentStateTransitions)
            {
                if(item.EventTriggered(manager, ch))
                {
                    return item.DestinationState;
                }
            }
            return CurrentState;
        }

        /// <summary>
        /// Updates the current state of the machine based on the current
        /// world state.
        /// </summary>
        public void MoveToNextState(GameManager manager, Character ch)
        {
            PreviousState = CurrentState;
            CurrentState = GetNextState(manager, ch);
        }

        /// <summary>
        /// Gets the next state that the Strategic Machine should move to based
        /// on the current state of the game.
        /// </summary>
        /// <returns>
        /// The next state that should be moved to after examining
        /// the current state of the world.
        /// </returns>
        public State GetNextStrategicState(GameManager manager)
        {
            // See if any transitions out of the current state are triggerable
            IEnumerable<Transition> currentStateTransitions;
            if (!Transitions.TryGetValue(CurrentState, out currentStateTransitions))
            {
                Debug.LogWarning("No transitions defined for state " + CurrentState);
                return CurrentState;
            }
            foreach (var item in currentStateTransitions)
            {
                if (item.StrategyEventTriggered(manager))
                {
                    return item.DestinationState;
                }
            }
            return CurrentState;
        }

        /// <summary>
        /// Updates the current state of the Strategic Machine based on the current
        /// world state.
        /// </summary>
        public void MoveToNextStrategicState(GameManager manager)
        {
            PreviousState = CurrentState;
            CurrentState = GetNextStrategicState(manager);
        }
    }
}
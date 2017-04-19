using System;
using System.Collections.Generic;
using UnityEngine;
using FSM.StateBehaviors;

namespace FSM
{
    /// <summary>
    /// A Finite State Machine which provides a mapping of strategic
    /// behavior to each state
    /// </summary>
    public class StrategicStateMachine : StateMachine
    {
        private Dictionary<State, IStateBehavior> StateBehaviors;

        //=================================================================================================
        
        /// <summary>
        ///Constructs a StrategicStateMachine Object
        /// </summary>
        public StrategicStateMachine(State initialState, Dictionary<State, IEnumerable<Transition>> transitions) : base(initialState, transitions)
        {
            StateBehaviors = new Dictionary<State, IStateBehavior>();
        }

        //=================================================================================================

        /// <summary>
        /// Gets the next command that should be executed based on the current state.
        /// </summary>
        /// <param name="agent">The agent which is acting</param>
        /// <param name="gameManager">The game manager holding the world state</param>
        /// <returns>The desired command for the given agent</returns>
        public Command GetCommand(Character agent, GameManager gameManager)
        {
            IStateBehavior behavior;
            if (!StateBehaviors.TryGetValue(CurrentState, out behavior))
            {
                // There was no behavior defined for the current state, return an empty command
                return new Command(Vector2.zero, Vector2.zero, false, false, false);
            }
            return behavior.GetCommand(agent, gameManager);
        }

        //=================================================================================================

        /// <summary>
        /// Adds a new behavior definition to the state machine.
        /// </summary>
        /// <param name="s">The state whose behavior is being described</param>
        /// <param name="behavior">The behavior class used to get the behavior at the given state</param>
        public void AddStateBehavior(State s, IStateBehavior behavior)
        {
            StateBehaviors.Add(s, behavior);
        }

        //=================================================================================================

    }//END: StrategicStateMachine Class
}//END: FSM namespace
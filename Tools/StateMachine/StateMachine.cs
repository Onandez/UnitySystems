using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameKit.Tools
{
    public struct StateChangeEvent<T> where T : struct, IComparable, IConvertible, IFormattable
    {
        public GameObject Target;
        public StateMachine<T> TargetStateMachine;
        public T NewState;
        public T PreviousState;

        public StateChangeEvent(StateMachine<T> stateMachine)
        {
            Target = stateMachine.Target;
            TargetStateMachine = stateMachine;
            NewState = stateMachine.CurrentState;
            PreviousState = stateMachine.PreviousState;
        }
    }

    // Public interface for the state machine. This is used by the StateMachineProcessor.
    public interface IStateMachine
    {
        bool TriggerEvents { get; set; }
    }

    public class StateMachine<T> : IStateMachine where T : struct, IComparable, IConvertible, IFormattable
    {
        /// If TriggerEvents to true, the state machine will trigger events when entering and exiting a state. 
		/// If use a StateMachineProcessor, it'll trigger events for the current state on FixedUpdate, LateUpdate, but also
		/// on Update (separated in EarlyUpdate, Update and EndOfUpdate, triggered in this order at Update()
		/// To listen to these events, from any class, in its Start() method (or wherever you prefer), use MMEventManager.StartListening(gameObject.GetInstanceID().ToString()+"XXXEnter",OnXXXEnter);
		/// where XXX is the name of the state you're listening to, and OnXXXEnter is the method you want to call when that event is triggered.
		/// MMEventManager.StartListening(gameObject.GetInstanceID().ToString()+"CrouchingEarlyUpdate",OnCrouchingEarlyUpdate); for example will listen to the Early Update event of the Crouching state, and 
		/// will trigger the OnCrouchingEarlyUpdate() method. 
		public bool TriggerEvents { get; set; }
        /// the name of the target gameobject
        public GameObject Target;
        /// the current character's movement state
        public T CurrentState { get; protected set; }
        /// the character's movement state before entering the current one
        public T PreviousState { get; protected set; }

        public delegate void OnStateChangeDelegate();
        /// an event you can listen to to listen locally to changes on that state machine
        /// to listen to them, from any class : 
        /// void OnEnable()
        /// {
        ///    yourReferenceToTheStateMachine.OnStateChange += OnStateChange;
        /// }
        /// void OnDisable()
        /// {
        ///    yourReferenceToTheStateMachine.OnStateChange -= OnStateChange;
        /// }
        /// void OnStateChange()
        /// {
        ///    // Do something
        /// }
        public OnStateChangeDelegate OnStateChange;

        // Creates a new StateMachine, with a targetName (used for events, usually use GetInstanceID()), and whether you want to use events with it or not
        public StateMachine(GameObject target, bool triggerEvents)
        {
            this.Target = target;
            this.TriggerEvents = triggerEvents;
        }

        // Changes the current movement state to the one specified in the parameters, and triggers exit and enter events if needed
        public virtual void ChangeState(T newState)
        {
            // if the "new state" is the current one, we do nothing and exit
            if (newState.Equals(CurrentState)) return;

            // we store our previous character movement state
            PreviousState = CurrentState;
            CurrentState = newState;

            OnStateChange?.Invoke();

            if (TriggerEvents)
            {
                EventManager.TriggerEvent(new StateChangeEvent<T>(this));
            }
        }

        // Returns the character to the state it was in before its current state
        public virtual void RestorePreviousState()
        {
            // we restore our previous state
            CurrentState = PreviousState;
            Debug.Log("STATEMACHINE_ENGINE " + Target.gameObject.name + " restore state to new state");

            OnStateChange?.Invoke();

            if (TriggerEvents)
            {
                EventManager.TriggerEvent(new StateChangeEvent<T>(this));
            }
        }
    }
}

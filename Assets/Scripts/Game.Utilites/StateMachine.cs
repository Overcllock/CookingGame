using System.Collections.Generic;
using UnityEngine;

namespace Game.Utilites
{
    public class StateMachine<T>
    {
        private class State
        {
            public T id;
            public StateAction OnEnter;
            public StateAction OnUpdate;
            public StateAction OnExit;

            public State(T id, StateAction OnEnter, StateAction OnUpdate, StateAction OnExit)
            {
                this.id = id;
                this.OnEnter = OnEnter;
                this.OnUpdate = OnUpdate;
                this.OnExit = OnExit;
            }
        }

        private bool _enableLog = true;

        private State _currentState;
        private Dictionary<T, State> _states = new Dictionary<T, State>();
        
        public delegate void StateAction();

        public void Add(T id, StateAction OnEnter, StateAction OnUpdate, StateAction OnExit)
        {
            var newState = new State(id, OnEnter, OnUpdate, OnExit);
            _states.Add(id, newState);
        }

        public bool HasState()
        {
            return _currentState != null;
        }

        public T CurrentState()
        {
            return HasState() ? _currentState.id : default;
        }

        public void Update()
        {
            if (_currentState != null)
            {
                _currentState?.OnUpdate();
            }
        }

        public void Shutdown()
        {
            if (_currentState != null)
            {
                _currentState?.OnExit();
            }

            _currentState = null;
        }

        public void TrySwitchTo(T state)
        {
            if (_currentState != null && _currentState.id.Equals(state))
            {
                return;
            }

            SwitchTo(state);
        }

        public void SwitchTo(T state)
        {
            Error.Assert(_states.ContainsKey(state), $"Trying to switch to unknown state {state}");
            Error.Assert(_currentState == null || !_currentState.id.Equals(state),
                $"Trying to switch to {state} but that is already current state");

            var newState = _states[state];
            if (_enableLog)
            {
                Debug.Log($"Switching state: {(_currentState != null ? _currentState.id.ToString() : "null")} -> {state}");
            }

            _currentState?.OnExit?.Invoke();
            newState?.OnEnter?.Invoke();

            _currentState = newState;
        }

        public void LogSetEnabled(bool enabled)
        {
            _enableLog = enabled;
        }
    }
}
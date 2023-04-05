using System;
using System.Collections;
using System.Collections.Generic;

namespace Game.Events
{
    public class RoutineQueue : IDisposable
    {
        private Queue<IRoutine> _pendingRoutines;

        private bool _isExecuting;
        private bool _autoExecution;

        public IRoutine currentRoutine { get; private set; }
        public int remainingRoutines { get { return _pendingRoutines.Count; } }
        public bool isDepleted { get { return !isExecuting && _pendingRoutines.Count == 0; } }
        public bool isExecuting
        {
            get { return _isExecuting; }
            set
            {
                if (_isExecuting != value)
                {
                    _isExecuting = value;
                    executionChanged?.Invoke(value);
                }
            }
        }

        public event Action<bool> executionChanged;
        public event Action nextRoutineStarted;
        public event Action depleted;

        public RoutineQueue(bool autoExecution = false)
        {
            _autoExecution = autoExecution;
            _pendingRoutines = new Queue<IRoutine>();
        }

        public void Execute()
        {
            if (!isExecuting)
            {
                TryStartNext();
            }
        }

        public void Enqueue(Action action)
        {
            ActionRoutine newRoutine = new ActionRoutine(action);
            Enqueue(newRoutine);
        }

        public void Enqueue(IEnumerator routine)
        {
            SimpleUnityRoutine newRoutine = new SimpleUnityRoutine(routine);
            Enqueue(newRoutine);
        }

        public void Enqueue(IRoutine routine)
        {
            _pendingRoutines.Enqueue(routine);
            if (_autoExecution)
            {
                TryStartNext();
            }
        }

        private void TryStartNext()
        {
            if (_pendingRoutines.Count > 0)
            {
                IRoutine nextRoutine = _pendingRoutines.Dequeue();
                nextRoutine.completed += HandleRoutineComplete;

                currentRoutine = nextRoutine;

                isExecuting = true;
                nextRoutineStarted?.Invoke();

                nextRoutine.Start();
            }
        }

        private void HandleRoutineComplete(IRoutine routine)
        {
            routine.completed -= HandleRoutineComplete;
            routine.Dispose();

            currentRoutine = null;
            isExecuting = false;

            if (_pendingRoutines.Count == 0)
            {
                depleted?.Invoke();
            }

            TryStartNext();
        }

        public void Dispose()
        {
            if (currentRoutine != null)
            {
                currentRoutine.completed -= HandleRoutineComplete;
                currentRoutine.Dispose();
                currentRoutine = null;
            }

            while (_pendingRoutines.Count > 0)
            {
                var nextRoutine = _pendingRoutines.Dequeue();
                nextRoutine.Dispose();
            }
        }
    }
}
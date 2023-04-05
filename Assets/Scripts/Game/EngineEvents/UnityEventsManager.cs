using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Events
{
    public class UnityEventsManager : IEngineEventsManager, IDisposable
    {
        private EngineEventsComponent _engineEvents;
        private CoroutinesExecutionComponent _coroutines;

        private Dictionary<EventsType, EventSubscriptionsContainer> _containers;

        public bool applicationFocused { get; private set; } = true;
        public bool applicationPaused { get; private set; }
        public bool applicationQuitting { get; private set; }

        public UnityEventsManager()
        {
            GameObject componentsHolder = new GameObject("[UnityEngineEvents]");
            UnityEngine.Object.DontDestroyOnLoad(componentsHolder);

            _engineEvents = componentsHolder.AddComponent<EngineEventsComponent>();
            _coroutines = componentsHolder.AddComponent<CoroutinesExecutionComponent>();

            _engineEvents.eventUpdated += HandleEventUpdated;
        }

        public void Dispose()
        {
            _engineEvents.eventUpdated -= HandleEventUpdated;
        }

        private void HandleEventUpdated(EventsType type, bool value)
        {
            switch (type)
            {
                case EventsType.OnApplicationFocus:
                    applicationFocused = value;
                    break;

                case EventsType.OnApplicationPause:
                    applicationPaused = value;
                    break;
                
                case EventsType.OnApplicationQuit:
                    applicationQuitting = value;
                    break;
            }

            TryDispatchUpdate(type);
        }

        private void TryDispatchUpdate(EventsType type)
        {
            if (_containers == null) return;
            if (_containers != null && _containers.TryGetValue(type, out EventSubscriptionsContainer container))
            {
                container.Update();
            }
        }

        public void Subscribe(EventsType type, Action action)
        {
            if (_containers == null)
            {
                _containers = new Dictionary<EventsType, EventSubscriptionsContainer>();
            }

            if (_containers.TryGetValue(type, out EventSubscriptionsContainer container))
            {
                container.Add(action);
            }
            else
            {
                EventSubscriptionsContainer newContainer = new EventSubscriptionsContainer(type);
                newContainer.Add(action);

                _containers[type] = newContainer;
            }
        }

        public void Unsubscribe(EventsType type, Action action)
        {
            if (_containers == null)
            {
                Debug.LogWarning($"Could not unsubscribe from unexisting container type: {type}");
                return;
            }

            if (_containers.TryGetValue(type, out EventSubscriptionsContainer container))
            {
                container.TryRemove(action);
            }
        }

        public void ExecuteCoroutine(IEnumerator routine)
        {
            if (_coroutines != null)
                _coroutines.ExecuteCoroutine(routine);
        }

        public void CancelCoroutine(IEnumerator routine)
        {
            if (_coroutines != null)
                _coroutines?.CancelCoroutine(routine);
        }

        public void TerminateCoroutines()
        {
            if (_coroutines != null)
                _coroutines.TerminateCoroutines();
        }

        public void WaitUntilAndExecute(Action action, Func<bool> condition, int repeat = 1)
        {
            if (_coroutines != null)
                _coroutines.ExecuteCoroutine(WaitUntilAndExecuteRoutine(action, condition, repeat));
        }

        public void DelayAndExecute(Action action, float delay = 0, int repeat = 1)
        {
            if (_coroutines != null)
                _coroutines.ExecuteCoroutine(DelayAndExecuteRoutine(action, delay, repeat));
        }

        public void ExecuteForPeriod(Action action, float period, float delay = 0)
        {
            if (_coroutines != null)
                _coroutines.ExecuteCoroutine(ExecuteForPeriodRoutine(action, period, delay));
        }

        public void ForceClearSubscriptions()
        {
            foreach (var container in _containers.Values)
            {
                container.ClearAll();
            }
        }

        private IEnumerator DelayAndExecuteRoutine(Action action, float delay = 0, int repeat = 1)
        {
            if (delay > 0)
            {
                yield return Wait.ForSeconds(delay);
            }
            else
            {
                yield return null;
            }

            if (repeat > 1)
            {
                for (int i = 0; i < repeat; i++)
                {
                    action?.Invoke();
                    yield return null;
                }
            }
            else
            {
                action?.Invoke();
            }

        }

        private IEnumerator WaitUntilAndExecuteRoutine(Action action, Func<bool> condition, int repeat = 1)
        {
            yield return new WaitUntil(condition);

            if (repeat > 1)
            {
                for (int i = 0; i < repeat; i++)
                {
                    action?.Invoke();
                    yield return null;
                }
            }
            else
            {
                action?.Invoke();
            }

        }

        private IEnumerator ExecuteForPeriodRoutine(Action action, float period, float delay = 0)
        {
            if (delay > 0)
            {
                yield return Wait.ForSeconds(delay);
            }

            if (period == 0)
            {
                action?.Invoke();
            }
            else
            {
                float time = 0;
                while (time < period)
                {
                    time += Time.deltaTime;
                    action?.Invoke();
                    yield return null;
                }
            }
        }
    }
}
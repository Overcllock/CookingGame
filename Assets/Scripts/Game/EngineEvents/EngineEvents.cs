using System;
using System.Collections;

namespace Game.Events
{
    /// <summary>
    /// Facade for engine events manager.
    /// Provides API for event subscriptions and other useful game loop related stuff.
    /// </summary>
    public class EngineEvents : StaticWrapper<IEngineEventsManager>
    {
        public static bool applicationPaused { get { return _instance.applicationPaused; } }
        public static bool applicationFocused { get { return _instance.applicationFocused; } }
        public static bool applicationQuitting { get { return _instance != null && _instance.applicationQuitting; } }

        public static void Subscribe(EventsType type, Action action)
        {
            if (InitializationCheck())
            {
                _instance.Subscribe(type, action);
            }
        }

        public static void Unsubscribe(EventsType type, Action action)
        {
            if (InitializationCheck())
            {
                _instance.Unsubscribe(type, action);
            }
        }

        public static void ExecuteCoroutine(IEnumerator routine)
        {
            if (InitializationCheck())
            {
                _instance.ExecuteCoroutine(routine);
            }
        }

        public static void ExecuteCoroutine(Func<IEnumerator> routineFunc)
        {
            IEnumerator routine = routineFunc?.Invoke();
            ExecuteCoroutine(routine);
        }

        public static void CancelCoroutine(IEnumerator routine)
        {
            if (InitializationCheck())
            {
                _instance.CancelCoroutine(routine);
            }
        }

        public static void TerminateCoroutines()
        {
            if (InitializationCheck())
            {
                _instance.TerminateCoroutines();
            }
        }

        public static void DelayAndExecute(Action action, float delay = 0, int repeat = 1)
        {
            if (InitializationCheck())
            {
                _instance.DelayAndExecute(action, delay, repeat);
            }
        }

        public static void WaitUntilAndExecute(Action action, Func<bool> condition, int repeat = 1)
        {
            if (InitializationCheck())
            {
                _instance.WaitUntilAndExecute(action, condition, repeat);
            }
        }

        public static void ExecuteForPeriod(Action action, float period, float delay = 0)
        {
            if (InitializationCheck())
            {
                _instance.ExecuteForPeriod(action, period, delay);
            }
        }
    }
}
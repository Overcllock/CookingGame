using System;
using System.Collections;

namespace Game.Events
{
    public interface IEngineEventsManager
    {
        bool applicationFocused { get; }
        bool applicationPaused { get; }
        bool applicationQuitting { get; }

        void Subscribe(EventsType type, Action action);
        void Unsubscribe(EventsType type, Action action);

        void ExecuteCoroutine(IEnumerator routine);
        void CancelCoroutine(IEnumerator routine);
        void TerminateCoroutines();

        /// <summary>
        /// Wait for a period of time, then execute action.
        /// If delay is zero - then it will be executed on the next frame.
        /// If repeat is more than one - consequtive actions will be executed upon each upcoming frame.
        /// </summary>
        void DelayAndExecute(Action action, float delay = 0, int repeat = 1);

        /// <summary>
        /// Wait until the condition is met and then execute.
        /// If repeat is more than one - consequtive actions will be executed upon each upcoming frame.
        /// </summary>
        void WaitUntilAndExecute(Action action, Func<bool> condition, int repeat = 1);

        /// <summary>
        /// Execute action for some time, then stop.
        /// Has optional delay, if its zero - than it starts executing right away.
        /// </summary>
        void ExecuteForPeriod(Action action, float period, float delay = 0);
    }
}
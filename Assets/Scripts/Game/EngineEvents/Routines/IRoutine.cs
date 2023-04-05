using System;
using System.Collections;
using System.Threading.Tasks;

namespace Game.Events
{
    public interface IRoutine : IDisposable
    {
        bool isExecuting { get; }

        event Action<IRoutine> started;
        event Action<IRoutine> completed;

        void Start();
    }

    public abstract partial class Routine : IRoutine
    {
        public bool isExecuting { get; protected set; }

        public event Action<IRoutine> started;
        public event Action<IRoutine> completed;

        protected abstract void Body();

        public void Start()
        {
            isExecuting = true;
            started?.Invoke(this);
            Body();
        }

        protected virtual void Complete()
        {
            isExecuting = false;
            completed?.Invoke(this);
        }

        public virtual void Dispose()
        {
            isExecuting = false;
            started = null;
            completed = null;
        }
    }

    public abstract class AsyncRoutine : IRoutine
    {
        public bool isExecuting { get; protected set; }

        public event Action<IRoutine> started;
        public event Action<IRoutine> completed;

        protected abstract Task Body();

        public void Start()
        {
            AsyncExecution();
        }

        private async void AsyncExecution()
        {
            isExecuting = true;
            started?.Invoke(this);
            await Body();
            isExecuting = false;
            completed?.Invoke(this);
        }

        public void Dispose()
        {
            isExecuting = false;
            started = null;
            completed = null;
        }
    }

    public abstract class UnityRoutine : IRoutine
    {
        private IEnumerator _operation;

        public bool isExecuting { get; protected set; }

        public event Action<IRoutine> started;
        public event Action<IRoutine> completed;

        protected abstract IEnumerator Body();

        public void Start()
        {
            _operation = AsyncExecution();
            EngineEvents.ExecuteCoroutine(_operation);
        }

        private IEnumerator AsyncExecution()
        {
            isExecuting = true;
            started?.Invoke(this);
            yield return Body();
            isExecuting = false;
            completed?.Invoke(this);
            _operation = null;
        }

        public virtual void Dispose()
        {
            if (_operation != null)
            {
                EngineEvents.CancelCoroutine(_operation);
                _operation = null;
            }

            isExecuting = false;
            started = null;
            completed = null;
        }
    }

    public class SimpleUnityRoutine : UnityRoutine
    {
        private IEnumerator _cachedRoutine;

        public SimpleUnityRoutine(IEnumerator routine)
        {
            _cachedRoutine = routine;
        }

        protected override IEnumerator Body()
        {
            return _cachedRoutine;
        }

        public override void Dispose()
        {
            base.Dispose();
            _cachedRoutine = null;
        }
    }

    public class ActionRoutine : Routine
    {
        private Action _action;

        public ActionRoutine(Action action)
        {
            _action = action;
        }
        protected override void Body()
        {
            _action.Invoke();
            Complete();
        }

        public override void Dispose()
        {
            base.Dispose();
            _action = null;
        }
    }
}
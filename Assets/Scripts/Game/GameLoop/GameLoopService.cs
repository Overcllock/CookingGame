using System;
using Game.Events;
using Game.Utilites;

namespace Game
{
    public class GameLoopService : IDisposable
    {
        private StateMachine<GameMode> _fsm;

        private void AddState(GameLoopState state)
        {
            state.fsm = _fsm;
            _fsm.Add(state.GetMode(), state.OnEnter, state.OnUpdate, state.OnExit);
        }

        public GameLoopService()
        {
            _fsm = new StateMachine<GameMode>();
            
            AddState(new GameLoopStateTitleScreen());
            AddState(new GameLoopStateInGame());
            
            EngineEvents.Subscribe(EventsType.FixedUpdate, Tick);
        }

        private void Tick()
        {
            _fsm?.Update();
        }

        public bool TrySwitchTo(GameMode state)
        {
            if (_fsm.HasState() && _fsm.CurrentState() == state)
                return false;

            SwitchTo(state);
            return true;
        }

        private void SwitchTo(GameMode state)
        {
            _fsm.SwitchTo(state);
        }

        public void Dispose()
        {
            EngineEvents.Unsubscribe(EventsType.FixedUpdate, Tick);
            
            _fsm.Shutdown();
            _fsm = null;
        }
    }
}
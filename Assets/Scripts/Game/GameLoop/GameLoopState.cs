using Game.Utilites;

namespace Game
{
    public abstract class GameLoopState
    {
        public abstract GameMode GetMode();

        public virtual void OnEnter()
        {
        }

        public virtual void OnUpdate()
        {
        }

        public virtual void OnExit()
        {
        }

        public StateMachine<GameMode> fsm;
    }
}
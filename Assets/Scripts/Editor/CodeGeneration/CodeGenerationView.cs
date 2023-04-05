using System;

namespace Infrastructure.CodeGeneration
{
    public interface ICodeGenerationView
    {
        void Draw();
        void Initialize(CodeGenerationController controller);
    }

    public abstract class CodeGenerationView<T> : ICodeGenerationView where T : BaseCodeGenerator
    {
        protected Type type_;
        protected CodeGenerationController controller_;
        protected bool initialized_;

        public void Initialize(CodeGenerationController controller)
        {
            type_ = typeof(T);

            controller_ = controller;
            initialized_ = true;

            OnInitialize();
        }

        protected T GetGenerator()
        {
            if (controller_.TryGet(out T generator))
            {
                return generator;
            }

            UnityEngine.Debug.LogWarning($"Could not get generator type for view: [{typeof(T)}]");
            return default;
        }

        /// <summary>
        /// Call in OnGUI function.
        /// </summary>
        public void Draw()
        {
            if (!initialized_) return;
            OnDraw();
        }

        protected virtual void OnInitialize()
        {
        }
        protected abstract void OnDraw();
    }
}
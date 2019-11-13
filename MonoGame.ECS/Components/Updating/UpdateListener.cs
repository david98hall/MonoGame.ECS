using Microsoft.Xna.Framework;

namespace MonoGame.ECS.Components.Updating
{
    public class UpdateListener
    {

        public event UpdateAction OnUpdate;

        public delegate void UpdateAction(GameTime gameTime);

        internal void RegisterUpdate(GameTime gameTime)
        {
            OnUpdate?.Invoke(gameTime);
        }

    }
}

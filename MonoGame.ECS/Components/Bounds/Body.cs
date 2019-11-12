using Microsoft.Xna.Framework;

namespace MonoGame.ECS.Components.Bounds
{
    public abstract class Body
    {
        public virtual float MaxWidth { get; protected set; }

        public virtual float MaxHeight { get; protected set; }

        public float Area { get; protected set; }

        public Vector2 RelativePosition
        {
            get => relativePosition;
            set
            {
                relativePosition = value;
                UpdateRelaviteBounds();
            }
        }
        private Vector2 relativePosition;

        public (float Left, float Top, float Right, float Bottom) RelativeBounds { get; private set; }

        public Rectangle RelativeRectangleBounds { get; private set; }

        protected void UpdateRelaviteBounds()
        {
            RelativeBounds = (RelativePosition.X,
                RelativePosition.Y,
                RelativePosition.X + MaxWidth,
                RelativePosition.Y + MaxHeight);
            RelativeRectangleBounds = new Rectangle((int)RelativeBounds.Left, (int)RelativeBounds.Top, (int)MaxWidth, (int)MaxHeight);
        }

        public abstract bool IsPointWithin(Vector2 point);

    }
}

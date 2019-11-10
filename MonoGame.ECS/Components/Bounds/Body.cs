using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using MonoGame.Utils.Extensions;
using System.Collections.Generic;

namespace MonoGame.ECS.Components.Bounds
{
    public class Body
    {
        // Shape
        public Polygon Shape
        {
            get => shape;
            set
            {
                shape = value;

                // Update properties
                MaxWidth = shape.Right - shape.Left;
                MaxHeight = shape.Bottom - shape.Top;
                Area = shape.GetArea();
                UpdateRelaviteBounds();
            }
        }
        private Polygon shape;

        public float MaxWidth { get; private set; }

        public float MaxHeight { get; private set; }

        public float Area { get; private set; }

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

        public Body(IEnumerable<Vector2> vertices)
        {
            Shape = new Polygon(vertices);
            RelativePosition = new Vector2(0, 0);
        }

        public Body(RectangleF rectangle)
        {
            Shape = new Polygon(rectangle.GetCorners());
            RelativePosition = new Vector2(rectangle.X, rectangle.Y);
        }

        private void UpdateRelaviteBounds()
        {
            RelativeBounds = (RelativePosition.X,
                RelativePosition.Y,
                RelativePosition.X + shape.Right,
                RelativePosition.Y + shape.Bottom);
        }

    }
}

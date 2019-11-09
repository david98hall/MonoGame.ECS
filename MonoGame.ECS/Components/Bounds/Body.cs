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
                Area = shape.GetArea();
                RelativeBounds = (RelativePosition.X,
                    RelativePosition.Y,
                    RelativePosition.X + shape.Right,
                    RelativePosition.Y + shape.Bottom);
            }
        }
        private Polygon shape;

        public float MaxWidth => shape.Right - shape.Left;

        public float MaxHeight => shape.Bottom - shape.Top;

        public float Area { get; private set; }

        public Vector2 RelativePosition { get; set; }

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

    }
}

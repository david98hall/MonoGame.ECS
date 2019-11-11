using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using MonoGame.Utils.Extensions;
using MonoGame.Utils.Geometry;
using System.Collections.Generic;

namespace MonoGame.ECS.Components.Bounds
{
    public class PolygonBody : Body
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

        public PolygonBody(IEnumerable<Vector2> vertices)
        {
            Init(vertices, new Vector2(0, 0));
        }

        public PolygonBody(RectangleF rectangle)
        {
            Init(rectangle.GetCorners(), new Vector2(rectangle.X, rectangle.Y));
        }

        private void Init(IEnumerable<Vector2> vertices, Vector2 relativePosition)
        {
            Shape = new Polygon(vertices);
            RelativePosition = relativePosition;
        }

        public override bool IsPointWithin(Vector2 point)
        {
            return GeometryUtils.IsWithinPolygon(point, Shape.Vertices, RelativePosition);
        }
    }
}

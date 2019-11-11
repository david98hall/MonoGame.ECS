using Microsoft.Xna.Framework;
using MonoGame.ECS.Components.Bounds;
using MonoGame.Extended;
using System.Collections.Generic;

namespace MonoGame.ECS.Components.Input.Pointer
{
    public class PointerInputBody
    {

        public Body Body { get; set; }

        public PointerInputBody(IEnumerable<Vector2> vertices) : this(new PolygonBody(vertices))
        {
        }

        public PointerInputBody(RectangleF rectangle) : this(new PolygonBody(rectangle))
        {
        }

        public PointerInputBody(float horizontalRadius, float verticalRadius)
            : this(new EllipticBody(horizontalRadius, verticalRadius))
        {
        }

        public PointerInputBody(float radius) : this(new EllipticBody(radius))
        {
        }

        public PointerInputBody(Body body)
        {
            Body = body;
        }

    }
}

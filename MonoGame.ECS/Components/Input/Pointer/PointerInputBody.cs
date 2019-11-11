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

        public PointerInputBody(float horizontalRadius, float verticalRadius, float scale = 1)
            : this(new EllipticBody(horizontalRadius, verticalRadius, scale))
        {
        }

        public PointerInputBody(float radius, float scale = 1) : this(new EllipticBody(radius, scale))
        {
        }

        public PointerInputBody(Body body)
        {
            Body = body;
        }

    }
}

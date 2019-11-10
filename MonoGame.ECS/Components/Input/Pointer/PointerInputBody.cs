using Microsoft.Xna.Framework;
using MonoGame.ECS.Components.Bounds;
using MonoGame.Extended;
using System.Collections.Generic;

namespace MonoGame.ECS.Components.Input.Pointer
{
    public class PointerInputBody
    {

        public Body Body { get; set; }

        public PointerInputBody(IEnumerable<Vector2> vertices)
        {
            Body = new Body(vertices);
        }

        public PointerInputBody(RectangleF rectangle)
        {
            Body = new Body(rectangle);
        }

        public PointerInputBody(Body body)
        {
            Body = body;
        }

    }
}

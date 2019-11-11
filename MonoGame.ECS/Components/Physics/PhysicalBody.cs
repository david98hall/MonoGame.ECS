using Microsoft.Xna.Framework;
using MonoGame.ECS.Components.Bounds;
using MonoGame.Extended;
using System.Collections.Generic;

namespace MonoGame.ECS.Components.Physics
{
    public class PhysicalBody
    {

        // Mass
        public float Mass { get; private set; }

        public float AreaDensity { get; set; }

        // TODO public (float X, float Y) MassCenter { get; private set; }

        public Body Body
        {
            get => body;
            set
            {
                body = value;
                Mass = body.Area * AreaDensity;
                // TODO Calculate mass center
            }
        }
        private Body body;

        public PhysicalBody(Body body, float areaDensity = 1)
        {
            AreaDensity = areaDensity;
            Body = body;
        }

        public PhysicalBody(IEnumerable<Vector2> vertices, float areaDensity = 1)
            : this(new PolygonBody(vertices), areaDensity)
        {
        }

        public PhysicalBody(RectangleF rectangle, float areaDensity = 1)
            : this(new PolygonBody(rectangle), areaDensity)
        {
        }

        public PhysicalBody(float horizontalRadius, float verticalRadius, float areaDensity = 1)
            : this(new EllipticBody(horizontalRadius, verticalRadius), areaDensity)
        {
        }

        public PhysicalBody(float radius, float areaDensity = 1)
            : this(new EllipticBody(radius), areaDensity)
        {
        }

    }
}

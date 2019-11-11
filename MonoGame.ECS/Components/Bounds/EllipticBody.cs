using System;
using Microsoft.Xna.Framework;
using MonoGame.Utils.Geometry;

namespace MonoGame.ECS.Components.Bounds
{
    public class EllipticBody : Body
    {
        public override float MaxWidth
        {
            protected set
            {
                base.MaxWidth = value;
                horizontalRadius = value / 2;
                UpdateArea();
            }
        }

        public override float MaxHeight
        {
            protected set
            {
                base.MaxHeight = value;
                verticalRadius = value / 2;
                UpdateArea();
            }
        }

        public float HorizontalRadius
        {
            get => horizontalRadius;
            set
            {
                horizontalRadius = value;
                base.MaxWidth = value * 2;
                UpdateArea();
            }
        }
        private float horizontalRadius;

        public float VerticalRadius
        {
            get => verticalRadius;
            set
            {
                verticalRadius = value;
                base.MaxHeight = value * 2;
                UpdateArea();
            }
        }
        private float verticalRadius;

        public EllipticBody(float maxWidth, float maxHeight)
        {
            MaxWidth = maxWidth;
            MaxHeight = maxHeight;
        }

        public EllipticBody(float radius) : this(radius, radius)
        {
        }

        private void UpdateArea()
        {
            Area = (float)Math.PI * horizontalRadius * verticalRadius;
        }

        public override bool IsPointWithin(Vector2 point)
        {
            return GeometryUtils.IsWithinEllipse(
                point,
                horizontalRadius,
                verticalRadius,
                RelativePosition);
        }

    }
}

using System;
using Microsoft.Xna.Framework;
using MonoGame.Utils.Geometry;

namespace MonoGame.ECS.Components.Bounds
{
    public class EllipticBody : Body
    {
        public float HorizontalRadius
        {
            get => horizontalRadius;
            set
            {
                horizontalRadius = value;
                UpdateValues();
            }
        }
        private float horizontalRadius;

        public float VerticalRadius
        {
            get => verticalRadius;
            set
            {
                verticalRadius = value;
                UpdateValues();
            }
        }
        private float verticalRadius;

        public EllipticBody(float horizontalRadius, float verticalRadius)
        {
            this.horizontalRadius = horizontalRadius;
            this.verticalRadius = verticalRadius;
            UpdateValues();
        }

        public EllipticBody(float radius) : this(radius, radius)
        {
        }

        private void UpdateValues()
        {
            MaxWidth = 2 * horizontalRadius;
            MaxHeight = 2 * verticalRadius;
            UpdateArea();
            UpdateRelaviteBounds();
        }

        private void UpdateArea()
        {
            Area = (float)Math.PI * HorizontalRadius * VerticalRadius;
        }

        public override bool IsPointWithin(Vector2 point)
        {
            return GeometryUtils.IsWithinEllipse(
                point,
                HorizontalRadius,
                VerticalRadius,
                RelativePosition);
        }

    }
}

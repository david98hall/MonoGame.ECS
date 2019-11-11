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

        public float Scale
        {
            get => scale;
            set
            {
                scale = value;
                UpdateValues();
            }
        }
        private float scale;

        public EllipticBody(float horizontalRadius, float verticalRadius, float scale = 1)
        {
            this.horizontalRadius = horizontalRadius;
            this.verticalRadius = verticalRadius;
            this.scale = scale;
            UpdateValues();
        }

        public EllipticBody(float radius, float scale = 1) : this(radius, radius, scale)
        {
        }

        private void UpdateValues()
        {
            MaxWidth = horizontalRadius * Scale;
            MaxHeight = verticalRadius * Scale;
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
                Scale,
                RelativePosition);
        }

    }
}

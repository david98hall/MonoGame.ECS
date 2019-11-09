using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.ECS.Components.Input.Pointer;
using System;

namespace MonoGame.ECS.Systems.Input.Pointer
{
    public abstract class PointerInputSystem : EntitySystem, IUpdateSystem, ISystem, IDisposable
    {
        // Component mappers        
        protected ComponentMapper<PointerInputBody> bodyMapper;
        protected ComponentMapper<PointerInput> pointerInputMapper;
        protected ComponentMapper<Transform2> transformMapper;

        public ViewportAdapter ViewportAdapter { get; set; }

        public PointerInputSystem(ViewportAdapter viewportAdapter)
            : base(Aspect.All(typeof(PointerInputBody), typeof(PointerInput), typeof(Transform2)))
        {
            ViewportAdapter = viewportAdapter;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            bodyMapper = mapperService.GetMapper<PointerInputBody>();
            pointerInputMapper = mapperService.GetMapper<PointerInput>();
            transformMapper = mapperService.GetMapper<Transform2>();
        }

        public abstract void Update(GameTime gameTime);

        #region Is the pointer location within the entity?
        // Reference: https://www.geeksforgeeks.org/how-to-check-if-a-given-point-lies-inside-a-polygon/

        protected bool IsWithinEntity(int entity, Vector2 pointerLocation)
        {
            var body = bodyMapper.Get(entity).Body;
            var transform = transformMapper.Get(entity);

            var vertices = body.Shape.Vertices;
            var vertexCount = vertices.Length;

            // There must be at least 3 vertices in the body's shape
            if (vertexCount < 3)
            {
                return false;
            }

            // The body's bounds
            var (BodyLeft, BodyTop, BodyRight, BodyBottom) = body.RelativeBounds;
            BodyLeft += transform.Position.X;
            BodyRight += transform.Position.X;
            BodyTop += transform.Position.Y;
            BodyBottom += transform.Position.Y;
            var bodyPosition = body.RelativePosition + transform.Position;

            var inHorizontalBounds = BodyLeft <= pointerLocation.X && pointerLocation.X <= BodyRight;
            var inVerticalBounds = BodyTop <= pointerLocation.Y && pointerLocation.Y <= BodyBottom;
            if (inHorizontalBounds && inVerticalBounds)
            {
                // Ray casting:

                // Create a point for line segment from p to infinite 
                var extreme = new Vector2(int.MaxValue, pointerLocation.Y);

                // Count intersections of the above line with sides of polygon 
                int count = 0;
                int i = 0;
                do
                {
                    int next = (i + 1) % vertexCount;

                    var currentVertex = vertices[i] + bodyPosition;
                    var nextVertex = vertices[next] + bodyPosition;

                    // Check if the line segment from 'p' to 'extreme' intersects 
                    // with the line segment from 'polygon[i]' to 'polygon[next]' 
                    if (Intersects(currentVertex, nextVertex, pointerLocation, extreme))
                    {
                        // If the point 'p' is colinear with line segment 'i-next', 
                        // then check if it lies on segment. If it lies, return true, 
                        // otherwise false 
                        if (GetOrientation(currentVertex, pointerLocation, nextVertex) == 0)
                        {
                            return OnSegment(currentVertex, pointerLocation, nextVertex);
                        }

                        count++;
                    }

                    i = next;

                } while (i != 0);

                // Return true if count is odd, false otherwise 
                return count % 2 == 1;
            }

            return false;
        }

        // Given three colinear points p, q, r, the function checks if 
        // point q lies on line segment 'pr' 
        private bool OnSegment(Vector2 p, Vector2 q, Vector2 r)
        {
            return q.X <= Math.Max(p.X, r.X) && q.X >= Math.Min(p.X, r.X)
                && q.Y <= Math.Max(p.Y, r.Y) && q.Y >= Math.Min(p.Y, r.Y);
        }

        // To find orientation of ordered triplet (p, q, r). 
        // The function returns following values 
        // 0 --> p, q and r are colinear 
        // 1 --> Clockwise 
        // 2 --> Counterclockwise 
        private int GetOrientation(Vector2 p, Vector2 q, Vector2 r)
        {
            float val = (q.Y - p.Y) * (r.X - q.X) -
                    (q.X - p.X) * (r.Y - q.Y);

            if (val == 0) return 0; // colinear 
            return (val > 0) ? 1 : 2; // clock or counterclock wise 
        }

        // The function that returns true if line segment 'p1q1' 
        // and 'p2q2' intersect. 
        private bool Intersects(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2)
        {
            // Find the four orientations needed for general and 
            // special cases 
            int o1 = GetOrientation(p1, q1, p2);
            int o2 = GetOrientation(p1, q1, q2);
            int o3 = GetOrientation(p2, q2, p1);
            int o4 = GetOrientation(p2, q2, q1);

            return o1 != o2 && o3 != o4 // General case 
                                        // p1, q1 and p2 are colinear and p2 lies on segment p1q1 
                || o1 == 0 && OnSegment(p1, p2, q1)
                // p1, q1 and p2 are colinear and q2 lies on segment p1q1 
                || o2 == 0 && OnSegment(p1, q2, q1)
                // p2, q2 and p1 are colinear and p1 lies on segment p2q2 
                || o3 == 0 && OnSegment(p2, p1, q2)
                // p2, q2 and q1 are colinear and q1 lies on segment p2q2 
                || o4 == 0 && OnSegment(p2, q1, q2);
        }

        #endregion
    }

}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.ECS.Components.Input.Pointer;

namespace MonoGame.ECS.Systems.Input.Pointer
{

    internal class TouchInputSystem : PointerInputSystem
    {

        internal TouchInputSystem(ViewportAdapter viewportAdapter) : base(viewportAdapter)
        {
        }

        public override void Update(GameTime gameTime)
        {
            var touchCollection = TouchPanel.GetState();

            foreach (var touchLocation in touchCollection)
            {
                foreach (var entity in ActiveEntities)
                {
                    // Entity components                    
                    var pointerInput = pointerInputMapper.Get(entity);

                    // Pointer arguments
                    var pointerArgs = new PointerEventArgs(ViewportAdapter, gameTime.TotalGameTime, touchLocation);

                    // Analyze the input type and act accordingly
                    if (IsWithinEntity(entity, GetTouchPoint(touchLocation)))
                    {
                        switch (touchLocation.State)
                        {
                            case TouchLocationState.Pressed:
                                pointerInput.RegisterInputStart(pointerArgs);
                                break;
                            case TouchLocationState.Moved:
                                pointerInput.RegisterInputMove(pointerArgs);
                                break;
                            case TouchLocationState.Released:
                                pointerInput.RegisterInputEndInBounds(pointerArgs);
                                break;
                            case TouchLocationState.Invalid:
                                pointerInput.RegisterCancelledInput(pointerArgs);
                                break;
                        }
                    }
                    else if (pointerInput.IsDown && (touchLocation.State == TouchLocationState.Released
                            || touchLocation.State == TouchLocationState.Invalid))
                    {
                        // The previously down pointer was released outside of the bounds of the entity
                        pointerInput.RegisterInputEndOutsideBounds(pointerArgs);
                    }
                }
            }
        }

        private Vector2 GetTouchPoint(TouchLocation touchLocation)
        {
            var point = ViewportAdapter?.PointToScreen((int)touchLocation.Position.X, (int)touchLocation.Position.Y)
                ?? touchLocation.Position.ToPoint();
            return new Vector2(point.X, point.Y);
        }

    }
}

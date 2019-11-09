using MonoGame.Extended.ViewportAdapters;

namespace MonoGame.ECS.Systems.Input.Pointer
{
    public static class PointerInputSystemFactory
    {

        public static PointerInputSystem CreateTouchInputSystem(ViewportAdapter viewportAdapter)
        {
            return new TouchInputSystem(viewportAdapter);
        }

    }
}

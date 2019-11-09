using Microsoft.Xna.Framework.Input.Touch;
using MonoGame.Extended.ViewportAdapters;
using System;

namespace MonoGame.ECS.Components.Input.Pointer
{
    public class PointerEventArgs
    {

        public ViewportAdapter ViewportAdapter { get; }
        public TouchLocation RawTouchLocation { get; }
        public TimeSpan Time { get; }
        public (int X, int Y) Position { get; }

        public PointerEventArgs(ViewportAdapter viewportAdapter, TimeSpan time, TouchLocation location)
        {
            ViewportAdapter = viewportAdapter;
            RawTouchLocation = location;
            Time = time;
            var point = viewportAdapter?.PointToScreen((int)location.Position.X, (int)location.Position.Y)
                ?? location.Position.ToPoint();
            Position = (point.X, point.Y);
        }

    }
}

using System;
using System.Collections.Generic;

namespace MonoGame.ECS.Components.Input.Pointer
{
    /// <summary>
    /// A component that represents any kind of pointer input regarding an entity.
    /// </summary>
    public class PointerInput
    {
        // Events
        public event EventHandler<PointerEventArgs> OnStart;
        public event EventHandler<PointerEventArgs> OnEndInBounds;
        public event EventHandler<PointerEventArgs> OnEndOutsideBounds;
        public event EventHandler<PointerEventArgs> OnMove;
        public event EventHandler<PointerEventArgs> OnCancel;

        /// <summary>
        /// If at least one pointer is down within the entity in question.
        /// </summary>
        public bool IsDown => downPointers.Count > 0;

        /// <summary>
        /// True if this counts a moving pointer as this being down,
        /// even though it for example does not start within the entity
        /// in question.
        /// </summary>
        public bool CountMovingAsDown { get; set; }

        // The key is the pointer id and the value is if it's down or not
        private readonly HashSet<int> downPointers;

        /// <summary>
        /// Initializes and sets values.
        /// </summary>
        /// <param name="countMovingAsDown">
        /// True if this counts a moving pointer as this being down, 
        /// even though it for example does not start within the entity in question.
        /// </param>
        public PointerInput(bool countMovingAsDown = false)
        {
            CountMovingAsDown = countMovingAsDown;
            downPointers = new HashSet<int>();
        }

        #region Register events
        public void RegisterInputStart(PointerEventArgs args)
        {
            downPointers.Add(args.RawTouchLocation.Id);
            OnStart?.Invoke(this, args);
        }

        public void RegisterInputMove(PointerEventArgs args)
        {
            if (CountMovingAsDown)
            {
                downPointers.Add(args.RawTouchLocation.Id);
            }
            OnMove?.Invoke(this, args);
        }

        public void RegisterInputEndInBounds(PointerEventArgs args)
        {
            RegisterReleasedInput(args, OnEndInBounds);
        }

        public void RegisterInputEndOutsideBounds(PointerEventArgs args)
        {
            RegisterStoppedInput(args, OnEndOutsideBounds);
        }

        public void RegisterCancelledInput(PointerEventArgs args)
        {
            RegisterStoppedInput(args, OnCancel);
        }
        #endregion

        #region Helper methods
        private void RegisterStoppedInput(PointerEventArgs args, EventHandler<PointerEventArgs> eventHandler)
        {
            if (downPointers.Contains(args.RawTouchLocation.Id))
            {
                // Only input started in the entity in question will affect this PointerInput
                RegisterReleasedInput(args, eventHandler);
            }
        }

        private void RegisterReleasedInput(PointerEventArgs args, EventHandler<PointerEventArgs> eventHandler)
        {
            if (IsDown)
            {
                downPointers.Remove(args.RawTouchLocation.Id);
                if (!IsDown)
                {
                    // Only register a released input if there aren't any others 
                    // that are within the bounds of the entity in question
                    eventHandler?.Invoke(this, args);
                }
            }
        }
        #endregion

    }
}

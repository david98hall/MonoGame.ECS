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
        public event EventHandler<PointerEventArgs> OnCancelInBounds;
        public event EventHandler<PointerEventArgs> OnCancelOutsideBounds;

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

        /// <summary>
        /// True if all start inputs (e.g. a touch press) should invoke the
        /// corresponding event (OnStart). False if the OnStart event
        /// should only be invoked if there is no other active pointer that is down.
        /// </summary>
        public bool InvokeOnAllStartInputs { get; set; }

        /// <summary>
        /// Only relevant for pointer input started within the enity in question.
        /// True if all start inputs (e.g. a touch release) should invoke the
        /// corresponding events. False if end input events should only be invoked
        /// when there is no other active pointers down.
        /// </summary>
        public bool InvokeOnAllEndInputsInBounds { get; set; }

        // The key is the pointer id and the value is if it's down or not
        private readonly HashSet<int> downPointers;

        /// <summary>
        /// Initializes and sets values.
        /// </summary>
        /// <param name="countMovingAsDown">
        /// True if this counts a moving pointer as this being down, 
        /// even though it for example does not start within the entity in question.
        /// </param>
        /// <param name="invokeOnAllStartInputs">
        /// True if all start inputs (e.g. a touch press) should invoke the
        /// corresponding event (OnStart). False if the OnStart event
        /// should only be invoked if there is no other active pointer that is down.
        /// </param>
        /// <param name="invokeOnAllEndInputsInBounds">
        /// True if all start inputs (e.g. a touch release) should invoke the
        /// corresponding events. False if end input events should only be invoked
        /// when there is no other active pointers down.
        /// </param>
        public PointerInput(
            bool countMovingAsDown = false,
            bool invokeOnAllStartInputs = false,
            bool invokeOnAllEndInputsInBounds = false)
        {
            CountMovingAsDown = countMovingAsDown;
            InvokeOnAllStartInputs = invokeOnAllStartInputs;
            InvokeOnAllEndInputsInBounds = invokeOnAllEndInputsInBounds;
            downPointers = new HashSet<int>();
        }

        #region Register events
        internal void RegisterInputStart(PointerEventArgs args)
        {
            var shouldInvoke = InvokeOnAllStartInputs || !IsDown;
            downPointers.Add(args.RawTouchLocation.Id);
            if (shouldInvoke)
            {
                OnStart?.Invoke(this, args);
            }
        }

        internal void RegisterInputMove(PointerEventArgs args)
        {
            if (CountMovingAsDown)
            {
                downPointers.Add(args.RawTouchLocation.Id);
            }
            OnMove?.Invoke(this, args);
        }

        internal void RegisterInputEndInBounds(PointerEventArgs args)
        {
            RegisterReleasedInput(args, OnEndInBounds);
        }

        internal void RegisterInputEndOutsideBounds(PointerEventArgs args)
        {
            RegisterStoppedInput(args, OnEndOutsideBounds);
        }

        internal void RegisterCancelledInputInBounds(PointerEventArgs args)
        {
            RegisterStoppedInput(args, OnCancelInBounds);
        }

        internal void RegisterCancelledInputOutsideBounds(PointerEventArgs args)
        {
            RegisterStoppedInput(args, OnCancelOutsideBounds);
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
                if (!IsDown || InvokeOnAllEndInputsInBounds)
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

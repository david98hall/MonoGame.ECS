using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MonoGame.ECS.Worlds
{
    /// <summary>
    /// A singleton tool for navigating between worlds
    /// </summary>
    /// <typeparam name="T">E.g. an enum representing world identifications.</typeparam>
    public class WorldNavigator<T> where T : struct, IConvertible
    {
        // The only instance of this class
        private static WorldNavigator<T> instance;

        #region Navigation history
        /// <summary>
        /// True if there is any back history.
        /// </summary>
        public bool CanNavigateBack => backHistory.Count > 0;

        /// <summary>
        /// True if there is any forward "history".
        /// </summary>
        public bool CanNavigateForward => forwardHistory.Count > 0;

        /// <summary>
        /// True if there has been at least one previous navigation.
        /// </summary>
        public bool HasNavigated => visitedWorlds.Count > 0;

        private readonly Dictionary<T, World> visitedWorlds;

        private readonly Stack<T> backHistory;

        private readonly Stack<T> forwardHistory;
        #endregion

        #region World creation
        public CreateWorld CreateWorldAction { get; set; }

        public delegate World CreateWorld(T worldType);
        #endregion

        #region Current world
        /// <summary>
        /// The id of the current world (most recently navigated to).
        /// </summary>
        public T CurrentWorldId { get; private set; }

        /// <summary>
        /// The current world (most recently navigated to).
        /// </summary>
        public World CurrentWorld => visitedWorlds[CurrentWorldId];
        #endregion

        #region Events
        public event NavigationHandler OnNavigation;
        public event NavigationHandler OnBackNavigation;
        public event NavigationHandler OnForwardNavigation;
        public delegate void NavigationHandler(T worldId);
        #endregion

        private WorldNavigator()
        {
            visitedWorlds = new Dictionary<T, World>();
            backHistory = new Stack<T>();
            forwardHistory = new Stack<T>();
        }

        /// <summary>
        /// Gets the only instance of WorldNavigator.
        /// </summary>
        /// <returns>The instance.</returns>
        public static WorldNavigator<T> GetInstance()
        {
            if (instance == null)
            {
                instance = new WorldNavigator<T>();
            }

            return instance;
        }

        #region Navigation
        public void NavigateTo(T worldId, bool tryFindOld = false)
        {
            // Manage navigation history
            if (HasNavigated)
            {
                backHistory.Push(CurrentWorldId);
            }
            forwardHistory.Clear();

            UpdateCurrentWorld(worldId, tryFindOld);
        }

        public async void NavigateToAsync(T worldId, bool tryFindOld = false)
        {
            await Task.Run(() => NavigateTo(worldId, tryFindOld));
        }

        public void NavigateBack(bool tryFindOld = true)
        {
            forwardHistory.Push(CurrentWorldId);
            UpdateCurrentWorld(backHistory.Pop(), tryFindOld);
            OnBackNavigation?.Invoke(CurrentWorldId);
        }

        public async void NavigateBackAsync(bool tryFindOld = true)
        {
            await Task.Run(() => NavigateBack(tryFindOld));
        }

        public void NavigateForward(bool tryFindOld = true)
        {
            backHistory.Push(CurrentWorldId);
            UpdateCurrentWorld(forwardHistory.Pop(), tryFindOld);
            OnForwardNavigation?.Invoke(CurrentWorldId);
        }

        public async void NavigateForwardAsync(bool tryFindOld = true)
        {
            await Task.Run(() => NavigateForward(tryFindOld));
        }

        #endregion

        #region Clearing and resetting

        /// <summary>
        /// Clears all history.
        /// </summary>
        public void Reset()
        {
            visitedWorlds.Clear();
            ClearHistory();
        }

        public void ClearHistory()
        {
            ClearBackHistory();
            ClearForwardHistory();
        }

        public void ClearBackHistory() => backHistory.Clear();

        public void ClearForwardHistory() => forwardHistory.Clear();
        #endregion

        #region Helper methods
        private void UpdateCurrentWorld(T worldId, bool tryFindOld)
        {
            CreateWorldActionNullCheck();

            // Get a world with the passed identification
            var world = tryFindOld && visitedWorlds.ContainsKey(worldId)
                ? visitedWorlds[worldId]
                : CreateWorldAction(worldId);

            if (!tryFindOld && !visitedWorlds.ContainsKey(worldId))
            {
                // If chosen to create a new world, also add it to the 
                // dictionary of all visited worlds
                visitedWorlds.Add(worldId, world);
            }
            else if (!tryFindOld && visitedWorlds.ContainsKey(worldId))
            {
                visitedWorlds[worldId] = world;
            }

            // Update current world
            CurrentWorldId = worldId;

            // Notify listeners
            OnNavigation?.Invoke(CurrentWorldId);
        }

        private void CreateWorldActionNullCheck()
        {
            if (CreateWorldAction == null)
            {
                throw new Exception("The create world action (CreateWorldAction) is null!");
            }
        }
        #endregion

    }
}

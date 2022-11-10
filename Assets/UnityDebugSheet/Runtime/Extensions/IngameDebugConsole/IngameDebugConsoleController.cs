#if !EXCLUDE_UNITY_DEBUG_SHEET
using System;
using IngameDebugConsole;

namespace UnityDebugSheet.Runtime.Extensions.InGameDebugConsole
{
    /// <summary>
    ///     Controls the <see cref="IngameDebugConsole" />, which is used to display debug logs.
    /// </summary>
    public sealed class IngameDebugConsoleController : IDisposable
    {
        public delegate void StateChangedDelegate(State state, bool changedByController);

        public enum State
        {
            Closed,
            Minimized,
            Open
        }

        private readonly DebugLogManager _debugLogManager;

        private bool _isChangingState;

        public IngameDebugConsoleController(DebugLogManager debugLogManager)
        {
            _debugLogManager = debugLogManager;
            _debugLogManager.OnLogWindowShown += OnLogWindowShown;
            _debugLogManager.OnLogWindowHidden += OnLogWindowHidden;
        }

        public void Dispose()
        {
            _debugLogManager.OnLogWindowShown -= OnLogWindowShown;
            _debugLogManager.OnLogWindowHidden -= OnLogWindowHidden;
        }

        public event StateChangedDelegate StateChanged;

        private void OnLogWindowShown()
        {
            StateChanged?.Invoke(State.Open, _isChangingState);
        }

        private void OnLogWindowHidden()
        {
            if (_debugLogManager.PopupEnabled)
                StateChanged?.Invoke(State.Minimized, _isChangingState);
            else
                StateChanged?.Invoke(State.Closed, _isChangingState);
        }

        public void SetState(State state)
        {
            if (GetState() == state)
                return;

            switch (state)
            {
                case State.Closed:
                    _isChangingState = true;
                    _debugLogManager.PopupEnabled = false;
                    _debugLogManager.HideLogWindow();
                    _isChangingState = false;
                    break;
                case State.Minimized:
                    _isChangingState = true;
                    _debugLogManager.PopupEnabled = true;
                    _debugLogManager.HideLogWindow();
                    _isChangingState = false;
                    break;
                case State.Open:
                    _isChangingState = true;
                    _debugLogManager.ShowLogWindow();
                    _isChangingState = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        public State GetState()
        {
            if (_debugLogManager.IsLogWindowVisible)
                return State.Open;

            if (_debugLogManager.PopupEnabled)
                return State.Minimized;

            return State.Closed;
        }
    }
}
#endif

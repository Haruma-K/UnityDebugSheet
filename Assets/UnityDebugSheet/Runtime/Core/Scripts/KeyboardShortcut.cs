using System;
using UnityDebugSheet.Runtime.Foundation.Gestures.InputAdapters;
using UnityEngine;

namespace UnityDebugSheet.Runtime.Core.Scripts
{
    [Serializable]
    public sealed class KeyboardShortcut
    {
        [SerializeField] private bool _enabled = true;

        [SerializeField] [Tooltip("Windows: Control / Mac: Command")]
        private bool _control = true;

        [SerializeField] [Tooltip("Windows: Alt / Mac: Option")]
        private bool _alt;

        [SerializeField] [Tooltip("Windows: Shift / Mac: Shift")]
        private bool _shift = true;

        [SerializeField] private KeyCode _key = KeyCode.D;

        public bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        public bool Control
        {
            get => _control;
            set => _control = value;
        }

        public bool Alt
        {
            get => _alt;
            set => _alt = value;
        }

        public bool Shift
        {
            get => _shift;
            set => _shift = value;
        }

        public KeyCode Key
        {
            get => _key;
            set => _key = value;
        }

        public bool Evaluate()
        {
            if (!_enabled)
                return false;

            if (_control && !GetControlKey())
                return false;

            if (_alt && !InputAdapter.GetKey(KeyCode.LeftAlt) && !InputAdapter.GetKey(KeyCode.RightAlt))
                return false;

            if (_shift && !InputAdapter.GetKey(KeyCode.LeftShift) && !InputAdapter.GetKey(KeyCode.RightShift))
                return false;

            return InputAdapter.GetKeyDown(_key);
        }

        private bool GetControlKey()
        {
#if UNITY_EDITOR_WIN || UNITY_EDITOR_LINUX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
            return InputAdapter.GetKey(KeyCode.LeftControl) || InputAdapter.GetKey(KeyCode.RightControl);
#else
            return InputAdapter.GetKey(KeyCode.LeftCommand) || InputAdapter.GetKey(KeyCode.RightCommand);
#endif
        }
    }
}

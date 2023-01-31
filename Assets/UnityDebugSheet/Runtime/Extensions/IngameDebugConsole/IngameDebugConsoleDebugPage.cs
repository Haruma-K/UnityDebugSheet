using System;
using System.Collections;
using IngameDebugConsole;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells;
using UnityDebugSheet.Runtime.Extensions.InGameDebugConsole;
using UnityEngine;
#if UDS_USE_ASYNC_METHODS
using System.Threading.Tasks;
#endif

namespace UnityDebugSheet.Runtime.Extensions.IngameDebugConsole
{
    public class IngameDebugConsoleDebugPage : DefaultDebugPageBase
    {
        protected override string Title => "In-Game Debug Console";

        private DebugLogManager _debugLogManager;
        private IngameDebugConsoleController _ingameDebugConsoleController;
        private EnumPickerCellModel _consoleStatePickerModel;

        private bool _didSetup;

        /// <summary>
        ///     Set up with the specified <see cref="DebugLogManager" />.
        /// </summary>
        /// <param name="debugLogManager"></param>
        public void Setup(DebugLogManager debugLogManager)
        {
            _debugLogManager = debugLogManager;
            _ingameDebugConsoleController = new IngameDebugConsoleController(debugLogManager);
            _didSetup = true;
        }

#if UDS_USE_ASYNC_METHODS
        public override Task Initialize()
#else
        public override IEnumerator Initialize()
#endif
        {
            if (!_didSetup)
                throw new Exception(
                    $"{typeof(IngameDebugConsoleDebugPage)} is not set up. Call {nameof(Setup)}() first.");

            // Display State
            var consoleState = _ingameDebugConsoleController.GetState();
            var consoleStatePickerModel = new EnumPickerCellModel(consoleState);
            _consoleStatePickerModel = consoleStatePickerModel;
            consoleStatePickerModel.Text = "Display State";
            consoleStatePickerModel.ActiveValueChanged += OnConsoleStatePickerValueChanged;
            _ingameDebugConsoleController.StateChanged += OnIngameDebugConsoleStateChanged;
            AddEnumPicker(consoleStatePickerModel);

            // Copy All Logs
            AddButton("Copy All Logs to Clipboard",
                clicked: () => GUIUtility.systemCopyBuffer = _debugLogManager.GetAllLogs());

            Reload();

#if UDS_USE_ASYNC_METHODS
            return Task.CompletedTask;
#else
            yield break;
#endif
        }

#if UDS_USE_ASYNC_METHODS
        public override Task Cleanup()
#else
        public override IEnumerator Cleanup()
#endif
        {
            _consoleStatePickerModel.ActiveValueChanged -= OnConsoleStatePickerValueChanged;
            _ingameDebugConsoleController.StateChanged -= OnIngameDebugConsoleStateChanged;
            _ingameDebugConsoleController.Dispose();

#if UDS_USE_ASYNC_METHODS
            return Task.CompletedTask;
#else
            yield break;
#endif
        }

        private void OnConsoleStatePickerValueChanged(Enum value)
        {
            var state = (IngameDebugConsoleController.State)value;
            _ingameDebugConsoleController.SetState(state);
        }

        private void OnIngameDebugConsoleStateChanged(IngameDebugConsoleController.State state,
            bool changedByController)
        {
            // If changed by controller, do nothing because the value is already set by the controller.
            if (changedByController)
                return;

            _consoleStatePickerModel.ActiveValue = state;
            RefreshData();
        }
    }
}

#if !EXCLUDE_UNITY_DEBUG_SHEET
using System;
using IngameDebugConsole;
using Tayx.Graphy;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells;
using UnityDebugSheet.Runtime.Extensions.Unity;
using UnityEngine;
#if UDS_USE_ASYNC_METHODS
using System.Threading.Tasks;
#else
using System.Collections;
#endif

namespace Demo._99_Shared.Scripts.DebugTools
{
    public sealed class DebugToolsPage : DebugPageBase
    {
        private EnumPickerCellModel _consoleStatePickerModel;
        private DebugConsoleController _debugConsoleController;

        private EnumPickerCellModel _fpsStatePickerModel;

        private GraphyController _graphyController;
        private EnumPickerCellModel _ramStatePickerModel;
        protected override string Title => "Debug Tools";

        public void Setup(GraphyManager graphyManager, DebugLogManager debugLogManager)
        {
            _graphyController = new GraphyController(graphyManager);
            _debugConsoleController = new DebugConsoleController(debugLogManager);
        }

#if UDS_USE_ASYNC_METHODS
        public override Task Initialize()
#else
        public override IEnumerator Initialize()
#endif
        {
            // FPS
            var fpsState = _graphyController.GetModuleState(GraphyController.ModuleType.FPS);
            var fpsStatePickerModel = new EnumPickerCellModel(fpsState);
            _fpsStatePickerModel = fpsStatePickerModel;
            fpsStatePickerModel.Text = "FPS";
            fpsStatePickerModel.Icon.Sprite = Resources.Load<Sprite>(AssetKeys.Resources.Icon.FPS);
            fpsStatePickerModel.ActiveValueChanged += OnFPSStatePickerValueChanged;
            AddEnumPicker(fpsStatePickerModel);

            // RAM
            var ramState = _graphyController.GetModuleState(GraphyController.ModuleType.RAM);
            var ramStatePickerModel = new EnumPickerCellModel(ramState);
            _ramStatePickerModel = ramStatePickerModel;
            ramStatePickerModel.Text = "RAM";
            ramStatePickerModel.Icon.Sprite = Resources.Load<Sprite>(AssetKeys.Resources.Icon.RAM);
            ramStatePickerModel.ActiveValueChanged += OnRAMStatePickerValueChanged;
            AddEnumPicker(ramStatePickerModel);

            // Console
            var consoleState = _debugConsoleController.GetState();
            var consoleStatePickerModel = new EnumPickerCellModel(consoleState);
            _consoleStatePickerModel = consoleStatePickerModel;
            consoleStatePickerModel.Text = "Console";
            consoleStatePickerModel.Icon.Sprite = Resources.Load<Sprite>(AssetKeys.Resources.Icon.Console);
            consoleStatePickerModel.ActiveValueChanged += OnConsoleStatePickerValueChanged;
            _debugConsoleController.StateChanged += OnDebugConsoleStateChanged;
            AddEnumPicker(consoleStatePickerModel);

            // System Info
            AddPageLinkButton<SystemInfoDebugPage>("System Info",
                icon: Resources.Load<Sprite>(AssetKeys.Resources.Icon.Settings));

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
            _fpsStatePickerModel.ActiveValueChanged -= OnFPSStatePickerValueChanged;
            _ramStatePickerModel.ActiveValueChanged -= OnRAMStatePickerValueChanged;
            _consoleStatePickerModel.ActiveValueChanged -= OnConsoleStatePickerValueChanged;
            _debugConsoleController.StateChanged -= OnDebugConsoleStateChanged;

#if UDS_USE_ASYNC_METHODS
            return Task.CompletedTask;
#else
            yield break;
#endif
        }

        private void OnFPSStatePickerValueChanged(Enum value)
        {
            var state = (GraphyController.ModuleState)value;
            _graphyController.SetModuleState(GraphyController.ModuleType.FPS, state);
        }

        private void OnRAMStatePickerValueChanged(Enum value)
        {
            var state = (GraphyController.ModuleState)value;
            _graphyController.SetModuleState(GraphyController.ModuleType.RAM, state);
        }

        private void OnConsoleStatePickerValueChanged(Enum value)
        {
            var state = (DebugConsoleController.State)value;
            _debugConsoleController.SetState(state);
        }

        private void OnDebugConsoleStateChanged(DebugConsoleController.State state, bool changedByController)
        {
            // If changed by controller, do nothing because the value is already set by the controller.
            if (changedByController)
                return;

            _consoleStatePickerModel.ActiveValue = state;
            RefreshData();
        }
    }
}
#endif

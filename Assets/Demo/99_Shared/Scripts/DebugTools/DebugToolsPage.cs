#if !EXCLUDE_UNITY_DEBUG_SHEET
using System;
using System.Collections;
using IngameDebugConsole;
using Tayx.Graphy;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells;
using UnityEngine;

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

        public override IEnumerator Initialize()
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

            Reload();
            
            yield break;
        }

        public override IEnumerator Cleanup()
        {
            _fpsStatePickerModel.ActiveValueChanged -= OnFPSStatePickerValueChanged;
            _ramStatePickerModel.ActiveValueChanged -= OnRAMStatePickerValueChanged;
            _consoleStatePickerModel.ActiveValueChanged -= OnConsoleStatePickerValueChanged;
            _debugConsoleController.StateChanged -= OnDebugConsoleStateChanged;
            
            yield break;
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

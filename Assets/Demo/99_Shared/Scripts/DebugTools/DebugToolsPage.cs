#if !EXCLUDE_UNITY_DEBUG_SHEET
using System;
using System.Collections;
using IngameDebugConsole;
using Tayx.Graphy;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells;
using UnityDebugSheet.Runtime.Extensions.IngameDebugConsole;
using UnityDebugSheet.Runtime.Extensions.Unity;
using UnityEngine;
#if UDS_USE_ASYNC_METHODS
using System.Threading.Tasks;
#endif

namespace Demo._99_Shared.Scripts.DebugTools
{
    public sealed class DebugToolsPage : DebugPageBase
    {
        private EnumPickerCellModel _fpsStatePickerModel;

        private GraphyController _graphyController;
        private EnumPickerCellModel _ramStatePickerModel;
        protected override string Title => "Debug Tools";

        public void Setup(GraphyManager graphyManager)
        {
            _graphyController = new GraphyController(graphyManager);
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

            // In-Game Debug Console
            AddPageLinkButton<IngameDebugConsoleDebugPage>("In-Game Debug Console",
                icon: Resources.Load<Sprite>(AssetKeys.Resources.Icon.Console),
                onLoad: x => x.Setup(DebugLogManager.Instance));

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
    }
}
#endif

using System;
using System.Collections;
using Tayx.Graphy;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells;
#if UDS_USE_ASYNC_METHODS
using System.Threading.Tasks;
#endif

namespace UnityDebugSheet.Runtime.Extensions.Graphy
{
    public sealed class GraphyDebugPage : DebugPageBase
    {
        private GraphyManager _graphyManager;
        private EnumPickerCellModel _fpsStatePickerModel;

        private EnumPickerCellModel _ramStatePickerModel;
        protected override string Title => "Debug Tools";

        public void Setup(GraphyManager graphyManager)
        {
            _graphyManager = graphyManager;
        }

#if UDS_USE_ASYNC_METHODS
        public override Task Initialize()
#else
        public override IEnumerator Initialize()
#endif
        {
            // FPS
            var fpsStatePickerModel = new EnumPickerCellModel(_graphyManager.FpsModuleState);
            _fpsStatePickerModel = fpsStatePickerModel;
            fpsStatePickerModel.Text = "FPS";
            fpsStatePickerModel.ActiveValueChanged += OnFPSStatePickerValueChanged;
            AddEnumPicker(fpsStatePickerModel);

            // RAM
            var ramStatePickerModel = new EnumPickerCellModel(_graphyManager.RamModuleState);
            _ramStatePickerModel = ramStatePickerModel;
            ramStatePickerModel.Text = "RAM";
            ramStatePickerModel.ActiveValueChanged += OnRAMStatePickerValueChanged;
            AddEnumPicker(ramStatePickerModel);


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
            var state = (GraphyManager.ModuleState)value;
            _graphyManager.FpsModuleState = state;
        }

        private void OnRAMStatePickerValueChanged(Enum value)
        {
            var state = (GraphyManager.ModuleState)value;
            _graphyManager.RamModuleState = state;
        }
    }
}

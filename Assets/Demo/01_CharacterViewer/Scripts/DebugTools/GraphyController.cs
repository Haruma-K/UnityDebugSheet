#if !EXCLUDE_UNITY_DEBUG_SHEET
using System;
using Tayx.Graphy;

namespace Demo._01_CharacterViewer.Scripts.DebugTools
{
    public sealed class GraphyController
    {
        public enum ModuleState
        {
            Hidden,
            Minimized,
            Text,
            Graph
        }

        public enum ModuleType
        {
            FPS,
            RAM
        }

        private readonly GraphyManager _manager;

        public GraphyController(GraphyManager manager)
        {
            _manager = manager;
        }

        public ModuleState GetModuleState(ModuleType type)
        {
            switch (type)
            {
                case ModuleType.FPS:
                    return ConvertGraphyModuleStateToModuleState(_manager.FpsModuleState);
                case ModuleType.RAM:
                    return ConvertGraphyModuleStateToModuleState(_manager.RamModuleState);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public void SetModuleState(ModuleType type, ModuleState state)
        {
            switch (type)
            {
                case ModuleType.FPS:
                    _manager.FpsModuleState = ConvertModuleStateToGraphyModuleState(state);
                    break;
                case ModuleType.RAM:
                    _manager.RamModuleState = ConvertModuleStateToGraphyModuleState(state);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private ModuleState ConvertGraphyModuleStateToModuleState(GraphyManager.ModuleState graphyState)
        {
            switch (graphyState)
            {
                case GraphyManager.ModuleState.FULL:
                    return ModuleState.Graph;
                case GraphyManager.ModuleState.TEXT:
                    return ModuleState.Text;
                case GraphyManager.ModuleState.BASIC:
                    return ModuleState.Minimized;
                default:
                    return ModuleState.Hidden;
            }
        }

        private GraphyManager.ModuleState ConvertModuleStateToGraphyModuleState(ModuleState state)
        {
            switch (state)
            {
                case ModuleState.Hidden:
                    return GraphyManager.ModuleState.BACKGROUND;
                case ModuleState.Minimized:
                    return GraphyManager.ModuleState.BASIC;
                case ModuleState.Text:
                    return GraphyManager.ModuleState.TEXT;
                case ModuleState.Graph:
                    return GraphyManager.ModuleState.FULL;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}
#endif

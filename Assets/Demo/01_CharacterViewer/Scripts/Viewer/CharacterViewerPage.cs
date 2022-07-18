#if !EXCLUDE_UNITY_DEBUG_SHEET
using System.Collections;
using System.Linq;
using Demo._99_Shared.Scripts;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells;
using UnityEngine;

namespace Demo._01_CharacterViewer.Scripts.Viewer
{
    public sealed class CharacterViewerPage : DebugPageBase
    {
        private SwitchCellModel _autoRotateSwitchModel;
        private CharacterSpawner _characterSpawner;
        private PickerCellModel _modelPickerModel;
        private int _motionPickerId;
        private PickerCellModel _motionPickerModel;
        private SliderCellModel _positionXSliderModel;
        private int _rotationPickerId;
        private SliderCellModel _rotationSliderModel;
        private StandController _standController;

        protected override string Title => "Character Viewer";

        private void Update()
        {
            // Update rotation view if auto rotate is on.
            if (_autoRotateSwitchModel.Value)
            {
                _rotationSliderModel.Value = _standController.Rotation;
                RefreshDataAt(_rotationPickerId);
            }
        }

        public void Setup(CharacterSpawner characterSpawner, StandController standController)
        {
            _characterSpawner = characterSpawner;
            _standController = standController;
        }

        public override IEnumerator WillPushEnter()
        {
            var activeCharacterController = _characterSpawner.ActiveCharacterAnimationController;
            var modelNames = _characterSpawner.Prefabs.Select(x => x.name).ToArray();
            var motionNames = activeCharacterController.Clips.Select(x => x.name).ToArray();

            // Models
            var modelPickerModel = new PickerCellModel();
            _modelPickerModel = modelPickerModel;
            modelPickerModel.Text = "Models";
            modelPickerModel.SetOptions(modelNames, _characterSpawner.GetActiveCharacterControllerIndex());
            modelPickerModel.Icon.Sprite = Resources.Load<Sprite>(AssetKeys.Resources.Icon.Model);
            modelPickerModel.ActiveOptionChanged += OnModelPickerValueChanged;
            AddPicker(modelPickerModel);

            // Motions
            var motionPickerModel = new PickerCellModel();
            _motionPickerModel = motionPickerModel;
            motionPickerModel.Text = "Motions";
            motionPickerModel.SetOptions(motionNames, activeCharacterController.GetActiveClipIndex());
            motionPickerModel.Icon.Sprite = Resources.Load<Sprite>(AssetKeys.Resources.Icon.Motion);
            motionPickerModel.ActiveOptionChanged += OnMotionPickerValueChanged;
            _motionPickerId = AddPicker(motionPickerModel);

            // Position
            var positionSliderModel =
                new SliderCellModel(true, StandController.PositionXMin, StandController.PositionXMax);
            _positionXSliderModel = positionSliderModel;
            positionSliderModel.CellTexts.Text = "Position";
            positionSliderModel.Value = _standController.PositionX;
            positionSliderModel.Icon.Sprite = Resources.Load<Sprite>(AssetKeys.Resources.Icon.Position);
            positionSliderModel.ValueChanged += OnPositionSliderValueChanged;
            AddSlider(positionSliderModel);

            // Rotation
            var rotationSliderModel =
                new SliderCellModel(true, StandController.RotationMin, StandController.RotationMax);
            _rotationSliderModel = rotationSliderModel;
            rotationSliderModel.CellTexts.Text = "Rotation";
            rotationSliderModel.Value = _standController.Rotation;
            rotationSliderModel.Icon.Sprite = Resources.Load<Sprite>(AssetKeys.Resources.Icon.Rotation);
            rotationSliderModel.ValueChanged += OnRotationSliderValueChanged;
            _rotationPickerId = AddSlider(rotationSliderModel);

            // Auto Rotation
            var autoRotationSwitchModel = new SwitchCellModel(false);
            _autoRotateSwitchModel = autoRotationSwitchModel;
            autoRotationSwitchModel.CellTexts.Text = "Auto Rotation";
            autoRotationSwitchModel.Value = _standController.AutoRotation;
            autoRotationSwitchModel.Icon.Sprite = Resources.Load<Sprite>(AssetKeys.Resources.Icon.AutoRotation);
            autoRotationSwitchModel.ValueChanged += OnAutoRotationSwitchValueChanged;
            AddSwitch(autoRotationSwitchModel);

            yield break;
        }

        public override void DidPopExit()
        {
            _modelPickerModel.ActiveOptionChanged -= OnModelPickerValueChanged;
            _motionPickerModel.ActiveOptionChanged -= OnMotionPickerValueChanged;
            _positionXSliderModel.ValueChanged -= OnPositionSliderValueChanged;
            _rotationSliderModel.ValueChanged -= OnRotationSliderValueChanged;
            _autoRotateSwitchModel.ValueChanged -= OnAutoRotationSwitchValueChanged;
        }

        private void OnModelPickerValueChanged(int value)
        {
            var characterController = _characterSpawner.ChangeCharacter(value);
            OnCharacterChanged(characterController);
        }

        private void OnPositionSliderValueChanged(float value)
        {
            _standController.PositionX = value;
        }

        private void OnRotationSliderValueChanged(float value)
        {
            _standController.Rotation = value;
        }

        private void OnAutoRotationSwitchValueChanged(bool value)
        {
            _standController.AutoRotation = value;
        }

        private void OnMotionPickerValueChanged(int value)
        {
            _characterSpawner.ActiveCharacterAnimationController.ChangeClip(value);
        }

        private void OnCharacterChanged(CharacterAnimationController characterAnimationController)
        {
            _motionPickerModel.SetOptions(characterAnimationController.Clips.Select(x => x.name).ToArray(),
                characterAnimationController.GetActiveClipIndex());

            RefreshDataAt(_motionPickerId);
        }
    }
}
#endif

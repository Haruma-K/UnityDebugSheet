using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.CellParts;
using UnityEngine;
using UnityEngine.UI;

namespace UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells
{
    public sealed class LabelCell : Cell<LabelCellModel>
    {
        [SerializeField] private LayoutElement _layoutElement;

        public CellIcon icon;
        public CellTexts cellTexts;

        protected override void SetModel(LabelCellModel model)
        {
            // Icon
            icon.Setup(model.Icon);
            icon.gameObject.SetActive(model.Icon.Sprite != null);

            //Texts
            if (!model.UseSubText)
                model.CellTexts.SubText = null;
            cellTexts.Setup(model.CellTexts);

            // Height
            var height = ((RectTransform)cellTexts.transform).rect.height + 36;
            _layoutElement.preferredHeight = height;
        }
    }

    public sealed class LabelCellModel : CellModel
    {
        public LabelCellModel(bool useSubText)
        {
            UseSubText = useSubText;
        }

        public CellIconModel Icon { get; } = new CellIconModel();

        public bool UseSubText { get; }

        public CellTextsModel CellTexts { get; } = new CellTextsModel();
    }
}

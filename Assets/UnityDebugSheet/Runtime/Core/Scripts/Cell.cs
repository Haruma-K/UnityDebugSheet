using UnityEngine;

namespace UnityDebugSheet.Runtime.Core.Scripts
{
    public interface ICell
    {
        void Setup(CellModel model);
    }

    public abstract class Cell<TData> : MonoBehaviour, ICell where TData : CellModel
    {
        public void Setup(CellModel model)
        {
            var d = (TData)model;
            SetModel(d);
        }

        protected abstract void SetModel(TData model);
    }

    public abstract class CellModel
    {
    }
}

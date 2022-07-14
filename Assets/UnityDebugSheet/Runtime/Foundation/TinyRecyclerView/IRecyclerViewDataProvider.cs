using UnityEngine;

namespace UnityDebugSheet.Runtime.Foundation.TinyRecyclerView
{
    public interface IRecyclerViewDataProvider
    { 
        void SetupCell(int dataIndex, GameObject cell);
    }
}

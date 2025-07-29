using UnityEngine;

namespace UnityDebugSheet.TinyRecyclerView
{
    public interface IRecyclerViewDataProvider
    { 
        void SetupCell(int dataIndex, GameObject cell);
    }
}

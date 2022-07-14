using UnityEngine;

namespace UnityDebugSheet.Runtime.Foundation.TinyRecyclerView
{
    public interface IRecyclerViewCellProvider
    {
        GameObject GetCell(int dataIndex);

        void ReleaseCell(GameObject obj);
    }
}

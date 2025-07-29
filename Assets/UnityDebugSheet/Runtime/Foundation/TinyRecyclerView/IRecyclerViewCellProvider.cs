using UnityEngine;

namespace UnityDebugSheet.TinyRecyclerView
{
    public interface IRecyclerViewCellProvider
    {
        GameObject GetCell(int dataIndex);

        void ReleaseCell(GameObject obj);
    }
}

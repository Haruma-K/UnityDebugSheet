using System;
using System.Threading.Tasks;

namespace UnityDebugSheet
{
    public interface IAssetLoadHandleSetter<T>
    {
        void SetStatus(AssetLoadStatus status);

        void SetResult(T result);

        void SetPercentCompleteFunc(Func<float> percentComplete);

        void SetTask(Task<T> task);

        void SetOperationException(Exception ex);
    }
}
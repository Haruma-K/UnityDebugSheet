using System.Threading.Tasks;

namespace UnityDebugSheet.Runtime.Foundation.ObjectPooling
{
    internal class AsyncPooledObject<T>
    {
        public T Item { get; }

        private readonly IAsyncPoolableObject _poolable;
        
        public AsyncPooledObject(T item)
        {
            Item = item;
            _poolable = item as IAsyncPoolableObject;
        }

        public async Task OnBeforeUseAsync()
        {
            if (_poolable != null)
            {
                await _poolable.OnBeforeUseAsync();
            }
        }

        public async Task OnBeforeReleaseAsync()
        {
            if (_poolable != null)
            {
                await _poolable.OnBeforeReleaseAsync();
            }
        }

        public async Task OnBeforeClearAsync()
        {
            if (_poolable != null)
            {
                await _poolable.OnBeforeClearAsync();
            }
        }
    }
}
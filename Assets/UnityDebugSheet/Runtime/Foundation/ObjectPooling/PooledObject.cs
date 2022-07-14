namespace UnityDebugSheet.Runtime.Foundation.ObjectPooling
{
    internal class PooledObject<T>
    {
        public T Item { get; }

        private readonly IPoolableObject _poolable;
        
        public PooledObject(T item)
        {
            Item = item;
            _poolable = item as IPoolableObject;
        }

        public void OnBeforeUse()
        {
            _poolable?.OnBeforeUse();
        }

        public void OnBeforeRelease()
        {
            _poolable?.OnBeforeRelease();
        }

        public void OnBeforeClear()
        {
            _poolable?.OnBeforeClear();
        }
    }
}
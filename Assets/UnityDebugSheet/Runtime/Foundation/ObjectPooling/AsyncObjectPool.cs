using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Assertions;

namespace UnityDebugSheet.Runtime.Foundation.ObjectPooling
{
    public class AsyncObjectPool<T>
    {
        private readonly Dictionary<T, AsyncPooledObject<T>> _busyObjects;
        private readonly Func<Task<T>> _factoryFunc;
        private readonly Func<T, Task> _disposeFunc;
        private readonly Stack<AsyncPooledObject<T>> _waitingObjects;

        /// <summary>
        ///     Initialize.
        /// </summary>
        /// <param name="factoryFunc"></param>
        /// <param name="disposeFunc"></param>
        public AsyncObjectPool(Func<Task<T>> factoryFunc, Func<T, Task> disposeFunc = null)
        {
            Assert.IsNotNull(factoryFunc);

            Capacity = -1;
            AutoExpansion = true;
            _factoryFunc = factoryFunc;
            _disposeFunc = disposeFunc;
            _waitingObjects = new Stack<AsyncPooledObject<T>>();
            _busyObjects = new Dictionary<T, AsyncPooledObject<T>>();
        }

        /// <summary>
        ///     Capacity
        /// </summary>
        public int Capacity { get; private set; }

        /// <summary>
        ///     If true, objects will be created beyond capacity.
        /// </summary>
        public bool AutoExpansion { get; private set; }

        /// <summary>
        ///     Count of the waiting objects.
        /// </summary>
        public int WaitingObjectsCount => _waitingObjects.Count;

        /// <summary>
        ///     Pre-generate the objects.
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="autoExpansion"></param>
        public async Task WarmupAsync(int capacity, bool autoExpansion)
        {
            Capacity = capacity;
            AutoExpansion = autoExpansion;

            var createCount = capacity - _busyObjects.Count - _waitingObjects.Count;
            for (var i = 0; i < createCount; i++)
            {
                var obj = await _factoryFunc();
                var pooledObject = new AsyncPooledObject<T>(obj);
                _waitingObjects.Push(pooledObject);
            }
        }

        /// <summary>
        ///     Get a item from the object pool.
        /// </summary>
        /// <returns></returns>
        public async Task<T> UseAsync()
        {
            AsyncPooledObject<T> pooledObject;
            if (_waitingObjects.Count == 0)
            {
                if (AutoExpansion)
                {
                    var obj = await _factoryFunc();
                    pooledObject = new AsyncPooledObject<T>(obj);
                }
                else
                {
                    throw new InvalidOperationException("There are no waiting objects available in ObjectPool.");
                }
            }
            else
            {
                pooledObject = _waitingObjects.Pop();
            }

            await pooledObject.OnBeforeUseAsync();
            _busyObjects.Add(pooledObject.Item, pooledObject);
            return pooledObject.Item;
        }

        /// <summary>
        ///     Return a item to the object pool.
        /// </summary>
        /// <param name="item"></param>
        public async Task ReleaseAsync(T item)
        {
            Assert.IsTrue(_busyObjects.ContainsKey(item));

            var pooledObject = _busyObjects[item];
            await pooledObject.OnBeforeReleaseAsync();
            _busyObjects.Remove(item);
            _waitingObjects.Push(pooledObject);
        }

        /// <summary>
        ///     Clear all of the objects.
        /// </summary>
        public async Task ClearAsync()
        {
            foreach (var obj in _busyObjects.Values)
            {
                await obj.OnBeforeClearAsync();
                if (_disposeFunc != null)
                {
                    await _disposeFunc.Invoke(obj.Item);
                }
            }

            foreach (var obj in _waitingObjects)
            {
                await obj.OnBeforeClearAsync();
                if (_disposeFunc != null)
                {
                    await _disposeFunc.Invoke(obj.Item);
                }
            }
            
            _busyObjects.Clear();
            _waitingObjects.Clear();
            Capacity = -1;
            AutoExpansion = true;
        }
    }
}
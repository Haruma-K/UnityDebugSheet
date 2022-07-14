using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace UnityDebugSheet.Runtime.Foundation.ObjectPooling
{
    public class ObjectPool<T>
    {
        private readonly Dictionary<T, PooledObject<T>> _busyObjects;
        private readonly Func<T> _factoryFunc;
        private readonly Action<T> _disposeAction;
        private readonly Stack<PooledObject<T>> _waitingObjects;

        /// <summary>
        ///     Initialize.
        /// </summary>
        /// <param name="factoryFunc"></param>
        /// <param name="disposeAction"></param>
        public ObjectPool(Func<T> factoryFunc, Action<T> disposeAction = null)
        {
            Assert.IsNotNull(factoryFunc);

            Capacity = -1;
            AutoExpansion = true;
            _factoryFunc = factoryFunc;
            _disposeAction = disposeAction;
            _waitingObjects = new Stack<PooledObject<T>>();
            _busyObjects = new Dictionary<T, PooledObject<T>>();
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
        public void Warmup(int capacity, bool autoExpansion)
        {
            Capacity = capacity;
            AutoExpansion = autoExpansion;

            var createCount = capacity - _busyObjects.Count - _waitingObjects.Count;
            for (var i = 0; i < createCount; i++)
            {
                var pooledObject = new PooledObject<T>(_factoryFunc());
                _waitingObjects.Push(pooledObject);
            }
        }

        /// <summary>
        ///     Get a item from the object pool.
        /// </summary>
        /// <returns></returns>
        public T Use()
        {
            PooledObject<T> pooledObject;
            if (_waitingObjects.Count == 0)
            {
                if (AutoExpansion)
                {
                    pooledObject = new PooledObject<T>(_factoryFunc());
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

            pooledObject.OnBeforeUse();
            _busyObjects.Add(pooledObject.Item, pooledObject);
            return pooledObject.Item;
        }

        /// <summary>
        ///     Return a item to the object pool.
        /// </summary>
        /// <param name="item"></param>
        public void Release(T item)
        {
            Assert.IsTrue(_busyObjects.ContainsKey(item));

            var pooledObject = _busyObjects[item];
            pooledObject.OnBeforeRelease();
            _busyObjects.Remove(item);
            _waitingObjects.Push(pooledObject);
        }

        /// <summary>
        ///     Clear all of the objects.
        /// </summary>
        public void Clear()
        {
            foreach (var obj in _busyObjects.Values)
            {
                obj.OnBeforeClear();
                _disposeAction?.Invoke(obj.Item);
            }

            foreach (var obj in _waitingObjects)
            {
                obj.OnBeforeClear();
                _disposeAction?.Invoke(obj.Item);
            }
            
            _busyObjects.Clear();
            _waitingObjects.Clear();
            Capacity = -1;
            AutoExpansion = true;
        }
    }
}
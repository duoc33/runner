using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace PoolKit
{
    public abstract class Pool<T> : MonoBehaviour where T : class
    {
        protected ObjectPool<T> objectPool;
        protected virtual void Awake()
        {
            objectPool = new ObjectPool<T>(Create, OnGetAction, OnReleaseAction, OnDestroyAction, Check(), Capcity(), MaxSize());
        }
        protected virtual void OnDestroy()
        {
            objectPool.Clear();
        }
        protected abstract T Create();
        protected virtual int MaxSize() => 1000;
        /// <summary>
        /// 是否在进池前检查对象是否已经存在池中
        /// </summary>
        protected virtual bool Check() => false;
        protected virtual int Capcity() => 10;
        protected virtual void OnGetAction(T value) { }
        protected virtual void OnReleaseAction(T value) {}
        protected virtual void OnDestroyAction(T value) { }

        public void Clear() => objectPool.Clear();
        public T Get() => objectPool.Get();
        public void Release(T value)
        {
            if(value == null) return;
            objectPool.Release(value);
        }
    }
}
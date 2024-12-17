using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PoolKit;
using UnityEngine;

namespace Runner
{
    public class PlayerPool : Pool<GameObject>
    {
        public GameObject Prefab;
        protected override GameObject Create()
        {
            GameObject copy = Instantiate(Prefab,transform);
            copy.GetComponent<BehavioursController>().WhenDestroy = WhenPlayerDestroy;
            return copy;
        }
        private TimeSpan delayTime = TimeSpan.FromSeconds(1.8f);
        private void WhenPlayerDestroy(GameObject player)
        {
            PlayerDeath(player).SuppressCancellationThrow().Forget();
        }
        private async UniTask PlayerDeath(GameObject player)
        {
            await UniTask.Delay(delayTime);
            Release(player);
        }
        protected override void OnGetAction(GameObject value)
        {
            value.SetActive(true);
        }
        protected override void OnDestroyAction(GameObject value)
        {
            Destroy(value);
        }
        protected override void OnReleaseAction(GameObject value)
        {
            if(value == null) return;
            value.SetActive(false);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}


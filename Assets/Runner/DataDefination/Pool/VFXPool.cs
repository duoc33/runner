using System;
using System.Collections;
using System.Collections.Generic;
using PoolKit;
using UnityEngine;
namespace Runner
{

    public class VFXPool : Pool<GameObject>
    {
        public GameObject Prefab;
        public Vector3 ScaleOffset;
        protected override GameObject Create()
        {
            GameObject go = Instantiate(Prefab);
            go.transform.localScale.Scale(ScaleOffset == default? Vector3.one : ScaleOffset);
            return go;
        }
        protected override void OnGetAction(GameObject value)
        {
            // 确保获取的对象是活跃的
            value.SetActive(true);

            // 获取粒子系统组件
            ParticleSystem ps = value.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                // 清除遗留的粒子效果，重新播放
                ps.Clear();
                ps.Play();
            }
        }
        protected override void OnReleaseAction(GameObject value)
        {
            if(value == null) return;
            value.transform.SetParent(null);
            // 获取粒子系统组件
            ParticleSystem ps = value.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                // 停止播放粒子效果
                ps.Stop();
                ps.Clear();
            }

            // 将对象设置为非活跃
            value.SetActive(false);
        }
        public GameObject Get(Vector3 pos , Transform parent = null)
        {
            GameObject psGo = Get();
            psGo.transform.SetParent(parent);
            psGo.transform.localPosition = pos;
            return psGo;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace Runner
{
    public class ScoreAndPlayerCountChangeComponent : StraightPathLevelItemComponent
    {
        public int PlayerCountChangeValue;
        public bool IsNeedTextShow;
        public bool IsNeedDestroy;
        public float DestroyTime;
        private static string Path = "Runner/LevelPrefab/WorldCanvas";
        public override void DecorateWhenInstantiate(GameObject spawned, StraightPathLevelItem info)
        {
            EnterTriggerItem enterTriggerItem = spawned.AddComponent<EnterTriggerItem>();
            enterTriggerItem.PlayerCountDelta = PlayerCountChangeValue;
            enterTriggerItem.CenterOffset = info.model.GetCenterOffset();
            enterTriggerItem.IsNeedDestroy = IsNeedDestroy;
            enterTriggerItem.DestroyTime = DestroyTime;
            enterTriggerItem.modelSize = info.model.GetModelSize();
            
            Rigidbody rigidbody = spawned.GetComponent<Rigidbody>();
            if(rigidbody==null)
            {
                rigidbody = spawned.AddComponent<Rigidbody>();
            }
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;

            if (IsNeedTextShow)
            {
                AddText(spawned.transform, info.model.GetCenterOffset(), info.model.GetModelSize(),
                PlayerCountChangeValue > 0 ? "+" + PlayerCountChangeValue.ToString() : PlayerCountChangeValue.ToString());
            }
        }
        public override void DecorateWhenPostProcess(GameObject target, StraightPathLevelItem info)
        {
            
        }
        private void AddText(Transform target, Vector3 centerOffset, Vector3 size, string data)
        {
            GameObject text = Instantiate(Resources.Load<GameObject>(Path), target);
            text.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x, size.y);
            text.transform.localPosition = Vector3.zero;
            text.transform.localRotation = Quaternion.Euler(Vector3.up * 180);
            text.transform.localScale = Vector3.one;
            text.GetComponent<Canvas>().worldCamera = Camera.main;
            text.transform.localPosition = centerOffset - target.forward * size.z / 2.0f;
            text.GetComponentInChildren<TextMeshProUGUI>().text = data;
        }
    }
}


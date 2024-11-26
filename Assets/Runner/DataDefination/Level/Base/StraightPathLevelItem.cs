using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using ScriptableObjectBase;
using UnityEngine;
using UnityEngine.AI;
namespace Runner
{
    public abstract class StraightPathLevelItem : SOBase
    {
        public string ItemUrl;
        [Range(0, 1)]
        public float Properbility = 1;
        [JsonIgnore]
        [NonSerialized]
        public List<StraightPathLevelItemComponent> Components = new List<StraightPathLevelItemComponent>();
        [JsonIgnore]
        [NonSerialized]
        public ModelSO model;
        public override async UniTask Download()
        {
            await DownloadAndPostProcessGameObject(ItemUrl , PostProcess, DowloadAnimation);
        }
        private GameObject PostProcess(GameObject target)
        {
            model ??= CreateInstance<ModelSO>();
            ModelConfig(model);
            GameObject newGameObject = model.PostProcess(target);
            HandlePostProcess(newGameObject,model);
            ProcessComponents(newGameObject);
            return newGameObject;
        }
        protected virtual async UniTask<GameObject> DowloadAnimation(GameObject target)
        {
            return await UniTask.FromResult(target);
        }
        public GameObject Spawn(Vector3 pos , Transform parent)
        {
            GameObject spawn = Instantiate(model.GetPrefab(),pos,Quaternion.Euler(Vector3.up * 180 ),parent);
            HandleOnInstantiate(spawn,model);
            foreach (var componet in Components)
            {
                componet.DecorateWhenInstantiate(spawn , this);
            }
            return spawn;
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            Destroy(model);
            foreach (var componet in Components)Destroy(componet);
        }
        protected virtual void ModelConfig(ModelSO model)
        {
            model.colliderType = ModelSO.ColliderType.Box;
            model.pivotType = ModelSO.PivotType.Bottom;
            model.isTrigger = true;
            model.isStatic = true;
        }
        protected virtual List<StraightPathLevelItemComponent> AddComponents()
        {
            return null;
        }
        protected virtual void HandlePostProcess(GameObject gameObject,ModelSO model)
        {
            
        }
        protected virtual void HandleOnInstantiate(GameObject gameObject,ModelSO model)
        {
            
        }


        private void ProcessComponents(GameObject newGameObject)
        {
            List<StraightPathLevelItemComponent> components = AddComponents();
            Components ??=new List<StraightPathLevelItemComponent>();
            if(components!=null)
            {
                if(Components==null)
                {
                    Components = components;
                }
                else{
                    Components.AddRange(components);
                }
            }
            foreach (var componet in Components)
            {
                componet.DecorateWhenPostProcess(newGameObject , this);
            }
        }
        
    }

    
}


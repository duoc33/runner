using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;
namespace ScriptableObjectBase
{
    public abstract class SOBase : ScriptableObject
    {
        protected static Sprite GetDownloadedSrpite(string url) => GetResHandler?.Invoke(url) as Sprite;
        protected static Texture2D GetDownloadedTexture2D(string url) => GetResHandler?.Invoke(url) as Texture2D;
        protected static GameObject GetDownloadedGameObject(string url) => GetResHandler?.Invoke(url) as GameObject;
        protected static AudioClip GetDownloadedAudioClip(string url) => GetResHandler?.Invoke(url) as AudioClip;
        protected static AnimationClip GetDownloadedAnimationClip(string url) => GetResHandler?.Invoke(url) as AnimationClip;

        protected static async UniTask<GameObject> DownloadPrefab(string url)=> await DownloadModelHandler(url);
        protected static async UniTask<AudioClip> DownloadAudio(string url)=> await DownloadAudioHandler(url);
        protected static async UniTask<Sprite> DownloadSprite(string url)=> await DownloadSpriteHandler(url);
        protected static async UniTask<Texture2D> DownloadTexture(string url)=> await DownloadTextureHandler(url);
        protected static async UniTask<AnimationClip> DownloadAnimationClip(string url)=> await DownloadAnimationClipHandler(url);


        #region Base

        private static Func<string, UniTask<GameObject>> DownloadModelHandler;
        private static Func<string, UniTask<AudioClip>> DownloadAudioHandler;
        private static Func<string, UniTask<Sprite>> DownloadSpriteHandler;
        private static Func<string, UniTask<Texture2D>> DownloadTextureHandler;
        private static Func<string, UniTask<AnimationClip>> DownloadAnimationClipHandler;
        private static Func<string, UnityEngine.Object> GetResHandler;
        
        public static void InitLocal()
        {
            DownloadModelHandler = async (str) =>
            {
                if(string.IsNullOrEmpty(str)) return null;
                ResourceRequest request = Resources.LoadAsync<GameObject>(str);
                await request;
                return request.asset as GameObject;
            };
            DownloadSpriteHandler = async (str) =>
            {
                if(string.IsNullOrEmpty(str)) return null;
                ResourceRequest request = Resources.LoadAsync<Sprite>(str);
                await request;
                return request.asset as Sprite;
            };
            DownloadAudioHandler = async (str) =>
            {
                if(string.IsNullOrEmpty(str)) return null;
                ResourceRequest request = Resources.LoadAsync<AudioClip>(str);
                await request;
                return request.asset as AudioClip;
            };
            DownloadTextureHandler = async (str) =>
            {
                if(string.IsNullOrEmpty(str)) return null;
                ResourceRequest request = Resources.LoadAsync<Texture2D>(str);
                await request;
                return request.asset as Texture2D;
            };
            DownloadAnimationClipHandler = async (str) => {
                if(string.IsNullOrEmpty(str)) return null;
                ResourceRequest request = Resources.LoadAsync<Texture2D>(str);
                await request;
                return request.asset as AnimationClip;
            };
            GetResHandler = (str) => Resources.Load<UnityEngine.Object>(str);
            PostProcessGameObjectsPool?.Clear();
        }
        public static void InitServer()
        {
            DownloadModelHandler = DLUtils.LoadAddressableObject;
            DownloadSpriteHandler = DLUtils.DownloadSpriteData;
            DownloadAudioHandler = DLUtils.DownloadAudioData;
            DownloadTextureHandler = DLUtils.DownloadTextureData;
            DownloadAnimationClipHandler = DLUtils.LoadAnimationClip;
            GetResHandler = DLUtils.GetResources;
        }
        public static void Clear() 
        {
            DestroyImmediate(PostProcessGameObjectHolder);
            PostProcessGameObjectsPool?.Clear();
            DLUtils.ClearCache();
        }
        
        #endregion

        #region Post Process Resources 
        
        private static GameObject PostProcessGameObjectHolder;
        private static Dictionary<string,GameObject> PostProcessGameObjectsPool;
        protected static GameObject GetPooledGameObject(string url)
        {
            if(PostProcessGameObjectsPool.ContainsKey(url))
            {
                return PostProcessGameObjectsPool[url];
            }
            return null;
        }
        protected static async UniTask<string> DownloadAndPostProcessGameObject(string url, Func<GameObject,GameObject> PostProcessHandler = null ,Func<GameObject , UniTask<GameObject>> PostProcessAsyncHandler = null , bool IsInstantiate = false)
        {
            GameObject target = await DownloadModelHandler.Invoke(url);
            if(target == null) 
            {
                Debug.LogError("Failed to download model: " + url);
                return null;
            }
            if(PostProcessGameObjectsPool==null)
            {
                PostProcessGameObjectsPool = new Dictionary<string,GameObject>();
            }
            if(PostProcessGameObjectsPool.ContainsKey(url))
            {
                // url += PostProcessGameObjectCount ++ ;   
            }
            if(PostProcessGameObjectHolder == null)
            {
                PostProcessGameObjectHolder = new GameObject("PostProcessGameObjectHolder");
            }
            
            PostProcessGameObjectHolder.SetActive(false);
            GameObject poolItem = IsInstantiate ? target : Instantiate(target,Vector3.zero,Quaternion.identity) ;
            poolItem.name = target.name;
            GameObject newParent = null;
            if(PostProcessHandler!=null)
            {
                newParent = PostProcessHandler.Invoke(poolItem);
            }
            if(PostProcessAsyncHandler!=null)
            {
                newParent = await PostProcessAsyncHandler.Invoke( newParent ==null ? poolItem : newParent);
            }
            
            newParent.transform.SetParent(PostProcessGameObjectHolder.transform);
            PostProcessGameObjectsPool[url] = newParent;

            return url;
        }
        
        #endregion
        
        public virtual async UniTask Download() => await UniTask.Yield();
        public virtual void StartMixComponents()
        {
            
        }
        public virtual void OnDestroy() { }
    }
}

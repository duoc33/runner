using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.ResourceLocations;
namespace ScriptableObjectBase
{
    /// <summary>
    /// LoadContentCatalogAsync 的作用:
    /// Addressables.LoadContentCatalogAsync 是用来加载指定的 Content Catalog 文件。这个文件是一个 JSON 文件，描述了 Addressable 资源的信息，包括资源的位置、名称、类型等信息。
    /// 简单来说，Catalog 文件就是一个索引，里面记录了各个资源组中包含哪些资源、这些资源在哪里以及如何访问。
    /// 
    /// Catalog 和资源组的关系：
    /// 在 Addressables 系统中，你将多个资源打包成一个或多个 资源组（Addressable Groups）。这些资源组会有一个 Content Catalog，即.json 文件，
    /// 里面包含该组中所有资源的元数据（例如文件路径、资源 ID、类型等）。通过加载这个 Catalog，你可以知道这个资源组里有哪些资源，以及如何访问它们。
    /// 
    /// Catalog 文件的作用：
    /// Catalog 文件的目的是使得你不需要提前知道所有资源的具体位置，而是通过加载该文件来动态获取所有资源的信息。
    /// </summary>
    public class DLUtils
    {
        #region CC
        public static UnityEngine.Object GetResources(string url)
        {
            if (DownloadedAAAssets.ContainsKey(url))
            {
                return DownloadedAAAssets[url];
            }
            if(DownloadedAssets.ContainsKey(url))
            {
                return DownloadedAssets[url];
            }
            return null;
        }
        public static Dictionary<string, UnityEngine.Object> DownloadedAAAssets = new Dictionary<string, UnityEngine.Object>();
        public static Dictionary<string, UnityEngine.Object> DownloadedAssets = new Dictionary<string, UnityEngine.Object>();
        public static void ClearCache()
        {
            foreach (var obj in DownloadedAAAssets.Values)
            {
                if (obj != null)
                {
                    Addressables.Release(obj);
                }
            }
            foreach (var obj in DownloadedAssets.Values)
            {
                if(obj!=null) 
                {
                    UnityEngine.Object.Destroy(obj);
                }
            }
            DownloadedAssets.Clear();
            DownloadedAAAssets.Clear();
            ReleaseUnusedAssets().Forget();
        }
        public static async UniTask ReleaseUnusedAssets() => await Resources.UnloadUnusedAssets();

        public static async UniTask<AnimationClip> LoadAnimationClip(string address)
        {
            if (string.IsNullOrEmpty(address)) return null;

            if (DownloadedAAAssets.ContainsKey(address))
            {
                return (AnimationClip)DownloadedAAAssets[address];
            }
            AnimationClip animationClip = await LoadAddressableObject<AnimationClip>(address);
            DownloadedAAAssets[address] = animationClip;
            return animationClip;
        }

        public static async UniTask<string> DownloadJson(string url)
        {
            if (url.StartsWith("http", System.StringComparison.OrdinalIgnoreCase))
            {
                return await DownloadFromHttp(url);
            }
            else
            {
                return await ReadFromFile(url);
            }
        }

        public static async UniTask<GameObject> LoadAddressableObject(string address)
        {
            if (string.IsNullOrEmpty(address)) return null;

            if (DownloadedAAAssets.ContainsKey(address))
            {
                return (GameObject)DownloadedAAAssets[address];
            }

            GameObject objToReturn = null;
            var strings = address.Split("#");
            string catalogPath = "";
            string modelName = null;
            if (strings.Length >= 2)
            {
                var modelUrl = strings[0];
                modelName = strings[1];
                Debug.Log("Load AddressableObject : " + modelName + "|" + modelUrl);
                catalogPath = PHUtils.SplitUrl(modelUrl); // todo>?
            }
            else
            {
                modelName = address;
            }
            objToReturn = await LoadAddressableAsset<GameObject>(modelName, catalogPath);
            DownloadedAAAssets[address] = objToReturn;
            return objToReturn;
        }

        public static async UniTask<Texture2D> DownloadTextureData(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;
            Debug.Log("DownloadTextureData: " + url);
            if (string.IsNullOrEmpty(url) || !url.Contains("."))
            {
                // Debug.LogError("The image url is null");
                return null;
            }
            if (!url.Contains("://"))
            {
                url = "file://" + url;
            }
            if (url.Contains(".gif"))
            {
                url = url.Replace(".gif", ".png");
                Debug.Log("Find unsported type gif, and tring to load from " + url);
            }
            string textureKey = "texture-" + url;
            if (DownloadedAssets.ContainsKey(textureKey) && DownloadedAssets[textureKey] != null)
            {
                Debug.Log("Find Cached Image:" + url);
                return DownloadedAssets[textureKey] as Texture2D;
            }

            var www = UnityWebRequestTexture.GetTexture(url);
            await www.SendWebRequest();
            if (www.isDone == false || www.error != null)
            {
                Debug.LogError("Request = " + www.error + url);
                return null;
            }
            var result = DownloadHandlerTexture.GetContent(www);
            result = ChangeTextureFormat(result, TextureFormat.RGBA32);
            result.name = "Text2D: " + url;

            // if (width != -1 && height != -1)
            // {
            //    Debug.Log("Resize texture to " + width + "x" + height + " from " + result.width + "x" + result.height + "");
            //    result = PHUtils.ResizeTexture(result, width, height);
            // }

            www.Dispose();
            DownloadedAssets[textureKey] = result;

            return result;
        }

        public static async UniTask<AudioClip> DownloadAudioData(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;

            Debug.Log(url);

            if (DownloadedAssets.ContainsKey(url))
            {
                AudioClip clip = DownloadedAssets[url] as AudioClip;
                return clip;
            }

            string[] context = url.Split('.');
            string type = context[context.Length - 1];

            UnityWebRequest _audioRequest = null;
            if (type == "wav")
            {
                _audioRequest = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV);
            }
            else if (type == "mp3")
            {
                _audioRequest = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG);
            }
            else if (type == "aac")
            {
                _audioRequest = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.AUDIOQUEUE); // AAC 通过 AUDIOQUEUE 处理
            }
            else if (type == "ogg")
            {
                _audioRequest = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.OGGVORBIS);
            }
            else if (type == "aiff")
            {
                _audioRequest = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.AIFF);
            }
            else
            {
                // 处理其他类型或错误
                Debug.LogError("Null Muisc");
                return null;
            }
            await _audioRequest.SendWebRequest();

            if (_audioRequest.isDone == false || _audioRequest.error != null)
            {
                //Debug.LogError("Request = " + _audioRequest.error + " Url: " + url);
                return null;
            }
            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(_audioRequest);
            DownloadedAssets[url] = audioClip;
            return audioClip;
        }

        public static async UniTask<Sprite> DownloadSpriteData(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;

            string spriteKey = "sprite-" + url;
            string textureKey = "texture-" + url;
            if (DownloadedAssets.ContainsKey(spriteKey) && DownloadedAssets[spriteKey] != null) return DownloadedAssets[spriteKey] as Sprite;

            if (downloadingAssets.Contains(spriteKey))
            {
                while (!DownloadedAssets.TryGetValue(spriteKey, out UnityEngine.Object sp))
                {
                    await UniTask.Delay(30);
                }
                return DownloadedAssets[spriteKey] as Sprite;
            }

            downloadingAssets.Add(spriteKey);
            var texture = await DownloadTextureData(url);
            downloadingAssets.Remove(spriteKey);

            if (texture == null) return null;

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            DownloadedAssets[spriteKey] = (sprite);
            DownloadedAssets[textureKey] = (texture);
            return sprite;
        }

        private static async UniTask<string> DownloadFromHttp(string url)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                // request.SetRequestHeader("mode", "no-cors");
                await request.SendWebRequest();
                if (request.result != UnityWebRequest.Result.Success)
                {
                    throw new Exception("Error downloading JSON: " + request.error);
                }
                return request.downloadHandler.text;
            }
        }

        private static async UniTask<string> ReadFromFile(string filePath)
        {
            await UniTask.SwitchToMainThread();

            string json = File.ReadAllText(filePath);
            return json;
        }

        #endregion

        public static Dictionary<string, IResourceLocator> locators = new Dictionary<string, IResourceLocator>();

        #region Download 
        private static Texture2D ChangeTextureFormat(Texture2D original, TextureFormat newFormat)
        {
            // 创建一个新的Texture2D对象，使用新的格式
            Texture2D newTexture = new Texture2D(original.width, original.height, newFormat, false);

            // 将原始纹理的像素复制到新纹理中
            newTexture.SetPixels(original.GetPixels());
            newTexture.Apply(); // 应用更改

            return newTexture;
        }

        private static List<string> downloadingAssets = new List<string>();

        private static async UniTask<T> LoadAddressableObject<T>(string address) where T : UnityEngine.Object
        {
            T objToReturn = null;
            var strings = address.Split("#");
            string catalogPath = "";
            string modelName = null;
            if (strings.Length >= 2)
            {
                var modelUrl = strings[0];
                modelName = strings[1];
                Debug.Log("Load AddressableObject : " + modelName + "|" + modelUrl);
                catalogPath = PHUtils.SplitUrl(modelUrl); // todo>?
            }
            else
            {
                modelName = address;
            }
            objToReturn = await LoadAddressableAsset<T>(modelName, catalogPath);
            return objToReturn;
        }

        private static async UniTask<T> LoadAddressableAsset<T>(string modelName, string modelUrl) where T : UnityEngine.Object
        {
            IResourceLocator locator;
            IList<IResourceLocation> locations;

            T s = null;
            if (modelUrl == "" || !modelUrl.Contains(".json"))
            {
                s = await LoadAAAsset<T>(modelName);
            }
            else
            {
                if (locators.ContainsKey(modelUrl))
                {
                    locator = locators[modelUrl];
                }
                else
                {
                    locator = await Addressables.LoadContentCatalogAsync(modelUrl);
                    locators[modelUrl] = locator;
                }
                if (locator == null)
                {
                    s = await LoadAAAsset<T>(modelName);
                }
                else
                {
                    locator.Locate(modelName, typeof(T), out locations);
                    if (locations == null)
                    {
                        Debug.LogError("Cannot find " + modelName + " in " + modelUrl);
                        s = null;
                    }
                    else
                    {
                        s = await Addressables.LoadAssetAsync<T>(locations[0]);
                    }

                }
            }

            if (s == null)
            {
                s = await LoadAAAsset<T>(modelName);
            }
            return s;
        }

        private async static UniTask<T> LoadAAAsset<T>(object modelName) where T : UnityEngine.Object
        {
            if (modelName.ToString().Contains("Assets/Resources/"))
            {
                modelName = modelName.ToString().Replace("Assets/Resources/", "Assets/Resources_moved/");
            }
            var s = await Addressables.LoadAssetAsync<T>(modelName);
            return s;
        }

        #endregion
    }

}

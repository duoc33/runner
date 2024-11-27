using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;

namespace Modules.BackgroundMusic
{
    
    public class BackgroundMusicData : ExecutorData
    {
        [Header("Common Settings")]
        public List<Asset<AudioClip>> backgroundMusicList;     // Music asset to play
        public float volume = 1f;          // Music volume
        public float fadeDuration = 1f;    // Fade-in/out duration in seconds
        public bool loop;
        public bool random;


        public override async UniTask Config(object o)
        {
            for (var i = 0; i < backgroundMusicList.Count; i++)
            {
                await backgroundMusicList[i].Config(o);
            }
        }


        public override Type GetMonoType()
        {
            return typeof(BackgroundMusic);
        }
    }
}
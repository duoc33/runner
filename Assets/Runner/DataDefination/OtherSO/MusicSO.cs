using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ScriptableObjectBase;
using UnityEngine;
namespace Runner
{
    public class MusicSO : SOBase
    {
        public string RuntimeMusic;
        public string WinMusic;
        public string FailedMusic;
        public string CombatMusic;
        public string EnterTriggerMusic;

        private AudioClip runtimeMusic;
        private AudioClip winMusic;
        private AudioClip failedMusic;
        private AudioClip combatMusic;
        private AudioClip enterTriggerMusic;

        public override async UniTask Download()
        {
            runtimeMusic = await DownloadAudio(RuntimeMusic);
            winMusic = await DownloadAudio(WinMusic);
            failedMusic = await DownloadAudio(FailedMusic);
            combatMusic = await DownloadAudio(CombatMusic);
            enterTriggerMusic = await DownloadAudio(EnterTriggerMusic);
        }

        private GameObject musicGO;
        private AudioSource[] audioSources;


        public override void StartMixComponents()
        {
            musicGO = new GameObject("MusicGO");
            audioSources = new AudioSource[2];
            audioSources[0] = musicGO.AddComponent<AudioSource>();
            audioSources[0].playOnAwake = false;
            audioSources[0].priority = 1;
            audioSources[1] = musicGO.AddComponent<AudioSource>();
            audioSources[1].playOnAwake = false;
            audioSources[1].priority = 0;
        }

        public override void OnDestroy()
        {
            Destroy(musicGO);
        }

        public void PlayRuntimeMusic()
        {
            PlayMusic(runtimeMusic, 0, true);
        }

        public void PlayWinMusic()
        {
            PlayMusic(winMusic, 0, true);
        }

        public void PlayFailedMusic()
        {
            PlayMusic(failedMusic, 0,true);
        }

        public void PlayCombatMusic(bool stop = false)
        {
            PlayMusic(combatMusic, 1 , true , stop);
        }
        public void PlayEnterTriggerMusic(bool stop = false)
        {
            PlayMusic(enterTriggerMusic , 1 , false , stop);
        }
        public void Mute()
        {
            AudioSource audioSource = audioSources[1];
            audioSource.Stop();
        }
        private void PlayMusic(AudioClip audioClip, int index = 0, bool loop = false , bool stop = false)
        {
            if (audioClip == null)
            {
                // Debug.LogWarning("无法播放音乐，因为音频片段为空！");
                return;
            }
            AudioSource audioSource = audioSources[index];
            if(audioSource==null) return;
            if(stop)
            {
                if(audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
                return;
            }
            audioSource.loop = loop;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }

}

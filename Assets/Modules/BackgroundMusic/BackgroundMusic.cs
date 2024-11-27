using System.Collections;
using Managers;
using UnityEngine;

namespace Modules.BackgroundMusic
{
    public class BackgroundMusic : ExecutorBehaviour<BackgroundMusicData>
    {
        [SerializeField] private AudioSource audioSource;
        private int musicCount;
        private int currentIndex = 0;

        protected override void StartUp()
        {
            audioSource = PHUtils.TryAddComponent<AudioSource>(gameObject);
            musicCount = m_data.backgroundMusicList.Count;
            audioSource.volume = m_data.volume;

            if (musicCount == 0)
            {
                Debug.LogWarning("BackgroundMusic: No music clips found in the list.");
                return;
            }
            
            DataManager.EventContainer.AddHandler("NextBackgroundMusic", NextBGM);
            DataManager.EventContainer.AddHandler("PlayBackgroundMusic", PlayBGM);
            DataManager.EventContainer.AddHandler("StopBackgroundMusic", StopBGM);
            DataManager.EventContainer.AddHandler("PauseBGMBackgroundMusic", PauseBGM);
            DataManager.EventContainer.AddHandler("UnPauseBGMBackgroundMusic", UnPauseBGM);
            
            // InputManager.Create(gameObject).AddInput("NextBackgroundMusic", KeyCode.Alpha1, InputButton.Down).OnInputDown.AddListener(NextBGM);
            // InputManager.Create(gameObject).AddInput("PlayBackgroundMusic", KeyCode.Alpha2, InputButton.Down).OnInputDown.AddListener(PlayBGM);
            // InputManager.Create(gameObject).AddInput("StopBackgroundMusic", KeyCode.Alpha3, InputButton.Down).OnInputDown.AddListener(StopBGM);
            // InputManager.Create(gameObject).AddInput("PauseBGMBackgroundMusic", KeyCode.Alpha4, InputButton.Down).OnInputDown.AddListener(PauseBGM);
            // InputManager.Create(gameObject).AddInput("UnPauseBGMBackgroundMusic", KeyCode.Alpha5, InputButton.Down).OnInputDown.AddListener(UnPauseBGM);
            
            PlayBGM();
        }
        
        
        private void PlayBGM()
        {
            AudioClip clip = m_data.backgroundMusicList[currentIndex];
            StartCoroutine(FadeIn(clip));

            Debug.Log("Playing Background Music");
        }
        
        private void StopBGM()
        {
            StartCoroutine(FadeOut(() =>
            {
                audioSource.Stop();
                Debug.Log("Background Music Stopped.");
            }));
        }

        private void PauseBGM()
        {
            audioSource.Pause();
        }

        private void UnPauseBGM()
        {
            audioSource.UnPause();
        }
        
        private void NextBGM()
        {
            // StopBGM(s);
            if (musicCount == 0)
            {
                Debug.LogWarning("BackgroundMusic: No music available to play.");
                return;
            }
            if (m_data.random)
            {
                currentIndex = Random.Range(0, musicCount);
                audioSource.loop = false;
            }
            else
            {
                currentIndex = (currentIndex + 1) % musicCount;
                audioSource.loop = m_data.loop;
            }
            PlayBGM();
        }
        
        private IEnumerator FadeIn(AudioClip clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
            audioSource.volume = 0f;

            float elapsedTime = 0f;
            while (elapsedTime < m_data.fadeDuration)
            {
                audioSource.volume = Mathf.Lerp(0f, m_data.volume, elapsedTime / m_data.fadeDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            audioSource.volume = m_data.volume;
        }
        
        private IEnumerator FadeOut(System.Action onComplete)
        {
            float initialVolume = audioSource.volume;
            float elapsedTime = 0f;

            while (elapsedTime < m_data.fadeDuration)
            {
                audioSource.volume = Mathf.Lerp(initialVolume, 0f, elapsedTime / m_data.fadeDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            audioSource.volume = 0f;
            onComplete?.Invoke();
        }
        
        
    }
}
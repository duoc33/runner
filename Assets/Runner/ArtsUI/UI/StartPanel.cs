using Cysharp.Threading.Tasks;
using QFramework;
using UnityEngine.UI;
namespace Runner
{
    public class StartPanel : RunnerController
    {
        Button btnStart;
        void Start()
        {
            btnStart = GetComponentInChildren<Button>();
            btnStart.onClick.AddListener(StartGame);
        }
        async void StartGame()
        {
            await UniTask.Yield();
            Data.MusicSO?.PlayRuntimeMusic();
            this.SendCommand<OnStartGenerateMapCmd>();
            gameObject.SetActive(false);
        }
        void OnDestroy()
        {
            btnStart?.onClick.RemoveListener(StartGame);
        }
    }
}


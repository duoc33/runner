using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using ScriptableObjectBase;
using UnityEngine;
namespace Runner
{
    public class UIConfig : SOBase
    {
        public string StartPanelImage;
        public string StartButtonImage;
       

        public string ScoreImage;
        public string PlayerCountImage;
        

        public string SuccessPanelImage;
        public string FailedPanelImage;
        

        public string PlayAgainButtonImage;
        
        [JsonIgnore]
        public Sprite StartPanelSprite;
        [JsonIgnore]
        public Sprite StartButtonSprite;
        [JsonIgnore]
        public Sprite ScoreSprite;
        [JsonIgnore]
        public Sprite PlayerCountSprite;
        [JsonIgnore]
        public Sprite SuccessPanelSprite;
        [JsonIgnore]
        public Sprite FailedPanelSprite;
        [JsonIgnore]
        public Sprite PlayAgainButtonSprite;
        
        public override async UniTask Download()
        {
            StartPanelSprite = await DownloadSprite(StartPanelImage);
            StartButtonSprite = await DownloadSprite(StartButtonImage);
            ScoreSprite = await DownloadSprite(ScoreImage);
            PlayerCountSprite = await DownloadSprite(PlayerCountImage);
            SuccessPanelSprite = await DownloadSprite(SuccessPanelImage);
            FailedPanelSprite = await DownloadSprite(FailedPanelImage);
            PlayAgainButtonSprite = await DownloadSprite(PlayAgainButtonImage);
        }
        public void Apply(UIInfo uIInfo)
        {
            uIInfo.StartPanelSprite.sprite = StartPanelSprite;
            uIInfo.StartButtonSprite.sprite = StartButtonSprite;
            uIInfo.ScoreSprite.sprite = ScoreSprite;
            uIInfo.PlayerCountSprite.sprite = PlayerCountSprite;
            uIInfo.SuccessPanelSprite.sprite = SuccessPanelSprite;
            uIInfo.FailedPanelSprite.sprite = FailedPanelSprite;
            uIInfo.PlayAgainButtonFailedSprite.sprite = PlayAgainButtonSprite;
            uIInfo.PlayAgainButtonSuccessSprite.sprite = PlayAgainButtonSprite;
        }
    }
}

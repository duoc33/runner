using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Runner
{
    public class UIInfo : MonoBehaviour
    {
        public void Init()
        {
            StartPanel(transform.GetChild(0));
            RuntimePanel(transform.GetChild(1));
            SuccessPanel(transform.GetChild(2));
            FailedPanel(transform.GetChild(3));
        }

        private void StartPanel(Transform target)
        {
            target.gameObject.AddComponent<StartPanel>();
            StartPanelSprite = target.GetComponent<Image>();
            StartButtonSprite = target.GetChild(0).GetComponent<Image>();
        }

        private void RuntimePanel(Transform target)
        {
            RuntimePanel runtimePanel = target.gameObject.AddComponent<RuntimePanel>();
            ScoreSprite = target.GetChild(0).GetComponent<Image>();
            PlayerCountSprite = target.GetChild(1).GetComponent<Image>();
            runtimePanel.score = ScoreSprite.GetComponentInChildren<TextMeshProUGUI>();
            runtimePanel.player = PlayerCountSprite.GetComponentInChildren<TextMeshProUGUI>();
        }

        private void SuccessPanel(Transform target)
        {
            target.gameObject.AddComponent<SuccessPanel>();
            SuccessPanelSprite = target.GetComponent<Image>();
            PlayAgainButtonSuccessSprite = target.GetComponentInChildren<Button>().gameObject.AddComponent<PlayAgainButton>().GetComponent<Image>();
        }
        private void FailedPanel(Transform target)
        {
            FailedPanelSprite = target.GetComponent<Image>();
            PlayAgainButtonFailedSprite = target.GetComponentInChildren<Button>().gameObject.AddComponent<PlayAgainButton>().GetComponent<Image>();
        }


        public Image StartPanelSprite;
        public Image StartButtonSprite;
        public Image ScoreSprite;
        public Image PlayerCountSprite;
        public Image SuccessPanelSprite;
        public Image FailedPanelSprite;
        public Image PlayAgainButtonSuccessSprite;
        public Image PlayAgainButtonFailedSprite;
    }

}

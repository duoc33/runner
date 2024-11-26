using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
namespace Runner
{
    public class UISystem : AbstractSystem
    {
        private string UICanvasPath = "Runner/UICanvas/UICanvas";
        private GameObject uiCanvas;
        // private int startPanelIndex = 0;
        // private int runtimePanelIndex = 1;
        // private int successPanelIndex = 2;
        // private int failedPanelIndex = 3;
        protected override void OnInit()
        {
            
        }
        public void Show(int index)
        {
            if(uiCanvas == null)
            {
                uiCanvas = Object.Instantiate(Resources.Load<GameObject>(UICanvasPath));
            }
            uiCanvas.transform.GetChild(index).gameObject.SetActive(true);
        }
        public void Hide(int index)
        {
            if(uiCanvas == null)
            {
                uiCanvas = Object.Instantiate(Resources.Load<GameObject>(UICanvasPath));
            }
            uiCanvas.transform.GetChild(index).gameObject.SetActive(false);
        }

        protected override void OnDeinit()
        {
            Object.Destroy(uiCanvas);
        }
    }
}


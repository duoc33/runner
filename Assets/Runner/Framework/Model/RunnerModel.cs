using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
namespace Runner
{
    public class RunnerModel : AbstractModel
    {
        #region General相关的
        public GeneralSO GeneralSO;

        public MusicSO MusicSO => GeneralSO?.MusicSO;

        public VFXSO VFXSO => GeneralSO?.VFXSO;

        #endregion

        #region AgentGroup相关的
        public Transform PlayerGroupTarget;
        #endregion

        #region 由关卡生成的数据
        public StraightPathLevelSO levelSO;
        public List<Transform> LocomotionGroup => levelSO.Rule.LocomotionGroup;
        public Vector3 PlayerStartPos => levelSO.Rule.AimStartPos();
        public float SpawnColumnSpacing => levelSO.Rule.SpawnColumnSpacing;
        public Vector2 MapXRange => new Vector2(levelSO.GetMapBounds().min.x , levelSO.GetMapBounds().max.x) ;
        public Vector2 MapZRange => new Vector2(levelSO.GetMapBounds().min.z , levelSO.GetMapBounds().max.z) ;
        public GameObject LastLevelItem => levelSO.Rule.LastLevelItem;
        #endregion

        #region UI相关数据
        public BindableProperty<int> Score;
        public BindableProperty<int> PlayerCount;
        #endregion

        protected override void OnInit()
        {
            Score = new BindableProperty<int>();
            PlayerCount = new BindableProperty<int>();
        }
        protected override void OnDeinit()
        {
            base.OnDeinit();
        }
    }
    public class OnGameOverCmd : AbstractCommand
    {
        public bool isWin;
        protected override void OnExecute()
        {
            this.SendEvent(new OnGameOverEvent() { IsWin = isWin });
            UISystem uISystem  =this.GetSystem<UISystem>();
            uISystem.Hide(0);
            uISystem.Hide(1);
            RunnerModel data = this.GetModel<RunnerModel>();
            data.MusicSO?.Mute();
            if(isWin)
            {
                data.MusicSO?.PlayWinMusic();
                uISystem.Show(2);
            }
            else{
                data.MusicSO?.PlayFailedMusic();
                uISystem.Show(3);
            }
        }
    }
    public struct OnGameOverEvent
    {
        public bool IsWin;
    }
}

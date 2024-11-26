using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ScriptableObjectBase;
using UnityEngine;
namespace Runner
{
    public class GeneralSO : SOBase
    {
        public MusicSO MusicSO;
        public VFXSO VFXSO;
        public override async UniTask Download()
        {
            if(MusicSO!=null)
            {
                await MusicSO.Download();
            }
            
            if(VFXSO!=null)
            {
                await VFXSO.Download();
            }
            
        }
        public override void StartMixComponents()
        {
            MusicSO?.StartMixComponents();
            VFXSO?.StartMixComponents();
        }
        public override void OnDestroy()
        {
            MusicSO?.OnDestroy();
            VFXSO?.OnDestroy();
        }
    }
}
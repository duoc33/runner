using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
namespace Runner
{
    public abstract class RunnerController : MonoBehaviour , IController
    {
        public IArchitecture GetArchitecture() => RunnerApp.Interface;
        protected RunnerModel Data;
        protected virtual void Awake()
        {
            Data = this.GetModel<RunnerModel>();
        }
    }
}


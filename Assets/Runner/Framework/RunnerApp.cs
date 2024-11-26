using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
namespace Runner
{
    public class RunnerApp : Architecture<RunnerApp>
    {
        protected override void Init()
        {
            RegisterModel(new RunnerModel());
            RegisterSystem(new UISystem());
        }
    }
}

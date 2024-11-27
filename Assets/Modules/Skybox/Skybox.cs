using Managers;
using UnityEngine;

namespace Modules.Skybox
{
    public class Skybox : ExecutorBehaviour<SkyboxData>
    {
        protected override void StartUp()
        {
            DynamicGI.UpdateEnvironment();
        }
    }
}
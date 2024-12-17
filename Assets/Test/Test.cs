using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeComponentAdjustTools;
using Runner;
using TMPro;
using ScriptableObjectBase;
using Cysharp.Threading.Tasks;
using UnityEngine.AI;
using QFramework;
using UnityEngine.InputSystem;
using Foundation;
using Managers;

public class Test : RunnerController
{
    public AllConfig allConfig;
    protected override void Awake()
    {
        base.Awake();
        SOBase.InitLocal();
    }
    void Start()
    {
       
        string json = Serializer.Serialize( ScriptableObject.CreateInstance<UIConfig>());
        Serializer.WriteAllText(json,null, "UIConfig");
        // Run().Forget();
    }
    public async UniTask Run()
    {
        await allConfig.Download();
        allConfig.StartMixComponents();
    }

    void OnDestroy()
    {
        allConfig.OnDestroy();
        SOBase.Clear();
    }
}
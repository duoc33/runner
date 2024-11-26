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
        Run().Forget();
        // string Config = Serializer.Serialize(allConfig);
        // Serializer.WriteAllText(Config);
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
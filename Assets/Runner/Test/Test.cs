using UnityEngine;
using Runner;
using ScriptableObjectBase;
using Cysharp.Threading.Tasks;

public class Test : RunnerController
{
    public AllConfig allConfig;
    public GameObject cube;
    protected override void Awake()
    {
        base.Awake();
        SOBase.InitLocal();
    }
    async void Start()
    {
        // Serializer.WriteAllText(Serializer.Serialize(allConfig));
        await Run();
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
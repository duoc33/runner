using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using ScriptableObjectBase;
using Foundation;
using Cysharp.Threading.Tasks;
using Runner;

public class ParseMethod : MonoBehaviour
{
    void Start()
    {
        AllConfig allConfig = null;
        UIConfig uiConfig = null;

        GameLogic.PARSE_ACTION = async (string json) => {

            SOBase.InitServer();

            var list = Serializer.Deserialize<List<object>>(json);

            foreach (var item in list)
            {
                if(item is ExecutorData executorData){
                    await executorData.Active(new GameObject(executorData.GetMonoType().ToString()));
                }
                else if(item is AllConfig runner){
                    allConfig = runner;
                    await runner.Download();
                    // runner.StartMixComponents();
                }
                else if(item is UIConfig message){
                    uiConfig = message;
                    await uiConfig.Download();
                }
            }

            allConfig.SetUIConfig(uiConfig);
            allConfig.StartMixComponents();

            return true;
        };
    }
}

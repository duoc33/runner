using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using ScriptableObjectBase;
using Foundation;
using Cysharp.Threading.Tasks;

public class ParseMethod : MonoBehaviour
{
    void Start()
    {
        GameLogic.PARSE_ACTION = async (string json) => {

            SOBase.InitServer();

            var list = Serializer.Deserialize<List<object>>(json);

            foreach (var item in list)
            {
                if(item is ExecutorData executorData){
                    await executorData.Active(new GameObject(executorData.GetMonoType().ToString()));
                }
                else if(item is SOBase sOBase){
                    await sOBase.Download();
                }
            }

            return true;
        };
    }



}

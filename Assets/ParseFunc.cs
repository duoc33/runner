using System.Collections;
using System.Collections.Generic;
using Foundation;
using Managers;
using Runner;
using UnityEngine;

public class ParseFunc : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameLogic.PARSE_ACTION = async (string json) => {
            List<ExecutorData> executorDatas = await Serializer.Deserialize<List<ExecutorData>>(json);
            foreach (var item in executorDatas)
            {
                if(item is RunnerData runnerData)
                {
                    await runnerData.Active(new GameObject("Runner"));
                }
            }
            return true;
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

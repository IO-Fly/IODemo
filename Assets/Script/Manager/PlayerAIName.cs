using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAIName {


    static string[] nameList =
    {
        "真",
        "的",
        "没",
        "时",
        "间",
        "做",
        "啊",
        "周",
        "六",
        "还",
        "要",
        "加",
        "班",
    };

    // 暂不考虑AI与真玩家的重名
    public static List<string> curNameList = new List<string>();

    public static string GetUniqueName()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        string name;
        while (true)
        {
            int randomIndex = Random.Range(0, nameList.Length);
            name = nameList[randomIndex];
            if (!curNameList.Contains(name)){
                curNameList.Add(name);
                break;
            }
        }

        return name;
    }

}

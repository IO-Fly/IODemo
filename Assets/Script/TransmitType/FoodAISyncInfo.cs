using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodAISyncInfo{

    static int SyncNum = 8;
    public Vector3 position;
    public Quaternion rotation;
    public int ID;

    public static float[] Serialize(GameObject[] foodAIInstances)
    {
        int FoodAICount = foodAIInstances.Length;
        float[] foodAIInfo = new float[SyncNum * FoodAICount];
        for (int i = 0; i < FoodAICount; i++)
        {
            foodAIInfo[SyncNum * i + 0] = foodAIInstances[i].transform.position.x;
            foodAIInfo[SyncNum * i + 1] = foodAIInstances[i].transform.position.y;
            foodAIInfo[SyncNum * i + 2] = foodAIInstances[i].transform.position.z;
            foodAIInfo[SyncNum * i + 3] = foodAIInstances[i].transform.rotation.x;
            foodAIInfo[SyncNum * i + 4] = foodAIInstances[i].transform.rotation.y;
            foodAIInfo[SyncNum * i + 5] = foodAIInstances[i].transform.rotation.z;
            foodAIInfo[SyncNum * i + 6] = foodAIInstances[i].transform.rotation.w;
            foodAIInfo[SyncNum * i + 7] = foodAIInstances[i].GetComponent<SyncTranform>().ID;
        }
        return foodAIInfo;
    }

    public static FoodAISyncInfo[] Deserialize(float[] foodAIInfo)
    {
        int FoodAICount = foodAIInfo.Length / SyncNum;
        FoodAISyncInfo[] foodAIInfoObject = new FoodAISyncInfo[FoodAICount];
        for (int i = 0; i < FoodAICount; i++)
        {
            foodAIInfoObject[i] = new FoodAISyncInfo();
            foodAIInfoObject[i].position.x = foodAIInfo[SyncNum * i + 0];
            foodAIInfoObject[i].position.y = foodAIInfo[SyncNum * i + 1];
            foodAIInfoObject[i].position.z = foodAIInfo[SyncNum * i + 2];
            foodAIInfoObject[i].rotation.x = foodAIInfo[SyncNum * i + 3];
            foodAIInfoObject[i].rotation.y = foodAIInfo[SyncNum * i + 4];
            foodAIInfoObject[i].rotation.z = foodAIInfo[SyncNum * i + 5];
            foodAIInfoObject[i].rotation.w = foodAIInfo[SyncNum * i + 6];
            foodAIInfoObject[i].ID = (int)foodAIInfo[SyncNum * i + 7];
        }
        return foodAIInfoObject;
    }


}

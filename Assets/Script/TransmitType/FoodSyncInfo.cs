using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSyncInfo {

    static int SyncNum = 7;
    public Vector3 position;
    public Vector3 translation;
    public int ID;

    public static float[] Serialize(GameObject[] foodList)
    {
        int foodCount = foodList.Length;
        float[] foodInfo = new float[SyncNum * foodCount];
        for (int i = 0; i < foodCount; i++)
        {
            foodInfo[i * SyncNum + 0] = foodList[i].transform.position.x;
            foodInfo[i * SyncNum + 1] = foodList[i].transform.position.y;
            foodInfo[i * SyncNum + 2] = foodList[i].transform.position.z;
            foodInfo[i * SyncNum + 3] = foodList[i].GetComponent<FoodOverrideController>().translation.x;
            foodInfo[i * SyncNum + 4] = foodList[i].GetComponent<FoodOverrideController>().translation.y;
            foodInfo[i * SyncNum + 5] = foodList[i].GetComponent<FoodOverrideController>().translation.z;
            foodInfo[i * SyncNum + 6] = foodList[i].GetComponent<FoodOverrideController>().ID;
        }
        return foodInfo;
    }

    public static FoodSyncInfo[] Deserialize(float[] foodInfo)
    {
        int foodCount = foodInfo.Length / SyncNum;
        FoodSyncInfo[] foodInfoObject = new FoodSyncInfo[foodCount];
        for (int i = 0; i < foodCount; i++)
        {
            foodInfoObject[i] = new FoodSyncInfo();
            foodInfoObject[i].position.x = foodInfo[SyncNum * i + 0];
            foodInfoObject[i].position.y = foodInfo[SyncNum * i + 1];
            foodInfoObject[i].position.z = foodInfo[SyncNum * i + 2];
            foodInfoObject[i].translation.x = foodInfo[SyncNum * i + 3];
            foodInfoObject[i].translation.y = foodInfo[SyncNum * i + 4];
            foodInfoObject[i].translation.z = foodInfo[SyncNum * i + 5];
            foodInfoObject[i].ID = (int)foodInfo[SyncNum * i + 6];
        }
        return foodInfoObject;
    }


}

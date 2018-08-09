using UnityEngine;
using System.Collections;

public class bl_MMExampleManager : MonoBehaviour {

    public int MapID = 2;
    public const string MMName = "MMManagerExample";

    public GameObject[] Maps;
    private bool Rotation = true;
    private bl_MiniMap CurrentMiniMap;

    void Awake()
    {
        MapID = PlayerPrefs.GetInt("MMExampleMapID", 0);
        ApplyMap();
    }

    void ApplyMap()
    {
        for (int i = 0; i < Maps.Length; i++)
        {
            Maps[i].SetActive(false);
        }

        Maps[MapID].SetActive(true);
        CurrentMiniMap = Maps[MapID].GetComponentInChildren<bl_MiniMap>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ChangeRotation();
        }
    }

    void ChangeRotation()
    {
        Rotation = !Rotation;
        Maps[MapID].GetComponentInChildren<bl_MiniMap>().RotationMap(Rotation);

    }

    public void SetIconMulti(float v)
    {
        CurrentMiniMap.IconMultiplier = v;
    }

    public void SetGridSize(float v)
    {
        CurrentMiniMap.SetGridSize(v);
    }

    public void SetGrid(bool v)
    {
        CurrentMiniMap.SetActiveGrid(v);
    }

    public void SetDynamicRot(bool v)
    {
        CurrentMiniMap.SetMapRotation(v);
    }

    public void ChangeMap(int i)
    {
        PlayerPrefs.SetInt("MMExampleMapID",i);
        Application.LoadLevel(Application.loadedLevel);
    }

    public bl_MiniMap GetActiveMiniMap
    {
        get
        {
            bl_MiniMap m = Maps[MapID].GetComponentInChildren<bl_MiniMap>();
            return m;
        }
    }
}
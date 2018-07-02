using UnityEditor;
using UnityEngine;

public class MeshPresistence
{
    [MenuItem("Tools/Mesh/Presistence")]
    public static void Presistence()
    {
        GameObject selectedGo = Selection.activeGameObject;
        MeshFilter meshFliter = selectedGo.GetComponent<MeshFilter>();

        meshFliter.mesh = selectedGo.GetComponent<LowPolyBackground>().GenerateLowPoly();
    }

}

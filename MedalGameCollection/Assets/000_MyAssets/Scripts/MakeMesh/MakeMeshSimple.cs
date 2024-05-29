using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityMeshSimplifier;

public class MakeMeshSimple : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MeshSimplifier simplifier = new MeshSimplifier();
        simplifier.Initialize(this.GetComponent<MeshFilter>().mesh);
        simplifier.SimplifyMesh(0.1f);
        Mesh simplifiedMesh = simplifier.ToMesh();

#if UNITY_EDITOR
        string path = "Assets/000_MyAssets/Resources/MedalSimplifiedMesh.asset";
        UnityEditor.AssetDatabase.CreateAsset(simplifiedMesh, path);
#endif
    }

}

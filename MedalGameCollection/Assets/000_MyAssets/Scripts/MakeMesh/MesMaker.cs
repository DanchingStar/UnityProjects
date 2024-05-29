using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MakeMesh
{
    public class MesMaker : MonoBehaviour
    {
        public Material material;

        void Start()
        {
            GameObject trapezoid = new GameObject("TriangularPrism");

            // Add MeshFilter and MeshRenderer components
            MeshFilter mf = trapezoid.AddComponent<MeshFilter>();
            MeshRenderer mr = trapezoid.AddComponent<MeshRenderer>();

            Vector3[] verts = new Vector3[6];
            int[] trinangles =
                {
                0,1,2,
                1,3,2,
                0,2,4,
                1,5,3,
                0,4,1,
                1,4,5,
                2,3,4,
                3,5,4
                };

            verts[0] = new Vector3(0, 0, 0);
            verts[1] = new Vector3(30, 0, 0);
            verts[2] = new Vector3(0, 0, 20);
            verts[3] = new Vector3(30, 0, 20);
            verts[4] = new Vector3(0, 34.6f, 20);
            verts[5] = new Vector3(30, 34.6f, 20);

            // Create the mesh
            Mesh mesh = new Mesh();
            mesh.vertices = verts;
            mesh.triangles = trinangles;
            mesh.RecalculateNormals();

#if UNITY_EDITOR
            // Save the mesh to the Resources folder
            AssetDatabase.CreateAsset(mesh, "Assets/000_MyAssets/Resources/TriangularPrismMesh.asset");
#endif


            // Assign the mesh and material
            mf.sharedMesh = mesh;
            mr.sharedMaterial = material;


#if UNITY_EDITOR
            // Save the game object as a prefab
            string localPath = "Assets/000_MyAssets/Prefabs/TriangularPrism.prefab";
            PrefabUtility.SaveAsPrefabAsset(trapezoid, localPath);
#endif
        }
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor; 

public class MeshCreator : MonoBehaviour {
  
  private void Start() {
    float sqrtThree = Mathf.Sqrt(3f);
    Vector2[] triangle = new Vector2[] {
      new Vector2(-1f / 3f, sqrtThree * 2f / 3f),
      new Vector2(2f / 3f, -sqrtThree / 3f),
      new Vector2(-1f / 3f, -sqrtThree / 3f)
    };
    for (int i = 0; i < 3; i++) {
      // triangle[i] /= 3f; // 0.333
      // triangle[i] *= 0.5f; // 0.5
      // triangle[i] /= sqrtThree; // 0.577 ***
      // triangle[i] *= 0.5f * sqrtThree; // 0.86
      // neutral // 1
      // triangle[i] *= 2f / sqrtThree; // 1.154
      // triangle[i] *= sqrtThree; // 1.732
      // triangle[i] *= 2f; // 2
      
      // SCREW THIS just mult by mults of rt3 
      
      triangle[i] *= 0.5f / sqrtThree;
      
    }
    Mesh mesh = new Mesh();
    mesh.vertices = System.Array.ConvertAll<Vector2, Vector3>(triangle, v => v);
    mesh.triangles = new int[] { 0, 1, 2 };
    AssetDatabase.CreateAsset(mesh, "Assets\\TrigonMeshes\\N-5.mesh");
  }
}
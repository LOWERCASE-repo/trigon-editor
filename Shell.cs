using UnityEngine;
using System.Collections.Generic;

public class Shell : MonoBehaviour {
  
  private Vector2 posMid;
  private Vector3[] poss = new Vector3[2];
  private Color color;
  
  [Header("Components")]
  [SerializeField]
  private LineRenderer renderer;
  [SerializeField]
  private Transform vertexA;
  [SerializeField]
  private Transform vertexB;
  
  [Header("Debug")]
  [Range(0f, 1f)]
  public float vis = 1f;
  
  private void Update() {
    // TODO pass in tri array and index
    posMid = (poss[0] + poss[1]) / 2f;
    poss[0] = Vector2.Lerp(posMid, vertexA.localPosition, vis);
    poss[1] = Vector2.Lerp(posMid, vertexB.localPosition, vis);
    renderer.SetPositions(poss);
    
    color = renderer.startColor;
    color.a = Mathf.Sqrt(vis);
    renderer.startColor = color;
    renderer.endColor = renderer.startColor;
  }
}
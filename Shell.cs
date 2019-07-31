using UnityEngine;
using System.Collections.Generic;

public class Shell : MonoBehaviour {
  
  private Vector2 posMid;
  private Vector3[] poss = new Vector3[2];
  
  [Header("Components")]
  [SerializeField]
  private Trigon trigon;
  [SerializeField]
  private LineRenderer renderer;
  [SerializeField]
  private Transform vertexA;
  [SerializeField]
  private Transform vertexB;
  
  private void Update() {
    posMid = (poss[0] + poss[1]) / 2f;
    poss[0] = Vector2.Lerp(posMid, vertexA.localPosition, trigon.shellLen);
    poss[1] = Vector2.Lerp(posMid, vertexB.localPosition, trigon.shellLen);
    renderer.SetPositions(poss);
    
    Color color = trigon.shellColor;
    color.a = Mathf.Sqrt(trigon.shellLen); // should be in trigon?
    renderer.startColor = color;
    renderer.endColor = renderer.startColor;
  }
}
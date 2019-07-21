using UnityEngine;
using System.Collections.Generic;

public class Shell : MonoBehaviour {
  
  private Vector2 posMid;
  private Vector3[] poss = new Vector3[2];
  private Color color;
  
  // [SerializeField]
  // private float width = 0.1f;
  
  [Header("Debug")]
  [Range(0f, 1f)]
  public float vis = 1f;
  public Vector2 posA;
  public Vector2 posB;
  
  [Header("Components")]
  [SerializeField]
  private LineRenderer renderer;
  
  private void Update() {
    posMid = (poss[0] + poss[1]) / 2f;
    poss[0] = Vector2.Lerp(posMid, posA, vis);
    poss[1] = Vector2.Lerp(posMid, posB, vis);
    renderer.SetPositions(poss);
    
    color = renderer.startColor;
    color.a = Mathf.Sqrt(vis);
    renderer.startColor = color;
    renderer.endColor = renderer.startColor;
    // renderer.startWidth = width * Mathf.Sqrt(vis);
  }
}
using UnityEngine;
using System.Collections.Generic;

public class Shell : MonoBehaviour {
  
  private Vector2 posMid;
  private Vector3[] poss = new Vector3[2];
  private Color color;
  
  [Header("Debug")]
  [Range(0f, 1f)]
  public float vis = 1f;
  public Vector2 tetherA;
  public Vector2 tetherB;
  
  [Header("Components")]
  [SerializeField]
  private LineRenderer renderer;
  
  public void Init(Vector2 tetherA, Vector2 tetherB) {
    this.tetherA = tetherA;
    this.tetherB = tetherB;
  }
  
  private void Update() {
    // TODO pass in tri array and index
    posMid = (poss[0] + poss[1]) / 2f;
    poss[0] = Vector2.Lerp(posMid, tetherA, vis);
    poss[1] = Vector2.Lerp(posMid, tetherB, vis);
    renderer.SetPositions(poss);
    
    color = renderer.startColor;
    color.a = Mathf.Sqrt(vis);
    renderer.startColor = color;
    renderer.endColor = renderer.startColor;
  }
}
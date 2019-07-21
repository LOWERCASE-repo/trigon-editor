using UnityEngine;
using System.Collections.Generic;

public class Linker : MonoBehaviour {
  
  [Header("Debug")]
  public int legA;
  public int legB;
  public Vector2 dir = Vector2.zero;
  public float ang;
  
  private void OnTriggerStay2D(Collider2D col) {
    Linker other = col.gameObject.GetComponent<Linker>();
    float rot = transform.rotation.eulerAngles.z;
    float otherRot = other.transform.rotation.eulerAngles.z;
    float ab = Mathf.DeltaAngle(other.legB - otherRot, legA - rot);
    float ba = Mathf.DeltaAngle(other.legA - otherRot, legB - rot);
    
    ang = (Mathf.Abs(ab) < Mathf.Abs(ba)) ? ab : ba;
    dir = col.transform.position - transform.position;
    
    // verification makes multi cluster points emergent
  }
  
  private void OnTriggerExit2D(Collider2D col) {
    dir = Vector2.zero;
    ang = 0f;
  }
}
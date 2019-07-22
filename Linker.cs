using UnityEngine;
using System.Collections.Generic;

public class Linker : MonoBehaviour {
  
  private float sideLenA;
  private float sideLenB;
  public void SetLengths(int sideLenA, int sideLenB) {
    this.sideLenA = sideLenA;
    this.sideLenB = sideLenB;
    lenA = Mathf.Infinity;
    lenB = Mathf.Infinity;
  }
  
  private HashSet<Linker> linkers = new HashSet<Linker>();
  
  [Header("Debug")]
  public int angA; // inf for taken
  public int angB;
  public float lenA; // length of side left
  public float lenB;
  public float linkValue;
  
  // public Vector2 dir = Vector2.negativeInfinity;
  // public float ang = Mathf.NegativeInfinity;
  
  private void Attach() {
    Linker bestLinker;
    float bestValue;
    foreach (Linker linker in linkers) {
      if (EvalLinker(linker) > bestValue) {
        bestLinker = linker;
      }
    }
    
  }
  
  private float EvalLinker(Linker linker) { // need hashset for all?
    float ab = Mathf.NegativeInfinity; 
    float ba = Mathf.NegativeInfinity;
    float rot = transform.rotation.eulerAngles.z;
    float otherRot = linker.transform.rotation.eulerAngles.z;
    
    if (sideLenA < linker.lenB) {
      ab = Mathf.DeltaAngle(other.angB - otherRot, angA - rot);
    }
    if (sideLenA < linker.lenB) {
      ba = Mathf.DeltaAngle(other.angA - otherRot, angB - rot);
    }
    
    ab = Mathf.Abs(ab) - sideLenA;
    ba = Mathf.Abs(ba) - sideLenB; 
    
    return -1 * (ab < ba) ? ab : ba;
  }
  
  private void OnTriggerStay2D(Collider2D col) {
    
    // verify a-b, b-a
    /*
    for a-b
    check that this.sideLenA < other.avaLenB
      on init, avaLen = sideLen
      if avaLen = 0 on connection
    
    linkValue = angle - 
    
    if both true, is valid
    if both a-b, b-a, choose smaller angle, then bigger this.len
    
    
    onmouseup,
    sort by linkValue
    call Attach() on best linker
    
    */
    
    Linker other = col.gameObject.GetComponent<Linker>();
    float rot = transform.rotation.eulerAngles.z;
    float otherRot = other.transform.rotation.eulerAngles.z;
    float ab = Mathf.DeltaAngle(other.angB - otherRot, angA - rot);
    float ba = Mathf.DeltaAngle(other.angA - otherRot, angB - rot);
    Vector2 dir = col.transform.position - transform.position;
    // ang = (Mathf.Abs(ab) < Mathf.Abs(ba)) ? ab : ba;
  }
  
  private void OnTriggerEnter2D(Collider2D col) {
    linkers.Add(col.gameObject.GetComponent<Linker>());
  }
  
  private void OnTriggerExit2D(Collider2D col) {
    linkers.Remove(col.gameObject.GetComponent<Linker>());
  }
}
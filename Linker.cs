using UnityEngine;
using System.Collections.Generic;

public class Linker : MonoBehaviour {
  
  private Collider2D trigonCol;
  private Animator trigonAnimator;
  
  [Header("Debug")]
  public int legA;
  public int legB;
  public Vector2 dir = Vector2.zero;
  public float ang;
  
  private bool CheckValid() {
    Collider2D[] cols = new Collider2D[1];
    ContactFilter2D filter = new ContactFilter2D();
    trigonCol.OverlapCollider(filter.NoFilter(), cols);
    Debug.Log(cols[0] == null);
    return (cols[0] == null);
  }
  
  private bool CheckAngle(float ang) {
    transform.parent.RotateAround(transform.position, Vector3.forward, ang);
    bool valid;
    if (valid = CheckValid()) {
      this.ang = ang;
    }
    transform.parent.RotateAround(transform.position, Vector3.forward, -ang);
    return valid;
  }
  
  private void Awake() {
    trigonCol = transform.parent.GetComponent<Collider2D>();
    trigonAnimator = transform.parent.GetComponent<Animator>();
    dir = Vector2.negativeInfinity;
    ang = Mathf.NegativeInfinity;
  }
  
  private void OnTriggerStay2D(Collider2D col) {
    if (!trigonAnimator.GetBool("Selected")) return;
    Linker other = col.gameObject.GetComponent<Linker>();
    float rot = transform.rotation.eulerAngles.z;
    float otherRot = other.transform.rotation.eulerAngles.z;
    
    float ab = Mathf.DeltaAngle(other.legB - otherRot, legA - rot);
    float ba = Mathf.DeltaAngle(other.legA - otherRot, legB - rot);
    Vector2 dir = col.transform.position - transform.position;
    // ang = (Mathf.Abs(ab) < Mathf.Abs(ba)) ? ab : ba;
    
    Transform parent = transform.parent;
    parent.position += (Vector3)dir;
    if (Mathf.Abs(ab) < Mathf.Abs(ba)) {
      if (CheckAngle(ab) || CheckAngle(ba)) {
        this.dir = dir;
      } else {
        this.dir = Vector2.negativeInfinity;
        ang = Mathf.NegativeInfinity;
      }
    } else {
      if (CheckAngle(ba) || CheckAngle(ab)) {
        this.dir = dir;
      } else {
        this.dir = Vector2.negativeInfinity;
        ang = Mathf.NegativeInfinity;
      }
    }
    parent.position -= (Vector3)dir;
  }
  
  private void OnTriggerExit2D(Collider2D col) {
    dir = Vector2.negativeInfinity;
    ang = Mathf.NegativeInfinity;
  }
}
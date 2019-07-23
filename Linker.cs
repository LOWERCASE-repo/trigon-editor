using UnityEngine;
using System.Collections.Generic;

public class Linker : MonoBehaviour {
  
  private Collider2D trigonCol;
  private Animator trigonAnimator;
  
  private int index;
  private Vector2[] triangle;
  public void Init(int index, Vector2[] triangle) {
    this.index = index;
    this.triangle = triangle;
  }
  
  [Header("Debug")]
  public float legA; // store other two points and calc dynam?
  public float legB;
  public Vector2 dir = Vector2.zero;
  public float ang;
  
  private bool CheckValid() {
    Collider2D[] cols = new Collider2D[1];
    trigonCol.GetContacts(cols);
    Debug.Log(cols[0] == null);
    return (cols[0] == null);
    
    // triple linecast time
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
  
  private void Update() {
    Vector2 pos = triangle[index];
    if (index == 0) {
      legA = -Vector2.SignedAngle(Vector2.up, triangle[1] - triangle[index]);
      legB = -Vector2.SignedAngle(Vector2.up, triangle[2] - triangle[index]);
    }
    if (index == 1) {
      legA = -Vector2.SignedAngle(Vector2.up, triangle[2] - triangle[index]);
      legB = -Vector2.SignedAngle(Vector2.up, triangle[0] - triangle[index]);
    }
    if (index == 2) {
      legA = -Vector2.SignedAngle(Vector2.up, triangle[0] - triangle[index]);
      legB = -Vector2.SignedAngle(Vector2.up, triangle[1] - triangle[index]);
    }
    
    Debug.Log(legA + " " + legB);
    Debug.Log(triangle[0] + " " + triangle[1] + " " + triangle[2]);
  }
  
  private void OnTriggerEnter2D() {
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
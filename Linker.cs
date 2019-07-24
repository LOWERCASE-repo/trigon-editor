using UnityEngine;
using System.Collections.Generic;

public class Linker : MonoBehaviour {
  
  [SerializeField]
  private GameObject debugPoint;
  
  private Animator trigonAnimator;
  private HashSet<Link> links;
  
  private Transform legA;
  private Transform legB;
  // just store the transforms of the legs please
  // tired of thsi quaternion.angleaxis * shit
  
  [Header("Debug")]
  public float rotA;
  public float rotB;
  
  public void Init(HashSet<Link> links, Transform legA, Transform legB) {
    this.links = links;
    this.legA = legA;
    this.legB = legB;
  }
  
  public void UpdateLegs() { // TODO transforms not triangle
    rotA = -Vector2.SignedAngle(Vector2.up, legA.position - transform.position);
    rotB = -Vector2.SignedAngle(Vector2.up, legB.position - transform.position);
    rotA = Mathf.Round(rotA);
    rotB = Mathf.Round(rotB);
    // Debug.Log(rotA + " " + rotB);
  }
  
  private Link GetLink(Collider2D col) {
    Linker other = col.gameObject.GetComponent<Linker>();
    float rot = transform.rotation.eulerAngles.z; // wont need this with transforms
    float otherRot = other.transform.rotation.eulerAngles.z;
    rot = 0;
    otherRot = 0;
    
    float ab = Mathf.DeltaAngle(other.rotB - otherRot, rotA - rot);
    float ba = Mathf.DeltaAngle(other.rotA - otherRot, rotB - rot);
    float ang = (Mathf.Abs(ab) < Mathf.Abs(ba)) ? ab : ba;
    
    return new Link(transform, col.transform, ang);
  }
  
  private void Awake() {
    trigonAnimator = transform.parent.GetComponent<Animator>();
  }
  
  private void OnTriggerStay2D(Collider2D col) {
    UpdateLegs();
    if (!trigonAnimator.GetBool("Selected")) return;
    links.Add(GetLink(col));
  }
  
  private void OnTriggerExit2D(Collider2D col) {
    // TREAT ROTATION AS ON TRIG EXIT
    // if rotate and trigger doesnt update, fkt
    HashSet<Link> removes = new HashSet<Link>();
    foreach (Link link in links) {
      if (link.target == col.transform) {
        removes.Add(link);
      }
    }
    foreach (Link link in removes) {
      links.Remove(link);
    }
  }
}
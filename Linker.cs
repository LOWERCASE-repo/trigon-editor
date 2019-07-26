using UnityEngine;
using System.Collections.Generic;

public class Linker : MonoBehaviour {
  
  private HashSet<Link> links;
  
  [Header("Debug")]
  public float rotA;
  public float rotB;
  
  [Header("Components")]
  [SerializeField]
  private Collider2D collider;
  [SerializeField]
  private Animator trigonAnimator;
  [SerializeField]
  private Transform legA;
  [SerializeField]
  private Transform legB;
  
  public void UpdateLegs() {
    rotA = -Vector2.SignedAngle(Vector2.up, legA.position - transform.position);
    rotB = -Vector2.SignedAngle(Vector2.up, legB.position - transform.position);
    rotA = Mathf.Round(rotA);
    rotB = Mathf.Round(rotB);
    // Debug.Log(rotA + " " + rotB);
  }
  
  public void Init(HashSet<Link> links) {
    this.links = links;
    UpdateLegs();
  }
  
  private Link GetLink(Collider2D col) {
    Linker other = col.gameObject.GetComponent<Linker>();
    float ab = Mathf.DeltaAngle(other.rotB, rotA);
    float ba = Mathf.DeltaAngle(other.rotA, rotB);
    float ang = (Mathf.Abs(ab) < Mathf.Abs(ba)) ? ab : ba;
    
    return new Link(transform, col.transform, ang);
  }
  
  private void Awake() {
    trigonAnimator = transform.parent.GetComponent<Animator>();
  }
  
  private void OnTriggerStay2D(Collider2D col) {
    UpdateLegs();
    if (trigonAnimator.GetBool("Selected")) {
      links.Add(GetLink(col));
    }
  }
  
  private void OnTriggerExit2D(Collider2D col) {
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
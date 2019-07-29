using UnityEngine;
using System.Collections.Generic;

public class Linker : MonoBehaviour {
  
  private HashSet<Link> links;
  
  [Header("Debug")]
  public float rotA;
  public float rotB;
  
  [Header("Components")]
  [SerializeField]
  private CircleCollider2D cc;
  [SerializeField]
  private Animator trigonAnimator;
  [SerializeField]
  private Transform legA;
  [SerializeField]
  private Transform legB;
  [SerializeField]
  private Trigon trigon;
  
  public void UpdateLegs() {
    rotA = -Vector2.SignedAngle(Vector2.up, legA.position - transform.position);
    rotB = -Vector2.SignedAngle(Vector2.up, legB.position - transform.position);
    rotA = Mathf.Round(rotA);
    rotB = Mathf.Round(rotB);
    float lenA = (transform.position - legA.position).magnitude;
    float lenB = (transform.position - legB.position).magnitude;
    cc.radius = (lenA < lenB) ? lenA : lenB;
    cc.radius /= 3f;
  }
  
  private Link GetLink(Collider2D col) {
    Linker other = col.gameObject.GetComponent<Linker>();
    float ab = Mathf.DeltaAngle(other.rotB, rotA);
    float ba = Mathf.DeltaAngle(other.rotA, rotB);
    float ang = (Mathf.Abs(ab) < Mathf.Abs(ba)) ? ab : ba;
    
    return new Link(transform, col.transform, ang);
  }
  
  private void Start() {
    links = trigon.links;
    UpdateLegs();
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
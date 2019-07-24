using UnityEngine;

public class Link {
  public Transform linker;
  public Transform target;
  public float rot;
  
  public Link(Transform linker, Transform target, float rot) {
    this.linker = linker;
    this.target = target;
    this.rot = rot;
  }
  
  public override bool Equals(object obj) {
    Link link = (Link)obj;
    return link != null
      && this.linker == link.linker
      && this.target == link.target
      && this.rot == link.rot;
  }
  
  public override int GetHashCode() {
    return this.linker.GetHashCode()
      ^ this.target.GetHashCode()
      ^ this.rot.GetHashCode();
  }
}
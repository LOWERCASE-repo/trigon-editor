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
    && linker == link.linker
    && target == link.target
    && rot == link.rot;
  }
  
  public override int GetHashCode() {
    return linker.GetHashCode()
    ^ target.GetHashCode()
    ^ rot.GetHashCode();
  }
}
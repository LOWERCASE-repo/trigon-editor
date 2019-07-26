using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Trigon : MonoBehaviour {
  
  [Range(0f, 1f)]
  public float health;
  
  private HashSet<Link> links = new HashSet<Link>();
  private Vector2 prevMousePos = Vector2.negativeInfinity;
  private Vector2 mousePos;
  
  [Header("Components")]
  [SerializeField]
  private MeshFilter filter;
  [SerializeField]
  private MeshRenderer renderer;
  [SerializeField]
  private PolygonCollider2D collider;
  [SerializeField]
  private Animator animator;
  
  [Header("Children")]
  [SerializeField]
  private Linker[] linkers;
  [SerializeField]
  private Shell[] shells;
  [SerializeField]
  private GameObject mirrored;
  
  [Header("Prefabs")]
  [SerializeField]
  private Mesh normal;
  [SerializeField]
  private Mesh mirror;
  
  private void UpdateMesh() {
    Vector2[] triangle = System.Array.ConvertAll<Vector3, Vector2>(filter.mesh.vertices, v => v);
    for (int i = 0; i < linkers.Length; i++) {
      Quaternion rot = Quaternion.AngleAxis(transform.rotation.eulerAngles.z, Vector3.forward);
      linkers[i].transform.position = transform.position + rot * triangle[i];
      shells[i].Init(triangle[(i + 1) % linkers.Length], triangle[(i + 2) % linkers.Length]);
    }
    collider.points = triangle;
    filter.mesh.UploadMeshData(false);
  }
  
  private void Mirror() {
    mirrored.SetActive(!mirrored.activeSelf);
    if (mirrored.activeSelf) {
      filter.mesh = mirror;
    } else {
      filter.mesh = normal;
    }
    UpdateMesh();
  }
  
  private void Start() {
    for (int i = 0; i < 3; i++) {
      linkers[i].Init(links);
    }
    if (mirrored.activeSelf) {
      filter.mesh = mirror;
    } else {
      filter.mesh = normal;
    }
    UpdateMesh();
  }
  
  private void OnMouseDown() {
    animator.SetBool("Selected", true);
  }
  
  private void OnMouseDrag() {
    if (!prevMousePos.Equals(Vector2.negativeInfinity)) {
      transform.position = transform.position + (Vector3)(mousePos - prevMousePos);
    }
    prevMousePos = mousePos;
  }
  
  private void OnMouseUp() {
    animator.SetBool("Selected", false);
    prevMousePos = Vector2.negativeInfinity;
    
    Link link = links
    .OrderBy(t => Mathf.Abs(t.rot))
    .FirstOrDefault();
    
    if (link != null) {
      animator.SetTrigger("Attach");
      Vector3 dir = link.target.position - link.linker.position;
      float rot = link.rot;
      transform.RotateAround(link.linker.position, Vector3.forward, rot);
      transform.position += dir;
      if (link.target.parent != null) {
        Debug.Log("attaching to ship");
        // Destroy(rb);
        // collider.attachedRigidbody = link.target.parent.GetComponent<Rigidbody2D>();
      }
    }
  }
  
  private void Update() {
    mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    foreach (Shell shell in shells) {
      shell.vis = health;
    }
    if (animator.GetBool("Selected")) {
      Vector2 scroll = Input.mouseScrollDelta;
      if (!scroll.Equals(Vector2.zero)) {
        links.Clear();
        transform.RotateAround(mousePos, Vector3.forward, Input.mouseScrollDelta.y * 30f);
      }
      if (Input.GetButtonDown("Flip")) Mirror();
    }
  }
}
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Trigon : MonoBehaviour {
  
  [Range(0f, 1f)]
  public float health;
  public Vector2[] triangle;
  private Mesh mesh;
  
  private Linker[] linkers = new Linker[3];
  private HashSet<Link> links = new HashSet<Link>();
  private Vector2 prevMousePos = Vector2.negativeInfinity;
  private Vector2 mousePos;
  
  [Header("Components")]
  [SerializeField]
  private Linker linker;
  [SerializeField]
  private MeshFilter filter;
  [SerializeField]
  private MeshRenderer renderer;
  [SerializeField]
  private PolygonCollider2D collider;
  [SerializeField]
  private Animator animator;
  [SerializeField]
  private List<Shell> shells = new List<Shell>();
  
  private void AddLinker(int index) {
    Linker linker = Instantiate(this.linker, triangle[index], Quaternion.identity, transform);
    linkers[index] = linker;
  }
  
  private void InitLinker(int index) {
    Transform legA = linkers[(index + 1) % linkers.Length].transform;
    Transform legB = linkers[(index + 2) % linkers.Length].transform;
    linkers[index].Init(links, legA, legB);
    linkers[index].UpdateLegs();
  }
  
  private void UpdateTriangle() {
    collider.points = triangle;
    mesh.vertices = System.Array.ConvertAll<Vector2, Vector3>(triangle, v => v);
    filter.mesh = mesh;
    // foreach (Linker linker in linkers) {
    //   linker.UpdateLegs();
    // }
  }
  
  private void Awake() {
    linkers = new Linker[3];
    float sqrtThree = Mathf.Sqrt(3f);
    triangle = new Vector2[] {
      new Vector2(-1f / 3f, sqrtThree * 2f / 3f),
      new Vector2(2f / 3f, -sqrtThree / 3f),
      new Vector2(-1f / 3f, -sqrtThree / 3f)
    };
    mesh = new Mesh();
    mesh.vertices = System.Array.ConvertAll<Vector2, Vector3>(triangle, v => v);
    mesh.triangles = new int[] { 0, 1, 2 };
    
    for (int i = 0; i < triangle.Length; i++) {
      AddLinker(i);
      shells[i].posA = triangle[i]; // TODO shell dynam
      shells[i].posB = triangle[(i + 1) % shells.Count];
    }
    for (int i = 0; i < triangle.Length; i++) {
      InitLinker(i);
    }
    UpdateTriangle();
  }
  
  private void OnMouseDown() {
    animator.SetBool("Selected", true);
  }
  
  private void OnMouseDrag() {
    Debug.Log(links.Count);
    
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
    }
  }
  
  private void Update() {
    mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    foreach (Shell shell in shells) {
      shell.vis = health;
    }
    if (animator.GetBool("Selected")) {
      transform.RotateAround(mousePos, Vector3.forward, Input.mouseScrollDelta.y * 30);
      if (Input.GetButton("Flip")) {
        triangle = new Vector2[] {Vector2.zero, Vector2.up, Vector2.left};
        UpdateTriangle();
      }
    }
  }
}
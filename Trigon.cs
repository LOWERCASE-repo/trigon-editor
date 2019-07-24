using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Trigon : MonoBehaviour {
  
  [Range(0f, 1f)]
  public float health;
  private Vector2[] triangle;
  private Mesh mesh;
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
  private Linker[] linkers;
  [SerializeField]
  private Shell[] shells;
  
  private void UpdateMesh() {
    for (int i = 0; i < linkers.Length; i++) {
      linkers[i].transform.position = triangle[i];
    }
    mesh.vertices = System.Array.ConvertAll<Vector2, Vector3>(triangle, v => v);
    mesh.triangles = new int[] { 0, 1, 2 };
    filter.mesh = mesh;
    collider.points = triangle;
  }
  
  private void Start() {
    float sqrtThree = Mathf.Sqrt(3f);
    triangle = new Vector2[] {
      new Vector2(-1f / 3f, sqrtThree * 2f / 3f),
      new Vector2(2f / 3f, -sqrtThree / 3f),
      new Vector2(-1f / 3f, -sqrtThree / 3f)
    };
    
    mesh = new Mesh();
    UpdateMesh();
    
    for (int i = 0; i < triangle.Length; i++) {
      int indexA = (i + 1) % linkers.Length;
      int indexB = (i + 2) % linkers.Length;
      linkers[i].Init(links, linkers[indexA].transform, linkers[indexB].transform);
      shells[i].Init(triangle[indexA], triangle[indexB]);
      linkers[i].UpdateLegs();
    }
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
      if (Input.GetButtonDown("Flip")) {
        transform.RotateAround(transform.position, Vector3.forward, -transform.rotation.eulerAngles.z * 2f);
        for (int i = 0; i < triangle.Length; i++) {
          triangle[i].x *= -1f;
          Debug.Log(triangle[i].x);
        }
        Vector2 temp = triangle[1];
        triangle[1] = triangle[2];
        triangle[2] = temp;
        Debug.Log("Flip");
        UpdateMesh();
      }
    }
  }
}
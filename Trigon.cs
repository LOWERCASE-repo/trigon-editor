using UnityEngine;
using System.Collections.Generic;

public class Trigon : MonoBehaviour {
  
  [Range(0f, 1f)]
  public float health;
  
  private HashSet<Linker> linkers = new HashSet<Linker>();
  private Vector2 prevMousePos = Vector2.negativeInfinity;
  private Vector2[] triangle;
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
  
  private void AddLinker(int vertex, int legA, int legB) {
    linker = Instantiate(linker, triangle[vertex], Quaternion.identity, transform);
    linker.angA = legA;
    linker.angB = legB;
    linkers.Add(linker);
  }
  
  private void Awake() {
    
    triangle = new Vector2[] {
      new Vector2(0f, Mathf.Sqrt(3f)),
      new Vector2(1f, 0f),
      Vector2.zero
    };
    Vector2 centroid = Vector2.zero;
    foreach (Vector2 vertex in triangle) centroid += vertex;
    centroid /= triangle.Length;
    for (int i = 0; i < triangle.Length; i++) triangle[i] -= centroid;
    
    collider.points = triangle;
    Mesh mesh = new Mesh();
    mesh.vertices = System.Array.ConvertAll<Vector2, Vector3>(collider.points, v => v);
    mesh.triangles = new int[] { 0, 1, 2 };
    filter.mesh = mesh;
    
    AddLinker(0, 150, 180);
    AddLinker(1, 270, 330);
    AddLinker(2, 0, 90);
    
    for (int i = 0; i < shells.Count; i++) {
      shells[i].posA = mesh.vertices[i];
      shells[i].posB = mesh.vertices[(i < shells.Count - 1) ? i + 1 : 0];
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
    
    // communism linkers time
    
    // Vector3 dir = Vector3.zero;
    // float ang = 0f;
    // Vector2 piv = Vector2.zero;
    // foreach (Linker linker in linkers) {
    //   if (!linker.dir.Equals(Vector2.negativeInfinity) || linker.ang != Mathf.NegativeInfinity) {
    //     if (ang <= Mathf.Abs(linker.ang)){
    //       animator.SetTrigger("Attach");
    //       dir = linker.dir;
    //       ang = linker.ang;
    //       piv = linker.transform.position;
    //     }
    //   }
    // }
    // transform.RotateAround(piv, Vector3.forward, ang);
    // transform.position += dir;
  }
  
  private void Update() {
    mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    foreach (Shell shell in shells) {
      shell.vis = health;
    }
    if (animator.GetBool("Selected")) {
      transform.RotateAround(mousePos, Vector3.forward, Input.mouseScrollDelta.y * 30);
    }
  }
}
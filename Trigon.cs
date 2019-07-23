using UnityEngine;
using System.Collections.Generic;

public class Trigon : MonoBehaviour {
  
  [Range(0f, 1f)]
  public float health;
  public Vector2[] triangle;
  
  private HashSet<Linker> linkers = new HashSet<Linker>();
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
  
  private void AddLinker(int vertex, int legA, int legB) {
    linker = Instantiate(linker, triangle[vertex], Quaternion.identity, transform);
    linker.legA = legA;
    linker.legB = legB;
    linkers.Add(linker);
  }
  
  private void Awake() {
    
    float sqrtThree = Mathf.Sqrt(3f);
    triangle = new Vector2[] {
      new Vector2(-1f / 3f, sqrtThree * 2f / 3f),
      new Vector2(2f / 3f, -sqrtThree / 3f),
      new Vector2(-1f / 3f, -sqrtThree / 3f)
    };
    
    collider.points = triangle;
    Mesh mesh = new Mesh();
    mesh.vertices = System.Array.ConvertAll<Vector2, Vector3>(collider.points, v => v);
    mesh.triangles = new int[] { 0, 1, 2 };
    filter.mesh = mesh;
    
    AddLinker(0, 150, 180);
    AddLinker(1, 270, 330);
    AddLinker(2, 0, 90);
    
    for (int i = 0; i < shells.Count; i++) {
      shells[i].posA = triangle[i];
      shells[i].posB = triangle[(i + 1) % shells.Count];
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
    
    Vector3 dir = Vector3.zero;
    float ang = 0f;
    Vector2 piv = Vector2.zero;
    foreach (Linker linker in linkers) {
      if (!linker.dir.Equals(Vector2.negativeInfinity) || linker.ang != Mathf.NegativeInfinity) {
        if (ang <= Mathf.Abs(linker.ang)){
          animator.SetTrigger("Attach");
          dir = linker.dir;
          ang = linker.ang;
          piv = linker.transform.position;
        }
      }
    }
    transform.RotateAround(piv, Vector3.forward, ang);
    transform.position += dir;
  }
  
  private void Update() {
    mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    foreach (Shell shell in shells) {
      shell.vis = health;
    }
    if (animator.GetBool("Selected")) {
      transform.RotateAround(mousePos, Vector3.forward, Input.mouseScrollDelta.y * 30);
      // if getbutton flip, flip x on centroid y, update linker legs
      // dynamic linker legs? means no checking transform anymore
    }
  }
}
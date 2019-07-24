using UnityEngine;
using System.Collections.Generic;

public class Trigon : MonoBehaviour {
  
  [Range(0f, 1f)]
  public float health;
  public Vector2[] triangle;
  private Mesh mesh;
  
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
  
  private void AddLinker(int index) {
    linker = Instantiate(linker, triangle[index], Quaternion.identity, transform);
    linker.Init(index, triangle);
    linker.UpdateLegs();
    linkers.Add(linker);
  }
  
  private void UpdateTriangle() {
    collider.points = triangle;
    mesh.vertices = System.Array.ConvertAll<Vector2, Vector3>(triangle, v => v);
    filter.mesh = mesh;
    foreach (Linker linker in linkers) {
      linker.UpdateLegs();
    }
  }
  
  private void Awake() {
    
    float sqrtThree = Mathf.Sqrt(3f);
    triangle = new Vector2[] {
      new Vector2(-1f / 3f, sqrtThree * 2f / 3f),
      new Vector2(2f / 3f, -sqrtThree / 3f),
      new Vector2(-1f / 3f, -sqrtThree / 3f)
    };
    mesh = new Mesh();
    UpdateTriangle();
    mesh.triangles = new int[] { 0, 1, 2 };
    
    for (int i = 0; i < triangle.Length; i++) {
      AddLinker(i);
      // AddShell(i);
    }
    
    for (int i = 0; i < shells.Count; i++) {
      shells[i].posA = triangle[i]; // TODO shell dynam
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
      if (Input.GetButton("Flip")) {
        triangle = new Vector2[] {Vector2.zero, Vector2.up, Vector2.left};
        UpdateTriangle();
      }
    }
  }
}
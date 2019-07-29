using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Trigon : MonoBehaviour {
  
  [Range(0f, 1f)]
  public float health;
  // public Color shellColor {
  //   set {
  //     foreach (Shell shell in shells) {
  //       shell.
  //     }
  //   }
  // }
  // public Color trigonColor;
  
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
  [SerializeField] // TODO make into transform
  private GameObject status;
  
  [Header("Prefabs")]
  [SerializeField]
  private Mesh[] normals;
  [SerializeField]
  private Mesh[] mirrors;
  
  private void UpdateMesh() { // TODO still pass in index
    int index = (int)Mathf.Round(status.transform.position.x); // PrefabUtility doesnt support SerializeField
    Debug.Log("index " + index);
    filter.mesh = !status.activeSelf ? normals[index] : mirrors[index];
    Vector2[] triangle = System.Array.ConvertAll<Vector3, Vector2>(filter.mesh.vertices, v => v);
    for (int i = 0; i < linkers.Length; i++) {
      linkers[i].transform.localPosition = triangle[i];
      linkers[i].UpdateLegs();
      shells[i].Init(triangle[(i + 1) % linkers.Length], triangle[(i + 2) % linkers.Length]);
    }
    collider.points = triangle;
  }
  
  private void Start() {
    for (int i = 0; i < 3; i++) {
      linkers[i].Init(links);
    }
    UpdateMesh();
  }
  
  private void OnMouseDown() {
    animator.SetBool("Selected", true);
  }
  
  private void OnMouseDrag() { // TODO replace these and assign key to drag
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
      if (link.target.parent.parent != null) {
        Debug.Log("attaching to ship");
        transform.parent = link.target.parent.parent;
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
      if (Input.GetButtonDown("Flip")) {
        status.SetActive(!status.activeSelf);
        UpdateMesh();
      }
      if (Input.GetButtonDown("Resize")) {
        float index = (int)Mathf.Round(status.transform.localPosition.x + Input.GetAxisRaw("Resize")) + normals.Length;
        index %= normals.Length;
        status.transform.localPosition = new Vector3(index, 0f, 0f);
        UpdateMesh();
      }
    }
  }
}
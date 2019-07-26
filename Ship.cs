using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class Ship : MonoBehaviour {
  
  [SerializeField]
  protected float speed;
  [SerializeField]
  protected float acc;
  
  private Vector2 mousePos;
  
  [Header("Components")]
  [SerializeField]
  private Rigidbody2D rb;
  
  // holy grail methods
  private void Move(Vector2 pos) {
    rb.AddForce((pos - rb.position).normalized * acc);
  }
  
  private void Rotate(Vector2 dir) {
    float ang = Vector2.SignedAngle(Vector2.up, dir);
    float pol = 1f - Mathf.Abs(ang) / 180f;
    rb.MoveRotation(Mathf.LerpAngle(rb.rotation, ang, pol));
    // TODO mvmt rotation rel mass
  }
  
  private void Center() {
    Vector3 centroid = Vector3.zero;
    for (int i = 0; i < transform.childCount; i++) {
      centroid += transform.GetChild(i).transform.position;
    }
    centroid /= transform.childCount;
    for (int i = 0; i < transform.childCount; i++) {
      transform.GetChild(i).transform.position -= centroid;
    }
    // dont move transform, otherwise teleport exploit
  }
  
  private void Save(string name) {
    Center();
    char sep = Path.DirectorySeparatorChar;
    string path = "Assets" + sep + "Ships" + sep + name + ".prefab";
    PrefabUtility.SaveAsPrefabAsset(gameObject, path);
  }
  
  private void Start() {
    rb.drag = acc / speed;
  }
  
  private void Update() {
    mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
  }
  
  private void FixedUpdate() {
    if (Input.GetButton("Move")) Move(mousePos);
    if (Input.GetButtonDown("Save")) Save("Shoom");
    if (!rb.velocity.Equals(Vector2.zero)) Rotate(rb.velocity);
  }
}
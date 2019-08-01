using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class Ship : MonoBehaviour {
  
  [SerializeField]
  private Color trigonColor;
  [SerializeField]
  private Color moduleColor;
  [SerializeField]
  private Color shellColor;
  
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
    // mass only affects acc, doesnt result in unique behav
    // drag = acc / speed / mass
    rb.AddForce((pos - rb.position).normalized * acc);
  }
  
  private void Rotate(Vector2 dir) {
    float ang = Vector2.SignedAngle(Vector2.up, dir);
    float pol = 1f - Mathf.Abs(ang) / 360f;
    rb.MoveRotation(Mathf.LerpAngle(rb.rotation, ang, pol));
    // TODO mvmt rotation rel mass
  }
  
  private void Save() {
    for (int i = 0; i < transform.childCount; i++) {
      transform.GetChild(i).transform.localPosition -= (Vector3)rb.centerOfMass;
    }
    transform.position = Vector3.zero;
    transform.rotation = Quaternion.identity;
    char sep = Path.DirectorySeparatorChar;
    string path = "Assets" + sep + "Ships" + sep + gameObject.name + ".prefab";
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
    if (Input.GetButtonDown("Save")) Save();
    if (!rb.velocity.Equals(Vector2.zero)) Rotate(rb.velocity);
  }
  
  private void OnMouseDown() {
    // Debug.Log("shif");
  }
}
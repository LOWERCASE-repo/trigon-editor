using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Ship : MonoBehaviour {
  
  [SerializeField]
  protected float speed;
  [SerializeField]
  protected float turnSpeed;
  [SerializeField]
  protected float acc;
  
  private Vector2 mousePos;
  
  [Header("Components")]
  [SerializeField]
  private Rigidbody2D rb;
  
  private void Move(Vector2 pos) {
    rb.AddForce((pos - rb.position).normalized * acc);
    Debug.Log(rb.velocity.magnitude);
    
    float ang = Vector2.SignedAngle(Vector2.up, rb.velocity);
    // float turnSpeed = speed; //  / Mathf.PI / 2f
    float turnDir = Mathf.Clamp(Mathf.DeltaAngle(ang, rb.rotation), 0f, turnSpeed);
    float turnAcc = turnDir * Mathf.Max(turnSpeed - rb.angularVelocity, 0f);
    rb.AddTorque(turnAcc);
    
    // ship should turn to face pos
    // turn speed? impact on mvmt?
  }
  
  private void Start() {
    rb.drag = speed / (speed + acc);
    rb.angularDrag = acc / speed;
  }
  
  private void Update() {
    mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    Debug.DrawLine(rb.position, mousePos);
  }
  
  private void FixedUpdate() {
    Move(mousePos);
  }
}
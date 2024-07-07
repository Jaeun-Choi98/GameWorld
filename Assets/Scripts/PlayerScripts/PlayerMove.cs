using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
  Rigidbody rb;
  Collider col;
  PlayerData playerData;
  private float rotSpeed = 150f;
  private float xAngle = 0f;

  CamManager camManager;

  private LayerMask collisionMask;
  [SerializeField]
  private bool isJump = false;

  void Start()
  {
    rb = GetComponent<Rigidbody>();
    rb.freezeRotation = true;
    playerData = GetComponent<PlayerData>();
    col = GetComponent<Collider>();
    collisionMask = LayerMask.GetMask("Envirionment", "Player");
    camManager = Camera.main.GetComponent<CamManager>();
  }

  void Update()
  {
    Rotate();
    Jump();
  }

  void FixedUpdate()
  {
    Move();
  }

  void Move()
  {

    float h = Input.GetAxis("Horizontal");
    float v = Input.GetAxis("Vertical");

    Vector3 dir = new Vector3(h, 0, v);
    //dir.Normalize();
    // 오브젝트의 로컬 좌표계 -> 월드 좌표계로 변환
    dir = transform.TransformDirection(dir);
    if (isJump)
    {
      rb.MovePosition(rb.position + dir * playerData.speed * Time.deltaTime * 0.65f);
    }
    else
    {
      rb.MovePosition(rb.position + dir * playerData.speed * Time.deltaTime * 0.9f);
    }

    //rb.AddForce(dir*playerData.speed*Time.deltaTime,ForceMode.Impulse);
    //rb.velocity = new Vector3(dir.x * playerData.speed, rb.velocity.y, dir.z * playerData.speed);
  }

  private void Rotate()
  {
    if(camManager.veiwPoint == CamManager.VeiwPoint.fix)
    {
      return;
    }
    /*float dy = Input.GetAxis("Mouse Y");
    y -= dy * rotSpeed * Time.deltaTime;
    y = Mathf.Clamp(y, -90f, 90f);*/

    float dx = Input.GetAxis("Mouse X");
    xAngle += dx * rotSpeed * Time.deltaTime;

    Quaternion cameraRotation = Quaternion.Euler(0f, xAngle, 0f); // X 축 회전만 적용
    transform.rotation = cameraRotation;


    //transform.Rotate(Vector3.up * x);
  }

  private void Jump()
  {
    if (Input.GetButtonDown("Jump") && !isJump)
    {
      rb.AddForce(Vector3.up * playerData.jumpPower * 2f, ForceMode.Impulse);
    }

    if (CheckCollisionBelow())
    {
      if (isJump)
      {
        isJump = false;
      }
    }
    else
    {
      rb.AddForce(Vector3.down * 3f, ForceMode.Acceleration);
      isJump = true;
    }


  }

  private bool CheckCollisionBelow()
  {
    Ray ray = new Ray(transform.position, Vector3.down);
    RaycastHit hit;
    bool ret = Physics.Raycast(ray, out hit, col.bounds.extents.y+0.1f, collisionMask);
    return ret;
  }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
  private float h, v;

  Rigidbody rb;
  Collider col;
  //PlayerData playerData;
  private float rotSpeed = 150f;
  private float xAngle = 0f;

  CamManager camManager;

  private LayerMask collisionMask;
  [SerializeField]
  private bool isJump = false;
  [SerializeField]
  private bool isRoll = false;

  Animator animator;

  public bool isJumpMotion = false;

  [SerializeField]
  private TCPChatClient TCPchat;

  void Start()
  {
    rb = GetComponent<Rigidbody>();
    rb.freezeRotation = true;
    //playerData = GetComponent<PlayerData>();
    col = GetComponent<Collider>();
    collisionMask = LayerMask.GetMask("Envirionment", "Player", "Potal");
    camManager = Camera.main.GetComponent<CamManager>();
    animator = GetComponentInChildren<Animator>();
  }

  void Update()
  {
    if (TCPchat.isChatOn) return;
    Rotate();
    Jump();
    Roll();
    // 애님으로 인해 틀어진 position을 맞추는 작업
    transform.GetChild(0).transform.localPosition = new Vector3(0, -1, 0);
  }

  void FixedUpdate()
  {
    if (TCPchat.isChatOn) return;
    Move();
  }

  void Roll()
  {
    if (Input.GetKeyDown(KeyCode.LeftShift) && !isRoll && (h != 0 || v != 0) && !isJump)
    {
      isRoll = true;
      StartCoroutine(RollProcess());
    }
  }

  IEnumerator RollProcess()
  {
    Vector3 dir;
    if (v > 0f && h == 0f)
    {
      v = 1f;
    }
    else if (v > 0f && h > 0f)
    {
      h = 1f;
      v = 1f;
    }
    else if (v == 0f && h > 0f)
    {
      h = 1f;
    }
    else if (v < 0f && h > 0f)
    {
      h = 1f;
      v = -1f;
    }
    else if (v < 0f && h == 0f)
    {
      v = -1f;
    }
    else if (v < 0f && h < 0f)
    {
      h = -1f;
      v = -1f;
    }
    else if (v == 0f && h < 0f)
    {
      h = -1f;
    }
    else if (v > 0f && h < 0f)
    {
      h = -1f;
      v = 1f;
    }else
    {
      h = 0f;
      v = 0f;
    }
    animator.SetFloat("Horizontal", h);
    animator.SetFloat("Vertical", v);
    animator.SetTrigger("RunToRoll");
    dir = new Vector3(h, 0, v);
    dir = dir.normalized;
    dir = transform.TransformDirection(dir);
    rb.AddForce(dir * 10f, ForceMode.Impulse);
    yield return new WaitForSeconds(1.5f);
    isRoll = false;
  }

  void Move()
  {

    h = Input.GetAxis("Horizontal");
    v = Input.GetAxis("Vertical");
    if (isRoll)
    {
      return;
    }
    Vector3 dir = new Vector3(h, 0, v);

    animator.SetFloat("Horizontal", h);
    animator.SetFloat("Vertical", v);

    //dir.Normalize();
    // 오브젝트의 로컬 좌표계 -> 월드 좌표계로 변환
    dir = transform.TransformDirection(dir);
    if (isJump)
    {
      rb.MovePosition(rb.position + dir * Server.Instance.playerInfo.Speed * Time.deltaTime * 0.65f);
    }
    else
    {
      rb.MovePosition(rb.position + dir * Server.Instance.playerInfo.Speed * Time.deltaTime * 0.8f);
    }

    //rb.AddForce(dir*playerData.speed*Time.deltaTime,ForceMode.Impulse);
    //rb.velocity = new Vector3(dir.x * playerData.speed, rb.velocity.y, dir.z * playerData.speed);
  }

  private void Rotate()
  {
    if (camManager.veiwPoint == CamManager.VeiwPoint.fix)
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
    if (Input.GetButtonDown("Jump") && !isJump && !isJumpMotion && !isRoll)
    {
      isJumpMotion = true;
      animator.SetTrigger("RunToJump");
      rb.AddForce(Vector3.up * Server.Instance.playerInfo.JumpPower * 1.5f, ForceMode.Impulse);
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
      rb.AddForce(Vector3.down * 3.3f, ForceMode.Acceleration);
      isJump = true;
    }


  }

  private bool CheckCollisionBelow()
  {
    Ray ray = new Ray(transform.position, Vector3.down);
    RaycastHit hit;
    bool ret = Physics.Raycast(ray, out hit, col.bounds.extents.y + 0.1f, collisionMask);
    return ret;
  }
}

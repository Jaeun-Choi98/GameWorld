using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDimentionPlayerMove : MonoBehaviour
{
  private float h;
  private float jumpH;

  [SerializeField]
  private float speed = 4f;
  [SerializeField]
  private float gravity = -18f;
  [SerializeField]
  private float yVelocity = 0f;
  [SerializeField]
  private float jumpPower = 8f;
  [SerializeField]
  private bool isJump = false;

  [SerializeField]
  private CharacterController cc;

  private void Start()
  {
    cc = GetComponent<CharacterController>();
  }

  private void Update()
  {
    Move();
    Jump();
  }

  void Move()
  {
    Vector3 dir;
    if (isJump)
    {
      if (jumpH > 0)
      {
        jumpH = (jumpH > 0.6f) ? 0.8f : 0.5f;
      }
      else if (jumpH < 0)
      {
        jumpH = (jumpH < -0.6f) ? -0.8f : -0.5f;
      }
      dir = new Vector3(jumpH, 0, 0);
      cc.Move(dir * speed * 0.7f * Time.deltaTime);
    }
    else
    {
      h = Input.GetAxis("Horizontal");
      dir = new Vector3(h, 0, 0);
      cc.Move(dir * speed * 0.85f * Time.deltaTime);
    }

  }

  void Jump()
  {
    yVelocity += gravity * Time.deltaTime;
    if (Input.GetButtonDown("Jump") && !isJump)
    {
      yVelocity = jumpPower;
      isJump = true;
      jumpH = h;
    }
    cc.Move(Vector3.up * yVelocity * Time.deltaTime);
    if ((cc.collisionFlags & CollisionFlags.Below) != 0)
    {
      if (isJump)
      {
        isJump = false;
      }
      yVelocity = 0f;
    }
  }
}


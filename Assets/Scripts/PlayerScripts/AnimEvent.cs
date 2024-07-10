using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvent : MonoBehaviour
{
  [SerializeField]
  private PlayerMove playerMove;
  void Start()
  {
    playerMove = GetComponentInParent<PlayerMove>();
  }

  public void JumpMotionEnd()
  {
    playerMove.isJumpMotion = false;
  }
}

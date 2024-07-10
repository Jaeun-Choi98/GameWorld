using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimEvent : MonoBehaviour
{
  [SerializeField]
  private BossGamePlayerMove playerMove;
  void Start()
  {
    playerMove = GetComponentInParent<BossGamePlayerMove>();
  }

  public void JumpMotionEnd()
  {
    playerMove.isJumpMotion = false;
  }
}

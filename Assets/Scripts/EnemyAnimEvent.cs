using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimEvent : MonoBehaviour
{
  EnemyFSM fsm;
  BossGamePlayerMove bossGamePlayerMove;
  void Start()
  {
    fsm = GetComponentInParent<EnemyFSM>();
    bossGamePlayerMove = GameObject.Find("Player").GetComponent<BossGamePlayerMove>();
  }

  public void EnemyAttackEvnet()
  {
    if (!bossGamePlayerMove.isRoll)
    {
      fsm.EnemyAttackAction();
    }
  }
}

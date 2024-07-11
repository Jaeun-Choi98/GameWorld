using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimEvent : MonoBehaviour
{
  EnemyFSM fsm;

  void Start()
  {
    fsm = GetComponentInParent<EnemyFSM>();
  }

  public void EnemyAttackEvnet()
  {
    fsm.EnemyAttackAction();
  }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
  public enum WeaponMode
  {
    Basic,
    EnergyBolt
  }

  [SerializeField]
  private GameObject firePosition;
  [SerializeField]
  private GameObject energyboltFactory;
  [SerializeField]
  private GameObject aimingCamera;
  [SerializeField]
  private GameObject crosshair;

  public bool isAiming = false;

  public WeaponMode weaponMode;

  Animator anim;

  [SerializeField]
  private bool availableBasicAttack = true;
  private float basicAttackDelay;
  [SerializeField]
  private bool availableEngBoltAttack = true;
  private float energyBoltAttackDelay;

  // 구르기 및 점프 시 조준 불가능
  BossGamePlayerMove bossGamePlayerMove;

  void Start()
  {
    weaponMode = WeaponMode.Basic;
    anim = GetComponentInChildren<Animator>();
    basicAttackDelay = 1.5f;
    energyBoltAttackDelay = 2f;
    bossGamePlayerMove = GetComponent<BossGamePlayerMove>();
    aimingCamera.SetActive(false);
  }

  // Update is called once per frame
  void Update()
  {
    if (BoasGameManager.Instance.gameState != BoasGameManager.GameState.Run)
    {
      return;
    }
    Aiming();
    Attack();
  }

  private void Attack()
  {
    if (weaponMode == WeaponMode.Basic && isAiming && Input.GetKeyDown(KeyCode.Mouse0)
      && availableBasicAttack)
    {
      // 공격 애니메이션
      anim.SetTrigger("AttackBasic");
      StartCoroutine(AttackBasicProcess(basicAttackDelay));
    }
    else if (weaponMode == WeaponMode.EnergyBolt && isAiming && Input.GetKeyDown(KeyCode.Mouse0)
      && availableEngBoltAttack)
    {
      // 공격 애니메이션
      anim.SetTrigger("AttackEnergyBolt");
      StartCoroutine(AttackEnergyBoltProcess(energyBoltAttackDelay));
    }
  }

  IEnumerator AttackBasicProcess(float delay)
  {
    availableBasicAttack = false;
    yield return new WaitForSeconds(delay);
    availableBasicAttack = true;
  }

  IEnumerator AttackEnergyBoltProcess(float delay)
  {
    availableEngBoltAttack = false;
    yield return new WaitForSeconds(delay);
    availableEngBoltAttack = true;
  }

  private void Aiming()
  {
    if(bossGamePlayerMove.isJump || bossGamePlayerMove.isRoll)
    {
      return;
    }
    if (Input.GetMouseButtonDown(1))
    {
      isAiming = true;
      anim.SetTrigger("IdleToAttackIdle");
      aimingCamera.SetActive(true);
      crosshair.SetActive(true);
    }
    if (Input.GetMouseButtonUp(1))
    {
      isAiming = false;
      anim.SetTrigger("AttackIdleToIdle");
      aimingCamera.SetActive(false);
      crosshair.SetActive(false);
    }
  }
}

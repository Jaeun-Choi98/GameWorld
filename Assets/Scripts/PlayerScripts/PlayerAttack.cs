using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
  [SerializeField]
  private Text skillText;

  Animator anim;

  // 기본 공격
  [SerializeField]
  private bool availableBasicAttack = true;
  private float basicAttackDelay;
  [SerializeField]
  private ParticleSystem basicEffect;

  // 스킬 공격 ( 에너지 볼트 )
  [SerializeField]
  private bool availableEngBoltAttack = true;
  private float energyBoltAttackDelay;
  // 이펙트는 EnergyBolt에서 관리 ( 충돌 시 효과 적용 )
  public ParticleSystem energyBoltEffect;
  [SerializeField]
  private GameObject energyBoltFactory;
  // 비활성화를 위한 public
  public List<GameObject> energyBoltPool;
  [SerializeField]
  private int energyBoltPoolSize = 10;

  [SerializeField]
  private GameObject aimingEffectFactory;

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
    skillText.text = "기본 스킬";
    energyBoltPool = new List<GameObject>();
    for(int i = 0; i < energyBoltPoolSize; i++)
    {
      GameObject energyBolt = Instantiate(energyBoltFactory);
      energyBoltPool.Add(energyBolt);
      energyBolt.SetActive(false);
    }
  }

  /*
   * 에이밍과 어택은 sequentially 하게 진행돼야 함. 
   * 현재는 에이밍과 어택이 동시에 실행되는 문제가 발생.
   * 이후 에이밍과 어택을 시퀀셜리하게 처리해야함. ( 상태 다이어그램을 이용하거나 코루틴? 사용 )
   */

  void Update()
  {
    if (BossGameManager.Instance.gameState != BossGameManager.GameState.Run)
    {
      return;
    }
    if (Input.GetKeyDown(KeyCode.Alpha1))
    {
      weaponMode = WeaponMode.Basic;
      skillText.text = "기본 스킬";
    }
    if (Input.GetKeyDown(KeyCode.Alpha2))
    {
      weaponMode = WeaponMode.EnergyBolt;
      skillText.text = "에너지 볼트";
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
      Ray ray = new Ray(aimingCamera.transform.position, aimingCamera.transform.forward);
      RaycastHit hitInfo;
      if (Physics.Raycast(ray, out hitInfo, 30f))
      {
        if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
          hitInfo.transform.GetComponent<EnemyFSM>().AttackEnemy(3f);
          basicEffect.transform.position = hitInfo.transform.position;
          basicEffect.transform.rotation = hitInfo.transform.rotation;
          basicEffect.Play();
        }
        else
        {
          basicEffect.transform.position = hitInfo.transform.position;
          basicEffect.transform.rotation = hitInfo.transform.rotation;
          basicEffect.Play();
        }
      }
    }
    else if (weaponMode == WeaponMode.EnergyBolt && isAiming && Input.GetKeyDown(KeyCode.Mouse0)
      && availableEngBoltAttack)
    {
      // 공격 애니메이션
      anim.SetTrigger("AttackEnergyBolt");
      StartCoroutine(AttackEnergyBoltProcess(energyBoltAttackDelay));
      Ray ray = new Ray(aimingCamera.transform.position, aimingCamera.transform.forward);
      RaycastHit hitInfo;
      if (Physics.Raycast(ray, out hitInfo, 30f))
      {
        GameObject energyBolt = energyBoltPool[0];
        energyBolt.SetActive(true);
        energyBoltPool.Remove(energyBolt);
        energyBolt.transform.position = firePosition.transform.position;
        if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
          energyBolt.GetComponent<EnergyBolt>().Fire(hitInfo.transform.gameObject);
        }
        else
        {
          energyBolt.GetComponent<EnergyBolt>().Fire(null);
        }
      }
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
    if (bossGamePlayerMove.isJump || bossGamePlayerMove.isRoll)
    {
      return;
    }
    if (Input.GetMouseButtonDown(1))
    {
      aimingEffectFactory.SetActive(true);
      isAiming = true;
      anim.SetTrigger("IdleToAttackIdle");
      aimingCamera.SetActive(true);
      crosshair.SetActive(true);
    }
    if (Input.GetMouseButtonUp(1))
    {
      aimingEffectFactory.SetActive(false);
      isAiming = false;
      anim.SetTrigger("AttackIdleToIdle");
      aimingCamera.SetActive(false);
      crosshair.SetActive(false);
    }
  }
}

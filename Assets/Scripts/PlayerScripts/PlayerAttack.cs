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

  // �⺻ ����
  [SerializeField]
  private bool availableBasicAttack = true;
  private float basicAttackDelay;
  [SerializeField]
  private ParticleSystem basicEffect;

  // ��ų ���� ( ������ ��Ʈ )
  [SerializeField]
  private bool availableEngBoltAttack = true;
  private float energyBoltAttackDelay;
  // ����Ʈ�� EnergyBolt���� ���� ( �浹 �� ȿ�� ���� )
  public ParticleSystem energyBoltEffect;
  [SerializeField]
  private GameObject energyBoltFactory;
  // ��Ȱ��ȭ�� ���� public
  public List<GameObject> energyBoltPool;
  [SerializeField]
  private int energyBoltPoolSize = 10;

  [SerializeField]
  private GameObject aimingEffectFactory;

  // ������ �� ���� �� ���� �Ұ���
  BossGamePlayerMove bossGamePlayerMove;

  void Start()
  {
    weaponMode = WeaponMode.Basic;
    anim = GetComponentInChildren<Animator>();
    basicAttackDelay = 1.5f;
    energyBoltAttackDelay = 2f;
    bossGamePlayerMove = GetComponent<BossGamePlayerMove>();
    aimingCamera.SetActive(false);
    skillText.text = "�⺻ ��ų";
    energyBoltPool = new List<GameObject>();
    for(int i = 0; i < energyBoltPoolSize; i++)
    {
      GameObject energyBolt = Instantiate(energyBoltFactory);
      energyBoltPool.Add(energyBolt);
      energyBolt.SetActive(false);
    }
  }

  /*
   * ���ְ̹� ������ sequentially �ϰ� ����ž� ��. 
   * ����� ���ְ̹� ������ ���ÿ� ����Ǵ� ������ �߻�.
   * ���� ���ְ̹� ������ �����ȸ��ϰ� ó���ؾ���. ( ���� ���̾�׷��� �̿��ϰų� �ڷ�ƾ? ��� )
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
      skillText.text = "�⺻ ��ų";
    }
    if (Input.GetKeyDown(KeyCode.Alpha2))
    {
      weaponMode = WeaponMode.EnergyBolt;
      skillText.text = "������ ��Ʈ";
    }
    Aiming();
    Attack();
  }

  private void Attack()
  {
    if (weaponMode == WeaponMode.Basic && isAiming && Input.GetKeyDown(KeyCode.Mouse0)
      && availableBasicAttack)
    {
      // ���� �ִϸ��̼�
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
      // ���� �ִϸ��̼�
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

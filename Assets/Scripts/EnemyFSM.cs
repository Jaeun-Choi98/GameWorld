using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyFSM : MonoBehaviour
{
  enum EnemyState
  {
    Idle,
    Move,
    Attack,
    Return,
    Damaged,
    Die
  }

  EnemyState state;

  GameObject player;
  Rigidbody rb;

  [SerializeField]
  private float findDistance = 20f;
  [SerializeField]
  private float limitMoveDistance = 40f;
  [SerializeField]
  private float attackDistance = 3f;
  [SerializeField]
  private float moveSpeed = 3f;
  [SerializeField]
  private float hp = 30f;
  [SerializeField]
  private Slider enemyHpSlider;

  public float curHp;

  private Vector3 originPos;
  private Quaternion originRot;
  private Animator anim;

  void Start()
  {
    state = EnemyState.Idle;
    player = GameObject.Find("Player");
    rb = GetComponent<Rigidbody>();
    originPos = transform.position;
    originRot = transform.rotation;
    curHp = hp;
    anim = GetComponentInChildren<Animator>();
  }

  void Update()
  {
    switch (state)
    {
      case EnemyState.Idle:
        Idle();
        break;
      case EnemyState.Move:
        Move();
        break;
      case EnemyState.Attack:
        Attack();
        break;
      case EnemyState.Return:
        Return();
        break;
      case EnemyState.Damaged:
        break;
      case EnemyState.Die:
        break;
      default:
        break;
    }
    enemyHpSlider.value = (float)curHp / (float)hp;
  }

  void Idle()
  {
    if (Vector3.Distance(transform.position, player.transform.position) < findDistance)
    {
      state = EnemyState.Move;
      Debug.Log("idle -> move");
      // �ִϸ��̼� ��� -> �̵�
      anim.SetTrigger("IdleToMove");
    }
  }

  void Move()
  {
    if (Vector3.Distance(transform.position, originPos) > limitMoveDistance)
    {
      state = EnemyState.Return;
      Debug.Log("move -> return");
    }
    else if (Vector3.Distance(transform.position, player.transform.position) > attackDistance)
    {
      // ��� �̵�
      Vector3 dir = (player.transform.position - transform.position).normalized;
      rb.MovePosition(rb.position + dir * moveSpeed * Time.deltaTime);
      transform.forward = dir;

    }
    else
    {
      state = EnemyState.Attack;
      currentTime = attackDelay;
      Debug.Log("move -> attack");
      // ���� ��� �ִϸ��̼�
      anim.SetTrigger("MoveToAttackIdle");
    }
  }

  float currentTime = 0;
  float attackDelay = 2f;
  public float attackPower = 3f;

  void Attack()
  {
    if (Vector3.Distance(transform.position, player.transform.position) < attackDistance)
    {
      currentTime += Time.deltaTime;
      if (currentTime > attackDelay)
      {
        // ���� �ִϸ��̼�
        anim.SetTrigger("StartAttack");
        //player.GetComponent<PlayerState>().AttackPlayer(attackPower);
        currentTime = 0f;
      }
    }
    else
    {
      state = EnemyState.Move;
      Debug.Log("attack -> move");
      currentTime = 0f;
    }
  }

  public void EnemyAttackAction()
  {
    player.GetComponent<PlayerState>().AttackPlayer(attackPower);
  }

  void Return()
  {
    if (Vector3.Distance(transform.position, originPos) > 0.1f)
    {
      Vector3 dir = (originPos - transform.position).normalized;
      rb.MovePosition(rb.position + dir * moveSpeed * Time.deltaTime);
      transform.forward = dir;
      // hp�� ����ؼ� ����
      curHp = hp;
    }
    else
    {
      transform.position = originPos;
      transform.rotation = originRot;
      state = EnemyState.Idle;
      Debug.Log("return -> idle");
      // �ִ� ��� ����
      anim.SetTrigger("MoveToIdle");
    }
  }

  // ��ų�� ���� ���� ȿ�� �ð��� �ٸ��� �־��ָ� ���� �� ����.
  public void AttackEnemy(float power)
  {
    if (state == EnemyState.Damaged || state == EnemyState.Die)
    {
      return;
    }
    curHp -= power;

    if (curHp > 0f)
    {
      state = EnemyState.Damaged;
      // damaged �ִϸ��̼�
      Debug.Log("any state -> damaged");
      anim.SetTrigger("Damaged");
      Damaged();
    }
    else
    {
      state = EnemyState.Die;
      // die �ִϸ��̼�
      Debug.Log("any state -> die");
      anim.SetTrigger("Die");
      Die();
    }
  }

  void Damaged()
  {
    StartCoroutine(DamagedProcess());
  }

  IEnumerator DamagedProcess()
  {
    yield return new WaitForSeconds(1f);
    state = EnemyState.Move;
    Debug.Log("damged -> move");
  }

  void Die()
  {
    StopAllCoroutines();
    StartCoroutine(DieProcess());
  }

  IEnumerator DieProcess()
  {
    yield return new WaitForSeconds(2f);
    Destroy(gameObject);
  }


}

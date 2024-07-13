using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnergyBolt : MonoBehaviour
{
  [SerializeField]
  private float speed = 5f;

  /*[SerializeField]
  private ParticleSystem energyBoltEffect;*/

  [SerializeField]
  private PlayerAttack playerAttack;

  Rigidbody rb;
  Vector3 dir;
  // ���ؽ� �������� �� Ÿ�ٹ������� ��� ������ ( like ����ź )
  void Start()
  {
    rb = GetComponent<Rigidbody>();
    playerAttack = GameObject.Find("Player").GetComponent<PlayerAttack>();
    //energyBoltEffect = GameObject.Find("Electric C (Air)c C (Air)").GetComponent<ParticleSystem>();
  }

  // Update is called once per frame
  void Update()
  {
    rb.MovePosition(rb.position + dir * speed * Time.deltaTime);
  }

  public void Fire(GameObject target)
  {
    if(target == null)
    {
      dir = gameObject.transform.forward;
    }
    else
    {
      dir = (target.transform.position - transform.position).normalized;
    }
  }

  private void OnCollisionEnter(Collision collision)
  {
    if(collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
    {
      collision.gameObject.GetComponent<EnemyFSM>().AttackEnemy(10f);
    }
    playerAttack.energyBoltEffect.transform.position = transform.position;
    playerAttack.energyBoltEffect.Play();
    gameObject.SetActive(false);
    playerAttack.energyBoltPool.Add(gameObject);
    //energyBoltEffect.Play();
  }
}

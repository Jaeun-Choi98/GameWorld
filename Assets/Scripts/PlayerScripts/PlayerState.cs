using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/* 
 * 플레이어도 enemy 오브젝트처럼 fsm 다이어그램이 필요할 수도 있다고 생각
 * -> 플레이어가 Damaged 상태에서는 움직여선 안됨. flag 변수로 BossGamePlayerMove를 제어하는 방법을
 * 사용하면 되지만, 이후 코드 관리나 기능 확장 시 불편할 수도
*/
public class PlayerState : MonoBehaviour
{

  public float hp;
  public int jumpPower = 0;
  public int speed = 0;
  public int money = 0;

  PlayerData playerData;

  [SerializeField]
  private Slider playerHpSlider;
  [SerializeField]
  private Text playerHpText;

  public float curHp;

  Animator anim;
  [SerializeField]
  private GameObject attackPlayerEffect;

  public bool isDamaged = false;

  void Start()
  {
    playerData = GetComponent<PlayerData>();
    anim = GetComponentInChildren<Animator>();
  }

  public void PlayerStateLoad(float hp, int jumpPower, int speed, int money)
  {
    this.hp = hp;
    this.jumpPower = jumpPower;
    this.speed = speed;
    this.money = money;
    curHp = hp;
    playerHpSlider.value = (float)curHp / (float)hp;
    playerHpText.text = string.Format("{0} / {1} ({2}%)", curHp, hp, (curHp * 100) / hp);
  }

  public void AttackPlayer(float power)
  {
    curHp -= power;
    if(curHp > 0f)
    {
      anim.SetTrigger("Damaged");
      StartCoroutine(DamagedProcess());
    }
    else
    {
      anim.SetTrigger("Die");
      StopAllCoroutines();
      StartCoroutine(DieProcess());
    }
    playerHpSlider.value = (float)curHp / (float)hp;
    playerHpText.text = string.Format("{0} / {1} ({2}%)", curHp, hp, (curHp * 100) / hp);
  }

  IEnumerator DamagedProcess()
  {
    attackPlayerEffect.SetActive(true);
    isDamaged = true;
    yield return new WaitForSeconds(0.3f);
    attackPlayerEffect.SetActive(false);
    yield return new WaitForSeconds(0.7f);
    isDamaged = false;
  }

  IEnumerator DieProcess()
  {
    yield return new WaitForSeconds(2f);
    Destroy(gameObject);
  }
}

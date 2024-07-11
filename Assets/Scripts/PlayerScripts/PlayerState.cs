using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

  private float curHp;

  void Start()
  {
    playerData = GetComponent<PlayerData>();
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
    playerHpSlider.value = (float)curHp / (float)hp;
    playerHpText.text = string.Format("{0} / {1} ({2}%)", curHp, hp, (curHp * 100) / hp);
  }
}

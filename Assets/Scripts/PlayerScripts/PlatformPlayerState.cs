using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlatformPlayerState : MonoBehaviour
{
  public int money;
  public int jumpPower = 0;
  public int speed = 0;

  [SerializeField]
  private Text nameText;
  Vector3 offset;

  [SerializeField]
  private Text moneyText;

  void Start()
  {
    offset = nameText.transform.position - transform.position;
    nameText.text = Server.Instance.playerInfo.Name;
    moneyText.text = "재화: " + Server.Instance.playerInfo.Money;
  }

  void Update()
  {
    nameText.transform.position = gameObject.transform.position + offset;
    moneyText.text = "재화: " + Server.Instance.playerInfo.Money;
  }

 /* public void PlatformPlayerStateLoad(int speed, int money, int jumpPower, string name)
  {
    this.speed = speed;
    this.money = money;
    this.jumpPower = jumpPower;
    this.name = name;
    nameText.text = this.name;
    moneyText.text = "재화: " +  this.money.ToString();
  }*/
}

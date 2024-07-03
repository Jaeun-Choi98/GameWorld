using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
  public InputField id;
  public InputField password;

  public Text notify;

  public RawImage icon;

  Color32 color1;
  Color32 color2;
  bool colorSwitch = false;
  bool waitFlag = true;
  private void Start()
  {
    notify.text = "";
    color1 = new Color32(207, 163, 163, 255);
    color2 = new Color32(222, 210, 210, 255);
    icon.color = color1;
    
  }

  // Update is called once per frame
  void Update()
  {
    if (waitFlag)
    {
      StartCoroutine(EffectIcon(0.5f));
    }
  }

  IEnumerator EffectIcon(float time)
  {
    waitFlag = false;
    if (colorSwitch)
    {
      icon.color = color2;
    }
    else
    {
      icon.color = color1;
    }
    colorSwitch = !colorSwitch;
    yield return new WaitForSeconds(time);
    waitFlag = true;
  }

  public void SignUp()
  {
    /*if (!CheckIdOrPasswd(id.text, password.text))
    {
      return;
    }*/
    if (!PlayerPrefs.HasKey(id.text))
    {
      PlayerPrefs.SetString(id.text, password.text);
      notify.text = "아이디 생성이 완료됐습니다.";
    }
    else
    {
      notify.text = "이미 존재하는 아이디입니다.";
    }
  }

  public void SignIn()
  {
    /*if (!CheckIdOrPasswd(id.text, password.text))
    {
      return;
    }*/
    string pass = PlayerPrefs.GetString(id.text);
    if (password.text == pass)
    {
      SceneManager.LoadScene(1);
    }
    else
    {
      notify.text = "입력한 아이디 또는 패스워드가 일치하지 않습니다.";
    }
  }

  /*public bool CheckIdOrPasswd(string id, string pwd)
  {
    if (id == "" || pwd == "")
    {
      notify.text = "아이디 또는 패스워드를 입력하세요.";
      return false;
    }
    else
    {
      return true;
    }
  }*/
}


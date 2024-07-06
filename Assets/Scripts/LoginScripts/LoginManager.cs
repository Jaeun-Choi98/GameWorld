using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
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

  private class User
  {
    public string email;
    public string passwd;
    public int userId;
  }


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
    if (!CheckIdOrPasswd(id.text, password.text))
    {
      return;
    }

  }

  public void SignIn()
  {
    StartCoroutine(CorutineSignIn());
  }

  IEnumerator CorutineSignIn()
  {
    if (!CheckIdOrPasswd(id.text, password.text))
    {
      yield break;
    }

    string url = "http://localhost:8080/players/signin";
    User user = new User();
    user.email = id.text;
    user.passwd = password.text;
    string jsonStr = JsonUtility.ToJson(user);
    //Debug.Log("Request JSON: " + jsonStr);

    UnityWebRequest request = new UnityWebRequest(url, "POST");
    byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonStr);
    request.uploadHandler = new UploadHandlerRaw(jsonToSend);
    request.downloadHandler = new DownloadHandlerBuffer();
    request.SetRequestHeader("Content-Type", "application/json");

    yield return request.SendWebRequest();

    if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
    {
      Debug.LogError("Error: " + request.error);
      notify.text = request.error;
    }
    else
    {
      string jsonResponse = request.downloadHandler.text;
      int usreId = JsonUtility.FromJson<User>(jsonResponse).userId;
      //Debug.Log("Response: " + request.downloadHandler.text);
      
      // PlatformScene: 2
      LoadingManager.nextSceneNumber = 2;
      PlayerData.userId = usreId;
      SceneManager.LoadScene(1);
    }
  }

  public bool CheckIdOrPasswd(string id, string pwd)
  {
    if (id == "" || pwd == "")
    {
      notify.text = "이메일 또는 비밀번호를 입력하세요.";
      return false;
    }
    else
    {
      return true;
    }
  }
}


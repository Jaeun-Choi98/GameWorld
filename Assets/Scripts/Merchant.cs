using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Merchant : MonoBehaviour
{
  [SerializeField]
  private GameObject merchantCamera;

  // 이후에 UI 더 생길 수 있음. ex) 인벤토리 UI
  [SerializeField]
  private GameObject shopUI; // 스크롤 뷰 오브젝트

  public bool isAvailable = false;

  // 이후에 상점을 이용할 때 오브젝트의 움직임을 제한하기 위한 플래그 변수
  public bool isUseStore = false;

  [SerializeField]
  private GameObject imageF;


  public void OpenStore()
  {
    merchantCamera.SetActive(true);
    shopUI.SetActive(true);
    isUseStore = true;
    imageF.SetActive(false);
  }

  public void CloseStore()
  {
    merchantCamera.SetActive(false);
    // Notify 코루틴 때문
    StopAllCoroutines();
    shopUI.SetActive(false);
    isUseStore = false;
    if (isAvailable)
    {
      imageF.SetActive(true);
    }
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.transform.name == "Player")
    {
      imageF.SetActive(true);
      isAvailable = true;
    }
  }

  private void OnTriggerExit(Collider other)
  {
    if (other.transform.name == "Player")
    {
      imageF.SetActive(false);
      isAvailable = false;
    }
  }

}

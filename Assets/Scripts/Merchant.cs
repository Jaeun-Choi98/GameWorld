using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Merchant : MonoBehaviour
{
  [SerializeField]
  private GameObject merchantCamera;

  // ���Ŀ� UI �� ���� �� ����. ex) �κ��丮 UI
  [SerializeField]
  private GameObject shopUI; // ��ũ�� �� ������Ʈ

  public bool isAvailable = false;

  // ���Ŀ� ������ �̿��� �� ������Ʈ�� �������� �����ϱ� ���� �÷��� ����
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
    // Notify �ڷ�ƾ ����
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

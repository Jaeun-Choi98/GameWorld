using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossPotal : MonoBehaviour
{

  public GameObject imageF;
  private bool isAvailable;

  private void Start()
  {
    isAvailable = false;
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

  public void MoveToBossScene()
  {
    if (!isAvailable)
    {
      return;
    }
    LoadingManager.nextSceneNumber = 3;
    SceneManager.LoadScene(1);
  }
}

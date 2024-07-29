using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TwoDGame : MonoBehaviour
{
  private bool isAvailable = false;
  [SerializeField]
  private GameObject imageF;

  private void OnTriggerEnter(Collider other)
  {
    if(other.gameObject.layer == 7)
    {
      imageF.SetActive(true);
      isAvailable = true;
    }
  }

  private void OnTriggerExit(Collider other)
  {
    if (other.gameObject.layer == 7)
    {
      imageF.SetActive(false);
      isAvailable = false;
    }
  }

  void MoveTo2DGameScene()
  {
    if (!isAvailable)
    {
      return;
    }
    LoadingManager.nextSceneNumber = 4;
    SceneManager.LoadScene(1);
  }
}

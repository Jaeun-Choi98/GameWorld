using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{

  //public static LoadingManager instance;
  public static int nextSceneNumber;
  public Slider loadingBar;
  public Text loadingText;

  private void Awake()
  {
    /*if (instance == null)
    {
      instance = this;
    }*/
  }

  // Start is called before the first frame update
  void Start()
  {
    StartCoroutine(TransitionNextScene());
  }

  IEnumerator TransitionNextScene()
  {
    AsyncOperation ao = SceneManager.LoadSceneAsync(nextSceneNumber); 
    ao.allowSceneActivation = false;

    while (!ao.isDone)
    {
      loadingBar.value = ao.progress;
      loadingText.text = (ao.progress * 100f).ToString() + "%";

      if (ao.progress >= 0.9f)
      {
        yield return new WaitForSeconds(2f);
        ao.allowSceneActivation = true; 
      }
      yield return null;
    }
  }
}

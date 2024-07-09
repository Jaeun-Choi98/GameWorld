using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BoasGameManager : MonoBehaviour
{

  public static BoasGameManager Instance;

  public enum GameState
  {
    Ready,
    Run,
    Pause,
    GameOver
  }

  public GameState gameState;
  public GameObject gameOption;
  [SerializeField]
  private Text gameSateText;

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
    }
  }

  private void Start()
  {
    gameState = GameState.Ready;
    gameSateText = GameObject.Find("Text GameState").GetComponent<Text>();
    gameSateText.text = "Ready...";
    //gameSateText.color = new Color32(255, 185, 0, 255);
    StartCoroutine(ReadToStart());
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape) && gameState == GameState.Run)
    {
      OpenOptionWindow();
    }
    // 만약 플레이어의 hp가 0이하라면, 게임 상태 => 게임 오버
  }

  IEnumerator ReadToStart()
  {
    yield return new WaitForSeconds(2f);
    gameSateText.text = "Go!";
    yield return new WaitForSeconds(0.5f);
    gameSateText.gameObject.SetActive(false);
    gameState = GameState.Run;
  }

  public void OpenOptionWindow()
  {
    gameOption.SetActive(true);
    gameState = GameState.Pause;
    Time.timeScale = 0f;
  }

  public void CloseOptionWindow()
  {
    gameOption.SetActive(false);
    Time.timeScale = 1f;
    gameState = GameState.Run;
  }

  public void RestartGame()
  {
    Time.timeScale = 1f;
    SceneManager.LoadScene(3);
  }

  public void QuitGame()
  {
    Time.timeScale = 1f;
    LoadingManager.nextSceneNumber = 2;
    SceneManager.LoadScene(1);
  }
}

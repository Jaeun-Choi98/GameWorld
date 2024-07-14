using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static PlayerData;

public class BoasGameManager : MonoBehaviour
{

  public static BoasGameManager Instance;

  public enum GameState
  {
    Ready,
    Run,
    Pause,
    Victory,
    GameOver
  }

  public GameState gameState;
  public GameObject gameOption;
  [SerializeField]
  private Text gameSateText;

  [SerializeField]
  private PlayerState playerState;
  [SerializeField]
  private EnemyFSM enemyFSM;
  [SerializeField]
  private PlayerData playerData;

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
    gameSateText.text = "BossGame!";
    playerState = GameObject.Find("Player").GetComponent<PlayerState>();
    playerData = GameObject.Find("Player").GetComponent<PlayerData>();
    enemyFSM = GameObject.Find("Enemy").GetComponent<EnemyFSM>();
    //gameSateText.color = new Color32(255, 185, 0, 255);
    StartCoroutine(ReadToStart());
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape) && gameState == GameState.Pause)
    {
      CloseOptionWindow();
    }
    else if (Input.GetKeyDown(KeyCode.Escape) && gameState == GameState.Run)
    {
      OpenOptionWindow();
    }
    // ���� �÷��̾��� hp�� 0���϶��, ���� ���� => ���� ����
    if (playerState.curHp <= 0f && gameState == GameState.Run)
    {
      StartCoroutine(RunToGameover());
    }
    else if (enemyFSM.curHp <= 0f && gameState == GameState.Run)
    {
      StartCoroutine(RunToVictory()); ;
    }
  }

  IEnumerator RunToGameover()
  {
    gameState = GameState.GameOver;
    gameSateText.text = "Game Over";
    gameSateText.gameObject.SetActive(true);
    Time.timeScale = 0.3f;
    yield return new WaitForSeconds(1f);
    Time.timeScale = 1f;
    LoadingManager.nextSceneNumber = 2;
    SceneManager.LoadScene(1);
  }

  IEnumerator RunToVictory()
  {
    gameState = GameState.Victory;
    gameSateText.text = "Victory";
    gameSateText.gameObject.SetActive(true);
    Time.timeScale = 0.3f;
    yield return new WaitForSeconds(1f);
    Time.timeScale = 1f;

    Server.Instance.SaveBossGameData(PlayerData.userId, playerData.playerId,
      playerData.playerName, playerData.money+200, playerData.speed, playerData.jumpPower);
    LoadingManager.nextSceneNumber = 2;
    SceneManager.LoadScene(1);
  }

  IEnumerator ReadToStart()
  {
    yield return new WaitForSeconds(6f);
    gameSateText.text = "Start!";
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

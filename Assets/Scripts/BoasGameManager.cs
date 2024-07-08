using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

  IEnumerator ReadToStart()
  {
    yield return new WaitForSeconds(2f);
    gameSateText.text = "Go!";
    yield return new WaitForSeconds(0.5f);
    gameSateText.gameObject.SetActive(false);
    gameState = GameState.Run;
  }
}

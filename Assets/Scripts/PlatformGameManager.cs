using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGameManager : MonoBehaviour
{
  public static PlatformGameManager Instance;

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
    }
  }
}

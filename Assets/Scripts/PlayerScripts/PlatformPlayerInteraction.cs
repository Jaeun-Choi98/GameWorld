using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformPlayerInteraction : MonoBehaviour
{
  [SerializeField]
  private BossPotal bossPotal;
  
  void Start()
  {
    bossPotal = GameObject.Find("BossGamePotal").GetComponent<BossPotal>();
  }

  void Update()
  {
    GoBossScene();
  }

  private void GoBossScene()
  {
    if (Input.GetKeyDown(KeyCode.F))
    {
      bossPotal.MoveToBossScene();
    }
  }
}

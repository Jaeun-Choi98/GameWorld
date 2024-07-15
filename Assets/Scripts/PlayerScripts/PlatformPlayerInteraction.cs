using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformPlayerInteraction : MonoBehaviour
{
  [SerializeField]
  private BossPotal bossPotal;

  [SerializeField]
  private Merchant merchant;
  
  void Start()
  {
    bossPotal = GameObject.Find("BossGamePotal").GetComponent<BossPotal>();
    merchant = GameObject.Find("Merchant").GetComponent<Merchant>();
  }

  void Update()
  {
    GoBossScene();
    InteractMerchant();
  }

  private void GoBossScene()
  {
    if (Input.GetKeyDown(KeyCode.F))
    {
      bossPotal.MoveToBossScene();
    }
  }

  private void InteractMerchant()
  {
    if (Input.GetKeyDown(KeyCode.F))
    {
      if (merchant.isAvailable)
      {
        merchant.OpenStore();
      }
    }
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      if (merchant.isAvailable)
      {
        merchant.CloseStore();
      }
    }
    if (!merchant.isAvailable)
    {
      merchant.CloseStore();
    }
  }
}

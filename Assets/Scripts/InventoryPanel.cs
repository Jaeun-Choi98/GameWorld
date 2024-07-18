using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
  [SerializeField]
  private GameObject slotPrefabs;
  [SerializeField]
  private Transform slotContainer;

  [SerializeField]
  private float spacing = 50f;

  // 왼쪽 상단 ( 기준 )
  [SerializeField]
  private float offsetX;
  [SerializeField]
  private float offsetY;

  private void Start()
  {
    slotContainer = transform.GetChild(0);
    UploadInventory();
  }

  private void UploadInventory()
  {
    if (slotContainer != null)
    {
      foreach (Transform child in slotContainer)
      {
        Destroy(child.gameObject);
      }
    }

    offsetX = 100f; offsetY = -100f;
    foreach (Inventory i in Server.Instance.inventory)
    {
      GameObject newSlot = Instantiate(slotPrefabs, slotContainer);
      newSlot.GetComponent<Image>().sprite = Resources.Load<Sprite>(i.ItemName);
      Text[] texts = newSlot.GetComponentsInChildren<Text>();
      texts[0].text = i.ItemName;
      texts[1].text = i.Quantity.ToString();

      RectTransform rectTransform = newSlot.GetComponent<RectTransform>();
      rectTransform.anchoredPosition = new Vector2(offsetX, offsetY);
      if (offsetX >= 780)
      {
        offsetX = spacing;
        offsetY -= (rectTransform.sizeDelta.y + spacing);
      }
      else
      {
        offsetX += (rectTransform.sizeDelta.x + spacing);
      }
    }
  }

  public void ToggleInventory()
  {
    gameObject.SetActive(!gameObject.activeSelf);
    UploadInventory();
  }
}

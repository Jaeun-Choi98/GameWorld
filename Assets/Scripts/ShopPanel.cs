using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanel : MonoBehaviour
{
  public GameObject slotPrefabs;
  public Transform slotContainer;

  [SerializeField]
  private float spacing = 50f;

  // 왼쪽 상단 ( 기준 )
  [SerializeField]
  private float offsetX;
  [SerializeField]
  private float offsetY;


  // 아이템 선택/해제 기능 
  private Color selectedColor = Color.gray;
  private Color defaultColor = Color.white;

  [SerializeField]
  private List<Item> selectedItems;
  private Dictionary<int, bool> selectedItemsState;


  [SerializeField]
  private Button buyBtn;
  [SerializeField]
  private Button sellBtn;
  private int sumPrice;
  [SerializeField]
  private Text notify;

  private void OnEnable()
  {
    sumPrice = 0;
    selectedItems.Clear();
    selectedItemsState.Clear();
    notify.gameObject.SetActive(false);
    UploadInventory();
  }

  private void Start()
  {
    selectedItems = new List<Item>();
    selectedItemsState = new Dictionary<int, bool>();
    slotContainer = transform.GetChild(0);
    buyBtn = transform.GetChild(2).GetComponent<Button>();
    sellBtn = transform.GetChild(3).GetComponent<Button>();
    buyBtn.onClick.AddListener(() => BuySelectedItems());
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
    foreach (Item i in Server.Instance.items)
    {
      GameObject newSlot = Instantiate(slotPrefabs, slotContainer);
      newSlot.GetComponent<Image>().sprite = Resources.Load<Sprite>(i.itemName);
      Text[] texts = newSlot.GetComponentsInChildren<Text>();
      texts[0].text = i.itemName;
      texts[1].text = i.price.ToString();

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

      Button btn = newSlot.GetComponent<Button>();
      btn.onClick.AddListener(() => ToggleItemSelection(btn, i));
      selectedItemsState.Add(i.itemId, false);
    }
  }

  void ToggleItemSelection(Button btn, Item i)
  {
    if (selectedItemsState[i.itemId])
    {
      btn.image.color = defaultColor;
      sumPrice -= i.price;
      selectedItems.Remove(i);
      selectedItemsState[i.itemId] = false;
    }
    else
    {
      btn.image.color = selectedColor;
      sumPrice += i.price;
      selectedItems.Add(i);
      selectedItemsState[i.itemId] = true;
    }
  }

  void BuySelectedItems()
  {
    if (selectedItems.Count == 0)
    {
      return;
    }

    if (sumPrice > Server.Instance.playerInfo.Money)
    {
      StartCoroutine(NotifyText("재화가 부족합니다."));
      return;
    }

    foreach (Item item in selectedItems)
    {
      InventoryManager.Instance.AddItem(item);
    }
    Server.Instance.playerInfo.Money -= sumPrice;
    // 변경사항 서버에 저장
    Server.Instance.SavePlayerInfoAndInventory();
  }

  IEnumerator NotifyText(string text)
  {
    notify.text = text;
    notify.gameObject.SetActive(true);
    yield return new WaitForSeconds(1f);
    notify.gameObject.SetActive(false);
    yield break;
  }
}

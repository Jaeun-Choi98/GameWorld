using System;
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

  // 구매탭
  [SerializeField]
  private List<Item> selectedItems;
  private Dictionary<int, bool> selectedItemsState;

  // 판매탭
  [SerializeField]
  private List<Inventory> selectedInventories;
  private Dictionary<int, bool> selectedInventoriesState;
  

  [SerializeField]
  private Button buyBtn;
  [SerializeField]
  private Button sellBtn;
  private int sumPrice;
  [SerializeField]
  private Text notify;

  [SerializeField]
  private InventoryManager inventoryManager;

  [SerializeField]
  private Button buyTab;
  [SerializeField]
  private Button sellTab;
  [SerializeField]
  private bool isSellTab;
  private void OnEnable()
  {
    sumPrice = 0;
    selectedItems.Clear();
    selectedItemsState.Clear();
    selectedInventories.Clear();
    selectedInventoriesState.Clear();
    isSellTab = false;
    buyTab.image.color = selectedColor;
    sellTab.image.color = defaultColor;
    notify.gameObject.SetActive(false);
    UploadInventory();
  }

  private void Start()
  {
    selectedItems = new List<Item>();
    selectedItemsState = new Dictionary<int, bool>();
    selectedInventories = new List<Inventory>();
    selectedInventoriesState = new Dictionary<int, bool>();
    
    slotContainer = transform.GetChild(0);
    buyBtn = transform.GetChild(2).GetComponent<Button>();
    sellBtn = transform.GetChild(3).GetComponent<Button>();
    buyTab = transform.GetChild(5).GetComponent<Button>();
    sellTab = transform.GetChild(6).GetComponent<Button>();
    buyTab.image.color = selectedColor;
    sellTab.image.color = defaultColor;
    notify.gameObject.SetActive(false);
    buyBtn.onClick.AddListener(() => BuySelectedItems());
    sellBtn.onClick.AddListener(() => SellSelectedInventroies());
    buyTab.onClick.AddListener(() => OpenBuyTab());
    sellTab.onClick.AddListener(() => OpenSellTab());
    UploadInventory();
  }

  private void OpenBuyTab()
  {
    if (!isSellTab) return;
    buyTab.image.color = selectedColor;
    sellTab.image.color = defaultColor;
    isSellTab = false;
    selectedInventories.Clear();
    selectedInventoriesState.Clear();
    sumPrice = 0;
    UploadInventory();
  }
  private void OpenSellTab()
  {
    if (isSellTab) return;
    buyTab.image.color = defaultColor;
    sellTab.image.color = selectedColor;
    isSellTab = true;
    selectedItems.Clear();
    selectedItemsState.Clear();
    sumPrice = 0;
    UploadInventory();
  }


  private void UploadInventory()
  {
    if (!isSellTab)
    {
      if (slotContainer != null)
      {
        foreach (Transform child in slotContainer)
        {
          Destroy(child.gameObject);
        }
      }

      offsetX = 100f; offsetY = -130f;
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
    else
    {
      if (slotContainer != null)
      {
        foreach (Transform child in slotContainer)
        {
          Destroy(child.gameObject);
        }
      }
      offsetX = 100f; offsetY = -130f;
      foreach (Inventory i in Server.Instance.inventory)
      {
        GameObject newSlot = Instantiate(slotPrefabs, slotContainer);
        newSlot.GetComponent<Image>().sprite = Resources.Load<Sprite>(i.ItemName);
        Text[] texts = newSlot.GetComponentsInChildren<Text>();
        texts[0].text = i.ItemName;
        texts[1].text = (Server.Instance.dicItemPrice[i.ItemId] * 0.6).ToString();

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
        btn.onClick.AddListener(() => ToggleInventorySelection(btn, i));
        selectedInventoriesState.Add(i.ItemId, false);
        
      }
    }
  }

  void ToggleInventorySelection(Button btn, Inventory i)
  {
    if (selectedInventoriesState[i.ItemId])
    {
      btn.image.color = defaultColor;
      sumPrice -= (int)Math.Ceiling(Server.Instance.dicItemPrice[i.ItemId] * 0.6);
      selectedInventories.Remove(i);
      selectedInventoriesState[i.ItemId] = false;
    }
    else
    {
      btn.image.color = selectedColor;
      sumPrice += (int)Math.Ceiling(Server.Instance.dicItemPrice[i.ItemId] * 0.6);
      selectedInventories.Add(i);
      selectedInventoriesState[i.ItemId] = true;
    }
  }

  void SellSelectedInventroies()
  {
    if (!isSellTab) return;
    if (selectedInventories.Count == 0) return;
    int buf = 0;
    foreach (Inventory i in selectedInventories)
    {
      if(i.Quantity == 1)
      {
        /*selectedInventories.Remove(i);
        selectedInventoriesState.Remove(i.ItemId); 
        Destroy(selectedInventoriesBtn[i.ItemId].gameObject);*/
        buf += (int)Math.Ceiling(Server.Instance.dicItemPrice[i.ItemId] * 0.6);
      }
      inventoryManager.RemoveItem(i.ItemId);
    }
    Server.Instance.playerInfo.Money += sumPrice;
    sumPrice -= buf;
    selectedInventories.Clear();
    selectedInventoriesState.Clear();
    UploadInventory();
    Server.Instance.SavePlayerInfoAndInventory();
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
    if (isSellTab) return;

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
      //InventoryManager.Instance.AddItem(item);
      inventoryManager.AddItem(item);
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

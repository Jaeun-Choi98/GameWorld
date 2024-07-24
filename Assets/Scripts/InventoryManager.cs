using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
  /*private static InventoryManager instance = null;

  public static InventoryManager Instance
  {
    get
    {
      if (instance == null)
      {
        return null;
      }
      return instance;
    }
  }

  private void Awake()
  {
    if (instance == null)
    {
      instance = this;
      DontDestroyOnLoad(gameObject);
    }
    else
    {
      Destroy(gameObject);
    }
  }*/

  public int maxSize = 10;

  [SerializeField]
  private InventoryPanel inventoryPanel;

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.I))
    {
      inventoryPanel.ToggleInventory();
    }
  }

  public bool AddItem(Item item)
  {
    if (Server.Instance.inventory != null && Server.Instance.inventory.Count < maxSize)
    {
      bool exist = false;
      foreach (var i in Server.Instance.inventory)
      {
        if(i.ItemId == item.itemId)
        {
          exist = true;
          i.Quantity += 1;
        }
      }
      if (!exist)
      {
        Inventory newInventory = new Inventory();
        newInventory.ItemId = item.itemId;
        newInventory.Quantity = 1;
        newInventory.ItemName = item.itemName;
        Server.Instance.inventory.Add(newInventory);
      }
    }
    return false;
  }

  public void RemoveItem(int itemId)
  {
    for (int i=0;i<Server.Instance.inventory.Count;i++)
    {
      if (Server.Instance.inventory[i].ItemId == itemId)
      {
        if (Server.Instance.inventory[i].Quantity > 1)
        {
          Server.Instance.inventory[i].Quantity -= 1;
        }
        else
        {
          Server.Instance.inventory.Remove(Server.Instance.inventory[i]);
        }
      }
    }
  }
}

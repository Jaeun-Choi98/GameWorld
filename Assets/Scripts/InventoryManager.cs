using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
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
    if (Server.Instance.inventory.Count < maxSize)
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

  public void RemoveItem(Item item)
  {
    foreach (var i in Server.Instance.inventory)
    {
      if (i.ItemId == item.itemId)
      {
        if(i.Quantity > 1)
        {
          i.Quantity -= 1;
        }
        else
        {
          Server.Instance.inventory.Remove(i);
        }
      }
    }
  }
}

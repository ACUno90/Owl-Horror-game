using System;
using UnityEngine;
[System.Serializable]


public class ChestItem
{
  public ItemSO item;
    public int amount;  
}

public class ChestInventory : MonoBehaviour,IInteractable
{
   public int ChestSize = 20;

    public ChestItem[] StoredItems;

    private static GameObject ChestUI;
    private static Slot[] slots;
    private bool isOpen;

    private void Awake()
    {

        StoredItems = new ChestItem[ChestSize];

        for(int i = 0; i < ChestSize; i++)
        {
            StoredItems[i] = new ChestItem();
        }

        if(ChestUI == null)
        {
            ChestUI = Inventory.Instance.ChestSlotParent.gameObject;
            slots = ChestUI.GetComponentsInChildren<Slot>(true);

            ChestUI.SetActive(false);
        }
    }

    public void Interact()
    {
        if (!isOpen)
        {
            OpenChest();
           
        }
        else
        {
            CloseChest();
        }
    }

    public void OpenChest()
    {
        GameManger.Instance.InventoryMenu();
        isOpen = true;
        ChestUI.SetActive(true);

        for(int i = 0; i < slots.Length; i++)
        {
            var data = StoredItems[i];
            if (data.item != null)
            {
                slots[i].SetItem(data.item, data.amount);
            }
            else
            {
                slots[i].ClearSLot();
            }
        }
    }

    public void CloseChest()
    {
       if (!isOpen)
            return; 
        isOpen = false;

        for(int i = 0; i < slots.Length; i++)
        {
            if(slots[i].HasItem())  
            {
                StoredItems[i].item = slots[i].GetItem();
                StoredItems[i].amount = slots[i].GetAmount();
            }
            else
            {
                StoredItems[i].item = null;
                StoredItems[i].amount = 0;
            }
        }
        ChestUI.SetActive(false);


    }
}

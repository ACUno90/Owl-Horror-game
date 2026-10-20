using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
public class Inventory : MonoBehaviour
{
    public ItemSO RandomItemTest;
    public ItemSO RandomItemTest2;

    public GameObject InventorySlotParent;
    public List<Slot> InventorySlots = new List<Slot>();
    public List<Slot> AllSlots = new List<Slot>();

    private void Awake()
    {
            InventorySlots.AddRange(InventorySlotParent.GetComponentsInChildren<Slot>());
        AllSlots.AddRange(InventorySlots);


    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            AddItem(RandomItemTest, 1);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            AddItem(RandomItemTest2, 1);
        }
    }

    public void AddItem(ItemSO itemToAdd, int amount)
    {
       int remainingAmount = amount;
        foreach (Slot slot in AllSlots)
        {
           if(slot.HasItem() && slot.GetItem() == itemToAdd)
            {
               int currentAmount = slot.GetAmount();    
                int maxStackSize = itemToAdd.MaxStackSize;
                if (currentAmount < maxStackSize)
                {
                    int spaceLeft = maxStackSize - currentAmount;
                    int amountToAdd = Mathf.Min(spaceLeft, remainingAmount);
                    slot.SetItem(itemToAdd, currentAmount + amountToAdd);
                    remainingAmount -= amountToAdd;
                    if (remainingAmount <= 0)
                    {
                        return;
                    }
                }
            }
        }
        // Now, if there are still remaining items to add, we need to find empty slots to place them in.
        foreach (Slot slot in AllSlots)
        {
            if (!slot.HasItem())
            {
              int amountToPlace = Mathf.Min(itemToAdd.MaxStackSize, remainingAmount);
                slot.SetItem(itemToAdd, amountToPlace);
                remainingAmount -= amountToPlace;
                if (remainingAmount <= 0)
                {
                    return;
                }   
            }
        }

        if(remainingAmount > 0)
        {
            Debug.LogWarning("Shit full, can't add more. Remaining amount: " + remainingAmount);
        }

    }
   


}

using UnityEngine;
using System.Collections.Generic;

[System.Serializable]

public class InventorySaveData
{
    public List<SlotData> Slots = new List<SlotData>();
}

[System.Serializable]

public class SlotData
{
    public string ItemID;
   
    public int amount;
}

public class ItemDataBase : MonoBehaviour
{
    public static ItemDataBase instance;

    public List<ItemSO> AllItems;

    private Dictionary<string, ItemSO> itemLookUp = new Dictionary<string, ItemSO>();

    public void Awake()
    {
        instance = this;
        foreach (var item in AllItems)
        {
            itemLookUp[item.itemID] = item;
        }
    }

    public ItemSO GetItemByID(string id)
    {
        if (itemLookUp.ContainsKey(id))
        {
            return itemLookUp[id];
        }
        else
        {
            Debug.LogWarning("Item with ID " + id + " not found in the database.");
            return null;
        }
    }
}

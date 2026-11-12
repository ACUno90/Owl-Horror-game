using UnityEngine;
using System.IO;
public class SaveSystem : MonoBehaviour
{
 private string path;

    private void Awake()
    {
        path = Application.persistentDataPath + "/savefile.json";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            SaveInventory();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            LoadInventory();
        }
    }

    public void SaveInventory()
    {
        InventorySaveData data = new InventorySaveData();

        foreach(Slot slot in Inventory.Instance.GetAllSlots())
        {
            SlotData slotData = new SlotData();

            if(slot.HasItem())
            {
                slotData.ItemID = slot.GetItem().itemID;
                slotData.amount = slot.GetAmount();
            }
            else
            {
                slotData.ItemID = "";
                slotData.amount = 0;
            }
            data.Slots.Add(slotData);
        }
        //we can change to false to take up less space 
        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(path, json);

        Debug.Log("saved to: " + path);
    }

    public void LoadInventory()
    {
        if (!File.Exists(path))
        {
            Debug.Log("Save file not found at: ");
            return;
        }

        string json = File.ReadAllText(path);
        InventorySaveData data = JsonUtility.FromJson<InventorySaveData>(json);

        var slots = Inventory.Instance.GetAllSlots();
         
        for(int i = 0; i < slots.Count; i++)
        {

            if(i >= data.Slots.Count)
            {
                break;
            }
         
            SlotData slotData = data.Slots[i];
            if (!string.IsNullOrEmpty(slotData.ItemID))
            {
                ItemSO item = ItemDataBase.instance.GetItemByID(slotData.ItemID);
                slots[i].SetItem(item, slotData.amount);
            }
            else
            {
                slots[i].ClearSLot();
            }
        }

    }
}

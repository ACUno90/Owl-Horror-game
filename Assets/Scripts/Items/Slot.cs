using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler    
{
    public bool Hovering;
    private ItemSO HeldItem;
    private int itemAmount;

    private Image IconImage;

    private TextMeshProUGUI AmountText;

    void Awake()
    {
        IconImage = GetComponent<Image>();
        AmountText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public ItemSO GetItem()
    {
        return HeldItem;
    }

    public int GetAmount()
    {
        return itemAmount;
    }   

    public  void SetItem(ItemSO item, int amount)
    {
        HeldItem = item;
        itemAmount = amount;
        UpdateSlotUI();
    }

    private void UpdateSlotUI()
    {
        if(IconImage == null )
        {
            IconImage = GetComponent<Image>();
            AmountText = GetComponentInChildren<TextMeshProUGUI>();
        }

        if (HeldItem != null)
        {
            IconImage.enabled = true;   
            IconImage.sprite = HeldItem.Icon;
            AmountText.text = itemAmount.ToString();
        }
        else
        {
            IconImage.sprite = null;
            AmountText.text = string.Empty;
        }
    }

    public int AddAmount(int amountToAdd)
    {
        if (HeldItem == null)
        {
            Debug.LogWarning("Slot: Cannot add amount to an empty slot.");
            return 0;
        }
        itemAmount += amountToAdd;
        UpdateSlotUI();
        return amountToAdd;
    }


    public void RemoveAmount(int amountToRemove) {
        itemAmount -= amountToRemove;
        if (itemAmount <= 0)
        {
            ClearSLot();
        }
        else
        {
            UpdateSlotUI();
        }

    }

    public void ClearSLot()
    {
        HeldItem = null;
        itemAmount = 0;
        UpdateSlotUI();
    }

    public bool HasItem()
    {
        return HeldItem != null;
    }

    // Added implementations to satisfy the interfaces:
    public void OnPointerEnter(PointerEventData eventData)
    {
        Hovering = true;
        // Optional: show tooltip or highlight
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Hovering = false;
        // Optional: hide tooltip or remove highlight
    }
}

using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using System.Runtime.CompilerServices;
using UnityEngine.UI;
//using UnityEngine.UIElements;
using UnityEditor;
using TMPro;
public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    public Image DragIcon;

    public GameObject InventorySlotParent;
    public GameObject HotbarSlotParent;
   public Transform ChestSlotParent;
    public List<Slot> InventorySlots = new List<Slot>();
    public List<Slot> HotbarSlots = new List<Slot>();   
    public List<Slot> AllSlots = new List<Slot>();
    public List<Slot> ChestSlots = new List<Slot>();

    public Slot DragedSlot = null;
    private bool isDragging = false;

    public float pickupRange = 3f; // The range within which the player can pick up items
    private Item LookedAtItem = null;
    private Renderer LookedAtRenderer = null;
    [SerializeField] private LayerMask itemLayerMask;

    private int EquippedHotBarIndex = 0;//0-2
    public float equippedOpacity = 0.9f;
    public float normalOpacity = 0.58f;

    public Transform Hand;
    private GameObject CurrentHandItem;

    public GameObject ItemDescriptionParent;
    public Image ItemDescriptionImage;

    public TextMeshProUGUI DescriptionItemNameText;
    public TextMeshProUGUI DescriptionItemText;


    [Header("audio")]
    public AudioClip AUDPickUpSound;
    public AudioClip AUDDropSound;
    public float AUDInventoryVol;
    public AudioSource InventoryAUD;

    private void Awake()
    {
        Instance = this;
        HotbarSlots.AddRange(HotbarSlotParent.GetComponentsInChildren<Slot>());
        InventorySlots.AddRange(InventorySlotParent.GetComponentsInChildren<Slot>());
        AllSlots.AddRange(InventorySlots);
        AllSlots.AddRange(HotbarSlots);
        ChestSlots.AddRange(ChestSlotParent.GetComponentsInChildren<Slot>());


        ChestSlotParent.gameObject.SetActive(false);
    }

    void Update()
    {
    
        DetectLookedAtItem();
        PickUp();

        StartDrag();
        UpdateDragItemPosition();
        EndDrag();

        HandleHotBarSelection();
        HandleDropEquippedItem();
        UpdateHotBarOpacity();
        UpdateItemDescription();
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
    
    private void PickUp()
    {
        if(LookedAtRenderer != null && Input.GetKeyDown(KeyCode.E))
        {
           Item itemComponent = LookedAtRenderer.GetComponent<Item>();
            if (itemComponent != null)
            {
                InventoryAUD.PlayOneShot(AUDPickUpSound, AUDInventoryVol);
                AddItem(itemComponent.item, itemComponent.amount);
                Destroy(LookedAtRenderer.gameObject);
                EquippedHandItem();
                Debug.Log("Item picked up.");
            }
        }
        else
        {
            Debug.Log("No item detected in front of player.");
        }

    }

    private void DetectLookedAtItem()
    {
        if (LookedAtRenderer != null)
        {
            LookedAtRenderer = null;
        }

       
        // ray cast from camera to detect item in front of player and hits only item layer

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if(Physics.Raycast(ray, out RaycastHit hitInfo, pickupRange,itemLayerMask))
        {
           Item item = hitInfo.collider.GetComponent<Item>();
            if (item != null)
            {
               Renderer rend = item.GetComponent<Renderer>();
                if (rend != null)
                {
                    LookedAtRenderer = rend;
                }
            }
        }
       
    }

    private void StartDrag()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Slot hovered = GetHoverSlot();

            if(hovered != null && hovered.HasItem())
            {
                DragedSlot = hovered;
                isDragging = true;
                //show drag item icon
                DragIcon.sprite = DragedSlot.GetItem().Icon;
                DragIcon.color = new Color(1, 1, 1, 0.5f);
                DragIcon.enabled = true;

            }
        }
    }

    private void EndDrag()
    {
        if(Input.GetMouseButtonUp(0)&& isDragging)
        {
            Slot hovered = GetHoverSlot();
            if(hovered != null)
            {
                HandleDrop(DragedSlot, hovered);
                DragIcon.enabled = false;

                DragedSlot = null;
                isDragging = false;
            }
        }
    }

    private void HandleDrop(Slot from, Slot to)
    {
        // to check if its the same spot, we return nothing
        if (from == to) return;
        //if the items are the same, we stack them
        if(to.HasItem()&& to.GetItem() == from.GetItem())
        {
            int max = to.GetItem().MaxStackSize;
            int space = max - to.GetAmount();

            if(space > 0)
            {
                int move = Mathf.Min(space, from.GetAmount());

                to.SetItem(to.GetItem(), to.GetAmount() + move);
                from.SetItem(from.GetItem(), from.GetAmount() - move);

                if(from.GetAmount() <= 0)
                {
                    from.ClearSLot();
                }
                return;
            }
        }
        //different item
        if (to.HasItem())
        {
            ItemSO TempItem = to.GetItem();
            int tempAmount = to.GetAmount();

            to.SetItem(from.GetItem(), from.GetAmount());
            from.SetItem(TempItem, tempAmount);
            return;
        }

        //empty slot
        to.SetItem(from.GetItem(), from.GetAmount());
        from.ClearSLot();
    }

    private Slot GetHoverSlot()
    {
        foreach (Slot slot in AllSlots)
        {
            if (slot.Hovering)
            {
                return slot;
            }
        }

        foreach (Slot CSlot in ChestSlots)
        {
            if (CSlot.Hovering)
            {
                return CSlot;
            }
        }
        return null;    
    }

    private void UpdateDragItemPosition()
    {
        if (isDragging)
        {
            DragIcon.transform.position = Input.mousePosition;
        }
    }

    private void UpdateHotBarOpacity()
    {
        for(int i = 0; i < HotbarSlots.Count; i++)
        {
            Image Icon = HotbarSlots[i].GetComponent<Image>();
            if(Icon != null)
            {
                Icon.color = (i == EquippedHotBarIndex) ? new Color(1, 1, 1, equippedOpacity) : new Color(1, 1, 1, normalOpacity);
            }
        }
    }


    private void HandleHotBarSelection()
    {
        for(int i =0; i < 6; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquippedHotBarIndex = i;
                UpdateHotBarOpacity();
                EquippedHandItem();
            }
        }
    }

    private void HandleDropEquippedItem()
    {
        if (!Input.GetKeyDown(KeyCode.Q))
            return;

        InventoryAUD.PlayOneShot(AUDDropSound, AUDInventoryVol);

        Slot EquippedItem = HotbarSlots[EquippedHotBarIndex];

        if (!EquippedItem.HasItem())
            return;

        ItemSO iSO = EquippedItem.GetItem();
        GameObject ItemPrefab = iSO.ItemPrefab;

        if (ItemPrefab == null)
            return;

        GameObject dropped = Instantiate(ItemPrefab, Camera.main.transform.position + Camera.main.transform.forward, Quaternion.identity);

        Item item = dropped.GetComponent<Item>();
        item.item = iSO;
        item.amount = EquippedItem.GetAmount();

        EquippedItem.ClearSLot();
        EquippedHandItem();
    }

    private void EquippedHandItem()
    {
        //clear up previous hand Item
        if (CurrentHandItem != null)
            Destroy(CurrentHandItem);

        Slot equippedSlot = HotbarSlots[EquippedHotBarIndex];
        if (!equippedSlot.HasItem())
            return;

        ItemSO iE = equippedSlot.GetItem();
        if (iE.HandItemPrefab == null)
            return;

        CurrentHandItem = Instantiate(iE.HandItemPrefab, Hand);
        CurrentHandItem.transform.localPosition = Vector3.zero;
        CurrentHandItem.transform.localRotation = iE.HandItemPrefab.transform.rotation; 

    }

    private void UpdateItemDescription()
    {
        Slot hoveredSlot = GetHoverSlot();

        if(hoveredSlot != null)
        {
            ItemSO HoverdItem = hoveredSlot.GetItem();

            if(HoverdItem != null)
            {
                ItemDescriptionParent.SetActive(true);
                ItemDescriptionImage.sprite = HoverdItem.Icon;
                DescriptionItemText.text = HoverdItem.ItemDescription;
                DescriptionItemNameText.text = HoverdItem.itemName;
            }
        }
        else
        {
            ItemDescriptionParent.SetActive(false);
        }

       
    }

    public void ConsumeItem(int amount =1)
    {
        Slot es = HotbarSlots[EquippedHotBarIndex];
        if (!es.HasItem())
            return;
        int NewAmount = es.GetAmount() - amount;

        if(NewAmount <= 0)
        {
            es.ClearSLot();
        }
        else
        {
            es.SetItem(es.GetItem(), NewAmount);
        }
    }
}

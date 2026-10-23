using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using System.Runtime.CompilerServices;
using UnityEngine.UI;
//using UnityEngine.UIElements;
using UnityEditor;
public class Inventory : MonoBehaviour
{
    public ItemSO RandomItemTest;
    public ItemSO RandomItemTest2;

    public Image DragIcon;

    public GameObject InventorySlotParent;
    public GameObject HotbarSlotParent;
    public List<Slot> InventorySlots = new List<Slot>();
    public List<Slot> HotbarSlots = new List<Slot>();   
    public List<Slot> AllSlots = new List<Slot>();

    public Slot DragedSlot = null;
    private bool isDragging = false;

    public float pickupRange = 3f; // The range within which the player can pick up items
    private Item LookedAtItem = null;
    private Renderer LookedAtRenderer = null;
    [SerializeField] private LayerMask itemLayerMask;

    private void Awake()
    {
        HotbarSlots.AddRange(HotbarSlotParent.GetComponentsInChildren<Slot>());
        InventorySlots.AddRange(InventorySlotParent.GetComponentsInChildren<Slot>());
        AllSlots.AddRange(InventorySlots);
        AllSlots.AddRange(HotbarSlots);


    }

    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Q))
        //{
        //    AddItem(RandomItemTest, 1);
        //}
        //else if (Input.GetKeyDown(KeyCode.E))
        //{
        //    AddItem(RandomItemTest2, 1);
        //}
        DetectLookedAtItem();
        PickUp();

        StartDrag();
        UpdateDragItemPosition();
        EndDrag();
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
        if(LookedAtRenderer != null && Input.GetKeyDown(KeyCode.Q))
        {
           Item itemComponent = LookedAtRenderer.GetComponent<Item>();
            if (itemComponent != null)
            {
                AddItem(itemComponent.item, itemComponent.amount);
                Destroy(LookedAtRenderer.gameObject);
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
        return null;    
    }

    private void UpdateDragItemPosition()
    {
        if (isDragging)
        {
            DragIcon.transform.position = Input.mousePosition;
        }
    }
}

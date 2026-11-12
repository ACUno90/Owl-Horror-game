using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "NewItem")]
public class ItemSO : ScriptableObject
{
    public string itemID;
    public string itemName;
    public Sprite Icon;
     public int MaxStackSize;
    public GameObject ItemPrefab;
    public GameObject HandItemPrefab;

    public string ItemDescription;
}

using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "NewItem")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public Sprite itemType;
     public int MaxStackSize;
    public GameObject ItemPrefab;
    public GameObject HandItemPrefab;
}

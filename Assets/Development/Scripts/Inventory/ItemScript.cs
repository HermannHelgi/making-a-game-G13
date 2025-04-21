using UnityEngine;

[CreateAssetMenu(fileName = "ItemScript", menuName = "Scriptable Objects/ItemScript")]
public class ItemScript : ScriptableObject
{
    public string itemname;
    public int index;
    public Sprite icon;
    public GameObject model;
    public ItemScript[] craftingrecipe;
    public bool onetimecraft;
    public bool consumable;
    public float hungergain;
    public bool unique;
    public string craftingDescription;
}

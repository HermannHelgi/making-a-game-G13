using UnityEngine;

[CreateAssetMenu(fileName = "ItemScript", menuName = "Scriptable Objects/ItemScript")]
public class ItemScript : ScriptableObject
{
    public string itemname;
    public Sprite icon;
    public GameObject model;

}

using UnityEngine;
using UnityEngine.Serialization;

public class ItemLogic : MonoBehaviour
{
    [SerializeField] private bool m_useBothHands = false;
    public bool UseBothHands { get { return m_useBothHands; } }

    [SerializeField, FormerlySerializedAs("m_PreferredHand")] private HandPreference m_preferredHand = HandPreference.Right;
    public HandPreference PreferredHand { get { return m_preferredHand; } }

    [SerializeField, FormerlySerializedAs("m_itemTypeId")] private ItemType m_itemType = ItemType.Other;
    public int ItemTypeID { get { return (int)m_itemType; } }

    [SerializeField] private Transform m_dummyPoint = null;
    public Transform DummyPoint { get { return m_dummyPoint; } }

    [SerializeField] private ItemAnimationsObject m_itemAnimations = null;
    public ItemAnimationsObject ItemAnimations { get { return m_itemAnimations; } }

    [SerializeField] private AccessoryLogic m_accessoryLogic = null;
    public AccessoryLogic AccessoryLogic { get { return m_accessoryLogic; } }

    [SerializeField] private FlipHand m_flipXForHand = FlipHand.None;
    [SerializeField] private FlipHand m_flipYForHand = FlipHand.None;
    [SerializeField] private FlipHand m_flipZForHand = FlipHand.None;

    public FlipHand FlipXForHand { get { return m_flipXForHand; } }
    public FlipHand FlipYForHand { get { return m_flipYForHand; } }
    public FlipHand FlipZForHand { get { return m_flipZForHand; } }

    private void OnValidate()
    {
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if(meshCollider != null) { Debug.LogError(name + ": MeshCollider present with ItemLogic - Consider removing as it generates runtime errors!"); }
    }

    public virtual void OnPickup() { }

    public virtual void OnDrop() { }

    public enum FlipHand
    {
        None,
        Right,
        Left
    }

    public enum HandPreference
    {
        Right,
        Left,
        Either
    }

    public enum ItemType
    {
        AssaultRifle = 0,
        SniperRifle = 1,
        Pistol = 2,

        Other = 3
    }
}

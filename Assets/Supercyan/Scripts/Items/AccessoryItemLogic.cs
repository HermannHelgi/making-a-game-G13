using UnityEngine;

// For items that have animations and need to be attached to the rig
public class AccessoryItemLogic : ItemLogic
{
    [SerializeField] private SkinnedMeshRenderer m_renderer = null;
    public SkinnedMeshRenderer Renderer { get { return m_renderer; } }

    [SerializeField] private GameObject m_rig = null;
    public void DeleteRig() { Destroy(m_rig); }
}

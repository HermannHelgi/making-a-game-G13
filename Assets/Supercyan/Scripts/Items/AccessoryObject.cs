#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CreateAssetMenu()]
public class AccessoryObject : ScriptableObject
{
    [SerializeField] private GameObject m_prefab = null;

    public AccessoryLogic Accessory { get { return m_prefab != null ? m_prefab.GetComponent<AccessoryLogic>() : null; } }

    private void OnValidate()
    {
        if (m_prefab == null) { return; }

        if (m_prefab.GetComponent<AccessoryLogic>() == null)
        {
            Debug.LogError("AccessoryObject: Prefab '" + m_prefab.name + "' is not valid because it's missing an AccessoryLogic script!");
            m_prefab = null;
            EditorUtility.SetDirty(this);
        }
    }
}

#endif
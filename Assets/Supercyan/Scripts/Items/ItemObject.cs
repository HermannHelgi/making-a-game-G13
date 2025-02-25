#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CreateAssetMenu()]
public class ItemObject : ScriptableObject
{
    [SerializeField] private GameObject m_prefab = null;

    public ItemLogic Item { get { return m_prefab != null ? m_prefab.GetComponent<ItemLogic>() : null; } }

    private void OnValidate()
    {
        if (m_prefab == null) { return; }

        if(m_prefab.GetComponent<ItemLogic>() == null)
        {
            Debug.LogError("ItemObject: Prefab '" + m_prefab.name + "' is not valid because it's missing an ItemLogic script!");
            m_prefab = null;
            EditorUtility.SetDirty(this);
        }
    }
}

#endif
using UnityEngine;

public class EffigyScript : MonoBehaviour, IDataPersistence
{
    public GameObject lureInWorld;

    public void activateLure()
    {
        GameManager.instance.lurePlaced = true;
        lureInWorld.SetActive(true);

        ObjectiveManager.instance.placedLure();
    }

    public void loadData(GameData data)
    {
        if (data.lurePlaced)
        {
            lureInWorld.SetActive(true);
        }
    }

    public void saveData(ref GameData data)
    {
        data.lurePlaced = lureInWorld.activeSelf;
    }
}
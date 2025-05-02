using UnityEngine;

public class PlayerSaveScript : MonoBehaviour, IDataPersistence
{
    public void loadData(GameData data)
    {
        this.gameObject.transform.position = data.playerPos;
    }

    public void saveData(ref GameData data)
    {
        data.playerPos = gameObject.transform.position;
    }
}

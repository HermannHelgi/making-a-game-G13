using UnityEngine;

public interface IDataPersistence 
{
    void loadData(GameData data);
    void saveData(ref GameData data);
}

using System.Threading;
using UnityEngine;


public class WendigoResting : WendigoBehaviour
{   
    GameManager gameManager;
    WendigoSpawnPointTracker wendigoSpawnPointTracker;
    
    public override void Run()
    {
        if (isActive)
        {
            transform.parent.transform.position = wendigoSpawnPointTracker.despawnPoint.position;
            if (gameManager.isNight == true && gameManager.safeArea == false)
            {
                isEnding = true;
            }
        }
        if (isEnding)
        {
            isEnding = false;
        }
    }
}

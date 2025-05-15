using System.Threading;
using UnityEngine;


public class WendigoResting : WendigoBehaviour
{   
    public WendigoSpawnPointTracker wendigoSpawnPointTracker;
    public WendigoEffigy wendigoEffigy;
    public Transform caveArea;
    public GameObject wendigo;   
     
    public override void Run()
    {
        if (isActive)
        {
            transform.parent.transform.position = wendigoSpawnPointTracker.despawnPoint.position;
            if (GameManager.instance.isNight == true && GameManager.instance.safeArea == false)
            {   
                if(GameManager.instance.lureCrafted)
                {   
                    if(GameManager.instance.lurePlaced)
                    {
                        isEnding = true;
                    }

                }
                else
                {
                    isEnding = true;
                }
                if(GameManager.instance.dangerZone)
                {   
                    wendigoEffigy.SpawnWendigo(caveArea);
                    isEnding = true;
                    wendigoEffigy.inAggressionRange = true;
                }
            }
            
        }
        if (isEnding)
        {
            isEnding = false;
        }
    }
}

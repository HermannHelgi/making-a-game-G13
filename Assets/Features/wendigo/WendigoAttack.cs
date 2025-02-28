
using System.Collections;
using System.Threading;
using UnityEngine;


public class WendigoAttack : MonoBehaviour
{

[Header("Attack Settings")]
    public PlayerDeathHandler playerDeathHandler;


    void Start()
    {
        

    }

    private void Scream()
    {
        // Play Wendigo scream sound
    }

    public void Attack()
    {
            // Scream();
            // Attack player
        playerDeathHandler.die("You were slain by the monster!");
        
    }

}

    
  

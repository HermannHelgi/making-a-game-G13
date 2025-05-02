
// using System.Threading;
// using UnityEngine;


// public class WendigoStateMachineOld : MonoBehaviour
// {

//     public WendigoRaycast wendigoRaycasts;
//     public wendigoRandomizedSpawner wendigoRandomizedSpawner;
//     public PlayerLineofSight playerLineOfSight;
//     public WendigoFollowPlayer wendigoFollowPlayer;

//     public WendigoLookForPlayer wendigoLookForPlayer;
//     public WendigoAttack wendigoAttack;

//     public GameObject Wendigo;

//     private float idleTimer;
//     private float lookingTimer;
//     private float spawnBehindTimer;
//     private bool playerProximity;


//     private GameManager gameManager;
//     private enum State
//     {
//         Resting,
//         Teleporting,
//         Idle,
//         Despawned,
//         SpawnBehindPlayer,
//         FollowingPlayer,
//         LookingForPlayer,
//         AttackPlayer

//     }



//     private State currentState;

//     void Start()
//     {
//         currentState = State.Resting;
//         if (gameManager == null)
//         {
//             gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
//         }

//     }

//     void Update()
//     {

//         if (gameManager.isNight == false || gameManager.safeArea == true)
//         {
//             wendigoRandomizedSpawner.playerSightings = 0;
//             // wendigoFollowPlayer.staticSound.Stop();
//             currentState = State.Resting;
//         }

//         switch (currentState)
//         {
//             case State.Resting:
//                 Resting();
//                 break;
//             case State.Teleporting:
//                 Teleporting();
//                 break;
//             case State.Idle:
//                 Idle();
//                 break;
//             case State.Despawned:
//                 Despawned();
//                 break;
//             case State.SpawnBehindPlayer:
//                 SpawnBehindPlayer();
//                 break;
//             case State.FollowingPlayer:
//                 FollowingPlayer();
//                 break;
//             case State.LookingForPlayer:
//                 LookingForPlayer();
//                 break;
//             case State.AttackPlayer:
//                 AttackPlayer();
//                 break;
//         }
//     }

//     private void Idle()
//     {   

//         // Debug.Log("Idle");
//         if(!playerLineOfSight.isLookingAtWendigo())
//         {   
//             // Debug.Log("Is not looking at wendigo!");
//             currentState = State.Despawned;
//         }
//         Transform currentWendigoPosition = wendigoRandomizedSpawner.ReturnCurrentPosition();
//         float distance = Vector3.Distance(playerLineOfSight.transform.position , currentWendigoPosition.position);
//         if(distance < 10)
//         {   
//             playerProximity = true;
//             currentState =State.SpawnBehindPlayer;
//         }
        
//     }

//     private void Despawned()
//     {
//         // Debug.Log("Despawned");
//         // Despawned state logic
//         wendigoRandomizedSpawner.DespawnWendigo();
//         idleTimer += Time.deltaTime;
//         playerLineOfSight.isLooking = false;
//         if(idleTimer >= wendigoRandomizedSpawner.spawnTimer)
//         {
//             if (wendigoRandomizedSpawner.playerSightings >= wendigoRandomizedSpawner.maxPlayerSightings)
//             {
//                 idleTimer = 0;
//                 currentState = State.SpawnBehindPlayer;

//             }
//             else
//             {
//                 idleTimer = 0;
//                 currentState = State.Teleporting;
//             }
//         }
//     }

//     private void Resting()
//     {
//         // Debug.Log("Resting");
//         Wendigo.transform.position = wendigoRandomizedSpawner.despawnPoint.position;
//         if (gameManager.isNight == true && gameManager.safeArea == false)
//         {
//             currentState = State.Teleporting;
//         }

//     }

//     private void Teleporting()
//     {
        
//         wendigoRandomizedSpawner.SpawnWendigo();
//         idleTimer += Time.deltaTime;
//         if(playerLineOfSight.isLookingAtWendigo() & idleTimer >= 1.5f)
//         {
//             wendigoRandomizedSpawner.playerSightings++;

//             Debug.Log("Spotted Wendigo " + wendigoRandomizedSpawner.playerSightings + " times!");
//             currentState = State.Idle;
//             idleTimer = 0; 
//         }
//     }

//     private void SpawnBehindPlayer()
//     {
//         spawnBehindTimer += Time.deltaTime;
//         if(playerProximity)
//         {
//             wendigoFollowPlayer.SpawnBehindPlayer(wendigoRandomizedSpawner.ReturnCurrentPosition());
//             playerProximity = false;
//             wendigoRandomizedSpawner.DespawnWendigo();
//             currentState = State.FollowingPlayer;
//         }
//         if (spawnBehindTimer >= 20f)
//         {
//             // Debug.Log("SpawnBehindPlayer");
//             wendigoFollowPlayer.SpawnBehindPlayer();
//             currentState = State.FollowingPlayer;

//         }
//         // SpawnBehindPlayer state logic
//     }

//     private void FollowingPlayer()
//     {
//         // Debug.Log("FollowingPlayer");
//         wendigoFollowPlayer.FollowPlayer();
//         // Follow player until player is out of sight
//         if (wendigoFollowPlayer.attackPlayer)
//         {
//             // Debug.Log("Attack Player");
//             currentState = State.AttackPlayer;
//         }
//         else if (wendigoFollowPlayer.lostPlayer)
//         {
//             // Debug.Log("Lost Player");
//             currentState = State.LookingForPlayer;
//         }

//         // FollowingPlayer state logic
//     }


//     private void LookingForPlayer()
//     {

//         lookingTimer += Time.deltaTime;
//         wendigoLookForPlayer.TrackFootsteps();


//         if (lookingTimer > 60f)
//         {
//             wendigoRandomizedSpawner.playerSightings = 1;
//             lookingTimer = 0;
//             currentState = State.Teleporting;
//         }
//         else if (wendigoRaycasts.detected)
//         {
//             // Debug.Log("Player detected");
//             currentState = State.FollowingPlayer;
//         }

//         // LookingForPlayer state logic
//     }

//     private void AttackPlayer()
//     {
//         // Debug.Log("Attacking player");
//         wendigoAttack.Attack();

//         // AttackPlayer state logic
//     }







// }
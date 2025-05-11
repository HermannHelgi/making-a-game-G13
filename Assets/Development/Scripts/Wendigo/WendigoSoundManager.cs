using System.Threading;
using UnityEngine;
using UnityEngine.AI;


public class WendigoSoundManager : MonoBehaviour
{
    public GameObject wendigo;

    public SoundManager soundmanager;
    private Vector3 wendigoVelocity;

    void Awake()
    {
        wendigoVelocity = wendigo.GetComponent<NavMeshAgent>().velocity;
    }

    void Update()
    {
        // Debug.Log("wendigo speed: "+ wendigoVelocity);
    }


}

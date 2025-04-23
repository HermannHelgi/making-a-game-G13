using StarterAssets;
using UnityEngine;

public class PlayerAudioHandler : MonoBehaviour
{

    public SoundManager soundManager;
    public Component controllerComponent;

    private bool _walked;
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _walked = controllerComponent.GetComponent<FirstPersonController>()._walked;
        if(_walked)
        {
            soundManager.PlayGroup("PLAYER_WALKING");
        }

    }
}

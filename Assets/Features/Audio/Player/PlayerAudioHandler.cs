using StarterAssets;
using UnityEngine;

public class PlayerAudioHandler : MonoBehaviour
{

    public Component controllerComponent;

    private bool _walked;


    // Update is called once per frame
    void Update()
    {
        _walked = controllerComponent.GetComponent<FirstPersonController>()._walked;
        if(_walked)
        {
            SoundManager.instance.PlayGroup("PLAYER_WALKING");
        }

    }
}

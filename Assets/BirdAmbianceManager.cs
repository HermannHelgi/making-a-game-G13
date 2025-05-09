using UnityEngine;

public class BirdAmbianceManager : MonoBehaviour
{

    public bool isNight;
    public SoundManager soundManager;
    public float timeTilNextChrip = 100f;
    public float _timeTilNextChrip;
    private float _rate = 1f;


    private void Start() 
    {
        _timeTilNextChrip = timeTilNextChrip;    
    }

    private void Update() 
    {
            if(_timeTilNextChrip < 0)
            {
                if(isNight)
                {
                    soundManager.PlayGroup("OWL_CHRIPS");
                }
                else
                {
                    soundManager.PlayGroup("BIRD_CHRIPS");
                }
                _timeTilNextChrip = timeTilNextChrip;
            }
            else
            {
                _timeTilNextChrip -= _rate * Time.deltaTime;
            }


    }

}

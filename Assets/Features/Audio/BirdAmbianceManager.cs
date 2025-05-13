using UnityEngine;

public class BirdAmbianceManager : MonoBehaviour
{

    public float timeTilNextChrip = 100f;
    private float _timeTilNextChrip;
    private float _rate = 1f;


    private void Start() 
    {
        _timeTilNextChrip = timeTilNextChrip;    
    }

    private void Update() 
    {
            if(_timeTilNextChrip < 0)
            {
                if(GameManager.instance.isNight)
                {
                    SoundManager.instance.PlayGroup("OWL_CHRIPS");
                }
                else
                {
                    SoundManager.instance.PlayGroup("BIRD_CHRIPS");
                }
                _timeTilNextChrip = timeTilNextChrip;
            }
            else
            {
                _timeTilNextChrip -= _rate * Time.deltaTime;
            }


    }

}

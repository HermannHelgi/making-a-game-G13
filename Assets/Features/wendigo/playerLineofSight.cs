using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class playerLineofSight : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public float audioIncreaseRate, audioDecreaseRate;

    public bool looking;

    public AudioSource staticSound;


    public wendigoRaycast detectedScript;

    public float maxStaticAmount;

    void Start()

    {


    }

    void OnBecameVisible()

    {

        looking = true;

    }

    void OnBecameInvisible()

    {

        looking = false;

    }

    void Update()

    {





        if(detectedScript.detected == true)

        {

            if (looking == true)

            {



                staticSound.volume = staticSound.volume + audioIncreaseRate * Time.deltaTime;

            }



        }

        if (looking == false || detectedScript.detected == false)

        {



            staticSound.volume = staticSound.volume - audioDecreaseRate * Time.deltaTime;

        }



    }

}



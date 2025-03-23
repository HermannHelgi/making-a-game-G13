using UnityEngine;

public class TorchScript : MonoBehaviour
{
    public GameObject particles;
    public GameObject pointlight;

    void Start()
    {
        if (!GameManager.instance.torchactive)
        {
            particles.SetActive(false);
            pointlight.SetActive(false);
        }
        else
        {
            particles.SetActive(true);
            pointlight.SetActive(true);
        }   
    }

    void Update()
    {
        if (!GameManager.instance.torchactive)
        {
            particles.SetActive(false);
            pointlight.SetActive(false);
        }
        else
        {
            particles.SetActive(true);
            pointlight.SetActive(true);
        }
    }
}

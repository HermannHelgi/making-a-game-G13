using UnityEngine;

public class EmberStoneScript : MonoBehaviour
{
    public Light light;
    public ParticleSystem particleSystem;


    void Update()
    {
        if (GameManager.instance != null)
        {
            if (GameManager.instance.emberstoneactive)
            {
                light.enabled = true;
                particleSystem.Play();
            }
            else
            {
                light.enabled = false;
                particleSystem.Stop();
            }
        }
    }
}

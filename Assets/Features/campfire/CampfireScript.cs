using UnityEngine;

public class CampfireScript : MonoBehaviour
{

    public float rangeofheat;
    // This must be negative!
    public float heatspeed;
    public float lengthofburn;

    public GameObject playercapsule;
    public NecessityBars necessitybars;


    private float burntimer = 0;
    private bool playerinrange;
    private float oldtemperaturedrainrate;


    void Update()
    {
        if (burntimer > 0)
        {
            burntimer -= Time.deltaTime;

            if (Vector3.Distance(playercapsule.transform.position, this.transform.position) <= rangeofheat && !playerinrange)
            {
                warmPlayer();
            }

            if (burntimer <= 0 && playerinrange)
            {
                turnOffHeat();
            }
        }

        if (playerinrange)
        {
            if (Vector3.Distance(playercapsule.transform.position, this.transform.position) > rangeofheat)
            {
                turnOffHeat();
            }
        }
    }

    void warmPlayer()
    {
        playerinrange = true;
        oldtemperaturedrainrate = necessitybars.temperaturedrainrate;
        necessitybars.temperaturedrainrate = heatspeed;
    }

    void turnOffHeat()
    {
        playerinrange = false;
        necessitybars.temperaturedrainrate = oldtemperaturedrainrate;
    }

    public void addFuel()
    {
        burntimer = lengthofburn;
    }
}

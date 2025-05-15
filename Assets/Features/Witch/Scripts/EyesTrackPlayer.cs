using UnityEngine;

public class EyesTrackPlayer : MonoBehaviour
{
    public void LookAtPlayer(Vector3 playerPosition)
    {

        transform.position = playerPosition;

    }
}

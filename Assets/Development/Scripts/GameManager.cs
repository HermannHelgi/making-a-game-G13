using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("GameManager variables")]
    [Tooltip("Base singleton instance of the Game Manager, do not touch!")]
    public static GameManager instance;

    [Header("Witch in the wall variables.")]
    [Tooltip("An item array for all the possible items in the game, ocne set to true the item is discovered.")]
    public bool[] discovereditems = new bool[18];

    [Header("Day and night variables.")]
    [Tooltip("A bool which says whether the game is day or night.")]
    public bool isNight = false;

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        
    }
}

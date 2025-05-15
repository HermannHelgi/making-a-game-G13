using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimerTilMainMenu : MonoBehaviour
{
    public string nameOfLoadedScene;
    public float timeTilMainMenu;
    private float timer;

    void Start()
    {
        timer = timeTilMainMenu;
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                SceneManager.LoadScene(nameOfLoadedScene);
            }
        }
    }
}

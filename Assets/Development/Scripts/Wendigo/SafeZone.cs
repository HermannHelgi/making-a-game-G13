using UnityEngine;

    public class SafeArea : MonoBehaviour
    {   

        private GameManager gameManager;
        private wendigoRandomizedSpawner wendigoRandomizedSpawner;
        void Start()
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            wendigoRandomizedSpawner = GameObject.Find("WendigoSpawner").GetComponent<wendigoRandomizedSpawner>();
        }
        
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Player entered safe area");
                gameManager.safeArea = true;
            }
            if(other.CompareTag("Wendigo"))
            {
                Debug.Log("Wendigo entered safe area");
                wendigoRandomizedSpawner.DespawnWendigo();
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Player exited safe area");
                gameManager.safeArea = false;
            }
        }


}
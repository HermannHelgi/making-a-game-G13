using UnityEngine;

    public class SafeArea : MonoBehaviour
    {   

        private GameManager gameManager;
        public StalkingBehaviour stalkingBehaviour;
        void Start()
        {
            }
        void Awake()
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            
        }

    void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // Debug.Log("Player entered safe area");
                gameManager.safeArea = true;
            }
            if(other.CompareTag("Wendigo"))
            {
                // Debug.Log("Wendigo entered safe area");
                // stalkingBehaviour.retreat();
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // Debug.Log("Player exited safe area");
                gameManager.safeArea = false;
            }
        }


}
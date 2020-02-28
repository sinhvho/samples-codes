using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    private static GameManager instance;
        public static GameManager Instance { get { return instance; } set { instance = value; } }

    public bool gameIsComplete = false;

    private void Awake() {
        if(instance == null) {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (gameIsComplete) {
            if (Input.GetMouseButtonDown(0)) {
                gameIsComplete = false;
                SceneManager.LoadScene(0);
            }
        }
        
    }
}

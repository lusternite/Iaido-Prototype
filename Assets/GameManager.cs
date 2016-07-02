using UnityEngine;
using UnityEditor.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            //Application.LoadLevel(0);
            EditorSceneManager.LoadScene(0);
        }
    }
}

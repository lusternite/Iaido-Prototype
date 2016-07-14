using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Application.targetFrameRate = 60;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.LoadLevel(0);
        }
    }
}

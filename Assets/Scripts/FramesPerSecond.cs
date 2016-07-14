using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FramesPerSecond : MonoBehaviour {

    public float DeltaTime = 0.0f;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        DeltaTime += (Time.deltaTime - DeltaTime) * 0.1f;
        float fps = 1.0f / DeltaTime;
        GetComponent<Text>().text = "FPS: " + fps;
	}
}

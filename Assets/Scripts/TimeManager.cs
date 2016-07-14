using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeManager : MonoBehaviour {

    public float CurrentTime = 0.0f;

	// Use this for initialization
	void Start () {
        GetComponent<Text>().text = "Time: " + CurrentTime + "s";
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetTime(float NewTime)
    {
        CurrentTime = NewTime;
        GetComponent<Text>().text = "Time: " + CurrentTime + "s";
    }

    public void AddTime(float NewTime)
    {
        CurrentTime += NewTime;
        GetComponent<Text>().text = "Time: " + CurrentTime + "s";
    }

    public float GetTime()
    {
        return CurrentTime;
    }
}

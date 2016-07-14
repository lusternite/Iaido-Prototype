using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MoveManager : MonoBehaviour {

    public int CurrentMoves = 0;

	// Use this for initialization
	void Start () {
        GetComponent<Text>().text = "Moves: " + CurrentMoves;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetMoves(int NewMoves)
    {
        CurrentMoves = NewMoves;
        GetComponent<Text>().text = "Moves: " + CurrentMoves;
    }

    public void AddMoves(int NewMoves)
    {
        CurrentMoves += NewMoves;
        GetComponent<Text>().text = "Moves: " + CurrentMoves;
    }

    public int GetMoves()
    {
        return CurrentMoves;
    }
}

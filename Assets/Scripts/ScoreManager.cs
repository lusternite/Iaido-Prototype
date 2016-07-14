using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour {

    //90+ = A
    //80+ = B
    //70+ = C
    //60+ = D
    //50+ = E
    //50- = F
    //Time and moves each make up 50
    //Time = 18.0s
    //moves = 20
    public float NumericalScore = 100.0f;
    public string AlphabeticalScore = "A";
    public TimeManager TimeScore;
    public MoveManager MoveScore;

	// Use this for initialization
	void Start () {
        TimeScore = FindObjectOfType<TimeManager>();
        MoveScore = FindObjectOfType<MoveManager>();
        GetComponent<Text>().text = "Score: " + AlphabeticalScore;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void CalculateScore()
    {
        float NumericalTimeScore = 0.0f;
        if (TimeScore.GetTime() <= 18.0f)
        {
            NumericalTimeScore = 50.0f;
        }
        else
        {
            NumericalTimeScore = 50.0f - ((TimeScore.GetTime() - 18.0f) * 4);
        }
        float NumericalMoveScore = 0.0f;
        if (MoveScore.GetMoves() <= 20)
        {
            NumericalMoveScore = 50.0f;
        }
        else
        {
            NumericalMoveScore = 50.0f - ((MoveScore.GetMoves() - 20) * 3);
        }
        NumericalScore = NumericalMoveScore + NumericalTimeScore;
        if (NumericalScore >= 90.0f)
        {
            AlphabeticalScore = "A";
        }
        else if (NumericalScore >= 80.0f)
        {
            AlphabeticalScore = "B";
        }
        else if (NumericalScore >= 70.0f)
        {
            AlphabeticalScore = "C";
        }
        else if (NumericalScore >= 60.0f)
        {
            AlphabeticalScore = "D";
        }
        else if (NumericalScore >= 50.0f)
        {
            AlphabeticalScore = "E";
        }
        else
        {
            AlphabeticalScore = "F";
        }
    }
}

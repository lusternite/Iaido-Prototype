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
    public float TimeMinSeconds = 18.0f;
    public int MoveMin = 20;

	// Use this for initialization
	void Start () {
        TimeScore = FindObjectOfType<TimeManager>();
        MoveScore = FindObjectOfType<MoveManager>();
        GetComponent<Text>().text = "Score: " + AlphabeticalScore;
    }
	
	// Update is called once per frame
	void Update () {
        CalculateScore();
	}

    public void CalculateScore()
    {
        float NumericalTimeScore = 0.0f;
        if (TimeScore.GetTime() <= TimeMinSeconds)
        {
            NumericalTimeScore = 50.0f;
        }
        else
        {
            NumericalTimeScore = 50.0f - ((TimeScore.GetTime() - TimeMinSeconds) * 4.0f);
        }
        float NumericalMoveScore = 0.0f;
        if (MoveScore.GetMoves() <= MoveMin)
        {
            NumericalMoveScore = 50.0f;
        }
        else
        {
            NumericalMoveScore = 50.0f - ((MoveScore.GetMoves() - MoveMin) * 3.0f);
        }
        NumericalScore = NumericalMoveScore + NumericalTimeScore;
        if (NumericalScore >= 95.0f)
        {
            AlphabeticalScore = "A";
        }
        else if (NumericalScore >= 85.0f)
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
        GetComponent<Text>().text = "Score: " + AlphabeticalScore;
    }
}

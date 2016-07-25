using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    PuzzlePlayerBehaviour Player;

	// Use this for initialization
	void Start () {
        Application.targetFrameRate = 60;
        Player = FindObjectOfType<PuzzlePlayerBehaviour>();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.LoadLevel(Application.loadedLevel);
        }
        if (Input.GetKeyUp(KeyCode.Backspace))
        {
            int NextLevel = (Application.loadedLevel + 1) % Application.levelCount;
            Application.LoadLevel(NextLevel);
        }
        if (Input.GetKeyUp(KeyCode.Return) && Player.IsDrawPhase)
        {
            Player.enabled = true;
            Player.GetComponent<MeshRenderer>().enabled = true;
            Player.GetComponent<BoxCollider>().enabled = true;
            Player.HandleDraw();
        }
        if (Input.GetKeyUp(KeyCode.Y))
        {
            GameObject[] AttackRadii = GameObject.FindGameObjectsWithTag("Attack Radius");
            foreach (GameObject AttackRadius in AttackRadii)
            {
                AttackRadius.GetComponent<MeshRenderer>().enabled = !AttackRadius.GetComponent<MeshRenderer>().enabled;
            }
        }
    }
}

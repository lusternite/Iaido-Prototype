using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour
{

    public GameObject ThePlayer;
    public PuzzlePlayerBehaviour PlayerScript;
    public float TimeToLive = 4.0f;
    public float TimeAlive = 0.0f;
    public float Velocity = 0.5f;

    // Use this for initialization
    void Start()
    {
        ThePlayer = GameObject.FindGameObjectWithTag("Player");
        PlayerScript = ThePlayer.GetComponent<PuzzlePlayerBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerScript.HasDrawn && (PlayerScript.CurrentPlayerState != PuzzlePlayerBehaviour.PlayerState.Idle || PlayerScript.Moving || !PlayerScript.IsDrawPhase))
        {
            DoThings();
        }
    }

    void OnCollisionEnter(Collision Col)
    {
        if (Col.gameObject.tag == "Player")
        {
            if (PlayerScript.CurrentPlayerState == PuzzlePlayerBehaviour.PlayerState.Parrying)
            {
                PlayerScript.UpArrowFlag = false;
                PlayerScript.DownArrowFlag = false;
                PlayerScript.LeftArrowFlag = false;
                PlayerScript.RightArrowFlag = false;

                PlayerScript.RecoveryTimer = 0.1f;
                PlayerScript.CurrentPlayerState = PuzzlePlayerBehaviour.PlayerState.Recovering;
                PlayerScript.EnemyCollided = true;
                PlayerScript.ParryDuration = 0.0f;
                PlayerScript.GetComponent<MeshRenderer>().material = PlayerScript.PlayerColors[3];
                if (FindObjectOfType<Archer>().IsPlanningPhase)
                {
                    PlayerScript.ActionTimers.Add(0.3f - PlayerScript.ParryDuration + 0.1f);
                }
            }
            else
            {
                Col.gameObject.GetComponent<PuzzlePlayerBehaviour>().enabled = false;
                Col.gameObject.GetComponent<MeshRenderer>().enabled = false;
                Col.gameObject.GetComponent<BoxCollider>().enabled = false;
                Col.rigidbody.velocity *= 0.0f;
            }
            Destroy(gameObject);
        }
        else if (Col.gameObject.tag == "Enemy")
        {
            Destroy(gameObject);
        }
    }

    void DoThings()
    {
        TimeAlive += Time.deltaTime;
        if (TimeAlive >= TimeToLive)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.position += transform.forward * Velocity;
        }
    }
}

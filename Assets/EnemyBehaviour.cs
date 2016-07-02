using UnityEngine;
using System.Collections;

public class EnemyBehaviour : MonoBehaviour {

    public enum EnemyState
    {
        Seeking,
        Attacking,
        Recovering,
        Stunned
    }

    public Material[] EnemyColors;
    public EnemyState CurrentEnemyState;
    public GameObject ThePlayer;
    public PlayerBehaviour PlayerScript;
    public float EnemyVelocity = 1.0f;
    Vector3 AttackPosition;
    float AttackTimer;
    public float RecoveryTimer;
    public bool IsActive;
    public float StunDuration;
    public int Health = 2;

    // Use this for initialization
    void Start () {
        CurrentEnemyState = EnemyState.Seeking;
        ThePlayer = GameObject.FindGameObjectWithTag("Player");
        PlayerScript = ThePlayer.GetComponent<PlayerBehaviour>();
        PlayerScript.GetComponent<MeshRenderer>().material = PlayerScript.PlayerColors[0];
        IsActive = true;
        AttackTimer = 9000.1f;
    }
	
	// Update is called once per frame
	void Update () {
        if (IsActive)
        {
            if (PlayerScript.HasDrawn)
            {
                DoThings();
            }
        }
	}

    void OnCollisionEnter(Collision Col)
    {
        if (Col.gameObject.tag == "Player")
        {
            if (PlayerScript.CurrentPlayerState == PlayerBehaviour.PlayerState.Attacking && PlayerScript.AttackingTimer < AttackTimer)
            {
                PlayerScript.UpArrowFlag = false;
                PlayerScript.DownArrowFlag = false;
                PlayerScript.LeftArrowFlag = false;
                PlayerScript.RightArrowFlag = false;
                PlayerScript.GetComponent<Rigidbody>().velocity *= 0.0f;
                PlayerScript.UpVelocity /= 5.0f;
                PlayerScript.RightVelocity /= 5.0f;
                PlayerScript.AttackingDuration = 0.0f;
                PlayerScript.GetComponent<MeshRenderer>().material = PlayerScript.PlayerColors[3];
                if (Health == 2)
                {
                    if (!PlayerScript.HasDrawn)
                    {
                        PlayerScript.HasDrawn = true;
                        PlayerScript.IsAttacking = false;
                        PlayerScript.CurrentPlayerState = PlayerBehaviour.PlayerState.Idle;
                        PlayerScript.GetComponent<MeshRenderer>().material = PlayerScript.PlayerColors[0];
                    }
                    else
                    {
                        PlayerScript.RecoveryTimer = 0.5f;
                        PlayerScript.CurrentPlayerState = PlayerBehaviour.PlayerState.Recovering;
                    }
                    StunDuration = 3.0f;
                    Health -= 1;
                    CurrentEnemyState = EnemyState.Stunned;
                    GetComponent<MeshRenderer>().material = EnemyColors[2];
                    AttackTimer = 9000.1f;
                }
                else
                {
                    PlayerScript.RecoveryTimer = 0.5f;
                    PlayerScript.CurrentPlayerState = PlayerBehaviour.PlayerState.Recovering;
                    Destroy(gameObject);
                }
            }
            else if (PlayerScript.CurrentPlayerState == PlayerBehaviour.PlayerState.Parrying)
            {
                PlayerScript.UpArrowFlag = false;
                PlayerScript.DownArrowFlag = false;
                PlayerScript.LeftArrowFlag = false;
                PlayerScript.RightArrowFlag = false;

                StunDuration = 2.0f;
                CurrentEnemyState = EnemyState.Stunned;
                GetComponent<MeshRenderer>().material = EnemyColors[2];
                PlayerScript.RecoveryTimer = 0.2f;
                PlayerScript.CurrentPlayerState = PlayerBehaviour.PlayerState.Recovering;
                AttackTimer = 9000.1f;
            }
            else if (CurrentEnemyState == EnemyState.Attacking)
            {
                Destroy(Col.gameObject);
                IsActive = false;
            }
            
            Debug.Log("COLLISION");
            Debug.Log("Player timer = " + PlayerScript.AttackingTimer);
            Debug.Log("AI timer = " + AttackTimer);
        }
    }

    void DoThings()
    {
        switch (CurrentEnemyState)
        {
            case EnemyState.Seeking:
                {
                    //Debug.Log(ThePlayer.GetComponent<Transform>().position);
                    transform.position = Vector3.MoveTowards(transform.position, ThePlayer.GetComponent<Transform>().position, EnemyVelocity * Time.deltaTime);
                    if (Vector3.Distance(transform.position, ThePlayer.GetComponent<Transform>().position) <= 2.5f)
                    {
                        CurrentEnemyState = EnemyState.Attacking;
                        GetComponent<MeshRenderer>().material = EnemyColors[1];
                        AttackPosition = ThePlayer.GetComponent<Transform>().position;
                        AttackTimer = Time.time;
                    }
                    break;
                }
            case EnemyState.Attacking:
                {
                    transform.position = Vector3.MoveTowards(transform.position, AttackPosition, EnemyVelocity * Time.deltaTime * 5.0f);
                    if (Vector3.Distance(transform.position, AttackPosition) == 0.0f)
                    {
                        CurrentEnemyState = EnemyState.Recovering;
                        GetComponent<MeshRenderer>().material = EnemyColors[2];
                        RecoveryTimer = 2.0f;
                        AttackTimer = 9000.1f;
                    }
                    break;
                }
            case EnemyState.Recovering:
                {
                    RecoveryTimer -= Time.deltaTime;
                    if (RecoveryTimer <= 0.0f)
                    {
                        CurrentEnemyState = EnemyState.Seeking;
                        GetComponent<MeshRenderer>().material = EnemyColors[0];
                    }
                    break;
                }
            case EnemyState.Stunned:
                {
                    StunDuration -= Time.deltaTime;
                    if (StunDuration <= 0.0f)
                    {
                        CurrentEnemyState = EnemyState.Seeking;
                        GetComponent<MeshRenderer>().material = EnemyColors[0];
                    }
                    break;
                }
        }
    }
}

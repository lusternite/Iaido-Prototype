using UnityEngine;
using System.Collections;

public class Archer : MonoBehaviour {

    public enum EnemyState
    {
        Seeking,
        Drawing,
        Recovering,
    }

    public bool IsPlanningPhase = true;
    public Vector3 SpawnPosition;
    public Material[] EnemyColors;
    public EnemyState CurrentEnemyState;
    public GameObject ThePlayer;
    public PuzzlePlayerBehaviour PlayerScript;
    public float EnemyVelocity = 1.0f;
    Vector3 AttackPosition;
    public float RecoveryTimer;
    public bool IsActive;
    public float StunDuration;
    public int Health = 1;
    public GameObject ArrowPrefab;
    public float SeekTimer = 0.0f;
    public float SeekDelay = 1.0f;
    public float DrawTimer = 0.0f;
    public float DrawDelay = 0.5f; 

    // Use this for initialization
    void Start()
    {
        CurrentEnemyState = EnemyState.Seeking;
        ThePlayer = GameObject.FindGameObjectWithTag("Player");
        PlayerScript = ThePlayer.GetComponent<PuzzlePlayerBehaviour>();
        PlayerScript.GetComponent<MeshRenderer>().material = PlayerScript.PlayerColors[0];
        IsActive = true;
        SeekTimer = 0.0f;
        SpawnPosition = transform.position;
        transform.forward = Vector3.Normalize(ThePlayer.transform.position - transform.position);
        //GetComponentInChildren<Transform>().forward = Vector3.Normalize(ThePlayer.transform.position - transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsActive)
        {
            if (PlayerScript.HasDrawn && (PlayerScript.CurrentPlayerState != PuzzlePlayerBehaviour.PlayerState.Idle || PlayerScript.Moving || !PlayerScript.IsDrawPhase))
            {
                DoThings();
            }
        }
    }

    void OnCollisionEnter(Collision Col)
    {
        if (Col.gameObject.tag == "Player")
        {
            if (PlayerScript.CurrentPlayerState == PuzzlePlayerBehaviour.PlayerState.Attacking)
            {
                PlayerScript.UpArrowFlag = false;
                PlayerScript.DownArrowFlag = false;
                PlayerScript.LeftArrowFlag = false;
                PlayerScript.RightArrowFlag = false;
                PlayerScript.GetComponent<Rigidbody>().velocity *= 0.0f;
                PlayerScript.IsAttacking = false;
                //PlayerScript.UpVelocity /= 5.0f;
                //PlayerScript.RightVelocity /= 5.0f;
                PlayerScript.GetComponent<MeshRenderer>().material = PlayerScript.PlayerColors[3];
                if (!PlayerScript.HasDrawn)
                {
                    PlayerScript.HasDrawn = true;

                    PlayerScript.CurrentPlayerState = PuzzlePlayerBehaviour.PlayerState.Idle;
                    PlayerScript.GetComponent<MeshRenderer>().material = PlayerScript.PlayerColors[0];
                    if (IsPlanningPhase)
                    {
                        PlayerScript.InputEnabled = true;
                        if (PlayerScript.IsParryStance)
                        {
                            //RecoveryTimer = 0.3f;
                            PlayerScript.ActionTimers.Add(0.2f - PlayerScript.AttackingDuration + 0.2f);
                        }
                        else
                        {
                            //RecoveryTimer = 0.5f;
                            PlayerScript.ActionTimers.Add(0.5f - PlayerScript.AttackingDuration + 0.2f);
                        }
                    }
                    else
                    {
                        PlayerScript.CurrentAction += 1;
                        PlayerScript.CurrentTimer = 0.0f;
                    }

                }
                else
                {
                    if (PlayerScript.IsParryStance)
                    {
                        PlayerScript.RecoveryTimer = 0.3f;
                    }
                    else
                    {
                        PlayerScript.RecoveryTimer = 0.5f;
                    }
                    PlayerScript.CurrentPlayerState = PuzzlePlayerBehaviour.PlayerState.Recovering;
                    PlayerScript.EnemyCollided = true;
                    if (IsPlanningPhase)
                    {
                        if (PlayerScript.IsParryStance)
                        {
                            RecoveryTimer = 0.3f;
                            PlayerScript.ActionTimers.Add(0.2f - PlayerScript.AttackingDuration + 0.3f);
                        }
                        else
                        {
                            RecoveryTimer = 0.5f;
                            PlayerScript.ActionTimers.Add(0.5f - PlayerScript.AttackingDuration + 0.5f);
                        }
                    }
                }
                PlayerScript.AttackingDuration = 0.0f;
                if (IsPlanningPhase)
                {
                    IsActive = false;
                    GetComponent<MeshRenderer>().enabled = false;
                    transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
                    GetComponent<SphereCollider>().enabled = false;
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    void DoThings()
    {
        switch (CurrentEnemyState)
        {
            case EnemyState.Seeking:
                {
                    transform.forward = Vector3.Normalize(ThePlayer.transform.position - transform.position);
                    SeekTimer += Time.deltaTime;
                    if (SeekTimer >= SeekDelay)
                    {
                        CurrentEnemyState = EnemyState.Drawing;
                        DrawTimer = 0.0f;
                        GetComponent<MeshRenderer>().material = EnemyColors[1];
                    }
                    break;
                }
            case EnemyState.Drawing:
                {
                    DrawTimer += Time.deltaTime;
                    if (DrawTimer >= DrawDelay)
                    {
                        //fire an arrow
                        GameObject NewArrow = (GameObject)Instantiate(ArrowPrefab, transform.position, transform.rotation);
                        NewArrow.transform.forward = transform.forward;
                        RecoveryTimer = 1.5f;
                        CurrentEnemyState = EnemyState.Recovering;
                        GetComponent<MeshRenderer>().material = EnemyColors[2];
                        transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
                        //GetComponent<MeshRenderer>().enabled = true;
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
                        SeekTimer = 0.0f;
                        transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                    }
                    break;
                }
        }
    }

    public void Reset()
    {
        CurrentEnemyState = EnemyState.Seeking;
        ThePlayer = GameObject.FindGameObjectWithTag("Player");
        PlayerScript = ThePlayer.GetComponent<PuzzlePlayerBehaviour>();
        PlayerScript.GetComponent<MeshRenderer>().material = PlayerScript.PlayerColors[0];
        GetComponent<MeshRenderer>().material = EnemyColors[0];
        IsActive = true;
        SeekTimer = 0.0f;
        transform.position = SpawnPosition;
        IsPlanningPhase = false;
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<SphereCollider>().enabled = true;  
        transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
        PlayerScript.HasDrawn = false;
        transform.forward = Vector3.Normalize(ThePlayer.transform.position - transform.position);
    }
}

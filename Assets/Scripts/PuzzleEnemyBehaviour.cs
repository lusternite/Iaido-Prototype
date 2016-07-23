using UnityEngine;
using System.Collections;

public class PuzzleEnemyBehaviour : MonoBehaviour
{

    public enum EnemyState
    {
        Seeking,
        Attacking,
        Parrying,
        Recovering,
        Stunned
    }

    public float ElapsedActiveTime;
    public bool IsPursuer = false;
    public bool IsPlanningPhase = true;
    public Vector3 SpawnPosition;
    public Material[] EnemyColors;
    public EnemyState CurrentEnemyState;
    public GameObject ThePlayer;
    public PuzzlePlayerBehaviour PlayerScript;
    public float EnemyVelocity = 1.0f;
    public float EnemyAttackRadius = 2.5f;
    Vector3 AttackPosition;
    float AttackTimer;
    float ParryDuration = 0.3f;
    float ParryTimer;
    public float RecoveryTimer;
    public bool IsActive;
    public float StunDuration;
    public int Health = 1;

    // Use this for initialization
    void Start()
    {
        CurrentEnemyState = EnemyState.Seeking;
        ThePlayer = GameObject.FindGameObjectWithTag("Player");
        PlayerScript = ThePlayer.GetComponent<PuzzlePlayerBehaviour>();
        PlayerScript.GetComponent<MeshRenderer>().material = PlayerScript.PlayerColors[0];
        IsActive = true;
        AttackTimer = 9000.1f;
        ParryTimer = 0.0f;
        SpawnPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsActive)
        {
            if (PlayerScript.HasDrawn && (PlayerScript.CurrentPlayerState != PuzzlePlayerBehaviour.PlayerState.Idle || PlayerScript.Moving || !PlayerScript.IsDrawPhase))
            {
                DoThings();
                if (!PlayerScript.IsDrawPhase)
                {
                    ElapsedActiveTime += Time.fixedDeltaTime;
                }
            }
        }
    }

    void OnCollisionEnter(Collision Col)
    {
        if (Col.gameObject.tag == "Player")
        {
            if (PlayerScript.CurrentPlayerState == PuzzlePlayerBehaviour.PlayerState.Attacking)
            {
                if (CurrentEnemyState == EnemyState.Parrying)
                {
                    PlayerScript.UpArrowFlag = false;
                    PlayerScript.DownArrowFlag = false;
                    PlayerScript.LeftArrowFlag = false;
                    PlayerScript.RightArrowFlag = false;
                    PlayerScript.GetComponent<Rigidbody>().velocity *= 0.0f;
                    PlayerScript.IsAttacking = false;
                    PlayerScript.GetComponent<MeshRenderer>().material = PlayerScript.PlayerColors[3];
                    PlayerScript.RecoveryTimer = 0.6f;
                    PlayerScript.CurrentPlayerState = PuzzlePlayerBehaviour.PlayerState.Recovering;
                    PlayerScript.EnemyCollided = true;
                    PlayerScript.AttackingDuration = 0.0f;
                    CurrentEnemyState = EnemyState.Recovering;
                    GetComponent<MeshRenderer>().material = EnemyColors[2];
                    RecoveryTimer = 0.3f;
                }
                else if (PlayerScript.AttackingTimer < AttackTimer)
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
                                PlayerScript.ActionTimers.Add(0.2f - PlayerScript.AttackingDuration + 0.3f);
                            }
                            else
                            {
                                //RecoveryTimer = 0.5f;
                                PlayerScript.ActionTimers.Add(0.4f - PlayerScript.AttackingDuration + 0.6f);
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
                            PlayerScript.RecoveryTimer = 0.6f;
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
                                RecoveryTimer = 0.6f;
                                PlayerScript.ActionTimers.Add(0.4f - PlayerScript.AttackingDuration + 0.6f);
                            }
                        }
                    }
                    PlayerScript.AttackingDuration = 0.0f;
                    if (IsPlanningPhase)
                    {
                        IsActive = false;
                        GetComponent<MeshRenderer>().enabled = false;
                        GetComponent<BoxCollider>().enabled = false;
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                }
            }
            else if (PlayerScript.CurrentPlayerState == PuzzlePlayerBehaviour.PlayerState.Parrying)
            {
                PlayerScript.UpArrowFlag = false;
                PlayerScript.DownArrowFlag = false;
                PlayerScript.LeftArrowFlag = false;
                PlayerScript.RightArrowFlag = false;

                StunDuration = 2.0f;
                CurrentEnemyState = EnemyState.Stunned;
                GetComponent<MeshRenderer>().material = EnemyColors[2];
                PlayerScript.RecoveryTimer = 0.2f;
                PlayerScript.CurrentPlayerState = PuzzlePlayerBehaviour.PlayerState.Recovering;
                PlayerScript.EnemyCollided = true;
                PlayerScript.ParryDuration = 0.0f;
                PlayerScript.GetComponent<MeshRenderer>().material = PlayerScript.PlayerColors[3];
                AttackTimer = 9000.1f;
                if (IsPlanningPhase)
                {
                    PlayerScript.ActionTimers.Add(0.3f - PlayerScript.ParryDuration + 0.2f);
                }
            }
            else if (CurrentEnemyState == EnemyState.Attacking)
            {
                //Destroy(Col.gameObject);
                Col.gameObject.GetComponent<PuzzlePlayerBehaviour>().enabled = false;
                Col.gameObject.GetComponent<MeshRenderer>().enabled = false;
                Col.gameObject.GetComponent<BoxCollider>().enabled = false;
                Col.rigidbody.velocity *= 0.0f;
                IsActive = false;
                GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (GameObject enemy in Enemies)
                {
                    enemy.GetComponent<PuzzleEnemyBehaviour>().IsActive = false;
                }
            }

            Col.transform.rotation = Quaternion.identity;
            Debug.Log("COLLISION");
            Debug.Log("Player timer = " + PlayerScript.AttackingTimer);
            Debug.Log("AI timer = " + AttackTimer);
        }
    }

    void OnCollisionStay(Collision Col)
    {
        if (Col.gameObject.tag == "Player")
        {
            if (PlayerScript.CurrentPlayerState == PuzzlePlayerBehaviour.PlayerState.Attacking)
            {
                if (CurrentEnemyState == EnemyState.Parrying)
                {
                    PlayerScript.UpArrowFlag = false;
                    PlayerScript.DownArrowFlag = false;
                    PlayerScript.LeftArrowFlag = false;
                    PlayerScript.RightArrowFlag = false;
                    PlayerScript.GetComponent<Rigidbody>().velocity *= 0.0f;
                    PlayerScript.IsAttacking = false;
                    PlayerScript.GetComponent<MeshRenderer>().material = PlayerScript.PlayerColors[3];
                    PlayerScript.RecoveryTimer = 0.6f;
                    PlayerScript.CurrentPlayerState = PuzzlePlayerBehaviour.PlayerState.Recovering;
                    PlayerScript.EnemyCollided = true;
                    PlayerScript.AttackingDuration = 0.0f;
                    CurrentEnemyState = EnemyState.Recovering;
                    GetComponent<MeshRenderer>().material = EnemyColors[2];
                    RecoveryTimer = 0.3f;
                }
                else if (PlayerScript.AttackingTimer < AttackTimer)
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
                                PlayerScript.ActionTimers.Add(0.2f - PlayerScript.AttackingDuration + 0.3f);
                            }
                            else
                            {
                                //RecoveryTimer = 0.5f;
                                PlayerScript.ActionTimers.Add(0.4f - PlayerScript.AttackingDuration + 0.6f);
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
                            PlayerScript.RecoveryTimer = 0.6f;
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
                                RecoveryTimer = 0.6f;
                                PlayerScript.ActionTimers.Add(0.4f - PlayerScript.AttackingDuration + 0.6f);
                            }
                        }
                    }
                    PlayerScript.AttackingDuration = 0.0f;
                    if (IsPlanningPhase)
                    {
                        IsActive = false;
                        GetComponent<MeshRenderer>().enabled = false;
                        GetComponent<BoxCollider>().enabled = false;
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                }
            }
            else if (PlayerScript.CurrentPlayerState == PuzzlePlayerBehaviour.PlayerState.Parrying)
            {
                PlayerScript.UpArrowFlag = false;
                PlayerScript.DownArrowFlag = false;
                PlayerScript.LeftArrowFlag = false;
                PlayerScript.RightArrowFlag = false;

                StunDuration = 2.0f;
                CurrentEnemyState = EnemyState.Stunned;
                GetComponent<MeshRenderer>().material = EnemyColors[2];
                PlayerScript.RecoveryTimer = 0.2f;
                PlayerScript.CurrentPlayerState = PuzzlePlayerBehaviour.PlayerState.Recovering;
                PlayerScript.EnemyCollided = true;
                PlayerScript.ParryDuration = 0.0f;
                PlayerScript.GetComponent<MeshRenderer>().material = PlayerScript.PlayerColors[3];
                AttackTimer = 9000.1f;
                if (IsPlanningPhase)
                {
                    PlayerScript.ActionTimers.Add(0.3f - PlayerScript.ParryDuration + 0.2f);
                }
            }
            else if (CurrentEnemyState == EnemyState.Attacking)
            {
                //Destroy(Col.gameObject);
                Col.gameObject.GetComponent<PuzzlePlayerBehaviour>().enabled = false;
                Col.gameObject.GetComponent<MeshRenderer>().enabled = false;
                Col.gameObject.GetComponent<BoxCollider>().enabled = false;
                Col.rigidbody.velocity *= 0.0f;
                IsActive = false;
            }

            Col.transform.rotation = Quaternion.identity;
        }
    }

    void OnTriggerEnter(Collider Col)
    {
        if (Col.gameObject.tag == "Player")
        {
            if (PlayerScript.CurrentPlayerState == PuzzlePlayerBehaviour.PlayerState.Attacking)
            {
                if (CurrentEnemyState == EnemyState.Parrying)
                {
                    PlayerScript.UpArrowFlag = false;
                    PlayerScript.DownArrowFlag = false;
                    PlayerScript.LeftArrowFlag = false;
                    PlayerScript.RightArrowFlag = false;
                    PlayerScript.GetComponent<Rigidbody>().velocity *= 0.0f;
                    PlayerScript.IsAttacking = false;
                    PlayerScript.GetComponent<MeshRenderer>().material = PlayerScript.PlayerColors[3];
                    PlayerScript.RecoveryTimer = 0.6f;
                    PlayerScript.CurrentPlayerState = PuzzlePlayerBehaviour.PlayerState.Recovering;
                    PlayerScript.EnemyCollided = true;
                    PlayerScript.AttackingDuration = 0.0f;
                    CurrentEnemyState = EnemyState.Recovering;
                    GetComponent<MeshRenderer>().material = EnemyColors[2];
                    RecoveryTimer = 0.3f;
                }
                else if (PlayerScript.AttackingTimer < AttackTimer)
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
                                PlayerScript.ActionTimers.Add(0.2f - PlayerScript.AttackingDuration + 0.3f);
                            }
                            else
                            {
                                //RecoveryTimer = 0.5f;
                                PlayerScript.ActionTimers.Add(0.4f - PlayerScript.AttackingDuration + 0.6f);
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
                            PlayerScript.RecoveryTimer = 0.6f;
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
                                RecoveryTimer = 0.6f;
                                PlayerScript.ActionTimers.Add(0.4f - PlayerScript.AttackingDuration + 0.6f);
                            }
                        }
                    }
                    PlayerScript.AttackingDuration = 0.0f;
                    if (IsPlanningPhase)
                    {
                        IsActive = false;
                        GetComponent<MeshRenderer>().enabled = false;
                        GetComponent<BoxCollider>().enabled = false;
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                }
            }
            else if (PlayerScript.CurrentPlayerState == PuzzlePlayerBehaviour.PlayerState.Parrying)
            {
                PlayerScript.UpArrowFlag = false;
                PlayerScript.DownArrowFlag = false;
                PlayerScript.LeftArrowFlag = false;
                PlayerScript.RightArrowFlag = false;

                StunDuration = 2.0f;
                CurrentEnemyState = EnemyState.Stunned;
                GetComponent<MeshRenderer>().material = EnemyColors[2];
                PlayerScript.RecoveryTimer = 0.2f;
                PlayerScript.CurrentPlayerState = PuzzlePlayerBehaviour.PlayerState.Recovering;
                PlayerScript.EnemyCollided = true;
                PlayerScript.ParryDuration = 0.0f;
                PlayerScript.GetComponent<MeshRenderer>().material = PlayerScript.PlayerColors[3];
                AttackTimer = 9000.1f;
                if (IsPlanningPhase)
                {
                    PlayerScript.ActionTimers.Add(0.3f - PlayerScript.ParryDuration + 0.2f);
                }
            }
            else if (CurrentEnemyState == EnemyState.Attacking)
            {
                //Destroy(Col.gameObject);
                Col.gameObject.GetComponent<PuzzlePlayerBehaviour>().enabled = false;
                Col.gameObject.GetComponent<MeshRenderer>().enabled = false;
                Col.gameObject.GetComponent<BoxCollider>().enabled = false;
                Col.GetComponent<Rigidbody>().velocity *= 0.0f;
                IsActive = false;
            }

            Col.transform.rotation = Quaternion.identity;
        }
    }

    void OnTriggerStay(Collider Col)
    {
        if (Col.gameObject.tag == "Player")
        {
            if (PlayerScript.CurrentPlayerState == PuzzlePlayerBehaviour.PlayerState.Attacking)
            {
                if (CurrentEnemyState == EnemyState.Parrying)
                {
                    PlayerScript.UpArrowFlag = false;
                    PlayerScript.DownArrowFlag = false;
                    PlayerScript.LeftArrowFlag = false;
                    PlayerScript.RightArrowFlag = false;
                    PlayerScript.GetComponent<Rigidbody>().velocity *= 0.0f;
                    PlayerScript.IsAttacking = false;
                    PlayerScript.GetComponent<MeshRenderer>().material = PlayerScript.PlayerColors[3];
                    PlayerScript.RecoveryTimer = 0.6f;
                    PlayerScript.CurrentPlayerState = PuzzlePlayerBehaviour.PlayerState.Recovering;
                    PlayerScript.EnemyCollided = true;
                    PlayerScript.AttackingDuration = 0.0f;
                    CurrentEnemyState = EnemyState.Recovering;
                    GetComponent<MeshRenderer>().material = EnemyColors[2];
                    RecoveryTimer = 0.3f;
                }
                else if (PlayerScript.AttackingTimer < AttackTimer)
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
                                PlayerScript.ActionTimers.Add(0.2f - PlayerScript.AttackingDuration + 0.3f);
                            }
                            else
                            {
                                //RecoveryTimer = 0.5f;
                                PlayerScript.ActionTimers.Add(0.4f - PlayerScript.AttackingDuration + 0.6f);
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
                            PlayerScript.RecoveryTimer = 0.6f;
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
                                RecoveryTimer = 0.6f;
                                PlayerScript.ActionTimers.Add(0.4f - PlayerScript.AttackingDuration + 0.6f);
                            }
                        }
                    }
                    PlayerScript.AttackingDuration = 0.0f;
                    if (IsPlanningPhase)
                    {
                        IsActive = false;
                        GetComponent<MeshRenderer>().enabled = false;
                        GetComponent<BoxCollider>().enabled = false;
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                }
            }
            else if (PlayerScript.CurrentPlayerState == PuzzlePlayerBehaviour.PlayerState.Parrying)
            {
                PlayerScript.UpArrowFlag = false;
                PlayerScript.DownArrowFlag = false;
                PlayerScript.LeftArrowFlag = false;
                PlayerScript.RightArrowFlag = false;

                StunDuration = 2.0f;
                CurrentEnemyState = EnemyState.Stunned;
                GetComponent<MeshRenderer>().material = EnemyColors[2];
                PlayerScript.RecoveryTimer = 0.2f;
                PlayerScript.CurrentPlayerState = PuzzlePlayerBehaviour.PlayerState.Recovering;
                PlayerScript.EnemyCollided = true;
                PlayerScript.ParryDuration = 0.0f;
                PlayerScript.GetComponent<MeshRenderer>().material = PlayerScript.PlayerColors[3];
                AttackTimer = 9000.1f;
                if (IsPlanningPhase)
                {
                    PlayerScript.ActionTimers.Add(0.3f - PlayerScript.ParryDuration + 0.2f);
                }
            }
            else if (CurrentEnemyState == EnemyState.Attacking)
            {
                //Destroy(Col.gameObject);
                Col.gameObject.GetComponent<PuzzlePlayerBehaviour>().enabled = false;
                Col.gameObject.GetComponent<MeshRenderer>().enabled = false;
                Col.gameObject.GetComponent<BoxCollider>().enabled = false;
                Col.GetComponent<Rigidbody>().velocity *= 0.0f;
                IsActive = false;
            }

            Col.transform.rotation = Quaternion.identity;
        }
    }

    void DoThings()
    {
        switch (CurrentEnemyState)
        {
            case EnemyState.Seeking:
                {
                    //Debug.Log(ThePlayer.GetComponent<Transform>().position);
                    if (IsPursuer)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, PlayerScript.GetPredictedForwardPosition(5.0f), EnemyVelocity * Time.fixedDeltaTime);
                    }
                    else
                    {
                        transform.position = Vector3.MoveTowards(transform.position, ThePlayer.GetComponent<Transform>().position, EnemyVelocity * Time.fixedDeltaTime);
                    }
                    if (Vector3.Distance(transform.position, ThePlayer.GetComponent<Transform>().position) <= EnemyAttackRadius)
                    {
                        if (PlayerScript.CurrentPlayerState == PuzzlePlayerBehaviour.PlayerState.Attacking)
                        {
                            CurrentEnemyState = EnemyState.Parrying;
                            GetComponent<MeshRenderer>().material = EnemyColors[3];
                            ParryTimer = 0.0f;
                        }
                        else
                        {
                            CurrentEnemyState = EnemyState.Attacking;
                            GetComponent<MeshRenderer>().material = EnemyColors[1];
                            AttackPosition = ThePlayer.GetComponent<Transform>().position;
                            AttackTimer = Time.time;
                        }
                    }
                    break;
                }
            case EnemyState.Attacking:
                {
                    transform.position = Vector3.MoveTowards(transform.position, AttackPosition, EnemyVelocity * Time.fixedDeltaTime * 5.0f);
                    if (Vector3.Distance(transform.position, AttackPosition) == 0.0f)
                    {
                        CurrentEnemyState = EnemyState.Recovering;
                        GetComponent<MeshRenderer>().material = EnemyColors[2];
                        RecoveryTimer = 2.0f;
                        AttackTimer = 9000.1f;
                    }
                    break;
                }
            case EnemyState.Parrying:
                {
                    ParryTimer += Time.fixedDeltaTime;
                    if (ParryTimer >= ParryDuration)
                    {
                        CurrentEnemyState = EnemyState.Recovering;
                        GetComponent<MeshRenderer>().material = EnemyColors[2];
                        RecoveryTimer = 1.0f;
                    }
                    break;
                }
            case EnemyState.Recovering:
                {
                    RecoveryTimer -= Time.fixedDeltaTime;
                    if (RecoveryTimer <= 0.0f)
                    {
                        CurrentEnemyState = EnemyState.Seeking;
                        GetComponent<MeshRenderer>().material = EnemyColors[0];
                    }
                    break;
                }
            case EnemyState.Stunned:
                {
                    StunDuration -= Time.fixedDeltaTime;
                    if (StunDuration <= 0.0f)
                    {
                        CurrentEnemyState = EnemyState.Seeking;
                        GetComponent<MeshRenderer>().material = EnemyColors[0];
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
        AttackTimer = 9000.1f;
        transform.position = SpawnPosition;
        IsPlanningPhase = false;
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<BoxCollider>().enabled = true;
        PlayerScript.HasDrawn = false;
    }
}

using UnityEngine;
using System.Collections;

public class NinjaBehaviour : MonoBehaviour {

    public enum NinjaState
    {
        Seeking,
        Attacking,
        Evading,
        Recovering,
        Stunned
    }

    public bool IsPursuer = false;
    public bool IsPlanningPhase = true;
    public Vector3 SpawnPosition;
    public Material[] NinjaColors;
    public NinjaState CurrentNinjaState;
    public GameObject ThePlayer;
    public PuzzlePlayerBehaviour PlayerScript;
    public float NinjaVelocity = 3.0f;
    public float NinjaAttackRadius = 1.5f;
    Vector3 AttackPosition;
    Vector3 EvadeDirection;
    float AttackTimer;
    float EvadeDuration = 0.3f;
    float EvadeTimer;
    float EvadeCooldown;
    public float RecoveryTimer;
    public bool IsActive;
    public float StunDuration;
    public int Health = 1;

    // Use this for initialization
    void Start()
    {
        CurrentNinjaState = NinjaState.Seeking;
        ThePlayer = GameObject.FindGameObjectWithTag("Player");
        PlayerScript = ThePlayer.GetComponent<PuzzlePlayerBehaviour>();
        PlayerScript.GetComponent<MeshRenderer>().material = PlayerScript.PlayerColors[0];
        IsActive = true;
        AttackTimer = 9000.1f;
        EvadeTimer = 0.0f;
        EvadeCooldown = 0.0f;
        SpawnPosition = transform.position;
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
                if (CurrentNinjaState == NinjaState.Evading)
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
                    CurrentNinjaState = NinjaState.Recovering;
                    GetComponent<MeshRenderer>().material = NinjaColors[2];
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
                        GetComponent<SphereCollider>().enabled = false;
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
                CurrentNinjaState = NinjaState.Stunned;
                GetComponent<MeshRenderer>().material = NinjaColors[2];
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
            else if (CurrentNinjaState == NinjaState.Attacking)
            {
                //Destroy(Col.gameObject);
                Col.gameObject.GetComponent<PuzzlePlayerBehaviour>().enabled = false;
                Col.gameObject.GetComponent<MeshRenderer>().enabled = false;
                Col.gameObject.GetComponent<BoxCollider>().enabled = false;
                Col.rigidbody.velocity *= 0.0f;
                IsActive = false;
            }

            Debug.Log("COLLISION");
            Debug.Log("Player timer = " + PlayerScript.AttackingTimer);
            Debug.Log("AI timer = " + AttackTimer);

            Col.transform.rotation = Quaternion.identity;
            Debug.Log(Col.transform.rotation);
        }
    }

    void OnCollisionStay(Collision Col)
    {
        if (Col.gameObject.tag == "Player")
        {
            if (PlayerScript.CurrentPlayerState == PuzzlePlayerBehaviour.PlayerState.Attacking)
            {
                if (CurrentNinjaState == NinjaState.Evading)
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
                    CurrentNinjaState = NinjaState.Recovering;
                    GetComponent<MeshRenderer>().material = NinjaColors[2];
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
                        GetComponent<SphereCollider>().enabled = false;
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
                CurrentNinjaState = NinjaState.Stunned;
                GetComponent<MeshRenderer>().material = NinjaColors[2];
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
            else if (CurrentNinjaState == NinjaState.Attacking)
            {
                //Destroy(Col.gameObject);
                Col.gameObject.GetComponent<PuzzlePlayerBehaviour>().enabled = false;
                Col.gameObject.GetComponent<MeshRenderer>().enabled = false;
                Col.gameObject.GetComponent<BoxCollider>().enabled = false;
                Col.rigidbody.velocity *= 0.0f;
                IsActive = false;
            }

            Debug.Log("COLLISION");
            Debug.Log("Player timer = " + PlayerScript.AttackingTimer);
            Debug.Log("AI timer = " + AttackTimer);

            Col.transform.rotation = Quaternion.identity;
            Debug.Log(Col.transform.rotation);
        }
    }

    void DoThings()
    {
        switch (CurrentNinjaState)
        {
            case NinjaState.Seeking:
                {
                    CalculateMovement();
                    if (Vector3.Distance(transform.position, ThePlayer.GetComponent<Transform>().position) <= NinjaAttackRadius)
                    {
                        if (DetermineIfCanAttack())
                        {
                            CurrentNinjaState = NinjaState.Attacking;
                            GetComponent<MeshRenderer>().material = NinjaColors[1];
                            AttackPosition = ThePlayer.GetComponent<Transform>().position;
                            AttackTimer = Time.time;
                        }
                        else
                        {
                            if (EvadeCooldown == 0.0f)
                            {
                                CurrentNinjaState = NinjaState.Evading;
                                GetComponent<MeshRenderer>().material = NinjaColors[3];
                                EvadeTimer = 0.0f;
                                EvadeDirection = Vector3.Normalize(transform.position - PlayerScript.transform.position);
                            }
                            else
                            {
                                CurrentNinjaState = NinjaState.Attacking;
                                GetComponent<MeshRenderer>().material = NinjaColors[1];
                                AttackPosition = ThePlayer.GetComponent<Transform>().position;
                                AttackTimer = Time.time;
                            }
                        }
                    }
                    break;
                }
            case NinjaState.Attacking:
                {
                    transform.position = Vector3.MoveTowards(transform.position, AttackPosition, NinjaVelocity * Time.deltaTime * 5.0f);
                    if (Vector3.Distance(transform.position, AttackPosition) == 0.0f)
                    {
                        CurrentNinjaState = NinjaState.Recovering;
                        GetComponent<MeshRenderer>().material = NinjaColors[2];
                        RecoveryTimer = 1.5f;
                        AttackTimer = 9000.1f;
                    }
                    break;
                }
            case NinjaState.Evading:
                {
                    EvadeTimer += Time.deltaTime;
                    transform.position += EvadeDirection * NinjaVelocity * 3.0f * Time.deltaTime;
                    if (EvadeTimer >= EvadeDuration)
                    {
                        //If distance between ninja and player is less than 8, attack the player
                        if (Vector3.Magnitude(PlayerScript.transform.position - transform.position) <= 8.0f)
                        {
                            CurrentNinjaState = NinjaState.Attacking;
                            GetComponent<MeshRenderer>().material = NinjaColors[1];
                            AttackPosition = ThePlayer.GetComponent<Transform>().position;
                            AttackTimer = Time.time;
                        }
                        else
                        {
                            CurrentNinjaState = NinjaState.Recovering;
                            GetComponent<MeshRenderer>().material = NinjaColors[2];
                            RecoveryTimer = 0.5f;
                        }
                    }
                    break;
                }
            case NinjaState.Recovering:
                {
                    RecoveryTimer -= Time.deltaTime;
                    if (RecoveryTimer <= 0.0f)
                    {
                        CurrentNinjaState = NinjaState.Seeking;
                        GetComponent<MeshRenderer>().material = NinjaColors[0];
                    }
                    break;
                }
            case NinjaState.Stunned:
                {
                    StunDuration -= Time.deltaTime;
                    if (StunDuration <= 0.0f)
                    {
                        CurrentNinjaState = NinjaState.Seeking;
                        GetComponent<MeshRenderer>().material = NinjaColors[0];
                    }
                    break;
                }
        }
    }

    public void Reset()
    {
        CurrentNinjaState = NinjaState.Seeking;
        ThePlayer = GameObject.FindGameObjectWithTag("Player");
        PlayerScript = ThePlayer.GetComponent<PuzzlePlayerBehaviour>();
        PlayerScript.GetComponent<MeshRenderer>().material = PlayerScript.PlayerColors[0];
        GetComponent<MeshRenderer>().material = NinjaColors[0];
        IsActive = true;
        AttackTimer = 9000.1f;
        transform.position = SpawnPosition;
        IsPlanningPhase = false;
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<SphereCollider>().enabled = true;
        PlayerScript.HasDrawn = false;
    }

    bool DetermineIfCanAttack()
    {
        switch(PlayerScript.GetOppositeDirection())
        {
            case PuzzlePlayerBehaviour.FacingDirection.Up:
                {
                    if (transform.position.z > ThePlayer.transform.position.z)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            case PuzzlePlayerBehaviour.FacingDirection.UpRight:
                {
                    if ((transform.position.z > ThePlayer.transform.position.z) && (transform.position.x > ThePlayer.transform.position.x))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            case PuzzlePlayerBehaviour.FacingDirection.UpLeft:
                {
                    if ((transform.position.z > ThePlayer.transform.position.z) && (transform.position.x < ThePlayer.transform.position.x))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            case PuzzlePlayerBehaviour.FacingDirection.Down:
                {
                    if (transform.position.z < ThePlayer.transform.position.z)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            case PuzzlePlayerBehaviour.FacingDirection.DownRight:
                {
                    if ((transform.position.z < ThePlayer.transform.position.z) && (transform.position.x > ThePlayer.transform.position.x))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            case PuzzlePlayerBehaviour.FacingDirection.DownLeft:
                {
                    if ((transform.position.z < ThePlayer.transform.position.z) && (transform.position.x < ThePlayer.transform.position.x))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            case PuzzlePlayerBehaviour.FacingDirection.Left:
                {
                    if (transform.position.x < ThePlayer.transform.position.x)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            case PuzzlePlayerBehaviour.FacingDirection.Right:
                {
                    if (transform.position.x > ThePlayer.transform.position.x)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
        }
        return false;
    }

    void CalculateMovement()
    {
        Vector3 TargetDirection = Vector3.Normalize(PlayerScript.GetPredictedBackwardPosition(60.0f) - transform.position);
        if (DetermineIfCanAttack())
        {
            transform.position += TargetDirection * NinjaVelocity * Time.deltaTime;
        }
        else
        {
            Vector3 CollisionDetection = TargetDirection * 0.5f + transform.position;
            if (Vector3.Magnitude(ThePlayer.transform.position - CollisionDetection) <= 5.0f)
            {
                Vector3 AvoidanceForce = Vector3.Normalize(CollisionDetection - ThePlayer.transform.position) * 0.5f;
                Debug.Log("Avoidance force = " + AvoidanceForce);
                transform.position += (TargetDirection + AvoidanceForce) * NinjaVelocity * Time.deltaTime;
            }
            else
            {
                transform.position += TargetDirection * NinjaVelocity * Time.deltaTime;
            }
            Debug.Log("Cannot Attack");
        }
        
    }
}

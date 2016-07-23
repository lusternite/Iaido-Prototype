using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PuzzlePlayerBehaviour : MonoBehaviour
{

    public enum PlayerState
    {
        Idle,
        Attacking,
        Parrying,
        Evading,
        Recovering
    }


    public float MoveSpeed = 2.0f;
    public bool UpArrowFlag = false;
    public bool LeftArrowFlag = false;
    public bool DownArrowFlag = false;
    public bool RightArrowFlag = false;
    public Vector3 UpVelocity;
    public Vector3 RightVelocity;
    public bool IsAttacking = false;
    public float AttackingTimer = 9000.2f;
    public float AttackingCooldown = 0.0f;
    public float AttackingDuration = 0.0f;
    public float ParryDuration = 0.0f;
    public float EvadeDuration = 0.0f;
    public float RecoveryTimer = 0.0f;
    public bool Collided = false;
    public bool HasDrawn = false;
    public PlayerState CurrentPlayerState = PlayerState.Idle;
    public Material[] PlayerColors;
    public bool IsParryStance = true;
    public bool IaiParry = false;

    //New things

    public enum Actions
    {
        Attack,
        Parry,
        Evade,
        StanceChange,
        MovementUpKeyDown,
        MovementUpKeyUp,
        MovementRightKeyDown,
        MovementRightKeyUp,
        MovementDownKeyDown,
        MovementDownKeyUp,
        MovementLeftKeyDown,
        MovementLeftKeyUp
    }

    public enum FacingDirection
    {
        Up,
        UpRight,
        Right,
        DownRight,
        Down,
        DownLeft,
        Left,
        UpLeft
    }

    public bool IsDrawPhase = true;
    public bool InputEnabled = true;
    public bool Moving = false;
    public bool EnemyCollided = false;
    public float MovementTimer = 0.0f;
    public List<Actions> ActionSequence;
    public List<float> ActionTimers;
    public int MovementButtonsDown = 0;
    public FacingDirection CurrentFacingDirection = FacingDirection.Right;
    public int CurrentAction = 0;
    public float CurrentTimer = 0.0f;
    public bool MovementActionCompleted = false;
    public bool MovementUpKeyCompleted = false;
    public bool PreviousWasUpKey = false;
    public bool LevelCleared = false;
    public TimeManager ActionClock;
    public MoveManager ActionMoves;
    public ScoreManager ActionScore;
    public Canvas InformationText;

    // Use this for initialization
    void Start()
    {
        UpVelocity = new Vector3(0.0f, 0.0f, MoveSpeed);
        RightVelocity = new Vector3(MoveSpeed, 0.0f, 0.0f);
        RecoveryTimer = 0.0f;
        ActionClock = FindObjectOfType<TimeManager>();
        ActionMoves = FindObjectOfType<MoveManager>();
        ActionScore = FindObjectOfType<ScoreManager>();
        InformationText = GameObject.FindGameObjectWithTag("Information").GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDrawPhase)
        {
            HandleDirectionalInput();
            HandleSlashing();
            HandleDodge();
            HandleParrying();
            ChangeStance();
        }
        else
        {
            if (CurrentAction < ActionSequence.Count)
            {
                HandleActionSequence();
            }
            if (HasDrawn)
            {
                ActionClock.AddTime(Time.deltaTime);
            }

        }
        TextToggle();
    }

    void FixedUpdate()
    {
        if (IsDrawPhase)
        {
            FixedHandleMovement();
            FixedHandleSlashing();
            FixedHandleParrying();
            FixedHandleDodge();
            HandleRecovery();
        }
        else
        {
            if (CurrentAction <= ActionSequence.Count)
            {
                FixedHandleActionSequence();
            }
        }
    }

    void HandleDirectionalInput()
    {
        if (InputEnabled)
        {
            //Keys down
            if (MovementButtonsDown < 2)
            {
                if (Input.GetKeyDown("up"))
                {
                    UpArrowFlag = true;
                    Moving = true;
                    GetComponent<Rigidbody>().velocity *= 0.0f;
                    if (LeftArrowFlag)
                    {
                        CurrentFacingDirection = FacingDirection.UpLeft;
                        GetComponent<Rigidbody>().velocity += UpVelocity - RightVelocity;
                    }
                    else if (RightArrowFlag)
                    {
                        CurrentFacingDirection = FacingDirection.UpRight;
                        GetComponent<Rigidbody>().velocity += UpVelocity + RightVelocity;
                    }
                    else
                    {
                        CurrentFacingDirection = FacingDirection.Up;
                        GetComponent<Rigidbody>().velocity += UpVelocity;
                    }
                    MovementButtonsDown += 1;
                    ActionSequence.Add(Actions.MovementUpKeyDown);
                    if (MovementTimer > 0.0f || MovementButtonsDown == 2)
                    {
                        ActionTimers.Add(MovementTimer);
                        MovementTimer = 0.0f;
                    }
                }
                if (Input.GetKeyDown("left"))
                {
                    LeftArrowFlag = true;
                    Moving = true;
                    GetComponent<Rigidbody>().velocity *= 0.0f;
                    if (UpArrowFlag)
                    {
                        CurrentFacingDirection = FacingDirection.UpLeft;
                        GetComponent<Rigidbody>().velocity += UpVelocity - RightVelocity;
                    }
                    else if (DownArrowFlag)
                    {
                        CurrentFacingDirection = FacingDirection.DownLeft;
                        GetComponent<Rigidbody>().velocity += -UpVelocity - RightVelocity;
                    }
                    else
                    {
                        CurrentFacingDirection = FacingDirection.Left;
                        GetComponent<Rigidbody>().velocity += -RightVelocity;
                    }
                    MovementButtonsDown += 1;
                    ActionSequence.Add(Actions.MovementLeftKeyDown);
                    if (MovementTimer > 0.0f || MovementButtonsDown == 2)
                    {
                        ActionTimers.Add(MovementTimer);
                        MovementTimer = 0.0f;
                    }
                }
                if (Input.GetKeyDown("down"))
                {
                    DownArrowFlag = true;
                    Moving = true;
                    GetComponent<Rigidbody>().velocity *= 0.0f;
                    if (LeftArrowFlag)
                    {
                        CurrentFacingDirection = FacingDirection.DownLeft;
                        GetComponent<Rigidbody>().velocity += -UpVelocity - RightVelocity;
                    }
                    else if (RightArrowFlag)
                    {
                        CurrentFacingDirection = FacingDirection.DownRight;
                        GetComponent<Rigidbody>().velocity += -UpVelocity + RightVelocity;
                    }
                    else
                    {
                        CurrentFacingDirection = FacingDirection.Down;
                        GetComponent<Rigidbody>().velocity += -UpVelocity;
                    }
                    MovementButtonsDown += 1;
                    ActionSequence.Add(Actions.MovementDownKeyDown);
                    if (MovementTimer > 0.0f || MovementButtonsDown == 2)
                    {
                        ActionTimers.Add(MovementTimer);
                        MovementTimer = 0.0f;
                    }
                }
                if (Input.GetKeyDown("right"))
                {
                    RightArrowFlag = true;
                    Moving = true;
                    GetComponent<Rigidbody>().velocity *= 0.0f;
                    if (UpArrowFlag)
                    {
                        CurrentFacingDirection = FacingDirection.UpRight;
                        GetComponent<Rigidbody>().velocity += UpVelocity + RightVelocity;
                    }
                    else if (DownArrowFlag)
                    {
                        CurrentFacingDirection = FacingDirection.DownRight;
                        GetComponent<Rigidbody>().velocity += -UpVelocity + RightVelocity;
                    }
                    else
                    {
                        CurrentFacingDirection = FacingDirection.Right;
                        GetComponent<Rigidbody>().velocity += RightVelocity;
                    }
                    MovementButtonsDown += 1;
                    ActionSequence.Add(Actions.MovementRightKeyDown);
                    if (MovementTimer > 0.0f || MovementButtonsDown == 2)
                    {
                        ActionTimers.Add(MovementTimer);
                        MovementTimer = 0.0f;
                    }
                }
                if (MovementButtonsDown == 2)
                {
                    if (UpArrowFlag && RightArrowFlag)
                    {
                        CurrentFacingDirection = FacingDirection.UpRight;
                    }
                    else if (UpArrowFlag && LeftArrowFlag)
                    {
                        CurrentFacingDirection = FacingDirection.UpLeft;
                    }
                    else if (DownArrowFlag && RightArrowFlag)
                    {
                        CurrentFacingDirection = FacingDirection.DownRight;
                    }
                    else if (DownArrowFlag && LeftArrowFlag)
                    {
                        CurrentFacingDirection = FacingDirection.DownLeft;
                    }

                }
            }

            //Keys up
            if (MovementButtonsDown > 0)
            {
                if (Input.GetKeyUp("right"))
                {
                    if (RightArrowFlag)
                    {
                        RightArrowFlag = false;
                        GetComponent<Rigidbody>().velocity -= RightVelocity;
                        MovementButtonsDown -= 1;
                        ActionSequence.Add(Actions.MovementRightKeyUp);
                        ActionTimers.Add(MovementTimer);
                        MovementTimer = 0.0f;
                    }
                }
                if (Input.GetKeyUp("up"))
                {
                    if (UpArrowFlag)
                    {
                        UpArrowFlag = false;
                        GetComponent<Rigidbody>().velocity -= UpVelocity;
                        MovementButtonsDown -= 1;
                        ActionSequence.Add(Actions.MovementUpKeyUp);
                        ActionTimers.Add(MovementTimer);
                        MovementTimer = 0.0f;
                    }
                }
                if (Input.GetKeyUp("down"))
                {
                    if (DownArrowFlag)
                    {
                        DownArrowFlag = false;
                        GetComponent<Rigidbody>().velocity += UpVelocity;
                        MovementButtonsDown -= 1;
                        ActionSequence.Add(Actions.MovementDownKeyUp);
                        ActionTimers.Add(MovementTimer);
                        MovementTimer = 0.0f;
                    }
                }
                if (Input.GetKeyUp("left"))
                {
                    if (LeftArrowFlag)
                    {
                        LeftArrowFlag = false;
                        GetComponent<Rigidbody>().velocity += RightVelocity;
                        MovementButtonsDown -= 1;
                        ActionSequence.Add(Actions.MovementLeftKeyUp);
                        ActionTimers.Add(MovementTimer);
                        MovementTimer = 0.0f;
                    }
                }
                if (MovementButtonsDown == 1)
                {
                    if (UpArrowFlag)
                    {
                        CurrentFacingDirection = FacingDirection.Up;
                    }
                    else if (RightArrowFlag)
                    {
                        CurrentFacingDirection = FacingDirection.Right;
                    }
                    else if (DownArrowFlag)
                    {
                        CurrentFacingDirection = FacingDirection.Down;
                    }
                    else if (LeftArrowFlag)
                    {
                        CurrentFacingDirection = FacingDirection.Left;
                    }
                }
                else if (MovementButtonsDown == 0)
                {
                    Moving = false;
                    ActionTimers.Add(0.0f);
                }
            }
        }
    }

    void FixedHandleMovement()
    {
        //Update movement timer
        if (Moving)
        {
            MovementTimer += Time.fixedDeltaTime;
        }
    }

    void HandleRecovery()
    {
        if (RecoveryTimer > 0.0f)
        {
            if (GetComponent<Rigidbody>().velocity.magnitude != 0.0f)
            {
                GetComponent<Rigidbody>().velocity *= 0.0f;
            }
            RecoveryTimer -= Time.fixedDeltaTime;
            if (RecoveryTimer < 0.0f)
            {
                RecoveryTimer = 0.0f;
                CurrentPlayerState = PlayerState.Idle;
                GetComponent<MeshRenderer>().material = PlayerColors[0];
                InputEnabled = true;
            }
        }
    }

    void HandleSlashing()
    {
        if (Input.GetKeyDown("space") && AttackingDuration == 0.0f && CurrentPlayerState == PlayerState.Idle && InputEnabled)
        {
            CurrentPlayerState = PlayerState.Attacking;
            IsAttacking = true;
            AttackingTimer = Time.time;
            if (IsParryStance)
            {
                AttackingDuration = 0.2f;
            }
            else
            {
                AttackingDuration = 0.4f;
            }

            //AttackingCooldown = 2.0f;
            //GetComponent<Rigidbody>().velocity *= 5.0f;
            //UpVelocity *= 5.0f;
            //RightVelocity *= 5.0f;
            GetComponent<MeshRenderer>().material = PlayerColors[1];
            InputEnabled = false;
            switch (CurrentFacingDirection)
            {
                case FacingDirection.Right:
                    {
                        GetComponent<Rigidbody>().velocity = RightVelocity * 5.0f;
                        break;
                    }
                case FacingDirection.UpRight:
                    {
                        GetComponent<Rigidbody>().velocity = RightVelocity * 5.0f + UpVelocity * 5.0f;
                        break;
                    }
                case FacingDirection.Up:
                    {
                        GetComponent<Rigidbody>().velocity = UpVelocity * 5.0f;
                        break;
                    }
                case FacingDirection.UpLeft:
                    {
                        GetComponent<Rigidbody>().velocity = RightVelocity * -5.0f + UpVelocity * 5.0f;
                        break;
                    }
                case FacingDirection.Left:
                    {
                        GetComponent<Rigidbody>().velocity = RightVelocity * -5.0f;
                        break;
                    }
                case FacingDirection.DownLeft:
                    {
                        GetComponent<Rigidbody>().velocity = RightVelocity * -5.0f + UpVelocity * -5.0f;
                        break;
                    }
                case FacingDirection.Down:
                    {
                        GetComponent<Rigidbody>().velocity = UpVelocity * -5.0f;
                        break;
                    }
                case FacingDirection.DownRight:
                    {
                        GetComponent<Rigidbody>().velocity = RightVelocity * 5.0f + UpVelocity * -5.0f;
                        break;
                    }
            }
            if (Moving)
            {
                UpArrowFlag = false;
                LeftArrowFlag = false;
                DownArrowFlag = false;
                RightArrowFlag = false;
                ActionTimers.Add(MovementTimer);
                MovementTimer = 0.0f;
                Moving = false;
                MovementButtonsDown = 0;
            }
            ActionSequence.Add(Actions.Attack);
        }

    }

    void FixedHandleSlashing()
    {
        if (AttackingDuration > 0.0f)
        {
            AttackingDuration -= Time.fixedDeltaTime;
            if (AttackingDuration < 0.0f)
            {
                //Readjust
                //transform.position += GetComponent<Rigidbody>().velocity * AttackingDuration;

                CurrentPlayerState = PlayerState.Recovering;
                AttackingDuration = 0.0f;
                //Punish if missed
                if (IsParryStance)
                {
                    RecoveryTimer = 0.5f;
                    ActionTimers.Add(0.7f);
                }
                else
                {
                    RecoveryTimer = 1.0f;
                    ActionTimers.Add(1.4f);
                }
                IsAttacking = false;
                GetComponent<Rigidbody>().velocity *= 0.0f;
                //UpVelocity /= 5.0f;
                //RightVelocity /= 5.0f;
                AttackingTimer = 9000.2f;
                UpArrowFlag = false;
                DownArrowFlag = false;
                LeftArrowFlag = false;
                RightArrowFlag = false;
                GetComponent<MeshRenderer>().material = PlayerColors[3];
            }
        }
    }

    void HandleParrying()
    {
        if (Input.GetKeyDown(KeyCode.W) && ParryDuration == 0.0f && CurrentPlayerState == PlayerState.Idle && IsParryStance && InputEnabled)
        {
            CurrentPlayerState = PlayerState.Parrying;
            GetComponent<Rigidbody>().velocity *= 0.0f;
            ParryDuration = 0.3f;
            GetComponent<MeshRenderer>().material = PlayerColors[2];
            InputEnabled = false;
            if (Moving)
            {
                UpArrowFlag = false;
                LeftArrowFlag = false;
                DownArrowFlag = false;
                RightArrowFlag = false;
                ActionTimers.Add(MovementTimer);
                MovementTimer = 0.0f;
                Moving = false;
                MovementButtonsDown = 0;
            }
            ActionSequence.Add(Actions.Parry);
        }
    }

    void FixedHandleParrying()
    {
        if (ParryDuration > 0.0f)
        {
            ParryDuration -= Time.fixedDeltaTime;
            if (ParryDuration < 0.0f)
            {
                ParryDuration = 0.0f;
                RecoveryTimer = 0.2f;
                CurrentPlayerState = PlayerState.Recovering;
                GetComponent<MeshRenderer>().material = PlayerColors[3];
                UpArrowFlag = false;
                DownArrowFlag = false;
                LeftArrowFlag = false;
                RightArrowFlag = false;
                ActionTimers.Add(0.5f);
            }
        }
    }

    void HandleDodge()
    {
        if (Input.GetKeyDown(KeyCode.R) && CurrentPlayerState == PlayerState.Idle && InputEnabled && EvadeDuration == 0.0f)
        {
            CurrentPlayerState = PlayerState.Evading;
            //GetComponent<Rigidbody>().velocity *= 8.0f;
            //UpVelocity *= 10.0f;
            //RightVelocity *= 10.0f;
            if (IsParryStance)
            {
                EvadeDuration = 0.22f;
            }
            else
            {
                EvadeDuration = 0.1f;
            }

            InputEnabled = false;
            switch (CurrentFacingDirection)
            {
                case FacingDirection.Right:
                    {
                        GetComponent<Rigidbody>().velocity = RightVelocity * 10.0f;
                        break;
                    }
                case FacingDirection.UpRight:
                    {
                        GetComponent<Rigidbody>().velocity = RightVelocity * 10.0f + UpVelocity * 10.0f;
                        break;
                    }
                case FacingDirection.Up:
                    {
                        GetComponent<Rigidbody>().velocity = UpVelocity * 10.0f;
                        break;
                    }
                case FacingDirection.UpLeft:
                    {
                        GetComponent<Rigidbody>().velocity = RightVelocity * -10.0f + UpVelocity * 10.0f;
                        break;
                    }
                case FacingDirection.Left:
                    {
                        GetComponent<Rigidbody>().velocity = RightVelocity * -10.0f;
                        break;
                    }
                case FacingDirection.DownLeft:
                    {
                        GetComponent<Rigidbody>().velocity = RightVelocity * -10.0f + UpVelocity * -10.0f;
                        break;
                    }
                case FacingDirection.Down:
                    {
                        GetComponent<Rigidbody>().velocity = UpVelocity * -10.0f;
                        break;
                    }
                case FacingDirection.DownRight:
                    {
                        GetComponent<Rigidbody>().velocity = RightVelocity * 10.0f + UpVelocity * -10.0f;
                        break;
                    }
            }
            if (Moving)
            {
                UpArrowFlag = false;
                LeftArrowFlag = false;
                DownArrowFlag = false;
                RightArrowFlag = false;
                ActionTimers.Add(MovementTimer);
                MovementTimer = 0.0f;
                Moving = false;
                MovementButtonsDown = 0;
            }
            ActionSequence.Add(Actions.Evade);
            if (IsParryStance)
            {
                ActionTimers.Add(0.42f);
            }
            else
            {
                ActionTimers.Add(0.2f);
            }
        }
    }

    void FixedHandleDodge()
    {
        if (EvadeDuration > 0.0f)
        {
            EvadeDuration -= Time.fixedDeltaTime;
            if (EvadeDuration < 0.0f)
            {
                //Readjust
                //transform.position -= GetComponent<Rigidbody>().velocity * EvadeDuration;

                CurrentPlayerState = PlayerState.Recovering;
                RecoveryTimer = 0.2f;
                EvadeDuration = 0.0f;
                GetComponent<Rigidbody>().velocity *= 0.0f;
                //UpVelocity /= 10.0f;
                //RightVelocity /= 10.0f;
                UpArrowFlag = false;
                DownArrowFlag = false;
                LeftArrowFlag = false;
                RightArrowFlag = false;
                GetComponent<MeshRenderer>().material = PlayerColors[3];
            }
        }
    }

    void ChangeStance()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            IsParryStance = true;
            ActionSequence.Add(Actions.StanceChange);
            ActionTimers.Add(0.1f);
            RecoveryTimer = 0.1f;
            CurrentPlayerState = PlayerState.Recovering;
            GetComponent<MeshRenderer>().material = PlayerColors[3];
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            IsParryStance = false;
            ActionSequence.Add(Actions.StanceChange);
            ActionTimers.Add(0.1f);
            RecoveryTimer = 0.1f;
            CurrentPlayerState = PlayerState.Recovering;
            GetComponent<MeshRenderer>().material = PlayerColors[3];
        }
    }

    void TextToggle()
    {
        if (Input.GetKeyUp(KeyCode.T))
        {
            //FindObjectOfType<Canvas>().enabled = !FindObjectOfType<Canvas>().enabled;
            InformationText.enabled = !InformationText.enabled;
        }
    }

    public void HandleDraw()
    {
        IsDrawPhase = false;
        transform.position = new Vector3(0.0f, 0.52f, 0.0f);
        PuzzleEnemyBehaviour[] Enemies = FindObjectsOfType<PuzzleEnemyBehaviour>();
        foreach (PuzzleEnemyBehaviour i in Enemies)
        {
            i.gameObject.GetComponent<BoxCollider>().enabled = true;
            i.Reset();
        }
        NinjaBehaviour[] Ninjas = FindObjectsOfType<NinjaBehaviour>();
        foreach (NinjaBehaviour j in Ninjas)
        {
            j.gameObject.GetComponent<SphereCollider>().enabled = true;
            j.Reset();
        }
        Arrow[] Arrows = FindObjectsOfType<Arrow>();
        for (int k = Arrows.Length - 1; k >= 0; k--)
        {
            Destroy(Arrows[k].gameObject);
        }
        CurrentPlayerState = PlayerState.Idle;
        IsParryStance = true;
        CurrentFacingDirection = FacingDirection.Right;
        InputEnabled = false;
        EnemyCollided = false;
        GetComponent<Rigidbody>().velocity *= 0.0f;
        GameObject.FindGameObjectWithTag("Scoreboard").GetComponent<Canvas>().enabled = true;
    }

    void HandleActionSequence()
    {
        if (CurrentPlayerState != PlayerState.Recovering)
        {
            switch (ActionSequence[CurrentAction])
            {
                case Actions.Attack:
                    {
                        if (CurrentPlayerState == PlayerState.Idle)
                        {
                            ActionMoves.AddMoves(1);
                            CurrentPlayerState = PlayerState.Attacking;
                            IsAttacking = true;
                            AttackingTimer = Time.time;
                            if (IsParryStance)
                            {
                                AttackingDuration = 0.2f;
                            }
                            else
                            {
                                AttackingDuration = 0.4f;
                            }

                            //AttackingCooldown = 2.0f;
                            //GetComponent<Rigidbody>().velocity *= 5.0f;
                            //UpVelocity *= 5.0f;
                            //RightVelocity *= 5.0f;
                            GetComponent<MeshRenderer>().material = PlayerColors[1];
                            //InputEnabled = false;
                            switch (CurrentFacingDirection)
                            {
                                case FacingDirection.Right:
                                    {
                                        GetComponent<Rigidbody>().velocity = RightVelocity * 5.0f;
                                        break;
                                    }
                                case FacingDirection.UpRight:
                                    {
                                        GetComponent<Rigidbody>().velocity = RightVelocity * 5.0f + UpVelocity * 5.0f;
                                        break;
                                    }
                                case FacingDirection.Up:
                                    {
                                        GetComponent<Rigidbody>().velocity = UpVelocity * 5.0f;
                                        break;
                                    }
                                case FacingDirection.UpLeft:
                                    {
                                        GetComponent<Rigidbody>().velocity = RightVelocity * -5.0f + UpVelocity * 5.0f;
                                        break;
                                    }
                                case FacingDirection.Left:
                                    {
                                        GetComponent<Rigidbody>().velocity = RightVelocity * -5.0f;
                                        break;
                                    }
                                case FacingDirection.DownLeft:
                                    {
                                        GetComponent<Rigidbody>().velocity = RightVelocity * -5.0f + UpVelocity * -5.0f;
                                        break;
                                    }
                                case FacingDirection.Down:
                                    {
                                        GetComponent<Rigidbody>().velocity = UpVelocity * -5.0f;
                                        break;
                                    }
                                case FacingDirection.DownRight:
                                    {
                                        GetComponent<Rigidbody>().velocity = RightVelocity * 5.0f + UpVelocity * -5.0f;
                                        break;
                                    }
                            }
                            if (Moving)
                            {
                                UpArrowFlag = false;
                                LeftArrowFlag = false;
                                DownArrowFlag = false;
                                RightArrowFlag = false;
                                //ActionTimers.Add(MovementTimer);
                                MovementTimer = 0.0f;
                                Moving = false;
                            }
                        }
                        
                        break;
                    }
                case Actions.Parry:
                    {
                        if (CurrentPlayerState == PlayerState.Idle)
                        {
                            ActionMoves.AddMoves(1);
                            CurrentPlayerState = PlayerState.Parrying;
                            GetComponent<Rigidbody>().velocity *= 0.0f;
                            ParryDuration = 0.3f;
                            GetComponent<MeshRenderer>().material = PlayerColors[2];
                            //if (!HasDrawn)
                            //{
                            //    HasDrawn = true;
                            //    IaiParry = true;
                            //}
                            //InputEnabled = false;
                            if (Moving)
                            {
                                UpArrowFlag = false;
                                LeftArrowFlag = false;
                                DownArrowFlag = false;
                                RightArrowFlag = false;
                                //ActionTimers.Add(MovementTimer);
                                MovementTimer = 0.0f;
                                Moving = false;
                            }
                        }
                        
                        break;
                    }
                case Actions.Evade:
                    {
                        if (CurrentPlayerState == PlayerState.Idle)
                        {
                            ActionMoves.AddMoves(1);
                            CurrentPlayerState = PlayerState.Evading;
                            //GetComponent<Rigidbody>().velocity *= 8.0f;
                            //UpVelocity *= 10.0f;
                            //RightVelocity *= 10.0f;
                            EvadeDuration = 0.22f;
                            //InputEnabled = false;
                            switch (CurrentFacingDirection)
                            {
                                case FacingDirection.Right:
                                    {
                                        GetComponent<Rigidbody>().velocity = RightVelocity * 10.0f;
                                        break;
                                    }
                                case FacingDirection.UpRight:
                                    {
                                        GetComponent<Rigidbody>().velocity = RightVelocity * 10.0f + UpVelocity * 10.0f;
                                        break;
                                    }
                                case FacingDirection.Up:
                                    {
                                        GetComponent<Rigidbody>().velocity = UpVelocity * 10.0f;
                                        break;
                                    }
                                case FacingDirection.UpLeft:
                                    {
                                        GetComponent<Rigidbody>().velocity = RightVelocity * -10.0f + UpVelocity * 10.0f;
                                        break;
                                    }
                                case FacingDirection.Left:
                                    {
                                        GetComponent<Rigidbody>().velocity = RightVelocity * -10.0f;
                                        break;
                                    }
                                case FacingDirection.DownLeft:
                                    {
                                        GetComponent<Rigidbody>().velocity = RightVelocity * -10.0f + UpVelocity * -10.0f;
                                        break;
                                    }
                                case FacingDirection.Down:
                                    {
                                        GetComponent<Rigidbody>().velocity = UpVelocity * -10.0f;
                                        break;
                                    }
                                case FacingDirection.DownRight:
                                    {
                                        GetComponent<Rigidbody>().velocity = RightVelocity * 10.0f + UpVelocity * -10.0f;
                                        break;
                                    }
                            }
                            if (Moving)
                            {
                                UpArrowFlag = false;
                                LeftArrowFlag = false;
                                DownArrowFlag = false;
                                RightArrowFlag = false;
                                //ActionTimers.Add(MovementTimer);
                                MovementTimer = 0.0f;
                                Moving = false;
                            }
                        }
                        
                        break;
                    }
                case Actions.StanceChange:
                    {
                        ActionMoves.AddMoves(1);
                        IsParryStance = !IsParryStance;
                        RecoveryTimer = 0.1f;
                        CurrentPlayerState = PlayerState.Recovering;
                        GetComponent<MeshRenderer>().material = PlayerColors[3];
                        break;
                    }

                case Actions.MovementDownKeyDown:
                    {
                        if (!MovementActionCompleted)
                        {
                            Moving = true;
                            DownArrowFlag = true;
                            MovementActionCompleted = true;
                            GetComponent<Rigidbody>().velocity *= 0.0f;
                            if (LeftArrowFlag)
                            {
                                CurrentFacingDirection = FacingDirection.DownLeft;
                                GetComponent<Rigidbody>().velocity += -UpVelocity - RightVelocity;
                            }
                            else if (RightArrowFlag)
                            {
                                CurrentFacingDirection = FacingDirection.DownRight;
                                GetComponent<Rigidbody>().velocity += -UpVelocity + RightVelocity;
                            }
                            else
                            {
                                CurrentFacingDirection = FacingDirection.Down;
                                GetComponent<Rigidbody>().velocity += -UpVelocity;
                            }
                        }
                        break;
                    }
                case Actions.MovementLeftKeyDown:
                    {
                        if (!MovementActionCompleted)
                        {
                            Moving = true;
                            LeftArrowFlag = true;
                            MovementActionCompleted = true;
                            GetComponent<Rigidbody>().velocity *= 0.0f;
                            if (UpArrowFlag)
                            {
                                CurrentFacingDirection = FacingDirection.UpLeft;
                                GetComponent<Rigidbody>().velocity += UpVelocity - RightVelocity;
                            }
                            else if (DownArrowFlag)
                            {
                                CurrentFacingDirection = FacingDirection.DownLeft;
                                GetComponent<Rigidbody>().velocity += -UpVelocity - RightVelocity;
                            }
                            else
                            {
                                CurrentFacingDirection = FacingDirection.Left;
                                GetComponent<Rigidbody>().velocity += -RightVelocity;
                            }
                        }
                        break;
                    }
                case Actions.MovementRightKeyDown:
                    {
                        if (!MovementActionCompleted)
                        {
                            Moving = true;
                            RightArrowFlag = true;
                            MovementActionCompleted = true;
                            GetComponent<Rigidbody>().velocity *= 0.0f;
                            if (UpArrowFlag)
                            {
                                CurrentFacingDirection = FacingDirection.UpRight;
                                GetComponent<Rigidbody>().velocity += UpVelocity + RightVelocity;
                            }
                            else if (DownArrowFlag)
                            {
                                CurrentFacingDirection = FacingDirection.DownRight;
                                GetComponent<Rigidbody>().velocity += -UpVelocity + RightVelocity;
                            }
                            else
                            {
                                CurrentFacingDirection = FacingDirection.Right;
                                GetComponent<Rigidbody>().velocity += RightVelocity;
                            }
                        }
                        break;
                    }
                case Actions.MovementUpKeyDown:
                    {
                        if (!MovementActionCompleted)
                        {
                            Moving = true;
                            UpArrowFlag = true;
                            MovementActionCompleted = true;
                            GetComponent<Rigidbody>().velocity *= 0.0f;
                            if (LeftArrowFlag)
                            {
                                CurrentFacingDirection = FacingDirection.UpLeft;
                                GetComponent<Rigidbody>().velocity += UpVelocity - RightVelocity;
                            }
                            else if (RightArrowFlag)
                            {
                                CurrentFacingDirection = FacingDirection.UpRight;
                                GetComponent<Rigidbody>().velocity += UpVelocity + RightVelocity;
                            }
                            else
                            {
                                CurrentFacingDirection = FacingDirection.Up;
                                GetComponent<Rigidbody>().velocity += UpVelocity;
                            }
                        }
                        break;
                    }
                case Actions.MovementDownKeyUp:
                    {
                        if (!MovementActionCompleted)
                        {
                            DownArrowFlag = false;
                            MovementActionCompleted = true;
                            GetComponent<Rigidbody>().velocity += UpVelocity;
                            if (ActionTimers.Count > CurrentAction + 1)
                            {
                                if (!(IsUpkey(ActionSequence[CurrentAction + 1]) && ActionTimers[CurrentAction + 1] == 0.0f && ActionTimers[CurrentAction] == 0.0f))
                                {
                                    if (LeftArrowFlag)
                                    {
                                        CurrentFacingDirection = FacingDirection.Left;
                                        Moving = true;
                                    }
                                    else if (RightArrowFlag)
                                    {
                                        CurrentFacingDirection = FacingDirection.Right;
                                        Moving = true;
                                    }
                                }
                            }
                            
                            
                            MovementUpKeyCompleted = true;
                        }
                        break;
                    }
                case Actions.MovementLeftKeyUp:
                    {
                        if (!MovementActionCompleted)
                        {
                            LeftArrowFlag = false;
                            MovementActionCompleted = true;
                            GetComponent<Rigidbody>().velocity += RightVelocity;
                            if (ActionTimers.Count > CurrentAction + 1)
                            {
                                if (!(IsUpkey(ActionSequence[CurrentAction + 1]) && ActionTimers[CurrentAction + 1] == 0.0f && ActionTimers[CurrentAction] == 0.0f))
                                {
                                    if (UpArrowFlag)
                                    {
                                        CurrentFacingDirection = FacingDirection.Up;
                                        Moving = true;
                                    }
                                    else if (DownArrowFlag)
                                    {
                                        CurrentFacingDirection = FacingDirection.Down;
                                        Moving = true;
                                    }
                                }
                            }
                            
                            
                            MovementUpKeyCompleted = true;
                        }
                        break;
                    }
                case Actions.MovementRightKeyUp:
                    {
                        if (!MovementActionCompleted)
                        {
                            RightArrowFlag = false;
                            MovementActionCompleted = true;
                            GetComponent<Rigidbody>().velocity -= RightVelocity;
                            if (ActionTimers.Count > CurrentAction + 1)
                            {
                                if (!(IsUpkey(ActionSequence[CurrentAction + 1]) && ActionTimers[CurrentAction + 1] == 0.0f && ActionTimers[CurrentAction] == 0.0f))
                                {
                                    if (UpArrowFlag)
                                    {
                                        CurrentFacingDirection = FacingDirection.Up;
                                        Moving = true;
                                    }
                                    else if (DownArrowFlag)
                                    {
                                        CurrentFacingDirection = FacingDirection.Down;
                                        Moving = true;
                                    }
                                }
                            }
                            
                            
                            MovementUpKeyCompleted = true;
                        }
                        break;
                    }
                case Actions.MovementUpKeyUp:
                    {
                        if (!MovementActionCompleted)
                        {
                            UpArrowFlag = false;
                            MovementActionCompleted = true;
                            GetComponent<Rigidbody>().velocity -= UpVelocity;
                            if (ActionTimers.Count > CurrentAction + 1)
                            {
                                if (!(IsUpkey(ActionSequence[CurrentAction + 1]) && ActionTimers[CurrentAction + 1] == 0.0f && ActionTimers[CurrentAction] == 0.0f))
                                {
                                    if (LeftArrowFlag)
                                    {
                                        CurrentFacingDirection = FacingDirection.Left;
                                        Moving = true;
                                    }
                                    else if (RightArrowFlag)
                                    {
                                        CurrentFacingDirection = FacingDirection.Right;
                                        Moving = true;
                                    }
                                }
                            }
                            MovementUpKeyCompleted = true;
                        }
                        break;
                    }
            }
        }
        //handle recovery
        else
        {
            
        }
        
    }

    void FixedHandleActionSequence()
    {
        //Recovery
        if (RecoveryTimer > 0.0f)
        {
            GetComponent<Rigidbody>().velocity *= 0.0f;
            RecoveryTimer -= Time.fixedDeltaTime;
            if (RecoveryTimer <= 0.0f)
            {
                RecoveryTimer = 0.0f;
                CurrentPlayerState = PlayerState.Idle;
                GetComponent<MeshRenderer>().material = PlayerColors[0];
                if (EnemyCollided)
                {
                    CurrentAction += 1;
                    CurrentTimer = 0.0f;
                    EnemyCollided = false;
                    if (CurrentAction > ActionSequence.Count)
                    {
                        Destroy(gameObject);
                    }
                    MovementActionCompleted = false;
                }
            }
        }

        //Handle action timer
        if (!EnemyCollided)
        {
            CurrentTimer += Time.fixedDeltaTime;
            if (CurrentTimer >= ActionTimers[CurrentAction])
            {
                MovementActionCompleted = false;
                if (CurrentAction + 1 == ActionSequence.Count)
                {
                    //Destroy(gameObject);
                    CurrentAction += 1;
                }
                else
                {
                    if (Moving)
                    {
                        Debug.Log("Current Timer = " + CurrentTimer);
                        Debug.Log("Current Action Timer = " + ActionTimers[CurrentAction]);
                        Debug.Log("Deltatime = " + Time.fixedDeltaTime);
                        float MovementReadjust = CurrentTimer - ActionTimers[CurrentAction];
                        //transform.position -= GetComponent<Rigidbody>().velocity * MovementReadjust;
                        Moving = false;
                    }
                    CurrentAction += 1;
                    CurrentTimer = 0.0f;
                    RecoveryTimer = 0.0f;
                    CurrentPlayerState = PlayerState.Idle;
                    GetComponent<MeshRenderer>().material = PlayerColors[0];
                    if (MovementUpKeyCompleted)
                    {
                        MovementUpKeyCompleted = false;
                        PreviousWasUpKey = true;
                        //HandleActionSequence();
                        //Debug.Log("upkey skipped");
                    }
                    else
                    {
                        PreviousWasUpKey = false;
                    }
                }
            }
        }

        //Attacking
        if (AttackingDuration > 0.0f)
        {
            AttackingDuration -= Time.fixedDeltaTime;
            if (AttackingDuration < 0.0f)
            {
                //Readjust
                //transform.position += GetComponent<Rigidbody>().velocity * AttackingDuration;

                CurrentPlayerState = PlayerState.Recovering;
                AttackingDuration = 0.0f;
                IsAttacking = false;
                //ActionSequence.Add(Actions.Attack);
                if (IsParryStance)
                {
                    RecoveryTimer = 0.5f;
                    //ActionTimers.Add(0.5f);
                }
                else
                {
                    RecoveryTimer = 1.0f;
                    //ActionTimers.Add(1.0f);
                }
                GetComponent<Rigidbody>().velocity *= 0.0f;
                //UpVelocity /= 5.0f;
                //RightVelocity /= 5.0f;
                AttackingTimer = 9000.2f;
                UpArrowFlag = false;
                DownArrowFlag = false;
                LeftArrowFlag = false;
                RightArrowFlag = false;
                GetComponent<MeshRenderer>().material = PlayerColors[3];
                //InputEnabled = true;
            }
        }

        //Parry
        if (ParryDuration > 0.0f)
        {
            ParryDuration -= Time.fixedDeltaTime;
            if (ParryDuration < 0.0f)
            {
                ParryDuration = 0.0f;
                RecoveryTimer = 0.2f;
                //ActionSequence.Add(Actions.Parry);
                //ActionTimers.Add(0.5f);
                CurrentPlayerState = PlayerState.Recovering;
                GetComponent<MeshRenderer>().material = PlayerColors[3];
                UpArrowFlag = false;
                DownArrowFlag = false;
                LeftArrowFlag = false;
                RightArrowFlag = false;
                //InputEnabled = true;
            }
        }

        //Dodge
        if (EvadeDuration > 0.0f)
        {
            EvadeDuration -= Time.fixedDeltaTime;
            if (EvadeDuration < 0.0f)
            {
                //Readjust
                //transform.position -= GetComponent<Rigidbody>().velocity * EvadeDuration;

                CurrentPlayerState = PlayerState.Recovering;
                RecoveryTimer = 0.2f;
                EvadeDuration = 0.0f;
                GetComponent<Rigidbody>().velocity *= 0.0f;
                //UpVelocity /= 10.0f;
                //RightVelocity /= 10.0f;
                UpArrowFlag = false;
                DownArrowFlag = false;
                LeftArrowFlag = false;
                RightArrowFlag = false;
                GetComponent<MeshRenderer>().material = PlayerColors[3];
                //InputEnabled = true;
                //ActionSequence.Add(Actions.Evade);
                //ActionTimers.Add(0.3f);
            }
        }

        
    }

    public Vector3 GetPredictedForwardPosition(float FramesAhead)
    {
        Vector3 PredictedForwardPosition = transform.position;
        float PredictedFrames = FramesAhead * Time.fixedDeltaTime;

        if (Moving)
        {
            PredictedForwardPosition += (UpVelocity + RightVelocity) * PredictedFrames;
        }
        else
        {
            switch (CurrentFacingDirection)
            {
                case FacingDirection.Up:
                    {
                        PredictedForwardPosition += UpVelocity * PredictedFrames;
                        break;
                    }
                case FacingDirection.UpLeft:
                    {
                        PredictedForwardPosition += (UpVelocity - RightVelocity) * PredictedFrames;
                        break;
                    }
                case FacingDirection.UpRight:
                    {
                        PredictedForwardPosition += (UpVelocity + RightVelocity) * PredictedFrames;
                        break;
                    }
                case FacingDirection.Down:
                    {
                        PredictedForwardPosition += -UpVelocity * PredictedFrames;
                        break;
                    }
                case FacingDirection.DownLeft:
                    {
                        PredictedForwardPosition += (-UpVelocity - RightVelocity) * PredictedFrames;
                        break;
                    }
                case FacingDirection.DownRight:
                    {
                        PredictedForwardPosition += (-UpVelocity + RightVelocity) * PredictedFrames;
                        break;
                    }
                case FacingDirection.Left:
                    {
                        PredictedForwardPosition += -RightVelocity * PredictedFrames;
                        break;
                    }
                case FacingDirection.Right:
                    {
                        PredictedForwardPosition += RightVelocity * PredictedFrames;
                        break;
                    }
            }
        }

        return PredictedForwardPosition;
    }

    public Vector3 GetPredictedBackwardPosition(float FramesAhead)
    {
        Vector3 PredictedBackwardPosition = transform.position;
        float PredictedFrames = FramesAhead * Time.fixedDeltaTime;

        if (Moving)
        {
            PredictedBackwardPosition += (UpVelocity + RightVelocity) * PredictedFrames;
        }
        else
        {
            switch (GetOppositeDirection())
            {
                case FacingDirection.Up:
                    {
                        PredictedBackwardPosition += UpVelocity * PredictedFrames;
                        break;
                    }
                case FacingDirection.UpLeft:
                    {
                        PredictedBackwardPosition += (UpVelocity - RightVelocity) * PredictedFrames;
                        break;
                    }
                case FacingDirection.UpRight:
                    {
                        PredictedBackwardPosition += (UpVelocity + RightVelocity) * PredictedFrames;
                        break;
                    }
                case FacingDirection.Down:
                    {
                        PredictedBackwardPosition += -UpVelocity * PredictedFrames;
                        break;
                    }
                case FacingDirection.DownLeft:
                    {
                        PredictedBackwardPosition += (-UpVelocity - RightVelocity) * PredictedFrames;
                        break;
                    }
                case FacingDirection.DownRight:
                    {
                        PredictedBackwardPosition += (-UpVelocity + RightVelocity) * PredictedFrames;
                        break;
                    }
                case FacingDirection.Left:
                    {
                        PredictedBackwardPosition += -RightVelocity * PredictedFrames;
                        break;
                    }
                case FacingDirection.Right:
                    {
                        PredictedBackwardPosition += RightVelocity * PredictedFrames;
                        break;
                    }
            }
        }

        return PredictedBackwardPosition;
    }

    public FacingDirection GetOppositeDirection()
    {
        switch (CurrentFacingDirection)
        {
            case FacingDirection.Up:
                {
                    return FacingDirection.Down;
                }
            case FacingDirection.UpLeft:
                {
                    return FacingDirection.DownRight;
                }
            case FacingDirection.UpRight:
                {
                    return FacingDirection.DownLeft;
                }
            case FacingDirection.Down:
                {
                    return FacingDirection.Up;
                }
            case FacingDirection.DownLeft:
                {
                    return FacingDirection.UpRight;
                }
            case FacingDirection.DownRight:
                {
                    return FacingDirection.UpLeft;
                }
            case FacingDirection.Left:
                {
                    return FacingDirection.Right;
                }
            case FacingDirection.Right:
                {
                    return FacingDirection.Left;
                }
        }
        return FacingDirection.Left;
    }

    public Vector3 ConvertDirectionToVelocity(FacingDirection Direction)
    {
        switch (Direction)
        {
            case FacingDirection.Up:
                {
                    return UpVelocity;
                }
            case FacingDirection.UpLeft:
                {
                    return UpVelocity - RightVelocity;
                }
            case FacingDirection.UpRight:
                {
                    return UpVelocity + RightVelocity;
                }
            case FacingDirection.Down:
                {
                    return -UpVelocity;
                }
            case FacingDirection.DownLeft:
                {
                    return -UpVelocity - RightVelocity;
                }
            case FacingDirection.DownRight:
                {
                    return -UpVelocity + RightVelocity;
                }
            case FacingDirection.Left:
                {
                    return -RightVelocity;
                }
            case FacingDirection.Right:
                {
                    return RightVelocity;
                }
        }
        return RightVelocity;
    }

    bool IsUpkey(Actions Action)
    {
        if (Action == Actions.MovementDownKeyUp || Action == Actions.MovementLeftKeyUp || Action == Actions.MovementRightKeyUp || Action == Actions.MovementUpKeyUp)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void CheckWin()
    {
        GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (Enemies.Length == 0)
        {
            LevelCleared = true;
            HandleDraw();
        }
    }
}

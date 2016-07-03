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


    public float MoveSpeed = 1.0f;
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

    // Use this for initialization
    void Start()
    {
        UpVelocity = new Vector3(0.0f, 0.0f, 1.0f);
        RightVelocity = new Vector3(1.0f, 0.0f, 0.0f);
        RecoveryTimer = 0.0f;
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
            HandleRecovery();
            HandleDraw();
        }
        else
        {
            HandleActionSequence();
        }
        TextToggle();
    }

    void HandleDirectionalInput()
    {
        if (InputEnabled)
        {
            if (MovementButtonsDown < 2)
            {
                if (Input.GetKeyDown("up"))
                {
                    UpArrowFlag = true;
                    Moving = true;
                    GetComponent<Rigidbody>().velocity += UpVelocity;
                    CurrentFacingDirection = FacingDirection.Up;
                    MovementButtonsDown += 1;
                    ActionSequence.Add(Actions.MovementUpKeyDown);
                    if (MovementTimer > 0.0f)
                    {
                        ActionTimers.Add(MovementTimer);
                        MovementTimer = 0.0f;
                    }
                }
                if (Input.GetKeyDown("left"))
                {
                    LeftArrowFlag = true;
                    Moving = true;
                    GetComponent<Rigidbody>().velocity -= RightVelocity;
                    CurrentFacingDirection = FacingDirection.Left;
                    MovementButtonsDown += 1;
                    ActionSequence.Add(Actions.MovementLeftKeyDown);
                    if (MovementTimer > 0.0f)
                    {
                        ActionTimers.Add(MovementTimer);
                        MovementTimer = 0.0f;
                    }
                }
                if (Input.GetKeyDown("down"))
                {
                    DownArrowFlag = true;
                    Moving = true;
                    GetComponent<Rigidbody>().velocity -= UpVelocity;
                    CurrentFacingDirection = FacingDirection.Down;
                    MovementButtonsDown += 1;
                    ActionSequence.Add(Actions.MovementDownKeyDown);
                    if (MovementTimer > 0.0f)
                    {
                        ActionTimers.Add(MovementTimer);
                        MovementTimer = 0.0f;
                    }
                }
                if (Input.GetKeyDown("right"))
                {
                    RightArrowFlag = true;
                    Moving = true;
                    GetComponent<Rigidbody>().velocity += RightVelocity;
                    CurrentFacingDirection = FacingDirection.Right;
                    MovementButtonsDown += 1;
                    ActionSequence.Add(Actions.MovementRightKeyDown);
                    if (MovementTimer > 0.0f)
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
            if (Moving)
            {
                MovementTimer += Time.deltaTime;
            }
        }
    }

    void HandleRecovery()
    {
        if (RecoveryTimer > 0.0f)
        {
            RecoveryTimer -= Time.deltaTime;
            if (RecoveryTimer < 0.0f)
            {
                RecoveryTimer = 0.0f;
                CurrentPlayerState = PlayerState.Idle;
                GetComponent<MeshRenderer>().material = PlayerColors[0];
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
                AttackingDuration = 0.5f;
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
        }
        if (AttackingDuration > 0.0f)
        {
            AttackingDuration -= Time.deltaTime;
            if (AttackingDuration < 0.0f)
            {
                CurrentPlayerState = PlayerState.Recovering;
                AttackingDuration = 0.0f;
                IsAttacking = false;
                ActionSequence.Add(Actions.Attack);
                if (IsParryStance)
                {
                    RecoveryTimer = 0.3f;
                    ActionTimers.Add(0.5f);
                }
                else
                {
                    RecoveryTimer = 0.5f;
                    ActionTimers.Add(1.0f);
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
                InputEnabled = true;
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
        }
        if (ParryDuration > 0.0f)
        {
            ParryDuration -= Time.deltaTime;
            if (ParryDuration < 0.0f)
            {
                ParryDuration = 0.0f;
                RecoveryTimer = 0.2f;
                ActionSequence.Add(Actions.Parry);
                ActionTimers.Add(0.5f);
                CurrentPlayerState = PlayerState.Recovering;
                GetComponent<MeshRenderer>().material = PlayerColors[3];
                UpArrowFlag = false;
                DownArrowFlag = false;
                LeftArrowFlag = false;
                RightArrowFlag = false;
                InputEnabled = true;
            }
        }
    }

    void HandleDodge()
    {
        if (Input.GetKeyDown(KeyCode.R) && CurrentPlayerState == PlayerState.Idle && InputEnabled)
        {
            CurrentPlayerState = PlayerState.Evading;
            //GetComponent<Rigidbody>().velocity *= 8.0f;
            //UpVelocity *= 10.0f;
            //RightVelocity *= 10.0f;
            EvadeDuration = 0.2f;
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
        }
        if (EvadeDuration > 0.0f)
        {
            EvadeDuration -= Time.deltaTime;
            if (EvadeDuration < 0.0f)
            {
                CurrentPlayerState = PlayerState.Recovering;
                RecoveryTimer = 0.1f;
                EvadeDuration = 0.0f;
                GetComponent<Rigidbody>().velocity *= 0.0f;
                //UpVelocity /= 10.0f;
                //RightVelocity /= 10.0f;
                UpArrowFlag = false;
                DownArrowFlag = false;
                LeftArrowFlag = false;
                RightArrowFlag = false;
                GetComponent<MeshRenderer>().material = PlayerColors[3];
                InputEnabled = true;
                ActionSequence.Add(Actions.Evade);
                ActionTimers.Add(0.3f);
            }
        }
    }

    void ChangeStance()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            IsParryStance = true;
            ActionSequence.Add(Actions.StanceChange);
            ActionTimers.Add(0.0f);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            IsParryStance = false;
            ActionSequence.Add(Actions.StanceChange);
            ActionTimers.Add(0.0f);
        }
    }

    void TextToggle()
    {
        if (Input.GetKeyUp(KeyCode.T))
        {
            FindObjectOfType<Canvas>().enabled = !FindObjectOfType<Canvas>().enabled;
        }
    }

    void HandleDraw()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            IsDrawPhase = false;
            transform.position = new Vector3(0.0f, 0.52f, 0.0f);
            PuzzleEnemyBehaviour[] Enemies = FindObjectsOfType<PuzzleEnemyBehaviour>();
            foreach (PuzzleEnemyBehaviour i in Enemies)
            {
                i.gameObject.GetComponent<BoxCollider>().enabled = true;
            }
            CurrentPlayerState = PlayerState.Idle;
            IsParryStance = true;
            CurrentFacingDirection = FacingDirection.Right;
            InputEnabled = false;
        }
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
                            CurrentPlayerState = PlayerState.Attacking;
                            IsAttacking = true;
                            AttackingTimer = Time.time;
                            if (IsParryStance)
                            {
                                AttackingDuration = 0.2f;
                            }
                            else
                            {
                                AttackingDuration = 0.5f;
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
                        if (AttackingDuration > 0.0f)
                        {
                            AttackingDuration -= Time.deltaTime;
                            if (AttackingDuration < 0.0f)
                            {
                                CurrentPlayerState = PlayerState.Recovering;
                                AttackingDuration = 0.0f;
                                IsAttacking = false;
                                //ActionSequence.Add(Actions.Attack);
                                if (IsParryStance)
                                {
                                    RecoveryTimer = 0.3f;
                                    //ActionTimers.Add(0.5f);
                                }
                                else
                                {
                                    RecoveryTimer = 0.5f;
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
                        break;
                    }
                case Actions.Parry:
                    {
                        if (CurrentPlayerState == PlayerState.Idle)
                        {
                            CurrentPlayerState = PlayerState.Parrying;
                            GetComponent<Rigidbody>().velocity *= 0.0f;
                            ParryDuration = 0.3f;
                            GetComponent<MeshRenderer>().material = PlayerColors[2];
                            if (!HasDrawn)
                            {
                                HasDrawn = true;
                                IaiParry = true;
                            }
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
                        if (ParryDuration > 0.0f)
                        {
                            ParryDuration -= Time.deltaTime;
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
                        break;
                    }
                case Actions.Evade:
                    {
                        if (CurrentPlayerState == PlayerState.Idle)
                        {
                            CurrentPlayerState = PlayerState.Evading;
                            //GetComponent<Rigidbody>().velocity *= 8.0f;
                            //UpVelocity *= 10.0f;
                            //RightVelocity *= 10.0f;
                            EvadeDuration = 0.2f;
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
                        if (EvadeDuration > 0.0f)
                        {
                            EvadeDuration -= Time.deltaTime;
                            if (EvadeDuration < 0.0f)
                            {
                                CurrentPlayerState = PlayerState.Recovering;
                                RecoveryTimer = 0.1f;
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
                        break;
                    }
                case Actions.StanceChange:
                    {
                        if (Input.GetKeyUp(KeyCode.Alpha1))
                        {
                            IsParryStance = true;
                        }
                        else if (Input.GetKeyUp(KeyCode.Alpha2))
                        {
                            IsParryStance = false;
                        }
                        CurrentAction += 1;
                        break;
                    }

                case Actions.MovementDownKeyDown:
                    {
                        if (!MovementActionCompleted)
                        {
                            DownArrowFlag = true;
                            MovementActionCompleted = true;
                            GetComponent<Rigidbody>().velocity -= UpVelocity;
                            if (LeftArrowFlag)
                            {
                                CurrentFacingDirection = FacingDirection.DownLeft;
                            }
                            else if (RightArrowFlag)
                            {
                                CurrentFacingDirection = FacingDirection.DownRight;
                            }
                            else
                            {
                                CurrentFacingDirection = FacingDirection.Down;
                            }
                        }
                        break;
                    }
                case Actions.MovementLeftKeyDown:
                    {
                        if (!MovementActionCompleted)
                        {
                            LeftArrowFlag = true;
                            MovementActionCompleted = true;
                            GetComponent<Rigidbody>().velocity -= RightVelocity;
                            if (UpArrowFlag)
                            {
                                CurrentFacingDirection = FacingDirection.UpLeft;
                            }
                            else if (DownArrowFlag)
                            {
                                CurrentFacingDirection = FacingDirection.DownLeft;
                            }
                            else
                            {
                                CurrentFacingDirection = FacingDirection.Left;
                            }
                        }
                        break;
                    }
                case Actions.MovementRightKeyDown:
                    {
                        if (!MovementActionCompleted)
                        {
                            RightArrowFlag = true;
                            MovementActionCompleted = true;
                            GetComponent<Rigidbody>().velocity += RightVelocity;
                            if (UpArrowFlag)
                            {
                                CurrentFacingDirection = FacingDirection.UpRight;
                            }
                            else if (DownArrowFlag)
                            {
                                CurrentFacingDirection = FacingDirection.DownRight;
                            }
                            else
                            {
                                CurrentFacingDirection = FacingDirection.Right;
                            }
                        }
                        break;
                    }
                case Actions.MovementUpKeyDown:
                    {
                        if (!MovementActionCompleted)
                        {
                            UpArrowFlag = true;
                            MovementActionCompleted = true;
                            GetComponent<Rigidbody>().velocity += UpVelocity;
                            if (LeftArrowFlag)
                            {
                                CurrentFacingDirection = FacingDirection.UpLeft;
                            }
                            else if (RightArrowFlag)
                            {
                                CurrentFacingDirection = FacingDirection.UpRight;
                            }
                            else
                            {
                                CurrentFacingDirection = FacingDirection.Up;
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
                        }
                        break;
                    }
            }
        }
        //handle recovery
        else
        {
            RecoveryTimer -= Time.deltaTime;
            if (RecoveryTimer < 0.0f)
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
            CurrentTimer += Time.deltaTime;
            if (CurrentTimer >= ActionTimers[CurrentAction])
            {
                CurrentAction += 1;
                CurrentTimer = 0.0f;
                MovementActionCompleted = false;
                if (CurrentAction > ActionSequence.Count)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}

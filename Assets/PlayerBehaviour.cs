using UnityEngine;
using System.Collections;

public class PlayerBehaviour : MonoBehaviour
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
        HandleDirectionalInput();
        HandleSlashing();
        HandleDodge();
        HandleParrying();
        HandleRecovery();
        ChangeStance();
        TextToggle();
    }

    void HandleDirectionalInput()
    {
        if (CurrentPlayerState != PlayerState.Recovering && CurrentPlayerState != PlayerState.Parrying)
        {
            if (Input.GetKeyDown("up"))
            {
                UpArrowFlag = true;
                GetComponent<Rigidbody>().velocity += UpVelocity;
            }
            if (Input.GetKeyDown("left"))
            {
                LeftArrowFlag = true;
                GetComponent<Rigidbody>().velocity -= RightVelocity;
            }
            if (Input.GetKeyDown("down"))
            {
                DownArrowFlag = true;
                GetComponent<Rigidbody>().velocity -= UpVelocity;
            }
            if (Input.GetKeyDown("right"))
            {
                RightArrowFlag = true;
                GetComponent<Rigidbody>().velocity += RightVelocity;
            }

            if (Input.GetKeyUp("right"))
            {
                if (RightArrowFlag)
                {
                    RightArrowFlag = false;
                    GetComponent<Rigidbody>().velocity -= RightVelocity;
                }
            }
            if (Input.GetKeyUp("up"))
            {
                if (UpArrowFlag)
                {
                    UpArrowFlag = false;
                    GetComponent<Rigidbody>().velocity -= UpVelocity;
                }
            }
            if (Input.GetKeyUp("down"))
            {
                if (DownArrowFlag)
                {
                    DownArrowFlag = false;
                    GetComponent<Rigidbody>().velocity += UpVelocity;
                }
            }
            if (Input.GetKeyUp("left"))
            {
                if (LeftArrowFlag)
                {
                    LeftArrowFlag = false;
                    GetComponent<Rigidbody>().velocity += RightVelocity;
                }
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
        if (Input.GetKeyDown("space") && AttackingDuration == 0.0f && CurrentPlayerState == PlayerState.Idle)
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
            GetComponent<Rigidbody>().velocity *= 5.0f;
            UpVelocity *= 5.0f;
            RightVelocity *= 5.0f;
            GetComponent<MeshRenderer>().material = PlayerColors[1];
        }
        if (AttackingDuration > 0.0f)
        {
            AttackingDuration -= Time.deltaTime;
            if (AttackingDuration < 0.0f)
            {
                CurrentPlayerState = PlayerState.Recovering;
                AttackingDuration = 0.0f;
                IsAttacking = false;
                if (IsParryStance)
                {
                    RecoveryTimer = 0.3f;
                }
                else
                {
                    RecoveryTimer = 0.5f;
                }
                GetComponent<Rigidbody>().velocity *= 0.0f;
                UpVelocity /= 5.0f;
                RightVelocity /= 5.0f;
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
        if (Input.GetKeyDown(KeyCode.W) && ParryDuration == 0.0f && CurrentPlayerState == PlayerState.Idle && IsParryStance)
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
        }
        if (ParryDuration > 0.0f)
        {
            ParryDuration -= Time.deltaTime;
            if (ParryDuration < 0.0f)
            {
                ParryDuration = 0.0f;
                if (IaiParry)
                {
                    IaiParry = false;
                    CurrentPlayerState = PlayerState.Idle;
                    GetComponent<MeshRenderer>().material = PlayerColors[0];
                }
                else
                {
                    RecoveryTimer = 0.2f;
                    CurrentPlayerState = PlayerState.Recovering;
                    GetComponent<MeshRenderer>().material = PlayerColors[3];
                }
                UpArrowFlag = false;
                DownArrowFlag = false;
                LeftArrowFlag = false;
                RightArrowFlag = false;
            }
        }
    }

    void HandleDodge()
    {
        if (Input.GetKeyDown(KeyCode.R) && CurrentPlayerState == PlayerState.Idle)
        {
            CurrentPlayerState = PlayerState.Evading;
            GetComponent<Rigidbody>().velocity *= 8.0f;
            UpVelocity *= 10.0f;
            RightVelocity *= 10.0f;
            EvadeDuration = 0.2f;
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
                UpVelocity /= 10.0f;
                RightVelocity /= 10.0f;
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
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            IsParryStance = false;
        }
    }

    void TextToggle()
    {
        if (Input.GetKeyUp(KeyCode.T))
        {
            FindObjectOfType<Canvas>().enabled = !FindObjectOfType<Canvas>().enabled;
        }
    }
}
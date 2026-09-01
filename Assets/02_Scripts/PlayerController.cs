using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{
    //입력 데이터
    [SerializeField] InputData inputData;

    //기능 단위 참조 컴포넌트
    InputHandler inputHandler;

    PlayerMovement playerMovement;

    PlayerRotation playerRotation;

    PlayerInteracter playerInteracter;

    CameraShake cameraShake;

    [Header("플레이어 수치값")]
    public float Speed;

    [Header("플레이어 상태값")]
    public bool isWalking;

    public bool animationing;
    public bool backing;
    [SerializeField] private float backDelayTime;
    private float backTime;
    public bool turning;
    public bool turnDirection; // false = left, true = right
    [SerializeField] private float turnDelayTime;
    private float turnTime;

    [Header("플레이어 보정값")]
    public float turnRange;

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();

        playerMovement = GetComponent<PlayerMovement>();

        playerRotation = GetComponent<PlayerRotation>();

        playerInteracter = GetComponent<PlayerInteracter>();

        cameraShake = GetComponentInChildren<CameraShake>();
    }

    private void Start()
    {
        inputData = inputHandler.inputData;
    }

    private void Update()
    {
        if (backing)
        {
            if(playerRotation.Back())
            {
                transform.rotation = Quaternion.Euler(0, Mathf.Round(transform.rotation.eulerAngles.y / 90) * 90, 0);
                backing = false;
                animationing = false;
                backTime = backDelayTime;
            }
        }

        if (turning)
        {
            bool isx = false;
            if(transform.forward.x > 0)
            {
                isx = true;
            }

            float XorZ=-1;
            if (turnDirection)//right
            {
                if (isx)
                {
                    XorZ = playerInteracter.rightPosition.x;
                }
                else
                {
                    XorZ = playerInteracter.rightPosition.z;
                }
            }
            else//left
            {
                if (isx)
                {
                    XorZ = playerInteracter.leftPosition.x;
                }
                else
                {
                    XorZ = playerInteracter.leftPosition.z;
                }
            }

            cameraShake.Shake();
            if (playerMovement.TargetWalk(XorZ, Speed, isx) < turnRange)
            {
                if (playerRotation.Turn())
                {
                    transform.rotation = Quaternion.Euler(0, Mathf.Round(transform.rotation.eulerAngles.y / 90) * 90, 0);
                    turning = false;
                    animationing = false;
                    turnTime = turnDelayTime;
                    cameraShake.StopShake();
                }
            }
        }

        DelayTimer();

        if (animationing) return;

        if (isWalking)
        {
            playerInteracter.CheckWay();
        }

        InputControl();

    }

    void DelayTimer()
    {
        if (turnTime > 0)
        {
            turnTime -= Time.deltaTime;
        }
    }

    void InputControl()
    {
        if(inputData.rotationValue != 0)
        {
            playerRotation.HeadRotate(inputData.rotationValue);
        }

        if(inputData.GetInteraction())
        {
            playerInteracter.Interact();
        }

        if (!isWalking) return;

        if (inputData.isWalking)
        {
            if (playerMovement.Walk(Speed))
            {
                cameraShake.Shake();
            }
        }
        else
        {
            cameraShake.StopShake();
        }

        if (inputData.GetBack() && backTime <= 0)
        {
            playerRotation.ReSetBack();
            playerRotation.ResetHead();
            backing = true;
            animationing = true;
            cameraShake.StopShake();
        }

        if (playerInteracter.canTurnLeft && inputData.moveValue < 0 && turnTime <= 0)
        {
            turnDirection = false;
            playerRotation.ReSetTurn(false);
            turning = true;
            turnDirection = false;
            animationing = true;
            cameraShake.StopShake();
        }

        if (playerInteracter.canTurnRight && inputData.moveValue > 0 && turnTime <= 0)
        {
            turnDirection = true;
            playerRotation.ReSetTurn(true);
            turning = true;
            turnDirection = true;
            animationing = true;
            cameraShake.StopShake();
        }
    }
}

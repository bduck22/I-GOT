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

    PlayerTurnInteracter playerTurnInteracter;

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

        playerTurnInteracter = GetComponent<PlayerTurnInteracter>();

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
            Backing();
        }

        if (turning)
        {
            Turning();
        }

        if (animationing) return;

        DelayTimer();

        if (isWalking)
        {
            playerTurnInteracter.CheckWay();
        }

        InputControl();

    }

    void Backing()
    {
        if (playerRotation.Back())
        {
            transform.rotation = Quaternion.Euler(0, Mathf.Round(transform.rotation.eulerAngles.y / 90) * 90, 0);
            backing = false;
            animationing = false;
            backTime = backDelayTime;
        }
    }

    bool isx;

    void Turning()
    {
        float XorZ = -1;
        if (turnDirection)//right
        {
            if (isx)
            {
                XorZ = playerTurnInteracter.rightPosition.x;
            }
            else
            {
                XorZ = playerTurnInteracter.rightPosition.z;
            }
        }
        else//left
        {
            if (isx)
            {
                XorZ = playerTurnInteracter.leftPosition.x;
            }
            else
            {
                XorZ = playerTurnInteracter.leftPosition.z;
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

    void DelayTimer()
    {
        if (turnTime > 0)
        {
            turnTime -= Time.deltaTime;
        }

        if(backTime > 0)
        {
            backTime -= Time.deltaTime;
        }
    }

    void InputControl()
    {
        if(inputData.rotationValue != 0)
        {
            playerRotation.HeadRotate(inputData.rotationValue);
        }

        if (!isWalking) return;

        if (inputData.isWalking)
        {
            if (playerMovement.Walk(Speed))
            {
                Debug.Log(MapManager.Instance.GetPlayerCell().Tile.coordinate);
                cameraShake.Shake();
            }
            else
            {
                cameraShake.StopShake();
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

        if (playerTurnInteracter.canTurnLeft && inputData.moveValue < 0 && turnTime <= 0)
        {
            LeftRightTurn(false);
        }

        if (playerTurnInteracter.canTurnRight && inputData.moveValue > 0 && turnTime <= 0)
        {
            LeftRightTurn(true);
        }
    }

    void LeftRightTurn(bool direction)
    {
        turnDirection = direction;
        playerRotation.ReSetTurn(direction);

        turning = true;
        animationing = true;
        if (Mathf.Abs(transform.forward.x) > 0.5f)
        {
            isx = true;
        }
        else
        {
            isx = false;
        }
        cameraShake.StopShake();
    }
}

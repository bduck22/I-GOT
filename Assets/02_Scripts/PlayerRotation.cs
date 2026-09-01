using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    public float RotationSpeed = 10;

    [SerializeField] private float backRotationSpeed;
    [SerializeField] private float turnRotationSpeed;

    public Transform headRotationTarget;

    [SerializeField] private float maxHeadRotation;
    [SerializeField] private float minHeadRotation;

    public float TurnLimit;

    public void HeadRotate(float rotationAxis)
    {
        headRotationTarget.Rotate(Vector3.up * rotationAxis * RotationSpeed * Time.deltaTime);

        Quaternion clampedRotation = headRotationTarget.localRotation;

        clampedRotation.y = Mathf.Clamp(clampedRotation.y, minHeadRotation/360f, maxHeadRotation/360f);

        headRotationTarget.localRotation = clampedRotation;
    }

    public void ResetHead()
    {
        headRotationTarget.localRotation = Quaternion.Euler(0, 0, 0);
    }

    [SerializeField] private float backSize = 180;

    public void ReSetBack()
    {
        backSize = 180;
    }

    public bool Back()
    {
        transform.Rotate(Vector3.up * 180 * backRotationSpeed * Time.deltaTime);
        backSize -= 180 * backRotationSpeed * Time.deltaTime;

        return backSize <= 0;
    }

    public void ReSetTurn(bool direction)
    {
        backSize = 90 * (direction ? 1 : -1);
    }

    public bool Turn()
    {
        int minus = backSize < 0 ? -1 : 1;

        transform.Rotate(Vector3.up * 90 * minus * turnRotationSpeed * Time.deltaTime);
        backSize -= minus * 90 * turnRotationSpeed * Time.deltaTime;

        return Mathf.Abs(backSize) <= TurnLimit;
    }
}

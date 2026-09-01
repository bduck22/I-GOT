using UnityEngine;

public class PlayerInteracter : MonoBehaviour
{
    [SerializeField] private float forwardOffset = 0.8f;
    [SerializeField] private float sideOffset = 0.6f;

    [SerializeField]
    private Vector3 checkSize =
        new Vector3(0.3f, 1f, 0.5f);

    [SerializeField] private LayerMask turnLayer;

    public bool canTurnLeft;
    public Vector3 leftPosition;
    public bool canTurnRight;
    public Vector3 rightPosition;

    Vector3 leftCenter =>
transform.position
+ transform.forward * forwardOffset
- transform.right * sideOffset;

    Vector3 rightCenter =>
transform.position
+ transform.forward * forwardOffset
+ transform.right * sideOffset;


    public void CheckWay()
    {
        RaycastHit hit;
        
        Physics.Raycast(transform.position, transform.forward, out hit, 10f, LayerMask.GetMask("Interactor"));

        Collider[] hits = Physics.OverlapBox(
    leftCenter,
    checkSize * 0.5f,
    transform.rotation,
    turnLayer
);

        if (hits.Length > 0&& hits[0] != hit.collider)
        {
            leftPosition = hits[0].transform.position;

            canTurnLeft = true;
        }
        else
        {
            canTurnLeft = false;
        }


        hits = Physics.OverlapBox(
    rightCenter,
    checkSize * 0.5f,
    transform.rotation,
    turnLayer
);

        if (hits.Length > 0&& hits[0] != hit.collider)
        {
            rightPosition = hits[0].transform.position;

            canTurnRight = true;
        }
        else
        {
            canTurnRight = false;
        }
    }

    public void Interact()
    {
        RaycastHit hit;

        //if (Physics.Raycast(Camera.main.transform.position, transform.forward, out hit, 10f, LayerMask.GetMask("Interactor")))
        //{
            
        //}
    }

    private void OnDrawGizmos()
    {
        DrawCheckBox(leftCenter, canTurnLeft ? Color.green : Color.red);
        DrawCheckBox(rightCenter, canTurnRight ? Color.green : Color.red);
    }

    private void DrawCheckBox(Vector3 center, Color color)
    {
        Gizmos.color = color;

        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);

        Gizmos.DrawWireCube(Vector3.zero, checkSize);

        Gizmos.matrix = oldMatrix;
    }
}

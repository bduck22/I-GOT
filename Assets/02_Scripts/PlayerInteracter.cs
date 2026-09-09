using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteracter : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float rayDistance = 100f;
    [SerializeField] private LayerMask layerMask;

    private void Update()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        if(Physics.Raycast(ray, out RaycastHit hit, rayDistance, layerMask))
        {
            if(hit.transform.TryGetComponent<Interactable>(out Interactable Interactor))
            {
                Interactor.Interact();
            }
        }
    }
}

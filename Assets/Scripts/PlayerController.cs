using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] float maxInteractDistance = 2.0f;

    [Header("References")]
    [SerializeField] GameObject eyes;
    [SerializeField] LayerMask interactableLayers;
    public Ship ship;

    public void Update()
    {
        if ( ship != null )
        {
            ship.Control();

            if (/*Exit Key Pressed*/ Input.GetKeyDown(KeyCode.Tilde))
                ExitShipControl();
        }
        else
        {
            Control();
        }
    }

    void Control()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        if (Input.GetMouseButton(0)&&Physics.Raycast(eyes.transform.position, eyes.transform.forward, out RaycastHit hitInfo, maxInteractDistance, interactableLayers))
        {
            Interactable interactable = hitInfo.collider.gameObject.GetComponent<Interactable>();

            interactable.Interact();

            if (interactable.isControlConsole)
            {
                ControlConsole console = interactable.GetComponent<ControlConsole>();
                EnterShipControl(console.ship);
            }
        }
    }

    void ExitShipControl()
    {
        ship = null;
    }

    void EnterShipControl(Ship s)
    {
        ship = s;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public bool isControlConsole;

    public abstract void Interact();
}


/// EXAMPLE INTERACTION CODE
///
/// void Interact(int interactType)
/// {
///     RaycastHit hit;
///     Ray ray = new Ray(transform.position, Head.transform.TransformDirection(Vector3.forward));
///     if (Physics.Raycast(ray, out hit, InteractRange))
///     {
///         hit.transform.SendMessageUpwards("Interaction", interactType);
///     }
///     Debug.DrawRay(transform.position, Head.transform.TransformDirection(Vector3.forward), Color.green, InteractRange);
/// }
///
/// 
///
/// public abstract class Interactable : MonoBehaviour
/// {
///     virtual public void Interaction(int interactType)
///     {
///         switch (interactType)
///         {
///             default:
///                 break;
///             case 0: //interaction: use object
///                 UseInteraction();
///                 break;
///             case 1: //interaction: break object     // change to damage object //
///                 BreakInteraction();
///                 break;
///         }
///     }
///
///     virtual public void UseInteraction()
///     {
/// 
///     }
/// 
///     virtual public void BreakInteraction()
///     {
///         Destroy(this.gameObject);
///     }
/// }
///
/// END EXAMPLE
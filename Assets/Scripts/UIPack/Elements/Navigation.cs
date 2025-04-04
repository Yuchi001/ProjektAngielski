using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UIPack.Elements
{
    // TODO: IMPLEMENT BUTTON NAVIGATION
    public class Navigation : MonoBehaviour
    {
        [SerializeField] private List<NavigationElement> navigationElements;

        public void SetNavigationElements(List<NavigationElement> elements)
        {
            navigationElements = elements;
        }

        public void OnNavigate(InputAction.CallbackContext context)
        {
            
        }
    }
}
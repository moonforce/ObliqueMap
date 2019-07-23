using UnityEngine;
using UnityEngine.EventSystems;

namespace Battlehub.UIControls.MenuControl
{
    public class ContextMenuTrigger : MonoBehaviour
    {
        [SerializeField]
        private Menu m_menu = null;

        private void Update()
        {
            if(Input.GetMouseButtonUp(1))
            {
                Canvas canvas = GetComponentInParent<Canvas>();
                Vector3 position;
                Vector2 pos = Input.mousePosition;

                if(canvas.renderMode != RenderMode.ScreenSpaceOverlay && !RectTransformUtility.RectangleContainsScreenPoint((RectTransform)transform, pos, canvas.worldCamera))
                {
                    return;
                }
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle((RectTransform)transform, pos, canvas.worldCamera, out position))
                {
                    m_menu.transform.position = position;
                    m_menu.Open();
                }
            }
        }

       
    }

}

using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

public class AutoResizeLayoutElementWidth : MonoBehaviour
{
    private TreeView m_Tree;

    private void Start()
    {
        m_Tree = transform.GetComponentInParent<TreeView>();
        Invoke("OnRectTransformDimensionsChange", 0.1f);
    }

    void OnRectTransformDimensionsChange()
    {
        float width = transform.GetComponent<RectTransform>().rect.width;
        var treeViewComponents = transform.GetComponentsInChildren<TreeViewComponent>();
        if (treeViewComponents.Length == 0 || m_Tree == null || m_Tree.DefaultItem == null)
            return;
        foreach (var treeViewComponent in treeViewComponents)
        {
            treeViewComponent.GetComponent<LayoutElement>().preferredWidth = width;
        }
        m_Tree.DefaultItem.GetComponent<LayoutElement>().preferredWidth = width;
    }
}

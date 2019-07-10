using UIWidgets;
using UnityEngine;

public class ObliqueMapTreeView : TreeView
{
    private GameObject m_LastGameObject = null;

    protected override void RemoveCallback(ListViewItem item)
    {
        base.RemoveCallback(item);
        if (item != null)
        {
            item.onDoubleClick.RemoveListener(DoubleClickListener);
        }
    }

    protected override void AddCallback(ListViewItem item)
    {
        base.AddCallback(item);
        item.onDoubleClick.AddListener(DoubleClickListener);
    }

    void DoubleClickListener(int index)
    {
        var node = DataSource[index].Node;        
        if (node.Parent == ProjectCtrl.Instance.ModelsNode)
        {
            if (m_LastGameObject)
                m_LastGameObject.SetActive(false);
            MeshAnaliser.Instance.ResetChoice();
            GameObject go = ObjLoadManger.Instance.transform.Find(node.Item.LocalizedName).gameObject;
            Bounds bounds = go.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh.bounds;
            Vector3 center = bounds.center;
            Vector3 size = bounds.size;
            Camera.main.GetComponent<OrbitCamera>().SetTarget(go, center, size);
            go.SetActive(true);
            m_LastGameObject = go;
        }
    }
}
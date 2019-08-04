using UIWidgets;
using UnityEngine;

public class ObliqueMapTreeView : TreeView
{
    public static GameObject CurrentGameObject = null;

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
            if (CurrentGameObject)
                CurrentGameObject.SetActive(false);
            MeshAnaliser.Instance.ResetChoice();
            GameObject go = ObjLoadManger.Instance.transform.Find(node.Item.LocalizedName).gameObject;
            Bounds bounds = go.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh.bounds;
            Vector3 center = bounds.center;
            Vector3 size = bounds.size;
            Camera.main.GetComponent<OrbitCamera>().SetTarget(go, center, size);
            go.SetActive(true);
            CurrentGameObject = go;
        }
        else if (node.Parent == ProjectCtrl.Instance.ObliqueImagesNode)
        {
            if (MeshAnaliser.Instance.Editting)
            {
                MeshAnaliser.Instance.ResetChoice();
            }
            string imageUrl = node.Item.Name;
            ImageController.Instance.ResetContent();
            StartCoroutine(DatabaseLoaderTexture_DDS.Load(Utills.ChangeExtensionToDDS(imageUrl), SetTexture));
        }        
    }

    private void SetTexture(Texture2D texture)
    {
        ImageController.Instance.setImageTexture(texture);
    }
}
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
        if (node.Parent == ProjectCtrl.Instance.ModelsTreeNode)
        {
            if (CurrentGameObject)
                CurrentGameObject.SetActive(false);
            MeshAnaliser.Instance.ResetChoice();
            GameObject go = ProjectCtrl.Instance.ModelContainer.Find(node.Item.LocalizedName).gameObject;
            Bounds bounds = go.transform.GetComponent<MeshFilter>().sharedMesh.bounds;
            Vector3 center = bounds.center;
            Vector3 size = bounds.size;
            Camera.main.GetComponent<OrbitCamera>().SetTarget(go, center, size);
            go.SetActive(true);
            CurrentGameObject = go;
        }
        else if (node.Parent == ProjectCtrl.Instance.ObliqueImagesTreeNode)
        {
            if (ProjectStage.Instance.FaceChosed || ProjectStage.Instance.FaceEditting)
            {
                MeshAnaliser.Instance.ResetChoice();
                OrbitCamera.Instance.ReplaceModel();
            }
            string imageUrl = node.Item.Name;
            TextureHandler.Instance.ResetContent();
            StartCoroutine(DatabaseLoaderTexture_DDS.Load(Utills.ChangeExtensionToDDS(imageUrl), SetTexture));
        }        
    }

    private void SetTexture(Texture2D texture)
    {
        TextureHandler.Instance.SetImage(texture);
    }
}
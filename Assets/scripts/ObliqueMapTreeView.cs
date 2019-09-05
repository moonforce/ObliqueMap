using System;
using UIWidgets;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObliqueMapTreeView : TreeView
{
    public static GameObject CurrentGameObject = null;
    public static TreeNode<TreeViewItem> DoubleClickNode = null;
    public static TreeViewComponent PointerEnterComponent = null;

    protected override void RemoveCallback(ListViewItem item)
    {
        base.RemoveCallback(item);
        if (item != null)
        {
            item.onDoubleClick.RemoveListener(DoubleClickListener);
            item.onPointerClick.RemoveListener(PointerClickListener);
            item.onPointerEnterItem.RemoveListener(PointerEnterListener);
        }
    }

    protected override void AddCallback(ListViewItem item)
    {
        base.AddCallback(item);
        item.onDoubleClick.AddListener(DoubleClickListener);
        item.onPointerClick.AddListener(PointerClickListener);
        item.onPointerEnterItem.AddListener(PointerEnterListener);
    }

    private void PointerEnterListener(ListViewItem item)
    {
        PointerEnterComponent = item.GetComponent<TreeViewComponent>();
    }

    private void PointerClickListener(PointerEventData pointerEventData)
    {
        if (pointerEventData.pointerId == -2)
        {
            ContextMenuHandler.Instance.OpenTreeContextMenu(PointerEnterComponent);
        }            
    }

    void DoubleClickListener(int index)
    {
        DoubleClickNode = DataSource[index].Node;
        if (DoubleClickNode.Parent == ProjectCtrl.Instance.ModelsTreeNode)
        {
            if (CurrentGameObject)
                CurrentGameObject.SetActive(false);
            MeshAnaliser.Instance.ResetChoice();
            GameObject go = ProjectCtrl.Instance.ModelContainer.Find(DoubleClickNode.Item.LocalizedName).gameObject;
            Bounds bounds = go.transform.GetComponent<MeshFilter>().sharedMesh.bounds;
            Vector3 center = bounds.center;
            Vector3 size = bounds.size;
            Camera.main.GetComponent<OrbitCamera>().SetTarget(go, center, size);
            go.SetActive(true);
            CurrentGameObject = go;
        }
        else if (DoubleClickNode.Parent == ProjectCtrl.Instance.ObliqueImagesTreeNode)
        {
            if (ProjectStage.Instance.FaceChosed || ProjectStage.Instance.FaceEditting)
            {
                MeshAnaliser.Instance.ResetChoice();
                //OrbitCamera.Instance.ReplaceModel();
            }
            string imageUrl = DoubleClickNode.Item.Name;
            TextureHandler.Instance.ResetContent();
            StartCoroutine(DatabaseLoaderTexture_DDS.Load(Utills.ChangeExtensionToDDS(imageUrl), SetTexture));
        }        
    }

    private void SetTexture(Texture2D texture)
    {
        TextureHandler.Instance.SetImage(texture);
    }

    public static void DeleteSingleNode(TreeNode<TreeViewItem> selectedNode)
    {
        if (selectedNode.Parent == ProjectCtrl.Instance.ObliqueImagesTreeNode)
        {
            if (DoubleClickNode == selectedNode)
            {
                ProjectCtrl.Instance.ClearWhenDeleteObliqueImage();
            }
            selectedNode.RemoveFromTree();
            ProjectCtrl.Instance.ModifyProjectPath();
        }
        else if (selectedNode.Parent == ProjectCtrl.Instance.ModelsTreeNode)
        {
            if (DoubleClickNode == selectedNode)
            {
                ProjectCtrl.Instance.ClearWhenDeleteModel();
            }
            ProjectCtrl.Instance.DestroyGameObject(ProjectCtrl.Instance.ModelContainer.Find(selectedNode.Item.LocalizedName).gameObject);
            selectedNode.RemoveFromTree();
            ProjectCtrl.Instance.ModifyProjectPath();
        }
        else if (selectedNode.Parent == ProjectCtrl.Instance.SceneriesTreeNode)
        {
            ProjectCtrl.Instance.DestroyGameObject(ObjLoadManger.Instance.transform.Find(selectedNode.Item.LocalizedName).gameObject);
            selectedNode.RemoveFromTree();
            ProjectCtrl.Instance.ModifyProjectPath();
        }
    }

    public static void DeleteMultipleNodes()
    {
        foreach(var selectedNode in ProjectCtrl.Instance.Tree.SelectedNodes)
        {
            DeleteSingleNode(selectedNode);
        }
    }
}
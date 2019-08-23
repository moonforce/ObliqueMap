using UnityEngine;
using AsImpL;
using System.Collections;

public class ObjLoadManger : Singleton<ObjLoadManger>
{
    protected ObjLoadManger(){}

    [SerializeField]
    private ImportOptions importOptions = new ImportOptions();

    private ObjectImporter objImporter;

    private void Start()
    {
        objImporter = gameObject.GetComponent<ObjectImporter>();
        GetComponent<ObjectImporter>().ImportError += ObjImporter_ImportedError;
        GetComponent<ObjectImporter>().ImportedModel += ObjImporter_ImportedModel;
    }

    private void ObjImporter_ImportedError(string modelPath)
    {
        MessageBoxCtrl.Instance.Show("模型加载失败");
        ProgressbarCtrl.Instance.Hide();
    }

    private void ObjImporter_ImportedModel(GameObject go, string path)
    {
        MessageBoxCtrl.Instance.Hide();
    }

    public IEnumerator ImportModelAsync(string objName, string filePath)
    {
        yield return StartCoroutine(objImporter.ImportModelAsync(objName, filePath, transform, importOptions));
    }
}

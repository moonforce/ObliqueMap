using UnityEngine;
using AsImpL;

public class ObjLoadManger : Singleton<ObjLoadManger>
{
    protected ObjLoadManger(){}

    [SerializeField]
    private ImportOptions importOptions = new ImportOptions();

    private ObjectImporter objImporter;

    private void Start()
    {
        objImporter = gameObject.GetComponent<ObjectImporter>();
        //GetComponent<ObjectImporter>().ImportedModel += ObjImporter_ImportedModel;
    }

    private void ObjImporter_ImportedModel(GameObject go, string path)
    {
        go.SetActive(false);
        ProgressbarCtrl.Instance.ProgressPlusPlus();
    }

    public void ImportModelAsync(string objName, string filePath)
    {
        objImporter.ImportModelAsync(objName, filePath, transform, importOptions);
    }
}

using UnityEngine;
using UnityEngine.UI;

namespace Battlehub.UIControls.MenuControl
{
    public class MenuDemo : MonoBehaviour
    {
        [SerializeField]
        private Text m_output = null;

        public void OnValidateCmd(MenuItemValidationArgs args)
        {
            Debug.Log("Validate Command: " + args.Command);

            if(args.Command == "DisabledCmd")
            {
                args.IsValid = false;
            }
        }

        public void OnCmd(string cmd)
        {
            Debug.Log("Run Cmd: " + cmd);

            switch(cmd)
            {
                case "Red":
                    m_output.color = Color.red;
                    break;
                case "Green":
                    m_output.color = Color.green;
                    break;
                case "Blue":
                    m_output.color = Color.blue;
                    break;
                case "White":
                    m_output.color = Color.white;
                    break;
                case "CreateCube":
                    GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = Camera.main.transform.position + Camera.main.transform.forward * 3;
                    break;
                case "CreateSphere":
                    GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position = Camera.main.transform.position + Camera.main.transform.forward * 3; 
                    break;
                case "CreateCapsule":
                    GameObject.CreatePrimitive(PrimitiveType.Capsule).transform.position = Camera.main.transform.position + Camera.main.transform.forward * 3; 
                    break;
                case "Exit":
                    Application.Quit();
                    break;
            }

            m_output.text = "Last Cmd: " + cmd;
        }
    }
}

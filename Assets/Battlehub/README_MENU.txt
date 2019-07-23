The Menu control allows hierarchal organization of elements associated with commands. It can be used to implement main and context menu of an application.

Getting Started: 

1. Create Canvas
2. Add MenuButton.prefab from Assets/Battelhub/UIControls/Menu/Prefabs to hierarchy.
3. Add Menu.prefab from Assets/Battelhub/UIControls/Menu/Prefabs to hierarchy
4. Set Menu field of the Menu Button
5. Create Empty Game Object and name it Command Handler
6. Create MenuCmdHandler script and add it to the Command Handler.
7. Set Action and Validate event handlers of each entry in Items array of the Menu.
8. Hit play

using UnityEngine;
using Battlehub.UIControls.MenuControl;

public class MenuCmdHandler : MonoBehaviour
{
    public void OnValidate(MenuItemValidationArgs args)
    {
        Debug.Log("Validate Command: " + args.Command);

        if(args.Command == "Item Cmd")
        {
            args.IsValid = false;
        }
    }

    public void OnAction(string cmd)
    {
        Debug.Log("Run Cmd: " + cmd);
    }
}

Full demo can be found in Assets/Battelhub/UIControls/Menu/Demo folder
For more info see online documentation: http://rteditor.battlehub.net/menu-control/
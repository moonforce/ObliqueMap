using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Battlehub.UIControls.MenuControl
{
    public class MenuDefinitionAttribute : Attribute
    { }

    public class MenuCommandAttribute : Attribute
    {
        public string Path
        {
            get;
            private set;
        }

        public string IconPath
        {
            get;
            private set;
        }

        public bool Validate
        {
            get;
            private set;
        }

        public bool Hide
        {
            get;
            private set;
        }

        public MenuCommandAttribute(string path, bool validate = false, bool hide = false)
        {
            Path = path;
            Validate = validate;
            Hide = hide;
        }

        public MenuCommandAttribute(string path, string iconPath)
        {
            Path = path;
            Validate = false;
            IconPath = iconPath;
        }

       
    }

    [DefaultExecutionOrder(-25)]
    public class MenuCreator : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_topMenu = null;

        [SerializeField]
        private GameObject m_menuPanel = null;

        [SerializeField]
        private MainMenuButton m_menuButtonPrefab = null;

        [SerializeField]
        private Menu m_menuPrefab = null;

        private void Awake()
        {
            var asName = new AssemblyName();
            asName.Name = "Assembly-CSharp";
            Assembly csAssembly = Assembly.Load(asName);
            if(csAssembly == null)
            {
                Debug.LogWarning("Unable to load Assembly-CSharp");
                return;
            }

            if (m_menuPanel == null)
            {
                m_menuPanel = gameObject;
            }

            if(m_topMenu == null)
            {
                m_topMenu = gameObject;
            }

            if(m_menuButtonPrefab == null)
            {
                Debug.LogError("Set Menu Button Prefab");
                return;
            }

            if(m_menuPrefab == null)
            {
                Debug.LogError("Set Menu Prefab");
                return;
            }

            bool wasButtonPrefabActive = m_menuButtonPrefab.gameObject.activeSelf;
            bool wasMenuPrefabActive = m_menuPrefab.gameObject.activeSelf;

            Dictionary<string, Menu> menuDictionary = new Dictionary<string, Menu>();
            Dictionary<string, List<MenuItemInfo>> menuItemsDictionary = new Dictionary<string, List<MenuItemInfo>>();
            Menu[] menus = m_menuPanel.GetComponentsInChildren<Menu>(true);
            for(int i = 0; i < menus.Length; ++i)
            {
                if(!menuDictionary.ContainsKey(menus[i].name))
                {
                    menuDictionary.Add(menus[i].name, menus[i]);

                    if(menus[i].Items != null)
                    {
                        menuItemsDictionary.Add(menus[i].name, menus[i].Items.ToList());
                    }
                    else
                    {
                        menuItemsDictionary.Add(menus[i].name, new List<MenuItemInfo>());
                    }
                }
            }
            

            Type[] menuDefinitions = csAssembly.GetTypesWithAttribute(typeof(MenuDefinitionAttribute)).ToArray();
            foreach(Type menuDef in menuDefinitions)
            {
                MethodInfo[] methods = menuDef.GetMethods(BindingFlags.Static | BindingFlags.Public);
                for (int i = 0; i < methods.Length; ++i)
                {
                    MethodInfo mi = methods[i];
                    MenuCommandAttribute cmd = (MenuCommandAttribute)mi.GetCustomAttributes(typeof(MenuCommandAttribute), true).FirstOrDefault();
                    if (string.IsNullOrEmpty(cmd.Path))
                    {
                        continue;
                    }

                    string[] pathParts = cmd.Path.Split('/');
                    if (pathParts.Length < 2)
                    {
                        continue;
                    }

                    string menuName = pathParts[0];

                    Menu menu;
                    if (!menuDictionary.ContainsKey(menuName))
                    {
                        m_menuButtonPrefab.gameObject.SetActive(false);
                        m_menuPrefab.gameObject.SetActive(false);

                        menu = Instantiate(m_menuPrefab, m_menuPanel.transform, false);
                        menu.Items = null;

                        MainMenuButton btn = Instantiate(m_menuButtonPrefab, m_topMenu.transform, false);
                        btn.name = menuName;
                        btn.Menu = menu;

                        Text txt = btn.GetComponentInChildren<Text>(true);
                        if (txt != null)
                        {
                            txt.text = menuName;
                        }

                        btn.gameObject.SetActive(true);

                        menuDictionary.Add(menuName, menu);
                        menuItemsDictionary.Add(menuName, new List<MenuItemInfo>());
                    }
                    else
                    {
                        menu = menuDictionary[menuName];
                    }

                    string path = string.Join("/", pathParts.Skip(1));
                    List<MenuItemInfo> menuItems = menuItemsDictionary[menuName];
                    MenuItemInfo menuItem = menuItems.Where(item => item.Path == path).FirstOrDefault();
                    if (menuItem == null)
                    {
                        menuItem = new MenuItemInfo();
                        menuItems.Add(menuItem);
                    }

                    menuItem.Path = string.Join("/", pathParts.Skip(1));
                    menuItem.Icon = !string.IsNullOrEmpty(cmd.IconPath) ? Resources.Load<Sprite>(cmd.IconPath) : null;
                    menuItem.Text = pathParts.Last();

                    if(cmd.Validate)
                    {
                        Func<bool> validate = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), mi, false);
                        if(validate == null)
                        {
                            Debug.LogWarning("method signature is invalid. bool Func() is expected. " + string.Join("/", pathParts));
                        }
                        else
                        {
                            menuItem.Validate = new MenuItemValidationEvent();
                            menuItem.Validate.AddListener(new UnityAction<MenuItemValidationArgs>(args => args.IsValid = validate()));
                        }
                    }
                    else
                    {
                        Action action = (Action)Delegate.CreateDelegate(typeof(Action), mi, false);
                        if (action == null)
                        {
                            Debug.LogWarning("method signature is invalid. void Action() is expected. " + string.Join("/", pathParts));
                        }
                        else
                        {
                            menuItem.Action = new MenuItemEvent();
                            menuItem.Action.AddListener(new UnityAction<string>(args => action()));
                        }
                    }

                    if(cmd.Hide)
                    {
                        menuItems.Remove(menuItem);
                    }
                }

                m_menuPrefab.gameObject.SetActive(wasMenuPrefabActive);
                m_menuButtonPrefab.gameObject.SetActive(wasButtonPrefabActive);
            }

            foreach(KeyValuePair<string, List<MenuItemInfo>> kvp in menuItemsDictionary)
            {
                menuDictionary[kvp.Key].SetMenuItems(kvp.Value.ToArray(), false);
            }
        }
    }
}

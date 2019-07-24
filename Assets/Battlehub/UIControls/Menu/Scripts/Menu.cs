using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Battlehub.UIControls.MenuControl
{
    [Serializable]
    public class MenuItemValidationArgs
    {
        public bool IsValid
        {
            get;
            set;
        }

        public string Command
        {
            get;
            private set;
        }

        public MenuItemValidationArgs(string command)
        {
            IsValid = true;
            Command = command;
        }
    }


    [Serializable]
    public class MenuItemValidationEvent : UnityEvent<MenuItemValidationArgs>
    {
    }

    [Serializable]
    public class MenuItemEvent : UnityEvent<string>
    {
    }


    [Serializable]
    public class MenuItemInfo
    {
        public string Path;
        public string Text;
        public Sprite Icon;

        public string Command;
        public MenuItemEvent Action;
        public MenuItemValidationEvent Validate;
    }


    public class Menu : MonoBehaviour
    {
        public event EventHandler Opened;
        public event EventHandler Closed;

        [SerializeField]
        private MenuItemInfo[] m_items = null;
        public MenuItemInfo[] Items
        {
            get { return m_items; }
            set
            {
                SetMenuItems(value, true);
            }
        }

        public void SetMenuItems(MenuItemInfo[] menuItems, bool databind = true)
        {
            m_items = menuItems;
            if(databind)
            {
                DataBind();
            }
        }

        [SerializeField]
        private MenuItem m_menuItemPrefab = null;

        [SerializeField]
        private RectTransform m_anchor = null;
        public RectTransform Anchor
        {
            get { return m_anchor; }
            set { m_anchor = value; }
        }

        [SerializeField]
        private Transform m_panel = null;

        private Transform m_root;

        private int m_depth;
        public int Depth
        {
            get { return m_depth; }
            set { m_depth = value; }
        }

        private MenuItem m_child;
        public MenuItem Child
        {
            get { return m_child; }
            set
            {
                if(m_child != null && m_child != value && m_child.Submenu != null)
                {
                    MenuItem oldChild = m_child;
                    m_child = value;
                    oldChild.Unselect();
                }
                else
                {
                    m_child = value;
                }

                if(m_child != null)
                {
                    m_child.Select(true);
                }
            }
        }

        private MenuItem m_parent;
        public MenuItem Parent
        {
            get { return m_parent; }
            set
            {
                m_parent = value;
            }
        }

        public int ActualItemsCount
        {
            get { return m_panel.childCount; }
        }

        [SerializeField]
        private CanvasGroup m_canvasGroup = null;

        [SerializeField]
        private float FadeInSpeed = 2;
     
        private void Awake()
        {
            if(m_panel == null)
            {
                m_panel = transform;
            }

            m_root = transform.parent;
            if(m_canvasGroup != null)
            {
                m_canvasGroup.alpha = 0;
            }
        }

        private void OnDestroy()
        {
            if(Closed != null)
            {
                Closed(this, EventArgs.Empty);
            }
        }

        private void DataBind()
        {
            Clear();

            Dictionary<string, MenuItemInfo> pathToItem = new Dictionary<string, MenuItemInfo>();
            Dictionary<string, List<MenuItemInfo>> pathToChildren = new Dictionary<string, List<MenuItemInfo>>();
            if (m_items != null)
            {
                for (int i = 0; i < m_items.Length; ++i)
                {
                    MenuItemInfo menuItemInfo = m_items[i];
                    if (string.IsNullOrEmpty(menuItemInfo.Path))
                    {
                        continue;
                    }

                    menuItemInfo.Path = menuItemInfo.Path.Replace("\\", "/");
                    string[] pathParts = menuItemInfo.Path.Split('/');
                    if (pathParts.Length == m_depth + 1)
                    {
                        if (string.IsNullOrEmpty(menuItemInfo.Text))
                        {
                            menuItemInfo.Text = pathParts[m_depth];
                        }
                        pathToItem[pathParts[m_depth]] = menuItemInfo;
                    }
                    else
                    {
                        string path = string.Join("/", pathParts, 0, m_depth + 1);
                        List<MenuItemInfo> childrenList;
                        if (!pathToChildren.TryGetValue(path, out childrenList))
                        {
                            childrenList = new List<MenuItemInfo>();
                            pathToChildren.Add(path, childrenList);
                        }

                        if (!pathToItem.ContainsKey(pathParts[m_depth]))
                        {
                            pathToItem[pathParts[m_depth]] = new MenuItemInfo
                            {
                                Text = pathParts[m_depth],
                                Path = path
                            };
                        }

                        if (string.IsNullOrEmpty(menuItemInfo.Text))
                        {
                            menuItemInfo.Text = pathParts[m_depth + 1];
                        }
                        childrenList.Add(menuItemInfo);
                    }
                }
            }

            foreach (MenuItemInfo menuItemInfo in pathToItem.Values)
            {
                MenuItem menuItem = Instantiate(m_menuItemPrefab, m_panel, false);
                menuItem.name = "MenuItem";
                menuItem.Depth = Depth + 1;
                menuItem.Root = m_root;

                List<MenuItemInfo> childrenList;
                if (pathToChildren.TryGetValue(menuItemInfo.Path, out childrenList))
                {
                    menuItem.Children = childrenList.ToArray();
                }

                menuItem.Item = menuItemInfo;
            }
        }

        private void Clear()
        {
            foreach (Transform child in m_panel)
            {
                Destroy(child.gameObject);
            }
            m_panel.DetachChildren();
        }

        public void Open()
        {
            gameObject.SetActive(true);

            RectTransform anchor = m_anchor;

            if (anchor != null)
            {
                Vector3[] corners = new Vector3[4];
                anchor.GetWorldCorners(corners);
                transform.position = corners[0];

                Vector3 lp = transform.localPosition;
                lp.z = 0;
                transform.localPosition = lp;
            }
            
            DataBind();
            
            if(m_anchor == null)
            {
                Fit();
            }   
         
            if(Opened != null)
            {
                Opened(this, EventArgs.Empty);
            }
        }

        public void Close()
        {
            Clear();
            gameObject.SetActive(false);

            if(Closed != null)
            {
                Closed(this, EventArgs.Empty);
            }
        }


        private void Fit()
        {
            RectTransform rootRT = (RectTransform)m_root;
            Vector3 position = rootRT.InverseTransformPoint(transform.position);

            Vector2 topLeft = -Vector2.Scale(rootRT.rect.size, rootRT.pivot);
            RectTransform rt = m_menuItemPrefab.GetComponent<RectTransform>();
            Vector2 size = new Vector2(rt.rect.width, rt.rect.height * m_panel.childCount);

            const float offset = 3;

            if (position.x + size.x - offset > topLeft.x + rootRT.rect.width)
            {
                position.x = position.x - size.x - offset;
            }
            else
            {
                position.x += offset;
            }

            if (position.y - size.y < topLeft.y)
            {
                position.y -= (position.y - size.y) - topLeft.y;
            }

            transform.position = rootRT.TransformPoint(position);

            Vector3 lp = transform.localPosition;
            lp.z = 0;
            transform.localPosition = lp;
        }


        private void LateUpdate()
        {
            if(m_canvasGroup != null && m_canvasGroup.alpha < 1)
            {
                m_canvasGroup.alpha += Time.deltaTime * FadeInSpeed;
            }

            if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                if(m_child == null)
                {
                    MenuItem parentMenuItem = m_parent;
                    while (parentMenuItem != null && !parentMenuItem.IsPointerOver)
                    {
                        Menu parentMenu = parentMenuItem.GetComponentInParent<Menu>();
                        if(parentMenu == null)
                        {
                            Destroy(gameObject);
                            return;
                        }

                        if(parentMenu.Child != null && parentMenu.Child.Submenu != this)
                        {
                            break;
                        }

                        parentMenuItem = parentMenu.m_parent;
                        if (parentMenuItem != null)
                        {
                            Destroy(parentMenu.gameObject);
                        }
                        else
                        {
                            parentMenu.Close();
                        }
                    }
                    

                    if(m_parent == null)
                    {
                        Close();
                    }
                    else
                    {
                        if (!m_parent.IsPointerOver)
                        {
                            Destroy(gameObject);
                        }
                    }
                }
            }
        }
    }
}

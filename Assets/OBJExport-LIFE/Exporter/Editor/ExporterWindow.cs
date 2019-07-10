//
// This file is part of a MASA library or program.
// Refer to the included end-user license agreement for restrictions.
//
// Copyright (c) 2014 MASA Group
//
// *****************************************************************************

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;

namespace mlv
{

    /// <summary>
    /// Cleans paths of invalid characters.
    /// </summary>
    public static class PathValidator
    {
        /// <summary>
        /// The set of invalid filename characters, kept sorted for fast binary search
        /// </summary>
        private readonly static char[] invalidFilenameChars;
        /// <summary>
        /// The set of invalid path characters, kept sorted for fast binary search
        /// </summary>
        private readonly static char[] invalidPathChars;

        static PathValidator()
        {
            // set up the two arrays -- sorted once for speed.
            invalidFilenameChars = System.IO.Path.GetInvalidFileNameChars();
            invalidPathChars = System.IO.Path.GetInvalidPathChars();
            Array.Sort(invalidFilenameChars);
            Array.Sort(invalidPathChars);

        }

        /// <summary>
        /// Cleans a filename of invalid characters
        /// </summary>
        /// <param name="input">the string to clean</param>
        /// <param name="errorChar">the character which replaces bad characters</param>
        /// <returns></returns>
        public static bool isValidFilename(string input )
        {
            return isValid(input, invalidFilenameChars);
        }

        private static bool isValid(string input, char[] invalidChars)
        {
            // null always sanitizes to null
            if (input == null) { return false; }
            foreach (var characterToTest in input)
            {
                // we binary search for the character in the invalid set. This should be lightning fast.
                if (Array.BinarySearch(invalidChars, characterToTest) >= 0)
                {
                    // we found the character in the array of
                    return false;
                }
            }

            // we're done.
            return true;
        }

    }

    public class ExporterWindow : EditorWindow
    {
        static Color noMesh = new Color( 255f/255f, 70f/255f, 0f/255f, 1f );
        static Color noStatic = Color.magenta;//new Color( 255f/255f, 70f/255f, 0f/255f, 1f );//new Color(11f / 255f, 47f / 255f, 142f / 255f, 1f);
        static Color noParent = Color.blue;//new Color(255f / 255f, 70f / 255f, 0f / 255f, 1f);
        static Color noExport = new Color(216f / 255f, 44f / 255f, 16f / 255f);
        static Color exportOk = Color.green;
        static void CopyArrayToSerialized( SerializedProperty dest, GameObject[] sources )
        {
            if( dest.arraySize != sources.Length )
            {
                dest.ClearArray();
                for( int i = 0; i < sources.Length; ++i )
                {
                    dest.InsertArrayElementAtIndex( i );
                }
            }
            for( int i = 0; i < sources.Length; ++i )
            {
                if( dest.GetArrayElementAtIndex( i ).objectReferenceValue != sources[i] )
                    dest.GetArrayElementAtIndex( i ).objectReferenceValue = sources[i];
            }
        }

        int exportNumber = 0;
        bool enableExport = true;
        bool enableGlobalExport = true;
        Vector2 scrollPos = Vector2.zero;
        bool displayInformation = false;
        GUIStyle boxStyle;

        [MenuItem ("Window/LIFE/Exporter")]
        static void Init()
        {
            ExporterWindow window = (ExporterWindow)EditorWindow.GetWindow (typeof (ExporterWindow), true, "MASA LIFE scene exporter");

            window.Show();
        }
        void OnEnable()
        {
            EditorApplication.update += ForceRepaint;
        }

        void OnDisable()
        {
            EditorApplication.update -= ForceRepaint;
        }

        void ForceRepaint()
        {
            Repaint();
        }

        void OnGUI()
        {
            Exporter exporter = FindObjectOfType< Exporter >();
            // Check if there is an exporter in the scene, if not, we create it and hide it
            if( exporter == null )
            {
                GameObject go = EditorUtility.CreateGameObjectWithHideFlags("Exporter", HideFlags.HideInInspector ^ HideFlags.HideInHierarchy);
                exporter = (Exporter) UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(go, @"Assets\LIFE\Exporter\Editor\ExporterWindow.cs (135,39)", "Exporter");
            }

            SerializedObject serializedExport = new SerializedObject( exporter );
            Color defaultColor = GUI.color;

            if (boxStyle == null)
            {
                boxStyle = new GUIStyle(GUI.skin.box);
                boxStyle.normal.textColor = Color.gray;
                boxStyle.richText = true;
            }

            // display MLV Logo centered
            GUILayout.BeginVertical();

            Texture2D texture = AssetDatabase.LoadAssetAtPath< Texture2D >("Assets\\LIFE\\Exporter\\life-icon.png");
            GUILayout.BeginHorizontal();

            // Hide or display information box
            if( GUILayout.Button (new GUIContent("About\nMASA LIFE", texture), GUILayout.Height(69), GUILayout.Width(125)) )
                displayInformation = !displayInformation;

            // Path for the expoted scen eand button to change it
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal ( GUI.skin.box ,GUILayout.ExpandWidth(false));
            GUILayout.Label ("Destination scene",GUILayout.ExpandWidth(false));
            SerializedProperty serPath = serializedExport.FindProperty("dotSceneExporterPath");
            if (serPath.stringValue == string.Empty || System.IO.Path.GetExtension(serPath.stringValue) == ".scene")
            {
                try
                {
                    serPath.stringValue = System.IO.Path.GetDirectoryName(EditorApplication.currentScene) + "/" + System.IO.Path.GetFileNameWithoutExtension(EditorApplication.currentScene) + ".mlscene";
                }
                catch (System.ArgumentException)
                {
                    serPath.stringValue = System.IO.Path.GetDirectoryName(Application.dataPath) + "/Untitled.mlscene";
                }

            }
            EditorGUILayout.SelectableLabel(serPath.stringValue, GUI.skin.textField, GUILayout.MaxHeight(16));
            if (GUILayout.Button ("...",GUILayout.ExpandWidth(false)))
            {
                string newFile = EditorUtility.SaveFilePanel( "Export to ..."
                                                             , Path.GetDirectoryName(serPath.stringValue)
                                                             , Path.GetFileName(serPath.stringValue)
                                                             , "mlscene" );
                if( newFile != string.Empty )
                {
                    serPath.stringValue = newFile;
                }
            }
            GUILayout.EndHorizontal();

            GUI.enabled = enableGlobalExport;

            // Export Button
            if( GUILayout.Button ("Export!", GUILayout.Height(40), GUILayout.ExpandWidth(true)) )
            {
                StreamWriter dotScene = exporter.CreateMLVScene();
                for( int i = 0; i < exporter.layers.Count; ++ i )
                {
                    if( exporter.layers[i].enabled )
                    {
                        ExportOneLayer( exporter, exporter.layers[i] );
                        exporter.AddLayerMLVScene( dotScene, exporter.layers[i].layerName, i == exporter.layers.Count-1 );
                    }
                }
                exporter.CloseMLVScene( dotScene );
                ShowNotification(new GUIContent("Scene exported!"));
            }
            enableGlobalExport = true;
            GUI.enabled = true;
            GUI.color = defaultColor;
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            // display information box + button redirecting to the website
            if ( displayInformation )
            {
                GUILayout.BeginHorizontal();
                GUILayout.Box(
                    "This editor exports scenes from Unity3D to the format of the <b>AI Creative Suite: MASA LIFE</b>.\n" +
                    "<b>MASA LIFE</b> enables you to create and execute a wide variety of intelligent, engaging and autonomous behaviors to drive actors in your game or simulation.\n" +
                    "Get further information and retrieve a free evaluation version on <b>www.masalife.net</b>.",
                    boxStyle,
                    GUILayout.Height(50),
                    GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if ( GUILayout.Button ("MASA LIFE website", GUILayout.Height(20), GUILayout.Width(200)) )
                {
                    Application.OpenURL("http://www.masalife.net/");
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            // Separator
            GUILayout.Box ("", GUILayout.ExpandWidth(true),GUILayout.Height(2));

            // Layers view
            scrollPos = EditorGUILayout.BeginScrollView (scrollPos);
            GUILayout.BeginHorizontal();
            if (exporter.layers.Count == 0)
            {
                exporter.layers.Add(new Exporter.LayerData());
                serializedExport.Update();
            }
            SerializedProperty serializedLayers = serializedExport.FindProperty( "layers" );
            for (int i = 0; i < serializedLayers.arraySize; ++i)
            {
                if( !LayerUI ( exporter, exporter.layers[i], serializedExport,serializedLayers.GetArrayElementAtIndex(i) ) )
                {
                    serializedLayers.DeleteArrayElementAtIndex( i );
                    break;
                }
            }
            // Add layer button
            if( GUILayout.Button ("+", GUILayout.ExpandWidth(false)) )
            {
                serializedLayers.InsertArrayElementAtIndex( serializedLayers.arraySize-1 );
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();

            serializedExport.ApplyModifiedProperties();
        }


        bool LayerUI( Exporter owner, Exporter.LayerData layerData, SerializedObject serializedExport, SerializedProperty serializedLayerData  )
        {
            Color defaultColor = GUI.color;
            Color defaultBackColor = GUI.backgroundColor;
            Color defaultContentColor = GUI.contentColor;

            GUILayout.BeginVertical(GUI.skin.box, GUILayout.MaxWidth(150), GUILayout.MinWidth(150),GUILayout.ExpandHeight(false));

            GUILayout.BeginHorizontal (GUI.skin.box);

            SerializedProperty serEnabled = serializedLayerData.FindPropertyRelative( "enabled" );
            serEnabled.boolValue = EditorGUILayout.Toggle ("", serEnabled.boolValue, GUILayout.Width(20));
            enableExport = true;
            // force unique name
            if( owner.ExistingName( layerData.layerName ) > 1 )
            {
                GUI.backgroundColor = noExport;
                enableExport = false;
                enableGlobalExport = false;
            }
            // valid file?
            if( layerData.layerName == string.Empty
               || layerData.layerName[ 0 ] == '.'
               || layerData.layerName[ 0 ] == ' '
               || layerData.layerName[ layerData.layerName.Length-1 ] == '.'
               || !PathValidator.isValidFilename( layerData.layerName ) )
            {
                GUI.backgroundColor = noExport;
                enableExport = false;
                enableGlobalExport = false;
            }

            SerializedProperty serLayerName = serializedLayerData.FindPropertyRelative( "layerName" );
            serLayerName.stringValue = EditorGUILayout.TextField (serLayerName.stringValue, GUILayout.ExpandWidth(true));

            GUI.backgroundColor = defaultBackColor;
            GUI.backgroundColor = noExport;
            if (GUILayout.Button("x", GUILayout.Width(24)))
            {
                GUI.backgroundColor = defaultBackColor;
                GUI.enabled = true;
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                return false;
            }
            GUI.backgroundColor = defaultBackColor;
            GUI.enabled = layerData.enabled;
            GUILayout.EndHorizontal();

            SerializedProperty serSelection = serializedLayerData.FindPropertyRelative( "selection" );

            // add all root objects ( ie all that have transform.parent == null )
            if( GUILayout.Button ( "Add root objects" ) )
            {
                Transform[] all = FindObjectsOfType< Transform >();
                List< GameObject > list = layerData.selection == null ? new List< GameObject >() : new List< GameObject >( layerData.selection );
                foreach( Transform trans in all )
                {
                    if( trans.parent == null && !list.Contains( trans.gameObject ) )
                        list.Add ( trans.gameObject );
                }
                CopyArrayToSerialized( serSelection, list.ToArray() );
            }
            // enable the next button if there is somthing selected
            GUI.enabled = serEnabled.boolValue && Selection.gameObjects.Length != 0;
            if( GUILayout.Button ( "Add selection" ) )
            {
                List< GameObject > list = layerData.selection == null ? new List< GameObject >() : new List< GameObject >( layerData.selection );
                foreach( GameObject go in Selection.gameObjects )
                {
                    if( !list.Contains( go ) )
                        list.Add ( go );
                }
                CopyArrayToSerialized( serSelection, list.ToArray() );
            }
            if (GUILayout.Button ("Remove selection"))
            {
                List< GameObject > list = layerData.selection == null ? new List< GameObject >() : new List< GameObject >( layerData.selection );
                foreach( GameObject go in Selection.gameObjects )
                {
                    if( list.Contains( go ) )
                        list.Remove ( go );
                }
                CopyArrayToSerialized( serSelection, list.ToArray() );
            }
            GUI.enabled = serEnabled.boolValue;
            bool clean = false;
            if (GUILayout.Button ("Remove unneeded"))
            {
                clean = true;
            }

            // configuration settings
            SerializedProperty serRecursive = serializedLayerData.FindPropertyRelative( "recursive" );
            SerializedProperty serStaticOnly = serializedLayerData.FindPropertyRelative( "staticOnly" );
            GUILayout.BeginVertical();
            serRecursive.boolValue = GUILayout.Toggle (serRecursive.boolValue, "Recursive export");
            serStaticOnly.boolValue = GUILayout.Toggle (serStaticOnly.boolValue, "Static only");
            GUILayout.EndVertical();


            GUI.contentColor = defaultContentColor;

            // display the selection
            List< GameObject > newList = new List< GameObject >();
            List< GameObject > allowedObjects = new List< GameObject >();

            GUI.enabled = true;
            for (int i = 0; i < serSelection.arraySize; ++i)
            {
                if (serSelection.GetArrayElementAtIndex(i).objectReferenceValue != null )
                    newList.Add( serSelection.GetArrayElementAtIndex(i).objectReferenceValue as GameObject);
            }

            Rect drop_area = DropAreaGUI();
            newList.Clear();

            GUI.enabled = enableExport && layerData.selection != null && layerData.selection.Length > 0;
            if (GUILayout.Button("Export to .obj"))
            {
                ExportOneLayer(owner, layerData);
                ShowNotification(new GUIContent("OBJ File exported!"));
            }

            GUI.enabled = true;
            GUI.contentColor = noMesh;
            GUILayout.BeginVertical("No mesh found", GUI.skin.window, GUILayout.MinWidth(300), GUILayout.ExpandHeight(false));
            GUI.contentColor = defaultBackColor;
            DisplayElements(serSelection, layerData, defaultContentColor, clean, newList, allowedObjects, noMesh);
            GUILayout.EndVertical();

            GUI.contentColor = noMesh;
            GUILayout.BeginVertical("No static found", GUI.skin.window, GUILayout.MinWidth(300), GUILayout.ExpandHeight(false));
            GUI.contentColor = defaultBackColor;
            DisplayElements(serSelection, layerData, defaultContentColor, clean, newList, allowedObjects, noStatic);
            GUILayout.EndVertical();

            GUI.contentColor = noMesh;
            GUILayout.BeginVertical("Parent included", GUI.skin.window, GUILayout.MinWidth(300), GUILayout.ExpandHeight(false));
            GUI.contentColor = defaultBackColor;
            DisplayElements(serSelection, layerData, defaultContentColor, clean, newList, allowedObjects, noParent);
            GUILayout.EndVertical();

            GUI.contentColor = defaultBackColor;
            GUILayout.BeginVertical("Exported", GUI.skin.window, GUILayout.MinWidth(300), GUILayout.ExpandHeight(false));
            DisplayElements(serSelection, layerData, defaultContentColor, clean, newList, allowedObjects, defaultContentColor);
            GUI.contentColor = defaultContentColor;
            GUILayout.EndVertical();

            DropAreaPerform(drop_area,serSelection, newList);

            // save any change to the exporter
            CopyArrayToSerialized(serSelection, newList.ToArray());
            SerializedProperty serAllowedObject = serializedLayerData.FindPropertyRelative("allowedObjects");
            CopyArrayToSerialized(serAllowedObject, allowedObjects.ToArray());

            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            return true;
        }

        void DisplayElements(SerializedProperty serSelection, Exporter.LayerData layerData, Color defaultContentColor, bool clean, List<GameObject> newList, List<GameObject> allowedObjects, Color match )
        {
            bool atLeastOne = false;
            if (serSelection.arraySize != 0)
            {
                for (int i = 0; i < serSelection.arraySize; ++i)
                {
                    SerializedProperty serSelected = serSelection.GetArrayElementAtIndex(i);
                    //GameObject selected = layerData.selection[i];
                    // the object might have been deleted
                    if (serSelected.objectReferenceValue != null)
                    {
                        bool toRemove = false;

                        GUI.contentColor = defaultContentColor;
                        // if in recursive mode and has a parent
                        if (layerData.recursive && ((GameObject)(serSelected.objectReferenceValue)).transform.parent)
                        {
                            // check that the parent is not already in the list
                            if (layerData.Contains(((GameObject)(serSelected.objectReferenceValue)).transform.parent.gameObject))
                            {
                                GUI.contentColor = noParent;
                                toRemove = true;
                            }
                        }
                        if (!toRemove)
                        {
                            bool hasTerrain = false;
                            Terrain ter = ((GameObject)(serSelected.objectReferenceValue)).GetComponent<Terrain>();
                            if (ter && ter.enabled && ter.gameObject.activeInHierarchy)
                                hasTerrain = true;
                            if (!hasTerrain && layerData.recursive)
                            {
                                Terrain[] ters = ((GameObject)(serSelected.objectReferenceValue)).GetComponentsInChildren<Terrain>();
                                foreach (Terrain subter in ters )
                                    if (subter.enabled && subter.gameObject.activeInHierarchy)
                                        hasTerrain = true;
                            }

                            // has a mesh component ?
                            bool hasMesh = false;
                            MeshRenderer mr = ((GameObject)(serSelected.objectReferenceValue)).GetComponent<MeshRenderer>();
                            if (mr && mr.enabled && mr.gameObject.activeInHierarchy )
                                hasMesh = true;
                            // or if recursive, has a mesh at least in children
                            if (!hasMesh && layerData.recursive)
                            {
                                MeshRenderer[] mrs =((GameObject)(serSelected.objectReferenceValue)).GetComponentsInChildren<MeshRenderer>();
                                foreach (MeshRenderer submr in mrs)
                                    if (submr.enabled && submr.gameObject.activeInHierarchy)
                                        hasMesh = true;
                            }
                            // no mesh found, nothing to export
                            if (!hasMesh && !hasTerrain )
                            {
                                GUI.contentColor = noMesh;
                                toRemove = true;
                            }
                            // check if the mesh belong to a static mesh
                            else if (layerData.staticOnly)
                            {
                                // if recursive export, check if the gameobject of at least one mesh is static
                                if (layerData.recursive)
                                {
                                    bool hasStatic = false;
                                    if (hasTerrain)
                                    {
                                        Terrain[] ters = ((GameObject)(serSelected.objectReferenceValue)).GetComponentsInChildren<Terrain>();
                                        foreach (Terrain terrain in ters)
                                        {
                                            if (terrain.gameObject.activeInHierarchy && terrain.gameObject.isStatic)
                                            {
                                                hasStatic = true;
                                                break;
                                            }
                                        }
                                    }
                                    if(hasMesh)
                                    {
                                        MeshRenderer[] renderers = ((GameObject)(serSelected.objectReferenceValue)).GetComponentsInChildren<MeshRenderer>();
                                        foreach (MeshRenderer render in renderers)
                                        {
                                            if (render.gameObject.activeInHierarchy && render.gameObject.isStatic)
                                            {
                                                hasStatic = true;
                                                break;
                                            }
                                        }
                                    }
                                    // no gameobject in the children ( or itself ) marked as static so nothing to export
                                    if (!hasStatic)
                                    {
                                        GUI.contentColor = noStatic;
                                        toRemove = true;
                                    }
                                }
                                else
                                {
                                    // just check that the current gameObject is static
                                    if (!((GameObject)(serSelected.objectReferenceValue)).isStatic)
                                    {
                                        GUI.contentColor = noStatic;
                                        toRemove = true;
                                    }
                                }
                            }
                        }

                        // allow a drag&drop of object
                        if (match == GUI.contentColor)
                        {
                            atLeastOne = true;
                            GUI.contentColor = defaultContentColor; ;
                            GameObject newObj = EditorGUILayout.ObjectField((GameObject)(serSelected.objectReferenceValue), typeof(GameObject), true) as GameObject;
                            if (!(clean && toRemove))
                                newList.Add(newObj);
                            // filtered for export
                            if (!toRemove)
                                allowedObjects.Add(newObj);
                        }
                        GUI.contentColor = defaultContentColor;
                    }
                }
            }
            if (!atLeastOne)
            {
                EditorGUILayout.LabelField("", GUILayout.ExpandHeight(false));
            }

        }
        public Rect DropAreaGUI( )
        {
            Rect drop_area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            GUI.Box(drop_area, "Drop GameObjects here",GUI.skin.textArea);
            return drop_area;
        }
        public void DropAreaPerform(Rect drop_area,SerializedProperty serSelection, List< GameObject > newList )
        {
            Event evt = Event.current;
            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!drop_area.Contains(evt.mousePosition))
                        return;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        foreach ( UnityEngine.Object dragged_object in DragAndDrop.objectReferences)
                        {
                            GameObject go = dragged_object as GameObject;
                            if (!newList.Contains(go))
                                newList.Add(go);
                        }
                        CopyArrayToSerialized(serSelection, newList.ToArray());
                    }
                    break;
            }
        }

        public void ExportObjects( Exporter exporter, StreamWriter tw, List< GameObject > allValidGameObjects, string path )
        {
            // iterate thru all meshes
            int vertexOffset = 0;
            for( int i = 0; i < allValidGameObjects.Count; ++i )
            {

                GameObject obj = allValidGameObjects[i];
                // might be long, so cancellable progress bar
                if( EditorUtility.DisplayCancelableProgressBar( "Exporting to " + path + " ...", obj.name, (float)(i+1)/(float)allValidGameObjects.Count ) )
                    break;

                // export the mesh data transformed

                MeshFilter mf = obj.GetComponent< MeshFilter >();
                if (mf)
                {
                    Mesh mesh = mf.sharedMesh;
                    vertexOffset += exporter.ExportObj(tw, mesh, obj.transform, obj.name, vertexOffset);
                }
                Terrain ter = obj.GetComponent< Terrain>();
                if (ter)
                {
                    vertexOffset += exporter.ExportTerrain(tw, ter, obj.transform, obj.name, vertexOffset);
                }
            }
            EditorUtility.ClearProgressBar();
        }

        public void ExportOneLayer( Exporter exporter, Exporter.LayerData layerData )
        {
            exportNumber = 0;
            // will hold all gameobject with a rendermesh component or terrain compoennt
            List< GameObject > allValidGameObjects = new List<GameObject>();

            for( int i = 0; i < layerData.allowedObjects.Length; ++i )
            {
                GameObject selected = layerData.allowedObjects[i];
                if (!selected.activeInHierarchy)
                    continue;

                // has a  MeshRenderer so ok  ( no need to check static or not as allowed is already filtered
                if (selected.GetComponent<MeshRenderer>() != null && selected.GetComponent<MeshFilter>() != null && selected.GetComponent<MeshRenderer>().enabled )
                    allValidGameObjects.Add( selected );
                if (selected.GetComponent<Terrain>() != null && selected.GetComponent<Terrain>() != null && selected.GetComponent<Terrain>().enabled)
                    allValidGameObjects.Add(selected);

                // if recursive
                if( layerData.recursive )
                {
                    // need to find out which gameObjects are ok
                    MeshRenderer[] renderers = selected.GetComponentsInChildren< MeshRenderer>();
                    foreach( MeshRenderer renderer in renderers )
                    {
                        // do not re add the parent
                        if( renderer.gameObject == selected )
                            continue;
                        if (!renderer.gameObject.activeInHierarchy)
                            continue;
                        if( !renderer.enabled )
                            continue;
                        // make sure it has a MeshFilter
                        if( renderer.GetComponent< MeshFilter >() )
                        {
                            // is it static or not and is it needed ?
                            if( !layerData.staticOnly || (layerData.staticOnly && renderer.gameObject.isStatic ) )
                            {
                                // just in case
                                if( !allValidGameObjects.Contains( renderer.gameObject ) )
                                    allValidGameObjects.Add ( renderer.gameObject );
                            }
                        }
                    }
                    Terrain[] terrains = selected.GetComponentsInChildren<Terrain>();
                    foreach (Terrain terrain in terrains)
                    {
                        // do not re add the parent
                        if (terrain.gameObject == selected)
                            continue;
                        if (!terrain.gameObject.activeInHierarchy)
                            continue;
                        if (!terrain.enabled)
                            continue;
                        // make sure it has a MeshFilter
                        // is it static or not and is it needed ?
                        if (!layerData.staticOnly || (layerData.staticOnly && terrain.gameObject.isStatic))
                        {
                            // just in case
                            if (!allValidGameObjects.Contains(terrain.gameObject))
                                allValidGameObjects.Add(terrain.gameObject);
                        }
                    }
                }
            }

            // create the file
            string path = Path.GetDirectoryName( exporter.dotSceneExporterPath ) + "/" + layerData.layerName + ".obj";
            StreamWriter tw = new StreamWriter( path );
            // write all objects merged
            ExportObjects( exporter, tw, allValidGameObjects, path );
            tw.Close();
        }
    }
}

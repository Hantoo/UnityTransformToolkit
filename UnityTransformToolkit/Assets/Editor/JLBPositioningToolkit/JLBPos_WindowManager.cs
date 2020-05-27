using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class JLBPos_WindowManager : EditorWindow
{

    private static JLBPos_WindowFunctions functionClass;

    private int ActiveInt, prevActiveInt = 0;
    
    string[] activeOption = {"Active", "Disabled"};
    //Curve Definition
    private JLBPos_WindowFunctions.curveType functionCurveType;
    private JLBPos_WindowFunctions.calculationOrder functioncalculationOrder;
    private JLBPos_WindowFunctions.toolNames functionNames;
    private JLBPos_WindowFunctions.spreadFunctions spreadFunction;
    private JLBPos_WindowFunctions.distrubtion distubitePattern;

    
    public static List<GameObject> selectionOrder = new List<GameObject>();
    public static List<GameObject> selectionOrder_background = new List<GameObject>();
    Tool LastTool = Tool.None;
    private int currentlySelectedIndex = 0;
    private GameObject movedObject;
    
    
    [MenuItem("Tools/JLB Pos Tools")]
    public static void ShowWindow()
    {
        EditorWindow window = EditorWindow.GetWindow <JLBPos_WindowManager>("JLB Pos Tools");
        window.minSize =new Vector2(323f, 150f);
        window.maxSize = new Vector2(324f, 800f);
    }
    
    void OnGUI()
    {
        ActiveInt = GUILayout.Toolbar(ActiveInt, activeOption);
        if (prevActiveInt != ActiveInt)
        {
            switch (ActiveInt)
            {
                case 0: //On
                    OnEnable();
                    break;

                case 1: //Off
                    OnDisable();
                    break;
            }

            prevActiveInt = ActiveInt;
        }

        if (GUILayout.Button("Select All Children In Hierarchy"))
        {
            getChildrenInHierarchy();
        }

        if (selectionOrder.Count > 0)
        {

            GUILayout.BeginHorizontal();

            if(GUILayout.Button("<<<", GUILayout.Width(60)))
            {
                currentlySelectedIndex = 0;
                Debug.Log("currentlySelectedIndex: "+ currentlySelectedIndex + ", selectionOrder: " + selectionOrder.Count + ", selectionOrder_background:" + selectionOrder_background.Count );
                selectionOrder.Clear();
                selectionOrder.Add(selectionOrder_background[currentlySelectedIndex]);
                RepaintSceneView();
            }
            if(GUILayout.Button("<", GUILayout.Width(60)))
            {
                currentlySelectedIndex = (currentlySelectedIndex - 1);
                Debug.Log("currentlySelectedIndex: "+ currentlySelectedIndex + ", selectionOrder: " + selectionOrder.Count + ", selectionOrder_background:" + selectionOrder_background.Count );
                if (currentlySelectedIndex == -1)
                {
                    currentlySelectedIndex = selectionOrder_background.Count - 1;
                    
                }
                Debug.Log("currentlySelectedIndex: "+ currentlySelectedIndex + ", selectionOrder: " + selectionOrder.Count + ", selectionOrder_background:" + selectionOrder_background.Count );
                selectionOrder.Clear();
                selectionOrder.Add(selectionOrder_background[currentlySelectedIndex]);
                RepaintSceneView();
            }
            if (GUILayout.Button("O", GUILayout.Width(60)))
            {
                currentlySelectedIndex = 0;
                Debug.Log("currentlySelectedIndex: "+ currentlySelectedIndex + ", selectionOrder: " + selectionOrder.Count + ", selectionOrder_background:" + selectionOrder_background.Count );
                selectionOrder.Clear();
                for (int i = 0; i < selectionOrder_background.Count; i++)
                {
                    selectionOrder.Add(selectionOrder_background[i]);
                }
                /*selectionOrder = selectionOrder_background;*/
                RepaintSceneView();
            }
            if(GUILayout.Button(">", GUILayout.Width(60)))
            {
                
                currentlySelectedIndex = (currentlySelectedIndex + 1) % selectionOrder_background.Count;
                Debug.Log("currentlySelectedIndex: "+ currentlySelectedIndex + ", selectionOrder: " + selectionOrder.Count + ", selectionOrder_background:" + selectionOrder_background.Count );
                selectionOrder.Clear();
                selectionOrder.Add(selectionOrder_background[currentlySelectedIndex]);
                
                RepaintSceneView();
            }
            if (GUILayout.Button(">>>", GUILayout.Width(60)))
            {
                currentlySelectedIndex = selectionOrder_background.Count - 1;
                Debug.Log("currentlySelectedIndex: "+ currentlySelectedIndex + ", selectionOrder: " + selectionOrder.Count + ", selectionOrder_background:" + selectionOrder_background.Count );
                selectionOrder.Clear();
                selectionOrder.Add(selectionOrder_background[currentlySelectedIndex]);
                RepaintSceneView();
            }

            GUILayout.EndHorizontal();

            functionNames = (JLBPos_WindowFunctions.toolNames) EditorGUILayout.EnumPopup("Tool", functionNames);

            switch (functionNames)
            {

                case JLBPos_WindowFunctions.toolNames.Spread:
                    spreadFunction =
                        (JLBPos_WindowFunctions.spreadFunctions) EditorGUILayout.EnumPopup("Spread Mode",
                            spreadFunction);
                    break;
                case JLBPos_WindowFunctions.toolNames.Distribute:
                    distubitePattern =
                        (JLBPos_WindowFunctions.distrubtion) EditorGUILayout.EnumPopup("Distribution Mode",
                            distubitePattern);
                    if (GUILayout.Button("Distribute Evenly"))
                    {
                        distrubte();
                    }

                    break;
                case JLBPos_WindowFunctions.toolNames.Move:
                    break;

            }

            GUILayout.Space(20);
            functioncalculationOrder =
                (JLBPos_WindowFunctions.calculationOrder) EditorGUILayout.EnumPopup("Offset Selection By ",
                    functioncalculationOrder);
            functionCurveType =
                (JLBPos_WindowFunctions.curveType) EditorGUILayout.EnumPopup("Curve", functionCurveType);

            // Window Code
        }
    }


    void RepaintSceneView()
    {
        EditorWindow view = EditorWindow.GetWindow<SceneView>();
        view.Repaint();
    }
    // Window has been selected
    void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
        LastTool = Tools.current;
        Tools.current = Tool.None;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        Tools.current = LastTool;
    }


    void getChildrenInHierarchy()
    {
        Transform[] Children = Selection.activeGameObject.GetComponentsInChildren<Transform>();
        GameObject[] ChildenGO = new GameObject[Children.Length];
        
        selectionOrder.Clear();
        selectionOrder_background.Clear();
        for(int i = 1; i < Children.Length; i++)
        {
            selectionOrder.Add(Children[i].gameObject);
            selectionOrder_background.Add(Children[i].gameObject);
            ChildenGO[i-1] = Children[i].gameObject;
            
        }
        Selection.objects = ChildenGO;
    }
    
    
    void OnSceneGUI(SceneView sv)
    {
        
        switch (functionNames)
        {
            case JLBPos_WindowFunctions.toolNames.Distribute:
                if (selectionOrder.Count > 0)
                {
                    Debug.DrawLine(selectionOrder[selectionOrder.Count - 1].transform.position,
                        selectionOrder[0].transform.position, Color.magenta);
                }

                break;
        }

        for (int i = 0; i < selectionOrder.Count; i++)
        {
           Vector3 movedObjPos =PositionHandle(
               selectionOrder[i].gameObject.transform.position,
               selectionOrder[i].gameObject.transform.rotation, selectionOrder[i].gameObject.name);
           if (movedObjPos != selectionOrder[i].gameObject.transform.position)
           {
               Vector3 axisChange = Vector3.zero;
               if (movedObjPos.x != selectionOrder[i].gameObject.transform.position.x) axisChange.x = 1;
               if (movedObjPos.y != selectionOrder[i].gameObject.transform.position.y) axisChange.y = 1;
               if (movedObjPos.z != selectionOrder[i].gameObject.transform.position.z) axisChange.z = 1;


               switch (functionNames)
               {
                   case JLBPos_WindowFunctions.toolNames.Move:
                       selectionOrder[i].transform.position = movedObjPos;
                       break;
                   case JLBPos_WindowFunctions.toolNames.Spread:
                       spreadObjects(selectionOrder[i].gameObject, movedObjPos, axisChange);
                       break;
                   
                   
               }
               
           }
             
        }

       
    }

    void distrubte()
    {
        Vector3 difference = selectionOrder[selectionOrder.Count-1].transform.position -
                             selectionOrder[0].transform.position;
        Vector3 distancePerStep = difference / (selectionOrder.Count-1);

        switch (distubitePattern)
        {
            case JLBPos_WindowFunctions.distrubtion.AllAxis:
                for (int i = 0; i < selectionOrder.Count; i++)
                {
                    selectionOrder[i].transform.position = selectionOrder[0].transform.position + (distancePerStep * i);
                }
                break;
            
            case JLBPos_WindowFunctions.distrubtion.XAxis:
                for (int i = 0; i < selectionOrder.Count; i++)
                {
                    selectionOrder[i].transform.position = new Vector3(selectionOrder[0].transform.position.x + (distancePerStep * i).x, 
                        selectionOrder[i].transform.position.y,
                        selectionOrder[i].transform.position.z);
                }
                break;
            
            case JLBPos_WindowFunctions.distrubtion.YAxis:
                for (int i = 0; i < selectionOrder.Count; i++)
                {
                    selectionOrder[i].transform.position = new Vector3(selectionOrder[i].transform.position.x, selectionOrder[0].transform.position.x + (distancePerStep * i).y,
                        selectionOrder[i].transform.position.z);
                }
                break;
            
            case JLBPos_WindowFunctions.distrubtion.ZAxis:
                for (int i = 0; i < selectionOrder.Count; i++)
                {
                    selectionOrder[i].transform.position = new Vector3(selectionOrder[i].transform.position.x, selectionOrder[i].transform.position.y,
                        selectionOrder[0].transform.position.x + (distancePerStep * i).z);
                }
                break;
                
            
        }

        
    }
    void spreadObjects(GameObject movedObj, Vector3 moveToPosition, Vector3 axisChange)
    {
        // Gives with the distance between where the object is currently located and where it wants to be
        Vector3 difference = moveToPosition - movedObj.transform.position;
        Vector3 distancePerStep = difference / selectionOrder.Count;
        
        int selectionHalfAmount = 0;
        
        if (selectionOrder.Count % 2 == 0) //Even
        { selectionHalfAmount = selectionOrder.Count / 2; }
        else //Odd 
        { selectionHalfAmount = (selectionOrder.Count - 1) / 2; }
        
        switch (spreadFunction)
        {
            case JLBPos_WindowFunctions.spreadFunctions.CornerL:
                for (int i = 0; i < selectionOrder.Count; i++)
                {
                    selectionOrder[i].transform.position = selectionOrder[i].transform.position + (distancePerStep*i);
                }
                break;
            
            case JLBPos_WindowFunctions.spreadFunctions.CornerR:
                for (int i = 0; i < selectionOrder.Count; i++)
                {
                    selectionOrder[i].transform.position = selectionOrder[i].transform.position + (distancePerStep*(selectionOrder.Count-i));
                }
                break;
            
            case JLBPos_WindowFunctions.spreadFunctions.Middle:
                distancePerStep = difference / selectionHalfAmount;
                for (int i = 0; i < selectionHalfAmount; i++)
                {
                    selectionOrder[i].transform.position = selectionOrder[i].transform.position + (distancePerStep*(i));
                    selectionOrder[(selectionOrder.Count-1)-i].transform.position = selectionOrder[(selectionOrder.Count-1)-i].transform.position + (distancePerStep*(i));
                    if (i == selectionHalfAmount-1)
                    {
                        if (selectionOrder.Count % 2 == 1)
                        {
                            selectionOrder[i + 1].transform.position =
                                selectionOrder[i + 1].transform.position + (distancePerStep * (i + 1));
                        }
                    }
                }
                break;
            
            case JLBPos_WindowFunctions.spreadFunctions.Edge:
                if (selectionOrder.Count % 2 == 1)
                {
                    selectionHalfAmount++;
                }

                distancePerStep = difference / selectionHalfAmount;
                for (int i = 0; i < selectionHalfAmount; i++)
                {
                    selectionOrder[i].transform.position = selectionOrder[i].transform.position + (distancePerStep*((selectionHalfAmount-1)-i));
                    selectionOrder[(selectionOrder.Count-1)-i].transform.position = selectionOrder[(selectionOrder.Count-1)-i].transform.position + (distancePerStep*((selectionHalfAmount-1)-i));
                    if (i == selectionHalfAmount-1)
                    {
                        if (selectionOrder.Count % 2 == 1)
                        {
                            selectionOrder[i + 1].transform.position =
                                selectionOrder[i + 1].transform.position + (distancePerStep * ((selectionHalfAmount-1)-i));
                        }
                    }
                }
                break;
        }
    }
   
    Vector3 PositionHandle ( Vector3 position, Quaternion rotation, string name )
    {
        // right Axis
        GUI.SetNextControlName ( name );
        Handles.color = new Color(1f, 0.45f, 0.47f, 0.75f);
        Vector3 newPos = Handles.Slider ( position, rotation*Vector3.right );

        // Up Axis
        GUI.SetNextControlName ( name );
        Handles.color = new Color(0.54f, 1f, 0.48f, 0.75f);
        newPos += Handles.Slider ( position, rotation*Vector3.up ) - position;

        // Forward Axis
        GUI.SetNextControlName ( name );
        Handles.color = new Color(0.36f, 0.81f, 1f, 0.75f);
        newPos += Handles.Slider ( position, rotation*Vector3.forward ) - position;

        float sizeMultiplier = 0.0f;

        if ( Camera.current.orthographic )
        {
            sizeMultiplier = Camera.current.orthographicSize * 2.5f;
        }
        else
        {
            Plane screen = new Plane ( Camera.current.transform.forward, Camera.current.transform.position );
            screen.Raycast ( new Ray ( position, Camera.current.transform.forward ), out sizeMultiplier );
        }

        GUI.SetNextControlName ( name );
        Handles.color = new Color ( 1.0f, 1.0f, 1.0f, 0.75f );
        newPos += Handles.FreeMoveHandle ( position, rotation, 0.02f * sizeMultiplier, Vector3.zero, Handles.RectangleCap ) - position;

        return newPos;
    }
    
   
    private void OnSelectionChange()
    {
        
        currentlySelectedIndex = 0;
        if (Selection.objects.Length <= 1)
        {
            selectionOrder.Clear();
            selectionOrder_background.Clear();
            currentlySelectedIndex = 0;
        }
        if (Selection.activeGameObject)
        {
            foreach (GameObject _object in Selection.gameObjects)
            {
                if (selectionOrder.IndexOf(_object) == -1)
                {
                    selectionOrder.Add(_object);
                    selectionOrder_background.Add(_object);
                }
            }
        }
    }
}

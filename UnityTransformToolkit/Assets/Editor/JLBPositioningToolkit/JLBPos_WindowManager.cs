using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class JLBPos_WindowManager : EditorWindow
{

    private static JLBPos_WindowFunctions functionClass = new JLBPos_WindowFunctions();

    private int ActiveInt, prevActiveInt = 0;
    
    string[] activeOption = {"Active", "Disabled"};
    //Curve Definition
    private JLBPos_WindowFunctions.curveType functionCurveType;
    private JLBPos_WindowFunctions.calculationOrder functioncalculationOrder;
    private JLBPos_WindowFunctions.toolNames functionNames;
    private JLBPos_WindowFunctions.spreadFunctions spreadFunction;
    private bool spreadEdgeMirror;
    private bool spreadEdgeAlterMiddleElements;
    private JLBPos_WindowFunctions.distrubtion distubitePattern;
    private JLBPos_WindowFunctions.objectAttrubutes attrubuteToAdjust;
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
            #region Selection
            
            GUILayout.BeginHorizontal();

            if(GUILayout.Button("<<<", GUILayout.Width(60)))
            {
                currentlySelectedIndex = 0;
                functionNames = JLBPos_WindowFunctions.toolNames.Move;
                selectionOrder.Clear();
                selectionOrder.Add(selectionOrder_background[currentlySelectedIndex]);
                RepaintSceneView();
            }
            if(GUILayout.Button("<", GUILayout.Width(60)))
            {
                currentlySelectedIndex = (currentlySelectedIndex - 1);
                functionNames = JLBPos_WindowFunctions.toolNames.Move;
                if (currentlySelectedIndex == -1)
                {
                    currentlySelectedIndex = selectionOrder_background.Count - 1;
                    
                }
                selectionOrder.Clear();
                selectionOrder.Add(selectionOrder_background[currentlySelectedIndex]);
                RepaintSceneView();
            }
            if (GUILayout.Button("O", GUILayout.Width(60)))
            {
                currentlySelectedIndex = 0;
                functionNames = JLBPos_WindowFunctions.toolNames.Move;
                selectionOrder.Clear();
                for (int i = 0; i < selectionOrder_background.Count; i++)
                {
                    selectionOrder.Add(selectionOrder_background[i]);
                }
                RepaintSceneView();
            }
            if(GUILayout.Button(">", GUILayout.Width(60)))
            {
                
                currentlySelectedIndex = (currentlySelectedIndex + 1) % selectionOrder_background.Count;
                functionNames = JLBPos_WindowFunctions.toolNames.Move;
                selectionOrder.Clear();
                selectionOrder.Add(selectionOrder_background[currentlySelectedIndex]);
                
                RepaintSceneView();
            }
            if (GUILayout.Button(">>>", GUILayout.Width(60)))
            {
                currentlySelectedIndex = selectionOrder_background.Count - 1;
                functionNames = JLBPos_WindowFunctions.toolNames.Move;
                selectionOrder.Clear();
                selectionOrder.Add(selectionOrder_background[currentlySelectedIndex]);
                RepaintSceneView();
            }

            GUILayout.EndHorizontal();
            
            #endregion

            functionClass.functionNames = (JLBPos_WindowFunctions.toolNames) EditorGUILayout.EnumPopup("Tool", functionClass.functionNames);
            
            functionClass.attrubuteToAdjust =
                (JLBPos_WindowFunctions.objectAttrubutes) EditorGUILayout.EnumPopup("Attribute To Adjust",
                    functionClass.attrubuteToAdjust);
        

            switch (functionClass.functionNames)
            {

                case JLBPos_WindowFunctions.toolNames.Spread:
                    functionClass.spreadFunction =
                        (JLBPos_WindowFunctions.spreadFunctions) EditorGUILayout.EnumPopup("Spread Mode",
                            functionClass.spreadFunction);
                    if (functionClass.spreadFunction == JLBPos_WindowFunctions.spreadFunctions.Edge)
                    {
                        
                        functionClass.spreadEdgeMirror = EditorGUILayout.Toggle("Edge Mirror", functionClass.spreadEdgeMirror);
                        functionClass.spreadEdgeAlterMiddleElements =
                            EditorGUILayout.Toggle("Spread Even Middle", functionClass.spreadEdgeAlterMiddleElements);
                    }
                    break;
                case JLBPos_WindowFunctions.toolNames.Distribute:
                    functionClass.distubitePattern =
                        (JLBPos_WindowFunctions.distrubtion) EditorGUILayout.EnumPopup("Distribution Mode",
                            functionClass.distubitePattern);
                    if (GUILayout.Button("Distribute Evenly"))
                    {
                        
                        switch (functionClass.attrubuteToAdjust)
                        {
                         case JLBPos_WindowFunctions.objectAttrubutes.Position:
                             functionClass.distrubtePosition(selectionOrder);
                             
                             break;
                         case JLBPos_WindowFunctions.objectAttrubutes.Rotation:
                             functionClass.distrubteRotation(selectionOrder);
                             break;
                        }
                        
                    }

                    break;
                case JLBPos_WindowFunctions.toolNames.Move:
                    break;

            }

            GUILayout.Space(20);
            functionClass.functioncalculationOrder =
                (JLBPos_WindowFunctions.calculationOrder) EditorGUILayout.EnumPopup("Offset Selection By ",
                    functionClass.functioncalculationOrder);
            functionClass.functionCurveType =
                (JLBPos_WindowFunctions.curveType) EditorGUILayout.EnumPopup("Curve", functionClass.functionCurveType);

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
        
        switch (functionClass.functionNames)
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
            switch (functionClass.attrubuteToAdjust)
            {
                case JLBPos_WindowFunctions.objectAttrubutes.Position:
                    Vector3 movedObjPos =PositionHandle(
                        selectionOrder[i].gameObject.transform.position,
                        selectionOrder[i].gameObject.transform.rotation, selectionOrder[i].gameObject.name);
                    if (movedObjPos != selectionOrder[i].gameObject.transform.position)
                    {
                        Vector3 axisChange = Vector3.zero;
                        if (movedObjPos.x != selectionOrder[i].gameObject.transform.position.x) axisChange.x = 1;
                        if (movedObjPos.y != selectionOrder[i].gameObject.transform.position.y) axisChange.y = 1;
                        if (movedObjPos.z != selectionOrder[i].gameObject.transform.position.z) axisChange.z = 1;


                        switch (functionClass.functionNames)
                        {
                            case JLBPos_WindowFunctions.toolNames.Move:
                                selectionOrder[i].transform.position = movedObjPos;
                                break;
                            case JLBPos_WindowFunctions.toolNames.Spread:
                                functionClass.spreadObjectsPosition(selectionOrder,selectionOrder[i].gameObject, movedObjPos);
                                break;
                   
                   
                        }
               
                    }

                    break;
                
                case JLBPos_WindowFunctions.objectAttrubutes.Rotation:
                    Quaternion Objectrotation = Handles.RotationHandle(selectionOrder[i].gameObject.transform.rotation,
                        selectionOrder[i].gameObject.transform.position);
                    if (Objectrotation != selectionOrder[i].transform.rotation)
                    {
                        switch (functionClass.functionNames)
                        {
                            case JLBPos_WindowFunctions.toolNames.Move:
                                selectionOrder[i].transform.rotation = Objectrotation;
                                break;
                            case JLBPos_WindowFunctions.toolNames.Spread:
                                functionClass.spreadObjectsRotation(selectionOrder[i].gameObject, Objectrotation, selectionOrder);
                                break;
                        }
                    }

                    break;
            }
            
             
        }

       
    }
    
    void OnInspectorUpdate()
    {
        Repaint();
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

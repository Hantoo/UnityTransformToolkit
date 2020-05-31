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

            functionNames = (JLBPos_WindowFunctions.toolNames) EditorGUILayout.EnumPopup("Tool", functionNames);
            
            attrubuteToAdjust =
                (JLBPos_WindowFunctions.objectAttrubutes) EditorGUILayout.EnumPopup("Attribute To Adjust",
                    attrubuteToAdjust);
        

            switch (functionNames)
            {

                case JLBPos_WindowFunctions.toolNames.Spread:
                    spreadFunction =
                        (JLBPos_WindowFunctions.spreadFunctions) EditorGUILayout.EnumPopup("Spread Mode",
                            spreadFunction);
                    if (spreadFunction == JLBPos_WindowFunctions.spreadFunctions.Edge)
                    {
                        
                        spreadEdgeMirror = EditorGUILayout.Toggle("Edge Mirror", spreadEdgeMirror);
                        spreadEdgeAlterMiddleElements =
                            EditorGUILayout.Toggle("Spread Even Middle", spreadEdgeAlterMiddleElements);
                    }
                    break;
                case JLBPos_WindowFunctions.toolNames.Distribute:
                    distubitePattern =
                        (JLBPos_WindowFunctions.distrubtion) EditorGUILayout.EnumPopup("Distribution Mode",
                            distubitePattern);
                    if (GUILayout.Button("Distribute Evenly"))
                    {
                        
                        switch (attrubuteToAdjust)
                        {
                         case JLBPos_WindowFunctions.objectAttrubutes.Position:
                             distrubtePosition();
                             
                             break;
                         case JLBPos_WindowFunctions.objectAttrubutes.Rotation:
                             /*if (distubitePattern != JLBPos_WindowFunctions.distrubtion.AllAxis)
                             {
                                 distubitePattern = JLBPos_WindowFunctions.distrubtion.AllAxis;
                                 Repaint();
                             }*/

                             distrubteRotation();
                             break;
                        }
                        
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
            switch (attrubuteToAdjust)
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


                        switch (functionNames)
                        {
                            case JLBPos_WindowFunctions.toolNames.Move:
                                selectionOrder[i].transform.position = movedObjPos;
                                break;
                            case JLBPos_WindowFunctions.toolNames.Spread:
                                spreadObjectsPosition(selectionOrder[i].gameObject, movedObjPos);
                                break;
                   
                   
                        }
               
                    }

                    break;
                
                case JLBPos_WindowFunctions.objectAttrubutes.Rotation:
                    Quaternion Objectrotation = Handles.RotationHandle(selectionOrder[i].gameObject.transform.rotation,
                        selectionOrder[i].gameObject.transform.position);
                    if (Objectrotation != selectionOrder[i].transform.rotation)
                    {
                        switch (functionNames)
                        {
                            case JLBPos_WindowFunctions.toolNames.Move:
                                selectionOrder[i].transform.rotation = Objectrotation;
                                break;
                            case JLBPos_WindowFunctions.toolNames.Spread:
                                spreadObjectsRotation(selectionOrder[i].gameObject, Objectrotation);
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

    void distrubtePosition()
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
    
    void distrubteRotation()
    {
        Vector3 Fromangle;
        Vector3 ToAngle;
        float change;
        Quaternion difference = Quaternion.Inverse(selectionOrder[0].transform.rotation) *
                                selectionOrder[selectionOrder.Count - 1].transform.rotation;
        
        switch (distubitePattern)
        {
            case JLBPos_WindowFunctions.distrubtion.AllAxis:
                Debug.Log("ALL AXIS");
                for (int i = 0; i < selectionOrder.Count; i++)
                {
                    selectionOrder[i].transform.rotation = selectionOrder[0].transform.rotation * Quaternion.Lerp(Quaternion.identity, difference, ((1f/(float)(selectionOrder.Count-1))*i)); 
                }
                break;
            
            
            case JLBPos_WindowFunctions.distrubtion.XAxis:
                Fromangle = selectionOrder[0].transform.rotation.eulerAngles;
                ToAngle = selectionOrder[selectionOrder.Count - 1].transform.rotation.eulerAngles;
                change = ToAngle.x - Fromangle.x;
                change = change / selectionOrder.Count;
                for (int i = 0; i < selectionOrder.Count-1; i++)
                {
                    selectionOrder[i].transform.rotation = Quaternion.Euler(new Vector3(selectionOrder[0].transform.rotation.eulerAngles.x + (change*i) , selectionOrder[i].transform.rotation.eulerAngles.y, selectionOrder[i].transform.rotation.eulerAngles.z));
                }
                break;
            case JLBPos_WindowFunctions.distrubtion.YAxis:
                Fromangle = selectionOrder[0].transform.rotation.eulerAngles;
                ToAngle = selectionOrder[selectionOrder.Count - 1].transform.rotation.eulerAngles;
                change = ToAngle.y - Fromangle.y;
                change = change / selectionOrder.Count;
                for (int i = 0; i < selectionOrder.Count-1; i++)
                {
                    selectionOrder[i].transform.rotation = Quaternion.Euler(new Vector3(selectionOrder[i].transform.rotation.eulerAngles.x, selectionOrder[0].transform.rotation.eulerAngles.y + (change*i) , selectionOrder[i].transform.rotation.eulerAngles.z));
                }
                break;
            case JLBPos_WindowFunctions.distrubtion.ZAxis:
                Fromangle = selectionOrder[0].transform.rotation.eulerAngles;
                ToAngle = selectionOrder[selectionOrder.Count - 1].transform.rotation.eulerAngles;
                change = ToAngle.z - Fromangle.z;
                change = change / selectionOrder.Count;
                for (int i = 0; i < selectionOrder.Count-1; i++)
                {
                    selectionOrder[i].transform.rotation = Quaternion.Euler(new Vector3(selectionOrder[i].transform.rotation.eulerAngles.x,  selectionOrder[i].transform.rotation.eulerAngles.y, selectionOrder[0].transform.rotation.eulerAngles.z + (change*i)));
                }
                break;
           }
    }
    void spreadObjectsPosition(GameObject movedObj, Vector3 moveToPosition)
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
                if (spreadEdgeAlterMiddleElements)
                {
                    if (selectionOrder.Count % 2 == 0)
                    {
                        selectionHalfAmount++;
                    }
                }

                distancePerStep = difference / selectionHalfAmount;
                for (int i = 0; i < selectionHalfAmount; i++)
                {
                    selectionOrder[i].transform.position = selectionOrder[i].transform.position + (distancePerStep*((selectionHalfAmount-1)-i));
                    if (spreadEdgeMirror)
                    {
                        selectionOrder[(selectionOrder.Count - 1) - i].transform.position =
                            selectionOrder[(selectionOrder.Count - 1) - i].transform.position +
                            (distancePerStep * ((selectionHalfAmount - 1) - i));
                    }
                    else
                    {
                        selectionOrder[(selectionOrder.Count - 1) - i].transform.position =
                            selectionOrder[(selectionOrder.Count - 1) - i].transform.position +
                            (-distancePerStep * ((selectionHalfAmount - 1) - i));
                    }

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
   
    
    void spreadObjectsRotation(GameObject movedObj, Quaternion moveToRotation)
    {
        Quaternion difference = Quaternion.Inverse(movedObj.transform.rotation) * moveToRotation;
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
                    selectionOrder[i].transform.rotation = selectionOrder[i].transform.rotation * Quaternion.Lerp(Quaternion.identity, difference, ((1f/(float)selectionOrder.Count)*i)); 
                } 
                break;
            
            case JLBPos_WindowFunctions.spreadFunctions.CornerR:
                for (int i = 0; i < selectionOrder.Count; i++)
                    {
                        selectionOrder[i].transform.rotation = selectionOrder[i].transform.rotation * Quaternion.Lerp(Quaternion.identity, difference, 1f-((1f/(float)selectionOrder.Count)+((1f/(float)selectionOrder.Count)*i))); 
                    }
                break;
            
            case JLBPos_WindowFunctions.spreadFunctions.Middle:
                for (int i = 0; i < selectionHalfAmount; i++)
                {
                    selectionOrder[i].transform.rotation = selectionOrder[i].transform.rotation * Quaternion.Lerp(Quaternion.identity, difference, ((1f/(float)selectionHalfAmount)*i));
                    selectionOrder[(selectionOrder.Count-1)-i].transform.rotation = selectionOrder[(selectionOrder.Count-1)-i].transform.rotation * Quaternion.Lerp(Quaternion.identity, difference, (1f/(float)selectionHalfAmount)*i) ; 
                    if (i == selectionHalfAmount-1)
                    {
                        if (selectionOrder.Count % 2 == 1)
                        {
                            selectionOrder[i + 1].transform.rotation =
                                selectionOrder[i + 1].transform.rotation * Quaternion.Lerp(Quaternion.identity, difference, ((1f/(float)selectionOrder.Count)*i+1));
                        }
                    }
                }
                break;
            
            case JLBPos_WindowFunctions.spreadFunctions.Edge:
                if (!spreadEdgeAlterMiddleElements)
                {
                    if (selectionOrder.Count % 2 == 0)
                    {
                        selectionHalfAmount--;
                    }
                }
                for (int i = 0; i < selectionHalfAmount; i++)
                {
                    selectionOrder[i].transform.rotation = selectionOrder[i].transform.rotation * Quaternion.Lerp(Quaternion.identity, difference, ((1f/(float)selectionHalfAmount)*((selectionHalfAmount)-i)));
                    if (!spreadEdgeMirror)
                    {
                        Quaternion AmountToRotate = Quaternion.Lerp(Quaternion.identity, difference,
                            ((1f / (float) selectionHalfAmount) * ((selectionHalfAmount) - i)));
                        selectionOrder[(selectionOrder.Count - 1) - i].transform.rotation =
                            selectionOrder[(selectionOrder.Count - 1) - i].transform.rotation *
                            (Quaternion.Inverse(AmountToRotate));

                    }
                    else
                    {
                        selectionOrder[(selectionOrder.Count - 1) - i].transform.rotation =
                            selectionOrder[(selectionOrder.Count - 1) - i].transform.rotation *
                            Quaternion.Lerp(Quaternion.identity, difference,
                                ((1f / (float) selectionHalfAmount) * ((selectionHalfAmount) - i)));
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

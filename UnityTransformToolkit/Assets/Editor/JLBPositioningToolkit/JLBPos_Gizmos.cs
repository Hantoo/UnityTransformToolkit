using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[InitializeOnLoad(), ExecuteAlways()]
public class JLBPos_Gizmos
{





    [DrawGizmo(GizmoType.Pickable | GizmoType.Selected | GizmoType.NonSelected | GizmoType.Active)]
    public static void OnDrawSceneGizmo(Transform transform, GizmoType gizmoType)
    {

        Vector3 HandlePosition = Vector3.zero;

        /*if (Selection.gameObjects.Length > 0 ) //&& Selection.activeGameObject.GetComponent<Waypoint>())
        {*/

        for (int i = 0; i < JLBPos_WindowManager.selectionOrder.Count; i++)
        {
            
            Handles.Label(JLBPos_WindowManager.selectionOrder[i].gameObject.transform.position, "#" + i);
            
        }

    }
    
    

}

#endif
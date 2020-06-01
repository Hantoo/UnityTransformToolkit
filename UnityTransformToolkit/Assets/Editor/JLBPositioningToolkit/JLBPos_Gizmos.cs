using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[InitializeOnLoad(), ExecuteAlways()]
public class JLBPos_Gizmos
{

    public static JLBPos_WindowFunctions.toolNames gizmoTools;
    


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

        switch (gizmoTools)
        {
                
            case JLBPos_WindowFunctions.toolNames.CircularSpread:
                Handles.color = new Color(1,1,1,0.1f);
                Gizmos.color = new Color(1,1,0, 0.2f);
                Handles.DrawSolidDisc(JLBPos_WindowManager.LastSelectedVector, Vector3.up, JLBPos_WindowManager.CircularDiscSize);
                /*JLBPos_WindowManager.LastSelectedVector = Handles.PositionHandle(JLBPos_WindowManager.LastSelectedVector, Quaternion.identity);*/
                float theta = 0f;
                float deltaTheta = (2f * Mathf.PI) / JLBPos_WindowManager.selectionOrder.Count;
                
                for (int i = 0; i < JLBPos_WindowManager.selectionOrder.Count; i++)
                {
                    
                    /*Vector3 Pos = JLBPos_WindowManager.LastSelectedVector +
                                  new Vector3(JLBPos_WindowManager.CircularDiscSize, 0, 0);*/
                    Vector3 Pos = JLBPos_WindowManager.LastSelectedVector +
                                  
                                  new Vector3(JLBPos_WindowManager.CircularDiscSize * Mathf.Cos (theta), 0, JLBPos_WindowManager.CircularDiscSize * Mathf.Sin (theta));
                    Gizmos.DrawSphere(Pos, 0.3f);
                    theta += deltaTheta;
                }
                break;
        }
        
    }
    
    

}

#endif
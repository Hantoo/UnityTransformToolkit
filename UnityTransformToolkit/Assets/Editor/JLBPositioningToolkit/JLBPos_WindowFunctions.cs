using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



[InitializeOnLoad(), CanEditMultipleObjects, ]
public class JLBPos_WindowFunctions 
{
    public enum curveType
    {
        Linear,
        EaseInOut,
        SCurve,
        SnapFithyFithy,
    }
    public enum calculationOrder
    {
        SelectionOrder,
        WorldSpace,
    }

    public enum spreadFunctions
    {
        CornerL,
        CornerR,
        Middle,
        Edge,
    }
    
    public enum toolNames
    {
        Move,
        Spread,
        Distribute,
    }
    
    public enum distrubtion
    {
        XAxis,
        YAxis,
        ZAxis,
        AllAxis
    }


    //Selection



    public static void MoveAllToPosition(List<GameObject> list, Vector3 Position)
    {
        foreach (var gmobject in list)
        {
            gmobject.transform.position = Position;
        }
    }

    public void pullFromLastSelection()
    {
        
    }
    
    
    




}



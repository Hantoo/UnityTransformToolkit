using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad(), CanEditMultipleObjects, ]
public class JLBPos_WindowFunctions 
{
    #region Variables

    #region Enums

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

    public enum objectAttrubutes
    {
        Position,
        Rotation,
        //Scale,
    }

    #endregion

    #region GetSet

    public curveType functionCurveType
    {
        get { return m_functionCurveType; }
        set { m_functionCurveType = value; }
    }
    public calculationOrder functioncalculationOrder
    {
        get { return m_functioncalculationOrder; }
        set { m_functioncalculationOrder = value; }
    }
    public toolNames functionNames
    {
        get { return m_functionNames; }
        set { m_functionNames = value; }
    }
    public spreadFunctions spreadFunction
    {
        get { return m_spreadFunction; }
        set { m_spreadFunction = value; }
    }
    public distrubtion distubitePattern
    {
        get { return m_distubitePattern; }
        set { m_distubitePattern = value; }
    }
    public objectAttrubutes attrubuteToAdjust
    {
        get { return m_attrubuteToAdjust; }
        set { m_attrubuteToAdjust = value; }
    }
    public bool spreadEdgeMirror
    {
        get { return m_spreadEdgeMirror; }
        set { m_spreadEdgeMirror = value; }
    }
    public bool spreadEdgeAlterMiddleElements
    {
        get { return m_spreadEdgeAlterMiddleElements; }
        set { m_spreadEdgeAlterMiddleElements = value; }
    }
    #endregion

    #region  privateVariables

    

    
    private curveType m_functionCurveType;
    private calculationOrder m_functioncalculationOrder;
    private toolNames m_functionNames;
    private spreadFunctions m_spreadFunction;
    private bool m_spreadEdgeMirror;
    private bool m_spreadEdgeAlterMiddleElements;
    private distrubtion m_distubitePattern;
    private objectAttrubutes m_attrubuteToAdjust;
    #endregion
    #endregion
    
    #region Spread Functions

   public void spreadObjectsPosition(List<GameObject> selectionOrder, GameObject movedObj, Vector3 moveToPosition)
    {
        createUndoHistory("Spread (Pos)", selectionOrder);
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
            case spreadFunctions.CornerL:
                for (int i = 0; i < selectionOrder.Count; i++)
                {
                    
                    selectionOrder[i].transform.position = selectionOrder[i].transform.position + (distancePerStep*i);
                }
                break;
            
            case spreadFunctions.CornerR:
                for (int i = 0; i < selectionOrder.Count; i++)
                {
                    selectionOrder[i].transform.position = selectionOrder[i].transform.position + (distancePerStep*((selectionOrder.Count-1)-i));
                }
                break;
            
            case spreadFunctions.Middle:
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
            
            case spreadFunctions.Edge:
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
   
   
       public void spreadObjectsRotation(GameObject movedObj, Quaternion moveToRotation, List<GameObject> selectionOrder)
    {
        createUndoHistory("Spread (Rot)", selectionOrder);
        Quaternion difference = Quaternion.Inverse(movedObj.transform.rotation) * moveToRotation;
        int selectionHalfAmount = 0;
        if (selectionOrder.Count % 2 == 0) //Even
        { selectionHalfAmount = selectionOrder.Count / 2; }
        else //Odd 
        { selectionHalfAmount = (selectionOrder.Count - 1) / 2; }
        
        switch (spreadFunction)
        {
            case spreadFunctions.CornerL:
                for (int i = 0; i < selectionOrder.Count; i++)
                {
                    selectionOrder[i].transform.rotation = selectionOrder[i].transform.rotation * Quaternion.Lerp(Quaternion.identity, difference, ((1f/(float)selectionOrder.Count)*i)); 
                } 
                break;
            
            case spreadFunctions.CornerR:
                for (int i = 0; i < selectionOrder.Count; i++)
                    {
                        selectionOrder[i].transform.rotation = selectionOrder[i].transform.rotation * Quaternion.Lerp(Quaternion.identity, difference, 1f-((1f/(float)selectionOrder.Count)+((1f/(float)selectionOrder.Count)*i))); 
                    }
                break;
            
            case spreadFunctions.Middle:
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
            
            case spreadFunctions.Edge:
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
       
    #endregion

    #region Distribute Functions

     public void distrubtePosition(List<GameObject> selectionOrder)
    {
        createUndoHistory("Distribute (Pos)", selectionOrder);
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
    
    public void distrubteRotation(List<GameObject> selectionOrder)
    {
        createUndoHistory("Distribute (Rot)", selectionOrder);
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
    
    #endregion
    
    #region Other Functions

    void createUndoHistory(string functionTitle, List<GameObject> _selectionOrder)
    {
        for (int i = 0; i < _selectionOrder.Count; i++)
        {
            Undo.RecordObject(_selectionOrder[i].transform, "JLBPos - " + functionTitle);
        }
    }

    #endregion

}



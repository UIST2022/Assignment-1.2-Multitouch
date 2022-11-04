
using UnityEngine;
using System.Collections;
using TUIO;
using System.Collections.Generic;
using Assets;

public partial class TouchReceiver : MonoBehaviour
{

    [SerializeField]
    Mesh debugMesh;
    [SerializeField]
    Material startMat;
    [SerializeField]
    Material currentMat;
    [SerializeField]
    GestureState currentState = GestureState.None;
    public TouchPoint FirstTouchPoint { get; set; }
    public TouchPoint SecondTouchPoint { get; set; }
    public float initialDistance { get; set; }

    private Vector3 initialScale = Vector3.zero;
    private Vector3 firstVecRot  = Vector3.zero;

    void Start()
    {
        Vector3 initialScale = this.transform.localScale;
    }

    void Update()
    {
        DrawDebugMeshes();
        HandleTouchInput();
    }

    void HandleTouchInput()
    {
        //TODO
        //Simple example:
        if (FirstTouchPoint != null)
        {
            this.transform.position = FirstTouchPoint.fromTUIO();
        }
    }

    /// <summary>
    /// Processes Tuio-Events
    /// </summary>
    /// <param name="events"></param>
	void processEvents(ArrayList events)
    {
        foreach (BBCursorEvent cursorEvent in events)
        {
            TuioCursor myCursor = cursorEvent.cursor;
            if (myCursor.getCursorID() == 0)
            {
                FirstTouchPoint = new TouchPoint(myCursor);
                //Restarting if the first touch was let go off
                initialDistance = 0f;
                initialScale = this.transform.localScale;

            }
            if (myCursor.getCursorID() == 1)
            {
                SecondTouchPoint = new TouchPoint(myCursor);
                Vector3 firstVector = new Vector3(FirstTouchPoint.X, FirstTouchPoint.Y);
                Vector3 secondVector = new Vector3(SecondTouchPoint.X, SecondTouchPoint.Y);

                if (initialDistance == 0f) {
                    initialDistance = Vector3.Distance(secondVector, firstVector);
                    firstVecRot = FirstTouchPoint.fromTUIO()-SecondTouchPoint.fromTUIO();
                } else {
                    Vector3 secondVecRot = FirstTouchPoint.fromTUIO()-SecondTouchPoint.fromTUIO();
                    transform.rotation = Quaternion.FromToRotation(firstVecRot, secondVecRot);
                    
                    float distance = Vector3.Distance(secondVector, firstVector);
                    float scale = distance/initialDistance;
                    scale = Mathf.Clamp(scale, 0.1f, 10f);
                    this.transform.localScale = initialScale * scale;
                }
                
            }
        }
    }

    void DrawDebugMeshes()
    {
        DrawDebugMeshForTouchPoint(FirstTouchPoint);
        DrawDebugMeshForTouchPoint(SecondTouchPoint);

    }

    private void DrawDebugMeshForTouchPoint(TouchPoint touchPoint)
    {
        if (touchPoint != null && touchPoint.Active)
        {
            DrawDebugMeshFor(touchPoint.fromTUIO());
        }
    }

    private void DrawDebugMeshFor(Vector3 touchPointVector)
    {
        Graphics.DrawMesh(debugMesh,
                            Matrix4x4.TRS(touchPointVector, Quaternion.identity,
                                new Vector3(0.05f, 0.05f, 0.05f)), currentMat, 0);
    }
}

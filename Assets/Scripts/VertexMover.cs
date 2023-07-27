using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VertexMover : MonoBehaviour
{
    public int selfVertex;
    CollectAllPoints collectAllPointsScript;

    private void Start()
    {
        collectAllPointsScript = GameObject.Find("test").GetComponent<CollectAllPoints>();
    }

    private void OnMouseDown()
    {
        Debug.Log("clicked on me");
    }

    private void OnMouseDrag()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 mousePositionWorldCoords = Camera.main.ScreenToWorldPoint(mousePosition);
        mousePositionWorldCoords.z = 0f;
        transform.position = mousePositionWorldCoords;
        collectAllPointsScript.UpdateEdgeOnVertexMove(selfVertex);
    }


}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utils;

public class Pair
{
    public Vector3 sourcePos;
    public Vector3 neighbourPos;
    public int vertex;
    public float weight;

    public Pair(Vector3 sourcePos, Vector3 neighbourPos, int vertex, float weight)
    {
        this.sourcePos = sourcePos;
        this.neighbourPos = neighbourPos;
        this.vertex = vertex;
        this.weight = weight;
    }
}

public class CollectAllPoints : MonoBehaviour
{
    [SerializeField] GameObject vertexToInstantiate;

    [SerializeField] LineRenderer lrPrefab;
    [SerializeField] Transform lineRendererParent;

    [SerializeField] UIInteraction uiInteractionScript;

    // you cant serialize a list<list<T>>
    List<List<Pair>> graph;
    List<GameObject> verticesList = new List<GameObject>();

    List<LineRenderer> finalLR;

    private float maxSize = 0;

    List<Vector2> inputPost = new List<Vector2>()
    {
        new Vector2(0f, 0f),
        new Vector2(2f, 2f),
        new Vector2(3f, 10f),
        new Vector2(5, 2f),
        new Vector2(7f, 0f),
        new Vector2(5f, 5f)
    };



    // Start is called before the first frame update
    void Start()
    {
        graph = new List<List<Pair>>();
        finalLR = new List<LineRenderer>();

        CreateVertexFromInputList();

        AddVerticesToGraph();

        AddEdgesToGraph();
        //InstantiateLineRenderers();
        Prims();

        uiInteractionScript.onVerticeButtonClicked += AddVertexCalledFromOtherScript;

        Camera.main.orthographicSize = maxSize + 2;
    }

    // Update is called once per frame
    void Update()
    {
        Prims();
    }


    void CreateVertexFromInputList()
    {
        maxSize = Int32.MinValue;
        int index = 0;

        foreach (Vector2 v in inputPost)
        {
            maxSize = Mathf.Max(maxSize, Mathf.Max(v.x, v.y));
            GameObject vertex = Instantiate(vertexToInstantiate, new Vector3(v.x, v.y, 0f), Quaternion.identity);
            vertex.GetComponent<VertexMover>().selfVertex = index;
            vertex.GetComponentInChildren<TextMeshPro>().text = index.ToString();
            //vertexnameDisplayTMP.name = index.ToString();
            vertex.transform.SetParent(transform);

            verticesList.Add(vertex);
            index++;
        }
    }

    void AddVertexCalledFromOtherScript(object sender, EventArgs eventArgs)
    {
        GameObject vertex = Instantiate(vertexToInstantiate);
        vertex.GetComponent<VertexMover>().selfVertex = verticesList.Count;
        vertex.GetComponentInChildren<TextMeshPro>().text = verticesList.Count.ToString();
        verticesList.Add(vertex);

        AddVerticesToGraph();
        AddEdgesToGraph();
    }

    void Prims()
    {
        foreach (Transform t in lineRendererParent)
        {
            Destroy(t.gameObject);
        }

        float minWeight = 0;

        List<int> visited = new List<int>(new int[verticesList.Count]);

        PriorityQueue<Pair, float> priorityQueue = new PriorityQueue<Pair, float>();
        priorityQueue.Enqueue(new Pair(graph[0][0].sourcePos, graph[0][0].sourcePos, 0, 0), 0);

        int index = 0;

        while (priorityQueue.Count > 0)
        {
            Pair removed = priorityQueue.Dequeue();

            if (visited[removed.vertex] == 1)
            {
                continue;
            }

            visited[removed.vertex] = 1;
            index++;
            LineRenderer lr = Instantiate(lrPrefab);
            lr.SetPosition(0, removed.sourcePos);
            lr.SetPosition(1, removed.neighbourPos);

            finalLR.Clear();
            finalLR.Add(lr);

            
            lr.transform.SetParent(lineRendererParent);

            minWeight += removed.weight;

            foreach (Pair p in graph[removed.vertex])
            {
                if (visited[p.vertex] == 0)
                {
                    priorityQueue.Enqueue(p, p.weight);
                }
            }
        }
    }

    private void AddVerticesToGraph()
    {
        graph.Clear();
        for (int i = 0; i < verticesList.Count; i++) { 
            graph.Add(new List<Pair>());
        }
    }

    private float CalWeight(Vector2 a, Vector2 b)
    {
        return Mathf.Abs(a.y - b.y) + Mathf.Abs(a.x - b.x);
    }

    void InstantiateLineRenderers()
    {
        for (int i = 0; i < graph.Count; i++)
        {
            for (int j = 0; j < graph[i].Count; j++)
            {
                LineRenderer lr = Instantiate(lrPrefab);
                lr.SetPosition(0, graph[i][j].sourcePos);
                lr.SetPosition(1, graph[i][j].neighbourPos);
            }
        }
    }

    private void AddEdgesToGraph()
    {
        /*for (int i = 0; i < inputPost.Count; i++)
        {
            for (int j = 0; j < inputPost.Count; j++)
            {
                if (i != j)
                {
                    graph[i].Add(new Pair(inputPost[i], inputPost[j], j, CalWeight(inputPost[i], inputPost[j])));
                }
            }
        }*/

        for (int i = 0; i < graph.Count; i++)
        {
            graph[i].Clear();
        }

        // use the verticesList to create graph so that its easier to add vertices on run time.
        for (int i = 0; i < verticesList.Count; i++)
        {
            for (int j = 0; j < verticesList.Count; j++)
            {
                if (i != j)
                {
                    graph[i].Add(new Pair(verticesList[i].transform.position, verticesList[j].transform.position, j, CalWeight(verticesList[i].transform.position, verticesList[j].transform.position)));
                }
            }
        }
        //Debug.Log("added edges to the graph");
    }

    public void UpdateEdgeOnVertexMove(int vertexChanged)
    {
        foreach (Pair p in graph[vertexChanged])
        {
            p.sourcePos = verticesList[vertexChanged].transform.position;
            p.weight = CalWeight(p.sourcePos, p.neighbourPos);
        }

        for (int i = 0; i < verticesList.Count; i++)
        {
            if (i != vertexChanged)
            {
                for (int j = 0; j < verticesList.Count - 1; j++)
                {
                    if (graph[i][j].vertex == vertexChanged)
                    {
                        graph[i][j].neighbourPos = verticesList[vertexChanged].transform.position;
                        graph[i][j].weight = CalWeight(graph[i][j].sourcePos, graph[i][j].neighbourPos);
                    }
                }
            }
        }
    }
}
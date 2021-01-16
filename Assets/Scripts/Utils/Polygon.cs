using System.Collections.Generic;
using UnityEngine;

public class Polygon : MonoBehaviour
{
    [SerializeField] private float _subVertexSpawnInterval = 0.1f;
    
    [SerializeField] private LayerMask _layerMask = default;

    [SerializeField] private PolygonVisualizer _polygonVisualizer = null;

    public bool IsClosed { get; private set; } = false;

    public List<Line> PolygonEncapsulation { get; private set; } = new List<Line>();

    private List<Line> _edges = new List<Line>();
    
    private List<Vertex> _vertices = new List<Vertex>();

    private Dictionary<Vertex, bool> _boundaryPoints = new Dictionary<Vertex, bool>();
    
    public void AddNewVertex(Vector2 vertex)
    {
        if (IsClosed) return;

        Vertex v = VertexFactory.Instance.CreateVertex(new VertexActivationData(vertex, _vertices.Count + 1, transform));
        
        _vertices.Add(v);
        
        if (_vertices.Count == 1)
            _edges.Add(LineFactory.Instance.CreateLine(new LineActivationData(_vertices[0], transform)));
        else
        {
            _edges[_edges.Count - 1].UpdateLine(v);
            
            _edges.Add(LineFactory.Instance.CreateLine(new LineActivationData(_vertices[_vertices.Count - 1], transform)));
        }
    }

    public void UpdateCurrentEdge(Vector2 vertex)
    {
        if (IsClosed) return;
        
        if (_edges.Count == 0) return;
        
        _edges[_edges.Count - 1].UpdateLine(vertex);
    }

    public void ClosePolygon()
    {
        if (IsClosed) return;

        IsClosed = true;
        
        _edges[_edges.Count - 1].UpdateLine(_vertices[0]);

        InitPolygonBoundaries();

        InitEncapsulationEdges();
        
        _polygonVisualizer.Visualize(PolygonEncapsulation);
    }

    private void InitPolygonBoundaries()
    {
        for (int i = 0; i < _edges.Count; i++)
            InitSubVerticesBetweenTwoVertex(_edges[i]);
    }

    private void InitSubVerticesBetweenTwoVertex(Line edge)
    {
        Vertex v1 = edge.StartingPoint;
        Vertex v2 = edge.FinishPoint;
        
        Vector2 dir = (v2.Position - v1.Position).normalized;

        float distance = (v2.Position - v1.Position).magnitude;
        int count = 1;

        while (distance > _subVertexSpawnInterval)
        {
            Vector2 nextPos = v1.Position + (count * _subVertexSpawnInterval) * dir;

            Vertex boundaryPoint = VertexFactory.Instance.CreateVertex(new VertexActivationData(nextPos, -1, transform));
            
            edge.BoundaryPoints.Add(boundaryPoint);
            
            bool encapsulatedByPolygon = CheckVertexEncapsulatedByPolygon(boundaryPoint);
            
            boundaryPoint.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            
            _boundaryPoints.Add(boundaryPoint, encapsulatedByPolygon);

            boundaryPoint.UpdateVertexColor(encapsulatedByPolygon ? Color.red : Color.green);

            distance = (v2.Position - nextPos).magnitude;

            count++;
        }
    }

    private bool CheckVertexEncapsulatedByPolygon(Vertex v)
    {
        for (int i = 0; i < 360; i++)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(v.Position, GetDirectionVector2D(i), Mathf.Infinity, _layerMask);

            List<GameObject> hitObject = new List<GameObject>();
            
            foreach (RaycastHit2D hit in hits)
            {
                foreach (Line edge in _edges)
                {
                    if (edge.gameObject == hit.collider.gameObject && 
                        !edge.BoundaryPoints.Contains(v))
                        hitObject.Add(edge.gameObject);
                }
            }

            if (hitObject.Count == 0)
                return false;
        }
        
        return true;
    }
    
    private bool CheckVertexEncByPolygon(Vertex v)
    {
        return v.CheckVertexEncapsulatedByEdges(_edges);
    }
    
    private Vector2 GetDirectionVector2D(float angle)
    {
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
    }

    private void InitEncapsulationEdges()
    {
        bool foundEncapsulated = false;
        
        Vertex[] vertices = new Vertex[2];

        Line prevEdge = null;
        Vertex prevVertex = null;

        bool inExtraTraverse = false;

        bool firstPointEncapsulated = _boundaryPoints[_edges[0].BoundaryPoints[0]];

        for (int edgeIndex = 0; edgeIndex < _edges.Count; edgeIndex++)
        {
            bool edgeCompletelyFree = true;

            int boundaryPointIndex = 0;
            
            for (boundaryPointIndex = 0;
                boundaryPointIndex < _edges[edgeIndex].BoundaryPoints.Count;
                boundaryPointIndex++)
            {
                Vertex boundaryPoint = _edges[edgeIndex].BoundaryPoints[boundaryPointIndex];

                if (_boundaryPoints[boundaryPoint])
                    edgeCompletelyFree = false;

                if (!_boundaryPoints[boundaryPoint] && firstPointEncapsulated)
                    firstPointEncapsulated = false;
                
                if (_boundaryPoints[boundaryPoint] && !foundEncapsulated && !firstPointEncapsulated)
                {
                    foundEncapsulated = true;

                    Line newEdge =
                        LineFactory.Instance.CreateLine(new LineActivationData(_edges[edgeIndex].StartingPoint,
                            transform));
                    
                    newEdge.UpdateLine(boundaryPoint);
                    
                    PolygonEncapsulation.Add(newEdge);
                    
                    vertices[0] = boundaryPoint;
                }     
                
                else if (!_boundaryPoints[boundaryPoint] && foundEncapsulated)
                {
                    foundEncapsulated = false;

                    vertices[1] = prevVertex;

                    Line encapsulationEdge =
                        LineFactory.Instance.CreateLine(new LineActivationData(vertices[0], transform));
                    encapsulationEdge.UpdateLine(vertices[1]);
                    
                    PolygonEncapsulation.Add(encapsulationEdge);

                    if (boundaryPointIndex > 0)
                    {
                        Line newEdge = LineFactory.Instance.CreateLine(new LineActivationData(prevVertex, transform));
                        newEdge.UpdateLine(_edges[edgeIndex].FinishPoint);
                        PolygonEncapsulation.Add(newEdge);
                    }
                    else
                    {
                        Line newEdge = LineFactory.Instance.CreateLine(new LineActivationData(prevVertex, transform));
                        newEdge.UpdateLine(prevEdge.FinishPoint);
                        PolygonEncapsulation.Add(newEdge);
                    }
                    
                    if (inExtraTraverse)
                        return;
                }
                
                if (edgeCompletelyFree && !PolygonEncapsulation.Contains(_edges[edgeIndex]))
                    PolygonEncapsulation.Add(_edges[edgeIndex]);
                
                prevVertex = boundaryPoint;
            }

            prevEdge = _edges[edgeIndex];
            
            if (edgeIndex == _edges.Count - 1 &&
                boundaryPointIndex == _edges[edgeIndex].BoundaryPoints.Count &&
                foundEncapsulated)
            {
                inExtraTraverse = true;
                    
                edgeIndex = -1;
            }
        }
    }
}

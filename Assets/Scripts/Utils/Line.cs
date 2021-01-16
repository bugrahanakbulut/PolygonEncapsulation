using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer = null;

    [SerializeField] private EdgeCollider2D _collider = null;
    
    public Vertex StartingPoint { get; private set; }
    public Vertex FinishPoint { get; private set; }
    
    public List<Vertex> BoundaryPoints { get; set; } = new List<Vertex>();

    public void InitLinePosition(Vertex vertex)
    {
        StartingPoint = vertex;
        FinishPoint = vertex;

        _lineRenderer.SetPosition(0, vertex.Position);
        _lineRenderer.SetPosition(1, vertex.Position);

        Vector2[] points = new[] {vertex.Position, vertex.Position};
        
        _collider.points = points;
    }

    public void UpdateLine(Vector2 finishPoint)
    {
        _lineRenderer.SetPosition(1, finishPoint);
        
        Vector2[] points = new[] { StartingPoint.Position, finishPoint };
        _collider.points = points;
    }

    public void UpdateLine(Vertex finishVertex)
    {
        FinishPoint = finishVertex;
        
        _lineRenderer.SetPosition(1, finishVertex.Position);
        
        Vector2[] points = new[] {StartingPoint.Position, FinishPoint.Position};
        _collider.points = points;
    }

    public bool IsInLeft(Vertex v)
    {
        if (v.Position == StartingPoint.Position ||
            v.Position == FinishPoint.Position)
            return false;
        
        Vector2 a = StartingPoint.Position;
        Vector2 b = FinishPoint.Position;

        float area = ((b[0] - a[0]) * (v.Position[1] - a[1])) - ((b[1] - a[1]) * (v.Position[0] - a[0]));
            
        area /= 2;
        
        return area > 0;
    }
}

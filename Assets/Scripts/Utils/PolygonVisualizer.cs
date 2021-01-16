using System.Collections.Generic;
using UnityEngine;

public class PolygonVisualizer : MonoBehaviour
{
    [SerializeField] private Material _material;
    
    public void Visualize(List<Line> edges)
    {
        List<Vector2> vertices2d = new List<Vector2>();

        foreach (Line edge in edges)
            if (!vertices2d.Contains(edge.StartingPoint.Position))
                vertices2d.Add(edge.StartingPoint.Position);

        
        Triangulator triangulator = new Triangulator(vertices2d.ToArray());

        int[] indices = triangulator.Triangulate();
        
        Vector3[] vertices = new Vector3[vertices2d.Count];
        for (int i=0; i<vertices.Length; i++) 
            vertices[i] = new Vector3(vertices2d[i].x, vertices2d[i].y, 0);
        
        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = indices;
        msh.RecalculateNormals();
        msh.RecalculateBounds();
        
        gameObject.AddComponent(typeof(MeshRenderer));
        MeshFilter filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        filter.mesh = msh;

        gameObject.GetComponent<MeshRenderer>().material = _material;
    }
}

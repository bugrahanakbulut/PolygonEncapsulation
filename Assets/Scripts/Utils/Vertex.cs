using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Vertex : MonoBehaviour
{
    [SerializeField] private TextMeshPro _text = null;

    [SerializeField] private SpriteRenderer _innerSprite = null;

    [SerializeField] private SpriteRenderer _outerSprite = null;
    
    public Vector2 Position { get; set; }

    public void InitVertex(Vector2 position)
    {
        Position = position;

        transform.position = position;
    }

    public void InitIndexText(int index)
    {
        if (index < 0)
            _text.enabled = false;
        
        _text.SetText(index.ToString());
    }

    public void UpdateVertexColor(Color c)
    {
        _innerSprite.color = c;
        _outerSprite.color = c;
    }

    public bool CheckVertexEncapsulatedByEdges(List<Line> edges)
    {
        float angle = 0;
        
        for (int i = 0; i < edges.Count; i++)
        {
            if (edges[i].BoundaryPoints.Contains(this))
                continue;
            
            float edgeV0 = Vector3.Distance(Position, edges[i].StartingPoint.Position);
            float edgeV1 = Vector3.Distance(Position, edges[i].FinishPoint.Position);
            float edgeV = Vector3.Distance(edges[i].StartingPoint.Position, edges[i].FinishPoint.Position);

            float cosV = (float) (Mathf.Pow(edgeV0, 2) + Mathf.Pow(edgeV1, 2) - Math.Pow(edgeV, 2)) / (2 * edgeV0 * edgeV1);

            float angleV = Mathf.Acos(cosV) * Mathf.Rad2Deg;

            if (edges[i].IsInLeft(this))
                angle += angleV;
            else
                angle -= angleV;

            if (angle > 360)
                return true;
        }

        return Mathf.Abs(angle - 360) < 1;
    }
}


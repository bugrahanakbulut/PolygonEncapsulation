using UnityEngine;

public class VertexActivationData
{
    public Vector2 Position { get; }
    
    public int Index { get; }
    
    public Transform Parent { get; }

    public VertexActivationData(Vector2 position, int index, Transform parent)
    {
        Position = position;
        Index = index;
        Parent = parent;
    }
}

public class VertexFactory : MonoBehaviour
{
    private static VertexFactory _instance;

    public static VertexFactory Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<VertexFactory>();

            return _instance;
        }   
    }

    [SerializeField] private Vertex _referenceVertex = null;

    public Vertex CreateVertex(VertexActivationData activationData)
    {
        Vertex vertex = Instantiate(
            _referenceVertex.gameObject,
            activationData.Position,
            Quaternion.identity,
            activationData.Parent).GetComponent<Vertex>();
        
        vertex.InitVertex(activationData.Position);
        
        vertex.InitIndexText(activationData.Index);

        return vertex;
    }
}

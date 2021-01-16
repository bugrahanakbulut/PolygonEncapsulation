using UnityEngine;

public class LineActivationData
{
    public Vertex StartingVertex { get; }
    
    public Transform Parent { get; }

    public LineActivationData(Vertex startingVertex, Transform parent)
    {
        StartingVertex = startingVertex;
        Parent = parent;
    }
}

public class LineFactory : MonoBehaviour
{
    private static LineFactory _instance;

    public static LineFactory Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<LineFactory>();

            return _instance;
        }   
    }

    [SerializeField] private Line _lineReference = null;

    public Line CreateLine(LineActivationData activationData)
    {
        Line line = Instantiate(_lineReference.gameObject, Vector3.zero, Quaternion.identity, activationData.Parent).GetComponent<Line>();

        line.InitLinePosition(activationData.StartingVertex);
        
        return line;
    }
}

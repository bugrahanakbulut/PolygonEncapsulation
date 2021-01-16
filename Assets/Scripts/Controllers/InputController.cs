using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] private Polygon _referencePolygon = null;

    [SerializeField] private Camera _mainCamera = null;
    
    private Polygon _curPolygon;

    private Polygon _CurPolygon
    {
        get
        {
            if (_curPolygon == null)
                _curPolygon = Instantiate(_referencePolygon.gameObject).GetComponent<Polygon>();

            return _curPolygon;
        }
    }
    
    private void Update()
    {
        CheckPolygonInput();

        CheckCreatingPointInput();
    }

    private void CheckPolygonInput()
    {
        if (Input.GetMouseButtonDown(0))
            _CurPolygon.AddNewVertex(GetMouseWorldPosition());
        
        if (Input.GetKeyDown(KeyCode.C))
            _CurPolygon.ClosePolygon();
        
        _CurPolygon.UpdateCurrentEdge(GetMouseWorldPosition());
    }

    private void CheckCreatingPointInput()
    {
        if (!_CurPolygon.IsClosed) 
            return;
        
        if (!Input.GetMouseButtonDown(0))
            return;

        Vector2 cursorPos = GetMouseWorldPosition();

        Vertex v = VertexFactory.Instance.CreateVertex(new VertexActivationData(cursorPos, -1, transform));

        bool encapsulated = v.CheckVertexEncapsulatedByEdges(_CurPolygon.PolygonEncapsulation);
        
        v.UpdateVertexColor(encapsulated ? Color.red : Color.green);
    }

    private Vector2 GetMouseWorldPosition()
    {
        return _mainCamera.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Constants.DEFAULT_DEPTH));
    }
}

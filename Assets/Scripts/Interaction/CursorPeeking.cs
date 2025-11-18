using UnityEngine;

public class CursorPeeking : MonoBehaviour
{

    [SerializeField] private Transform cursorPeekTransform;
    [SerializeField] private AnimationCurve peekCurveX;
    [SerializeField] private AnimationCurve peekCurveY;

    [SerializeField] private Vector2 maxPeek = Vector3.one;
    


    #region Enable / Disable
    void OnEnable()
    {
        InputManager.OnLookChanged += OnLookChanged;
    }
    void OnDisable()
    {
        InputManager.OnLookChanged -= OnLookChanged;
    }
    #endregion
    
    
    private void OnLookChanged(Vector2 mousePos)
    {
        Vector2 _mouseUV = new Vector2(mousePos.x / Screen.width, mousePos.y / Screen.height);
        
        _mouseUV = _mouseUV * 2 - Vector2.one;
        Debug.Log(_mouseUV);

        float _xModulate = Mathf.Sign(_mouseUV.x) * peekCurveX.Evaluate(Mathf.Abs(_mouseUV.x)) * maxPeek.x;
        float _yModulate = Mathf.Sign(_mouseUV.y) * peekCurveY.Evaluate(Mathf.Abs(_mouseUV.y)) * maxPeek.y;

        cursorPeekTransform.localRotation = Quaternion.Euler(-_yModulate, _xModulate, 0);
    }
}

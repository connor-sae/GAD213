using UnityEngine;

[RequireComponent(typeof(SpringJoint))]
public class Grabable : MonoBehaviour
{
    private SpringJoint _springJoint;
    private Transform _grabTransform;
    private float _defaultSpring = 500;
    private float _defaultDamp = 500;
    void Awake()
    {
        
        if (TryGetComponent(out SpringJoint joint))
        {
            _defaultDamp = joint.damper;
            _defaultSpring = joint.spring;
            _springJoint = joint;
            _springJoint.spring = 0; // disabled
            _springJoint.damper = 0; // disabled
        }
        else
            Debug.LogWarning("Grabbable object does not have Spring Joint!!");
    }
    public void OnGrabbed(Transform grabAnchor, Vector3 grabPos)
    {
        if (_springJoint != null)
        {
            transform.parent = null; // remove parent to make sure physics doesnt break
            Vector3 _relativeAnchorPos = transform.InverseTransformPoint(grabPos); // transform grab pos to local space
            //Debug.Log(_relativeAnchorPos);
            _springJoint.anchor = _relativeAnchorPos;
            _grabTransform = grabAnchor;

            _springJoint.spring = _defaultSpring;
            _springJoint.damper = _defaultDamp;
        }
    }

    void Update()
    {
        if(_grabTransform != null)
            _springJoint.connectedAnchor = _grabTransform.position;
    }

    public void OnReleased()
    {
        if (_springJoint != null)
        {
            _springJoint.spring = 0; // disabled
            _springJoint.damper = 0; // disabled
        }
        _grabTransform = null;
    }
}

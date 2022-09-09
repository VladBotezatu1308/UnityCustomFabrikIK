using UnityEngine;

public class IKNode : MonoBehaviour
{
    [field: SerializeField] public IKNode Parent { get; private set; }
    [field: SerializeField] public IKNode Child { get; private set; }
    public float Length => DirectionToParent.magnitude;
    public Vector3 DirectionToParent => Parent != null ? Parent.transform.position - transform.position : Vector3.zero;
    public Vector3 DirectionToChild => Child != null ? Child.transform.position - transform.position : Vector3.zero;
    
    public Vector3 DirectionToParentTarget => Parent != null ? Parent.TargetPosition - TargetPosition: Vector3.zero;
    public Vector3 DirectionToChildTarget => Child != null ? Child.TargetPosition - TargetPosition : Vector3.zero;
    [field: SerializeField]
    public Vector3 TargetPosition { get; set; }

    public void Init(IKNode parent, int depth)
    {
        Parent = parent;
        TargetPosition = transform.position;

        if (transform.childCount > 0 && depth > 0)
        {
            bool foundChildNode = false;
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent(out IKNode childNode))
                {
                    Child = childNode;
                    Child.Init(this, depth - 1);
                    foundChildNode = true;
                }
            }

            if (!foundChildNode)
            {
                Child = transform.GetChild(0).gameObject.AddComponent<IKNode>();
                Child.Init(this, depth - 1);
            }
        }
    }

    public void ApplyTargetPosition(bool recursive = false)
    {
        if (Child == null)
            return;

        // var childDir = transform.InverseTransformDirection(DirectionToChild);
        transform.rotation = Quaternion.FromToRotation(Child.transform.localPosition, DirectionToChildTarget);
        
        if (recursive)
            Child.ApplyTargetPosition(true);
    }
}
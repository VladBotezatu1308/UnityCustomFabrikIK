using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class FabrikIK : MonoBehaviour
{
    [Header("Solver Properties")]
    [SerializeField] private int solverIterations = 10;
    [SerializeField] private float solverMaxError = 0.01f;

    [Header("IK Config")]
    [SerializeField] private IKNode root;
    [SerializeField] private IKNode endEffector;
    [SerializeField] private int depth;
    [SerializeField] private bool initOnStart = false;

    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private Transform pole;

    [SerializeField] private List<IKNode> nodes = new List<IKNode>();
    private float m_totalLength => nodes.Aggregate(0.0f, (agg, node) => agg + node.Length);

    private List<Vector3> m_Positions;
    
    private void Start()
    {
        if (initOnStart)
        {
            SetupIK();
        }
        
    }

    [Button("Setup IK Rig")]
    void SetupIK()
    {
        int setupDepth = depth;
        if (root == null && !TryGetComponent(out root))
        {
            root = gameObject.AddComponent<IKNode>();
        }
        
        root.Init(null, setupDepth);
        
        nodes.Clear();
        
        IKNode currentNode = root;
        do
        {
            setupDepth--;
            nodes.Add(currentNode);

            if (currentNode.Child == null || setupDepth <= 0)
                endEffector = currentNode;

            currentNode = currentNode.Child;
        } while (currentNode != null && setupDepth > 0);

    }

    void Update()
    {
        if (target == null)
            return;

        nodes.ForEach(node => node.TargetPosition = node.transform.position);

        Vector3 directionToTarget = target.position - root.transform.position;
        
        if (directionToTarget.magnitude >= m_totalLength)
        {
            Straighten(directionToTarget);
        }
        else
        {
            for (int iteration = 0; iteration < solverIterations; iteration++)
            {
                ForwardChain();
                
                BackwardChain();

                if ((endEffector.TargetPosition - target.position).magnitude < solverMaxError)
                    break;
            }
        }
        
        PoleCheck();

        root.ApplyTargetPosition(true);
    }

    private void Straighten(Vector3 directionToTarget)
    {
        root.TargetPosition = root.transform.position;

        foreach (var node in nodes)
        {
            if (node.Parent != null)
                node.TargetPosition = node.Parent.TargetPosition + directionToTarget.normalized * node.Length;
        }
    }

    private void ForwardChain()
    {
        endEffector.TargetPosition = target.position;
        for (IKNode currentNode = endEffector.Parent;
             currentNode != null && currentNode.Parent != null;
             currentNode = currentNode.Parent)
        {
            currentNode.TargetPosition = currentNode.Child.TargetPosition +
                                         currentNode.Child.DirectionToParentTarget.normalized * currentNode.Child.Length;
        }
    }

    private void BackwardChain()
    {
        for (IKNode currentNode = root.Child;
             currentNode != null;
             currentNode = currentNode.Child)
        {
            currentNode.TargetPosition = currentNode.Parent.TargetPosition +
                                         currentNode.Parent.DirectionToChildTarget.normalized * currentNode.Length;
        }
    }

    private void PoleCheck()
    {
        if (pole != null)
        {
            for (IKNode currentNode = root.Child;
                 currentNode != null && currentNode.Child != null;
                 currentNode = currentNode.Child)
            {
                Vector3 rotationAxis =
                    (currentNode.Parent.TargetPosition - currentNode.Child.TargetPosition).normalized;

                Vector3 dirToPole = (pole.position - currentNode.Parent.TargetPosition).normalized;
                Vector3 dirToNode = currentNode.Parent.DirectionToChildTarget.normalized;
                
                Vector3.OrthoNormalize(ref rotationAxis, ref dirToNode);
                Vector3.OrthoNormalize(ref rotationAxis, ref dirToPole);

                Vector3 dirToNewPos = Vector3.RotateTowards(dirToNode, dirToPole, 5 * Time.deltaTime, 0);
                Quaternion angle = Quaternion.FromToRotation(dirToNode, dirToNewPos);
                currentNode.TargetPosition = angle * (currentNode.Parent.DirectionToChildTarget) +
                                             currentNode.Parent.TargetPosition;
            }
        }
    }
}
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Game.Monster
{
    public enum NodeStatus
    {
        Running,
        Success,
        Failure
    }

    public interface INode
    {
        public NodeStatus Evaluate();
    }

    public class ActionNode : INode
    {
        public Func<NodeStatus> _action;

        public ActionNode(Func<NodeStatus> action)
        {
            _action = action;
        }

        public NodeStatus Evaluate()
        {
            return _action?.Invoke() ?? NodeStatus.Failure;
        }
    }

    public class SelectorNode : INode
    {
        List<INode> _chilldren;

        public SelectorNode()
        {
            _chilldren = new List<INode>();
        }

        public SelectorNode(List<INode> chilldren)
        {
            _chilldren = chilldren;
        }

        public void AddChild(INode child)
        {
            _chilldren.Add(child);
        }

        public NodeStatus Evaluate()
        {
            foreach(INode child in _chilldren)
            {
                NodeStatus result = child.Evaluate();
                if(result == NodeStatus.Success)
                    return NodeStatus.Success;
                else if(result == NodeStatus.Running)
                    return NodeStatus.Running;
            }
            return NodeStatus.Failure;
        }
    }

    public class SequenceNode : INode
    {
        List<INode> _chilldren;

        public SequenceNode() { _chilldren= new List<INode>(); }
        public SequenceNode(List<INode> chilldren) { _chilldren= chilldren; }

        public NodeStatus Evaluate()
        {
            foreach (INode child in _chilldren)
            {
                NodeStatus result = child.Evaluate();
                if (result == NodeStatus.Failure)
                    return NodeStatus.Failure;
                else if (result == NodeStatus.Running)
                    return NodeStatus.Running;
            }
            return NodeStatus.Success;
        }
    }
}

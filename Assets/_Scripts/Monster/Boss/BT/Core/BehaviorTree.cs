using System.Collections.Generic;
using System;
using UnityEngine;

namespace Game.Monster
{
    /// <summary>
    /// 노드 상태 및 결과 이넘
    /// </summary>
    public enum NodeStatus
    {
        Running,
        Success,
        Failure
    }

    /// <summary>
    /// 노드 인터페이스
    /// </summary>
    public interface INode
    {
        public NodeStatus Evaluate();
    }

    /// <summary>
    /// 추상 노드 클래스
    /// </summary>
    public abstract class BTNode : INode
    {
        public String NodeName;
        public abstract NodeStatus Evaluate();
    }

    /// <summary>
    /// 액션 노드 => 리프 노드로 행동(메서드)을 통해 노드의 결과를 반환
    /// </summary>
    public class ActionNode : BTNode
    {
        private Func<NodeStatus> _action;

        public ActionNode(Func<NodeStatus> action, string nodeName = "NONE")
        {
            NodeName = nodeName;
            _action = action;
        }

        public override NodeStatus Evaluate()
        {
            //Debug.Log(NodeName);
            return _action?.Invoke() ?? NodeStatus.Failure;
        }
    }

    /// <summary>
    /// 셀렉터 노드: 한 개라도 성공하면 성공 반환
    /// </summary>
    public class SelectorNode : BTNode
    {
        List<INode> _chilldren;

        public SelectorNode(string nodeName = "NONE")
        {
            NodeName = nodeName;
            _chilldren = new List<INode>();
        }

        public SelectorNode(List<INode> chilldren, string nodeName = "NONE")
        {
            NodeName = nodeName;
            _chilldren = chilldren;
        }

        public void AddChild(INode child)
        {
            _chilldren.Add(child);
        }

        public override NodeStatus Evaluate()
        {
            //Debug.Log(NodeName);
            foreach (INode child in _chilldren)
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

    /// <summary>
    /// 시퀀스 노드: 한 개라도 실패하면 실패 반환
    /// </summary>
    public class SequenceNode : BTNode
    {
        List<INode> _chilldren;

        public SequenceNode(string nodeName = "NONE") { NodeName = nodeName; _chilldren= new List<INode>(); }
        public SequenceNode(List<INode> chilldren, string nodeName = "NONE") { NodeName = nodeName; _chilldren= chilldren; }

        public void AddChild(INode child)
        {
            _chilldren.Add(child);
        }

        public override NodeStatus Evaluate()
        {
            //Debug.Log(NodeName);
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

    /// <summary>
    /// 랜덤 셀렉터 노드: 해당 노드의 자식 노드 중 랜덤한 한개의 노드의 결과를 반환
    /// 만약 자식이 없으면 실패 반환
    /// </summary>
    public class RandomSelectorNode : BTNode
    {
        List<INode> _chilldren;

        public RandomSelectorNode(string nodeName = "NONE")
        {
            NodeName = nodeName;
            _chilldren = new List<INode>();
        }

        public RandomSelectorNode(List<INode> chilldren, string nodeName = "NONE")
        {
            NodeName = nodeName;
            _chilldren = chilldren;
        }

        public void AddChild(INode child)
        {
            _chilldren.Add(child);
        }

        public override NodeStatus Evaluate()
        {
            //Debug.Log(NodeName);
            if (_chilldren != null && _chilldren.Count != 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, _chilldren.Count);
                return _chilldren[randomIndex].Evaluate();
            }
            else
                return NodeStatus.Failure;
        }
    }

    /// <summary>
    /// 컨디션 노드: 조건을 확인하여 성공과 실패를 판단하여 반환하는 노드
    /// </summary>
    public class ConditionNode : BTNode
    {
        public Func<bool> _condition;

        public ConditionNode(Func<bool> func, string nodeName = "NONE")
        {
            NodeName = nodeName;
            _condition = func;
        }

        public override NodeStatus Evaluate()
        {
            //Debug.Log(NodeName);
            return _condition.Invoke() ? NodeStatus.Success : NodeStatus.Failure;
        }
    }

    /// <summary>
    /// 반전 노드: 자식 노드의 결과를 반전시키는 노드
    /// </summary>
    public class InvertNode : BTNode
    {
        public BTNode child;

        public InvertNode(BTNode child, string nodeName = "NONE")
        {
            NodeName = nodeName;
            this.child = child;
        }

        public override NodeStatus Evaluate()
        {
            NodeStatus nodeStatus = child.Evaluate();
            if (nodeStatus == NodeStatus.Success) { return NodeStatus.Failure; }
            else if (nodeStatus == NodeStatus.Failure) { return NodeStatus.Success; }
            else { return NodeStatus.Running; }
        }
    }
}

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

    #region Composite Node
    /// <summary>
    /// 셀렉터 노드: 한 개라도 성공하면 성공 반환
    /// </summary>
    public class SelectorNode : BTNode
    {
        List<INode> _chilldren;
        int _curIndex;

        public SelectorNode(string nodeName = "NONE")
        {
            NodeName = nodeName;
            _chilldren = new List<INode>();
            _curIndex = 0;
        }

        public SelectorNode(List<INode> chilldren, string nodeName = "NONE")
        {
            NodeName = nodeName;
            _chilldren = chilldren;
            _curIndex = 0;
        }

        public void AddChild(INode child)
        {
            _chilldren.Add(child);
        }

        /// <summary>
        /// 성공 -> 인덱스 초기화 & 성공 반환
        /// 진행중 -> 인덱스 유지 & 평가 종료
        /// 실패 -> 다음 노드 확인 & 모든 노드가 실패면 인덱스 초기화 및 실패 반환
        /// </summary>
        /// <returns>결과 상태</returns>
        public override NodeStatus Evaluate()
        {
            //Debug.Log(NodeName);
            for(;_curIndex < _chilldren.Count; _curIndex++)
            {
                NodeStatus result = _chilldren[_curIndex].Evaluate();
                if (result == NodeStatus.Success)
                {
                    _curIndex = 0;
                    return NodeStatus.Success;
                }
                else if (result == NodeStatus.Running)
                    return NodeStatus.Running;
            }
            _curIndex = 0;
            return NodeStatus.Failure;
        }
    }

    /// <summary>
    /// 시퀀스 노드: 한 개라도 실패하면 실패 반환
    /// </summary>
    public class SequenceNode : BTNode
    {
        List<INode> _chilldren;
        int _curIndex;

        public SequenceNode(string nodeName = "NONE") { NodeName = nodeName; _chilldren= new List<INode>(); _curIndex = 0; }
        public SequenceNode(List<INode> chilldren, string nodeName = "NONE") { NodeName = nodeName; _chilldren= chilldren; _curIndex = 0; }

        public void AddChild(INode child)
        {
            _chilldren.Add(child);
        }

        public override NodeStatus Evaluate()
        {
            //Debug.Log(NodeName);
            for(;_curIndex<_chilldren.Count; _curIndex++)
            {
                NodeStatus result = _chilldren[_curIndex].Evaluate();
                if (result == NodeStatus.Failure)
                {
                    _curIndex = 0;
                    return NodeStatus.Failure;
                }
                else if (result == NodeStatus.Running)
                    return NodeStatus.Running;
            }
            _curIndex = 0;
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
        // _curIndex = -1 => 인덱스가 초기화된 상태
        int _curIndex;

        public RandomSelectorNode(string nodeName = "NONE")
        {
            NodeName = nodeName;
            _chilldren = new List<INode>();
            _curIndex = -1;
        }

        public RandomSelectorNode(List<INode> chilldren, string nodeName = "NONE")
        {
            NodeName = nodeName;
            _chilldren = chilldren;
            _curIndex = -1;
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
                // 랜덤 인덱스가 초기화되지 않으면 이전 인덱스를 그대로 사용
                if(_curIndex == -1)
                    _curIndex = UnityEngine.Random.Range(0, _chilldren.Count);
                NodeStatus result = _chilldren[_curIndex].Evaluate();

                // 진행중이라면 인덱스 유지
                if (result == NodeStatus.Running)
                    return result;
                else
                {
                    _curIndex = -1;
                    return result;
                }
            }
            else
                return NodeStatus.Failure;
        }
    }
    #endregion

    #region 
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
    #endregion
}

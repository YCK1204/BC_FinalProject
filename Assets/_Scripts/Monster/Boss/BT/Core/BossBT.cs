using UnityEngine;

public abstract class BossBT
{
    protected BossMonster _owner;
    public BossMonster Owner {  get { return _owner; } }

    protected Game.Monster.SelectorNode _root;

    public abstract void Init(BossMonster bossMonster);
}

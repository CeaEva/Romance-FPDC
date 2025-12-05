using Godot;
using System;
namespace Combat{
    public struct ActionAttack : IAction
    {
        public int Damage { get; private set; }
        public DummyEnemy Target { get; private set; }
        public Animation ActAnimation { get; private set; }
        public PlayerActor Caller { get; private set; }
    
    
        public ActionAttack(PlayerActor caller, DummyEnemy target, Animation animation)
        {
            Caller = caller;
            Target = target;
            ActAnimation = animation;
            Damage = caller.PlayerStats.StandardAttack(target, caller.PlayerStats);
        }
    
    
    }
}

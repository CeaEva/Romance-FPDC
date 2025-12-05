using System;
using Godot;

namespace Combat
{
    public interface IAction
    {

        int Damage { get; }
        DummyEnemy Target { get; }
        Animation ActAnimation { get; }
        PlayerActor Caller { get; }


    }
}

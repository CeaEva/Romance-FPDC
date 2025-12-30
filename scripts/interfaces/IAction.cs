using System;
using System.Collections.Generic;
using Godot;

namespace Combat
{
    public interface IAction
    {

        int Damage { get; }
        List<IActor> Targets { get; }
        Animation ActAnimation { get; }
        IActor Caller { get; }

        void Execute();


    }
}

using Godot;
using System;
using System.Collections.Generic;

namespace Combat{

    public class ActionAttack : IAction
    {
        public int Damage { get; private set; }
        public List<IActor> Targets { get; private set; }
        public Animation ActAnimation { get; private set; }
        public IActor Caller { get; private set; }
    
    
       /* public ActionAttack()
        {
           
        }
    */
        public void Execute()
        {
            
            GD.Print("Acton execute");

        } 
    }
}

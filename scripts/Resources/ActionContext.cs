using System.Collections.Generic;
using Combat;
using Godot;

namespace Combat{

    [GlobalClass]
    public partial class ActionContext : Resource{

        public List<IActor> Targets;

        public IActor Caller;

        public IAction SelectedAction;

        public ActionContext(List<IActor> targets, IActor caller, IAction selectedAction )
        {
            Targets = targets;

            Caller = caller;

            SelectedAction = selectedAction;


        }
    
    
    } 
}
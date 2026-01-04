using System;
using System.Collections.Generic;
using Combat;
using Godot;
using Resources;

namespace Combat{

    [GlobalClass]
    public partial class ActionContext : Resource{

        public List<IActor> Targets;

        public IActor Caller;

        public Func<ActionContext, int, ActionResult> SelectedAction;

        public ActionContext(List<IActor> targets, IActor caller, Func<ActionContext, int, ActionResult> selectedAction )
        {
            Targets = targets;

            Caller = caller;

            SelectedAction = selectedAction;


        }
    
    
    } 
}
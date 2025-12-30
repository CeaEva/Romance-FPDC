using System;
using System.Collections.Generic;
using Combat;
using Godot;

namespace References
{
    
    public sealed class ActionReference
    {

        public IReadOnlyDictionary<string, IAction> ActionDictionary => _defs;

        Dictionary<string, IAction> _defs = new Dictionary<string, IAction>

        {
            ["Attack"] = new ActionAttack()

        } ;             

        







    }





}
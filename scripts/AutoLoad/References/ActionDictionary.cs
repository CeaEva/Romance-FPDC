using System;
using System.Collections.Generic;
using Combat;
using Godot;

namespace Resources
{
    [GlobalClass]
    public partial class ActionReference : Resource
    {

        public IReadOnlyDictionary<string, IAction> ActionDictionary => _defs;

        Dictionary<string, IAction> _defs = new Dictionary<string, IAction>

        {
            ["Attack"] = new ActionAttack()

        } ;             

        







    }





}
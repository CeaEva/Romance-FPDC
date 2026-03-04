using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Combat;
using Godot;

namespace Resources
{

    public enum ActionId
    {
        Standard,
        Spell,

    }

    public record ValueTarget (int Value, IActor Actor);

    public class ActionResult
    {
        public ActionId ActionTag;
        public ValueTarget ActorDamage;
        
        public ActionResult(ActionId actionId, ValueTarget actorDamage) => (ActionTag, ActorDamage) = (actionId, actorDamage);
    }


    //[GlobalClass]
    public static class ActionReference
    {


        private static Dictionary<string, Func<ActionContext, int, ActionResult>> _defs = new Dictionary<string, Func<ActionContext, int, ActionResult>>

        {
            ["Attack"] = (ctx, i) =>
            {
                
                var str = ctx.Caller.Stats.Str + ctx.Caller.Stats.Spd;
                var target = ctx.Targets[i];

                int damage = str/4;

                return new ActionResult(ActionId.Standard, new ValueTarget(damage, target));

            }

        } ;             

        public static IReadOnlyDictionary<string, Func<ActionContext, int, ActionResult>> ActionDictionary => _defs;
        







    }





}
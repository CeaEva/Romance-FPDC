using Godot;
using Resources;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Threading.Tasks;

namespace Combat
{
    

    public partial class BattleManager : Node
    {

        public List<IActor> PlayerList
        {
            get => _playerList; private set
            {
                _playerList = value;
            }

        }

        public List<IActor> EnemyList
        {
            get => _enemyList; private set
            {
                _enemyList = value;
            }

        }

        [Signal]
        public delegate void ActorsReadyEventHandler();

        List<IActor> _allActors = new List<IActor>();
        List<IActor> _playerList = new List<IActor>();
        List<IActor> _enemyList = new List<IActor>();
        Queue<Func<ActionContext, int, ActionResult>> _turnQueue = new();
        ProgressBar _atbBar;

        public override void _Ready()
        {
            
            ActorsToList("EnemyGroup");
            ActorsToList("PlayerGroup");
            GD.Print("Enemies: " + _allActors + " " +" Player: " + _playerList);
            EmitSignal(SignalName.ActorsReady);
            var atbTimer = GetNode<Timer>("AtbTimer");
            atbTimer.Timeout += AtbTick;
            _atbBar = GetNodeOrNull<ProgressBar>("%AtbBar");

        }

        public override void _Process(double delta)
        {
            

        }

        private void ActorsToList(string group)
        {
            var actors = GetTree().GetNodesInGroup(group);

            foreach (var actor in actors)
            {
                if (actor is IActor iActor)
                    _allActors.Add(iActor);

                if (actor is DummyEnemy dummy)
                    _enemyList.Add(dummy);

                if (actor is PlayerActor player)
                    _playerList.Add(player);

            }


        }

        private void AtbTick()
        {
            
            foreach (IActor n in _allActors)
            {
                if (n.State != CombatState.Wait)
                    continue;

                if (n is PlayerActor player){
                    player.Atb += player.Stats.Spd;
                    GD.Print(player.Name + " " + player.Atb);
                    _atbBar?.SetValueNoSignal(player.Atb);
                }

                if (n is DummyEnemy enemy)
                {
                    
                    GD.Print(enemy.Atb + ", " + enemy.State);
                    enemy.Atb += enemy.Stats.Spd;
                    
                }
            }

            StateCheck();
        }

        private void StateCheck()
        {
            var atbMax = 100;

            foreach (IActor n in _allActors)
            {
                if (n.State != CombatState.Wait)
                    continue;

                if (n.Atb >= atbMax && n is PlayerActor player)
                {
                    player.StateControl(CombatState.Menu);
                    GD.Print("Player can menu");
                }

            }

            foreach (DummyEnemy n in _enemyList)
            {
                if (_enemyList.Count <= 0){
                    GD.Print("Enemies dead");
                    return;
                }

                if (n.Atb >= atbMax && n.State != CombatState.Queued)
                {
                    n.State = CombatState.Queued;
                    GD.Print("Enemy can Tick");
                }

            }
            


        }

        private string EnemyNames()
        {
            var enemyString = "";

            foreach(DummyEnemy n in _enemyList)
            {
                enemyString = n.Name + " ";
                GD.Print(enemyString);
            }
            
            return enemyString;



        }

        public async Task EnqueueAction(ActionContext action)
        {
            if (action == null)
            {
                GD.PrintErr("EnqueueAction called with null ActionContext.");
                return;
            }

            var currentAct = action.SelectedAction;
            if (currentAct == null)
            {
                GD.PrintErr("EnqueueAction missing SelectedAction.");
                return;
            }

            _turnQueue.Enqueue(currentAct);
            //currentAct.Execute();

            await Execute();

            //for loop applying results

            if (action.Caller != null)
            {
                action.Caller.Atb = 0;
                action.Caller.State = CombatState.Wait;
            }
            else
                GD.PrintErr("EnqueueAction missing Caller.");
            _turnQueue.Dequeue();

            
                        
            async Task Execute()
            {
                var targets = action.Targets;
                var i = 0;

                foreach (var target in targets)
                {
                    var result = action.SelectedAction(action, i);
                    var beforeHp = target.CurrentHp;
                    target.CurrentHp -= result.ActorDamage.Value;

                    GD.Print(beforeHp, " current hp => ", target.CurrentHp);
                    i++;

                }

            }
            return;



        }


    }

}

using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;
using Resources;

namespace Combat
{
    

    public partial class BattleManager : Node
    {

        public List<IActor> PlayerList => _playerList;
        public List<IActor> EnemyList => _enemyList;
        public List<IActor> AllActorList => _allActors;

        [Signal] public delegate void ActorsReadyEventHandler();
        [Signal] public delegate void PlayerTurnFinishedEventHandler();

        readonly List<IActor> _allActors = new();
        readonly List<IActor> _playerList = new();
        readonly List<IActor> _enemyList = new();
        readonly Queue<ActionContext> _turnQueue = new();
        bool _isProcessing;
        ProgressBar _atbBar;

        public override void _Ready()
        {
            
            RefreshActorLists();
            EmitSignal(SignalName.ActorsReady);
            var atbTimer = GetNode<Timer>("AtbTimer");
            atbTimer.Timeout += AtbTick;
            _atbBar = GetNodeOrNull<ProgressBar>("%AtbBar");

        }

        private void RefreshActorLists()
        {
            _allActors.Clear();
            _playerList.Clear();
            _enemyList.Clear();
            ActorsToList("EnemyGroup");
            ActorsToList("PlayerGroup");
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
            foreach (var actor in _allActors)
            {
                if (actor.State != CombatState.Wait)
                    continue;

                if (actor is PlayerActor player)
                {
                    player.Atb += player.Stats.Spd;
                    _atbBar?.SetValueNoSignal(player.Atb);
                    continue;
                }

                if (actor is DummyEnemy enemy)
                    enemy.Atb += enemy.Stats.Spd;
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

            if (_enemyList.Count == 0)
            {
                GD.Print("Enemies dead");
                return;
            }

            var deadEnemies = new List<IActor>();

            foreach (var enemy in _enemyList)
            {
                if (enemy.Atb >= atbMax && enemy.State != CombatState.Queued)
                {
                    enemy.State = CombatState.Queued;
                    GD.Print("Enemy can Tick");
                }

                if (enemy.State == CombatState.Dead)
                {
                    deadEnemies.Add(enemy);
                }
            }

            if (deadEnemies.Count == 0)
                return;

            foreach (var enemy in deadEnemies)
                _enemyList.Remove(enemy);

            RefreshActorLists();
            EmitSignal(SignalName.PlayerTurnFinished);
            GD.Print("PlayerAction Signal");
        }

        public async Task EnqueueAction(ActionContext action)
        {
            if (action == null)
            {
                GD.PrintErr("EnqueueAction called with null ActionContext.");
                return;
            }

            _turnQueue.Enqueue(action);
            if (_isProcessing)
                return;

            _isProcessing = true;
            try
            {
                while (_turnQueue.Count > 0)
                {
                    var current = _turnQueue.Peek();
                    var executed = Execute(current);

                    if (executed)
                    {
                        if (current.Caller != null)
                        {
                            current.Caller.Atb = 0;
                            current.Caller.State = CombatState.Wait;
                        }
                        else
                        {
                            GD.PrintErr("EnqueueAction missing Caller.");
                        }
                    }

                    _turnQueue.Dequeue();
                    await Task.Yield();
                }
            }
            finally
            {
                _isProcessing = false;
            }

            bool Execute(ActionContext current)
            {
                var targets = current.Targets;
                var i = 0;
                var currentAct = current.SelectedAction;

                if (currentAct == null)
                {
                    GD.PrintErr("EnqueueAction missing SelectedAction.");
                    return false;
                }

                if (targets == null || targets.Count == 0)
                {
                    GD.PrintErr("EnqueueAction missing Targets.");
                    return false;
                }

                foreach (var target in targets)
                {
                    var result = currentAct(current, i);
                    var beforeHp = target.CurrentHp;
                    target.CurrentHp -= result.ActorDamage.Value;

                    GD.Print(beforeHp, " current hp => ", target.CurrentHp, " ", target.Name);
                    i++;
                }

                return true;
            }
        }
    }
}


    

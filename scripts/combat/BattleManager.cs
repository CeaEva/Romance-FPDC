using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;
using Resources;
using System.Runtime.Serialization.Formatters;

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
        readonly Dictionary<IActor, int> _stanceRecoveryTurns = new();
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
            PruneInvalidActors();

            foreach (var actor in _allActors)
            {
                if (!IsActorUsable(actor))
                    continue;

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
                if (!IsActorUsable(n))
                    continue;

                if (n.StanceValue > 0)
                {
                    n.StanceValue = Mathf.Max(0, n.StanceValue - (n.Stats.Spd / 2));
                    GD.Print(n.StanceValue + n.Name);
                }

                var stanceBreakThreshold = Mathf.RoundToInt(n.Stats.MaxHp * 0.75f) + n.Stats.Vit;
                if (n.StanceValue >= stanceBreakThreshold)
                    n.StanceBroken = true;

                if (n.StanceBroken)
                {
                    n.State = CombatState.Wait;
                    n.Atb = 0;
                    n.StanceValue = 0;
                    n.StanceBroken = false;
                    _stanceRecoveryTurns[n] = CalculateStanceRecoveryTurns(n);
                }
                
                if (n.State != CombatState.Wait)
                    continue;

                if (n.Atb >= atbMax && n is PlayerActor player)
                {
                    if (TryConsumeStanceRecoveryTurn(n))
                        continue;

                    player.StateControl(CombatState.Menu);
                    GD.Print("Player can menu");
                }

            }

            if (_enemyList.Count == 0)
            {
                GetParent()?.QueueFree();
                return;
            }

            var deadEnemies = new List<IActor>();

            foreach (var enemy in _enemyList)
            {
                if (!IsActorUsable(enemy))
                {
                    deadEnemies.Add(enemy);
                    continue;
                }

                if (enemy.Atb >= atbMax && enemy.State != CombatState.Queued)
                {
                    if (TryConsumeStanceRecoveryTurn(enemy))
                        continue;

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
            {
                _stanceRecoveryTurns.Remove(enemy);
                _enemyList.Remove(enemy);
            }

            RefreshActorLists();
            EmitSignal(SignalName.PlayerTurnFinished); //Let's menu nodes know when to update cursor elements 
            GD.Print("PlayerAction Signal");

        }

        private void PruneInvalidActors()
        {
            _allActors.RemoveAll(actor => !IsActorUsable(actor));
            _playerList.RemoveAll(actor => !IsActorUsable(actor));
            _enemyList.RemoveAll(actor => !IsActorUsable(actor));

            var staleRecoveryEntries = new List<IActor>();
            foreach (var pair in _stanceRecoveryTurns)
            {
                if (!IsActorUsable(pair.Key))
                    staleRecoveryEntries.Add(pair.Key);
            }

            foreach (var actor in staleRecoveryEntries)
                _stanceRecoveryTurns.Remove(actor);
        }

        private static bool IsActorUsable(IActor actor)
        {
            if (actor == null)
                return false;

            if (actor is not GodotObject godotObject)
                return true;

            if (!GodotObject.IsInstanceValid(godotObject))
                return false;

            if (actor is Node node && node.IsQueuedForDeletion())
                return false;

            return true;
        }

        private static int CalculateStanceRecoveryTurns(IActor actor)
        {
            return Mathf.Max(0, 3 - (actor.Stats.Vit / 10));
        }

        private bool TryConsumeStanceRecoveryTurn(IActor actor)
        {
            if (!_stanceRecoveryTurns.TryGetValue(actor, out var turnsRemaining) || turnsRemaining <= 0)
                return false;

            turnsRemaining--;
            actor.Atb = 0;

            if (turnsRemaining <= 0)
                _stanceRecoveryTurns.Remove(actor);
            else
                _stanceRecoveryTurns[actor] = turnsRemaining;

            GD.Print(actor.Name, " is stance-broken, turns left: ", turnsRemaining);
            return true;
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
                    var damage = result.ActorDamage.Value;
                    target.Damage(damage);  //Apply damage func to target for odohealth
                    target.AddStance(damage, current.Caller);

                    GD.Print(beforeHp, " current hp => ", target.CurrentHp, " ", target.Name);
                    i++;
                }

                return true;
            }
        }
    }
}


    

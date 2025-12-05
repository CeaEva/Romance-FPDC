using Godot;
using System;
using System.Collections.Generic;

namespace Combat
{
    

    public partial class BattleManager : Node
    {

        public List<PlayerActor> PlayerList
        {
            get => _playerList; private set
            {
                _playerList = value;
            }

        }

        public List<DummyEnemy> EnemyList
        {
            get => _enemyList; private set
            {
                _enemyList = value;
            }

        }

        List<Node> _allActors = new List<Node>();
        List<PlayerActor> _playerList = new List<PlayerActor>();
        List<DummyEnemy> _enemyList = new List<DummyEnemy>();
        //Queue _turnQueue = new();
        ProgressBar _atbBar;

        public override void _Ready()
        {
            
            ActorsToList("EnemyGroup");
            ActorsToList("PlayerGroup");
            GD.Print("Enemies: " + EnemyNames() +" " +" Player: " + _playerList);
            var atbTimer = GetNode<Timer>("AtbTimer");
            atbTimer.Timeout += AtbTick;
            _atbBar = GetNodeOrNull<ProgressBar>("%AtbBar");
            
        }

        public override void _Process(double delta)
        {
            

        }

        private void ActorsToList(string group)
        {
            var nodes = GetTree().GetNodesInGroup(group);

            foreach (Node node in nodes)
            {
                _allActors.Add(node);

                if (node is DummyEnemy dummy)
                    _enemyList.Add(dummy);
                else if (node is PlayerActor player)
                    _playerList.Add(player);

            }


        }

        private void AtbTick()
        {
            
            foreach (PlayerActor n in _playerList)
            {
                if (n.State != CombatState.Wait)
                    break;

                n.Atb += n.PlayerStats.Spd;
                GD.Print(n.Atb);
                _atbBar?.SetValueNoSignal(n.Atb);
            }

            foreach (DummyEnemy n in _enemyList)
            {
                if (n.State != CombatState.Wait)
                    break;

                n.Atb += n.EnemyStats.Spd;
                GD.Print(n.Atb + ", " + n.State);
                //atbBar.Value = n.Atb;
            }

            StateCheck();
        }

        private void StateCheck()
        {
            var atbMax = 100;

            foreach (PlayerActor n in _playerList)
            {
                if (n.State == CombatState.Menu || n.State == CombatState.Select)
                    return;

                if (n.Atb >= atbMax)
                {
                    n.State = CombatState.Menu;
                    GD.Print("Player can menu");
                }

            }

            foreach (DummyEnemy n in _enemyList)
            {
                if (n.Atb >= atbMax)
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
                enemyString = n.Name + "";

            }


            return enemyString;

        }


    }

}

using Godot;
using System.Threading.Tasks;
using DialogueManagerRuntime;
using GodotRng = Godot.RandomNumberGenerator;

namespace DC
{
    public partial class Player3d : CharacterBody3D
    {
        private const string ActionTurnRight = "TurnRight";
        private const string ActionTurnLeft = "TurnLeft";
        private const string ActionStepForward = "StepForward";
        private const string ActionDialogueTest = "DialougeTest";

        //Exports
        [Export] public float TurnDuration { get; set; } = 0.3f;
        [Export] public float StepDuration { get; set; }

        [Export(PropertyHint.Range, "0,1,0.01")]
        public float EnocunterPercent { get; set; }
        [Export] public int MinStepsBetweenEncounters { get; set; }

        //Flags
        public bool InEncounter = false;
        public bool IsInEncounter
        {
            get => InEncounter;
            set => InEncounter = value;
        }

        private readonly GodotRng _rng = new GodotRng();
        private int _stepsSinceLast;

        //Nodes
        private GridMap _grid;
        private Node3D _node3d;
        private Camera3D _camera;
        private AnimationPlayer _animationPlayer;
        private RichTextLabel _dialogueLabel;
        private Tween _turnTween;
        private PackedScene _combatTscn;
        private Node _combatInstance;

        public Tween TurningTween => _turnTween;
        public GridMap Grid => _grid;
        public Node3D Node3DRoot => _node3d;

        public override void _Ready()
        {
            //Establishing Node Path's
            _grid = GetNodeOrNull<GridMap>("../GridMap");
            _node3d = GetNodeOrNull<Node3D>("..");
            _camera = GetNodeOrNull<Camera3D>("../Player3d/Camera3D");
            _animationPlayer = GetNodeOrNull<AnimationPlayer>("../DialougeUIBackground");
            _dialogueLabel = GetNodeOrNull<RichTextLabel>("../CanvasLayer/Control/TextureRect/DialogueLabel");
            _combatTscn = ResourceLoader.Load<PackedScene>("res://scenes/ui/CombatScn.tscn");
            NodeTest(_grid);
            NodeTest(_camera);
            Stabilize(GlobalPosition);
            GD.Print(_stepsSinceLast);
            _rng.Randomize();
        }

        public override void _PhysicsProcess(double delta)
        {

           // PlayerMove();

        }


        public void NodeTest(Node TestNode) //Good for testing wether a node exsist's in tree
        {
            if (TestNode == null) // Test' wether GirdMap is in NodeTree
            {
                GD.Print("No Path Found");
            }
            else
            {
                GD.Print("Path Found");

            }
        }

        private void Stabilize(Vector3 GivenPosition) //Get's Cell center Cordinants and makes it the GlobalPosition
        {

            var localPos = _node3d.ToLocal(GivenPosition); // GlobalPos -> LocalPos

            var localMap = _grid.LocalToMap(localPos);
            var newGlobalPos = _grid.MapToLocal(localMap);
            newGlobalPos = _node3d.ToGlobal(newGlobalPos);
            GlobalPosition = newGlobalPos;

            GD.Print($"Cell: {GlobalPosition}");


        }

        public async void PlayerMove() // Not _UnhandledInput due to needing continuious held input's needed for movement
        {

            if (_turnTween?.IsRunning() == true || InEncounter) return;
            if (_grid == null || _node3d == null) return;

            var currentRotation = GlobalRotationDegrees;
            float targetLocation;

            if (Input.IsActionPressed(ActionTurnRight))
            {
                targetLocation = currentRotation.Y -= 90f;
                GD.Print($"{currentRotation}");
                await TurnTween(targetLocation);
            }

            else if (Input.IsActionPressed(ActionTurnLeft))
            {
                targetLocation = currentRotation.Y += 90f;
                GD.Print($"{currentRotation}");
                await TurnTween(targetLocation);
            }

            else if (Input.IsActionPressed(ActionStepForward))
            {
                float stepDistance = _grid.CellSize.Z;
                // Forward direction is the node's -Z axis in Godot
                await MoveTween(stepDistance);

                EncounterChance();
            }

            else if (Input.IsActionJustPressed(ActionDialogueTest))
            {
                ShowD();
            }

            else
            {
                return;
            }

        }

        public async Task TurnTween(float targetY)
        {
            _turnTween?.Kill();
            _turnTween = CreateTween()
                .SetProcessMode(Tween.TweenProcessMode.Physics)
                .SetEase(Tween.EaseType.InOut)
                .SetTrans(Tween.TransitionType.Cubic);


            _turnTween.TweenProperty(this, "global_rotation_degrees:y", targetY, TurnDuration);

            await ToSignal(_turnTween, Tween.SignalName.Finished);

            _turnTween = null;
            Stabilize(GlobalPosition);

        }

        public async Task MoveTween(float stepDistance)
        {
            _turnTween?.Kill();
            Vector3 forward = (-GlobalTransform.Basis.Z).Normalized();
            Vector3 targetPos = GlobalPosition + forward * stepDistance;
            _turnTween = CreateTween()
               .SetProcessMode(Tween.TweenProcessMode.Physics)
               .SetTrans(Tween.TransitionType.Linear);


            _turnTween.TweenProperty(this, "global_position", targetPos, StepDuration);

            await ToSignal(_turnTween, Tween.SignalName.Finished);

            _turnTween = null;
            Stabilize(GlobalPosition);
        }

        private void ShowD() //Simple Dialogue test
        {

            var dialogue = GD.Load<Resource>("res://data/Dialogue/waaa.dialogue");
            DialogueManager.ShowDialogueBalloon(dialogue, "start");

        }

        public void EncounterChance()
        {

            if (InEncounter) return;

            _stepsSinceLast++;

            if (_stepsSinceLast < MinStepsBetweenEncounters)
                return;


            if (_rng.Randf() <= EnocunterPercent)
                StartEncounter();

        }

        private void StartEncounter()
        {
            if (_combatInstance != null && IsInstanceValid(_combatInstance))
            {
                GD.Print("Encounter already running; skipping duplicate spawn.");
                return;
            }
            InEncounter = true;
            _combatInstance = _combatTscn.Instantiate();
            AddChild(_combatInstance);
            _combatInstance.TreeExiting += () =>
            {
                _combatInstance = null;
                InEncounter = false;
                _stepsSinceLast = 0;
            };

            GD.Print("Encounter started :");
        }
    }
}

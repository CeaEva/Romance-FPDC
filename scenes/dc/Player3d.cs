using Godot;

public partial class Player3d : CharacterBody3D
{

    [Export] public float TurnDuration { get; set; } = 0.3f;

    private GridMap _grid;
    private Node3D _node3d;
    private Camera3D _camera;

    private Tween _turnTween;

    public override void _Ready()
    {

        _grid = GetNodeOrNull<GridMap>("../GridMap");
        _node3d = GetNodeOrNull<Node3D>("..");
        _camera = GetNodeOrNull<Camera3D>("../Player3d/Camera3D");

        NodeTest(_grid);
        NodeTest(_camera);
        Stabilize(GlobalPosition);
    }

    public override void _PhysicsProcess(double delta)
    {
        PlayerTurn();


    }

    public void NodeTest(Node TestNode)
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

        _node3d.ToLocal(GivenPosition); // GlobalPos -> LocalPos

        var localPos = _grid.LocalToMap(GivenPosition);
        var newGlobalPos = _grid.MapToLocal(localPos);
        newGlobalPos = _node3d.ToGlobal(newGlobalPos);
        GlobalPosition = newGlobalPos;

        GD.Print($"Cell: {GlobalPosition}");

        
    }

    private void PlayerTurn()
    {

        if (_turnTween?.IsRunning() == true) return;

        var CurrentRotation = GlobalRotationDegrees;
        float targetLocation = CurrentRotation.Y;
        var CurrentPosition = GlobalPosition;
        var TargetPosition = CurrentPosition;
        

        string Tr = "TurnRight";
        string Tl = "TurnLeft";
        string Ta = "TrnRight";
        string Sf = "StepForward";


        if (Input.IsActionJustPressed(Tr))
        {
            targetLocation = CurrentRotation.Y -= 90f;
            GD.Print($"{CurrentRotation}");
            TurnTween(targetLocation);
        }

        else if (Input.IsActionJustPressed(Tl))
        {
            targetLocation = CurrentRotation.Y += 90f;
            GD.Print($"{CurrentRotation}");
            TurnTween(targetLocation);
        }

        else if (Input.IsActionJustPressed(Sf))
        {

             float stepDistance = _grid.CellSize.Z;
             // Forward direction is the node's -Z axis in Godot
             Vector3 forward = -GlobalTransform.Basis.Z;

             // Update position
             GlobalPosition += forward * stepDistance;
        }

        else
        {
            return;
        }
             
           
        
    }

    private void TurnTween(float targetY)
    {
        _turnTween?.Kill();
        _turnTween = CreateTween()
            .SetProcessMode(Tween.TweenProcessMode.Physics)        
            .SetEase(Tween.EaseType.InOut)
            .SetTrans(Tween.TransitionType.Cubic);

        
        _turnTween.TweenProperty(this, "global_rotation_degrees:y", targetY, TurnDuration);

    }
}

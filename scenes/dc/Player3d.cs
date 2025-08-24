using Godot;

public partial class Player3d : CharacterBody3D
{

    [Export] public float TurnDuration { get; set; } = 0.3f;

    private GridMap _grid;
    private Node3D _node3d;

    private Tween _turnTween;

    public override void _Ready()
    {

        _grid = GetNodeOrNull<GridMap>("../GridMap");
        _node3d = GetNodeOrNull<Node3D>("..");

        NodeTest(_grid);
        NodeTest(_node3d);
        Stabilize(GlobalPosition);


    }

    public override void _PhysicsProcess(double delta)
    {

        PlayerTurn("TurnRight");
        PlayerTurn("TurnLeft");

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

    private void PlayerTurn(string key)
    {

        if (_turnTween?.IsRunning() == true) return;

        var CurrentRotation = GlobalRotationDegrees;
        float targetLocation = CurrentRotation.Y;

        switch (key)
        {
            case "TurnRight":
                if (Input.IsActionJustPressed(key))
                {
                    targetLocation = CurrentRotation.Y -= 90f;
                    GD.Print($"{CurrentRotation}");
                    TurnTween(targetLocation);
                }
                break;
            case "TurnLeft":
                if (Input.IsActionJustPressed(key))
                {
                   targetLocation = CurrentRotation.Y += 90f;
                    GD.Print($"{CurrentRotation}");
                    TurnTween(targetLocation);
                }
                break;

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

using DC;
using Godot;

public partial class AutoMapLayer3 : Control
{
    [Export]
    public PackedScene AutoMapTscn { get; set; }
    [Export] 
    public NodePath SubViewportPath { get; set; }
    [Export] public NodePath Player3DPath { get; set; }
    private SubViewport _subViewport;
    private Player3d _player3D;

    public override void _Ready()
    {
        if (AutoMapTscn == null)
        {
            GD.PushError("AutoMapLayer3: No AutoMapTscn provided ");
            return;
        }
        if (SubViewportPath.IsEmpty)
        {
            GD.PushError("AutoMapLayer3: No subViewportpath provided ");
            return;
        }
        if (Player3DPath.IsEmpty)
        {
            GD.PushError("AutoMapLayer3: No Player3DPath provided");
            return;
        }

        _subViewport = GetNodeOrNull<SubViewport>(SubViewportPath);
        _player3D = GetNodeOrNull<Player3d>(Player3DPath);
        if (_subViewport == null)
        {
            GD.PushError($"AutoMapLayer3: SubViewport not found at '{SubViewportPath}'.");
            return;
        }
        if (_player3D == null)
        {
            GD.PushError($"AutoMapLayer3: Player3d not found at '{Player3DPath}'.");
            return;
        }

        var autoMapInstance = (AutoMapRoot)AutoMapTscn.Instantiate();
        autoMapInstance.Initialize(_player3D);
        _subViewport.AddChild(autoMapInstance);

    }

}

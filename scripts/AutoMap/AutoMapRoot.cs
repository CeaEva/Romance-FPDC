using DC;
using Godot;

public partial class AutoMapRoot : Node2D
{
    public Player3d Player3D { get; set; }

    public void Initialize(Player3d player3D)
    {
        Player3D = player3D;

    }
}

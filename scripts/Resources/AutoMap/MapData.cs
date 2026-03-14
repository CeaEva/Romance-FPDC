using Godot;
using Godot.Collections;

namespace Resources{

    [GlobalClass]
    public partial class MapData : Resource
    {
        [Export] public Array<Vector2I> UsedCellData { get; set; } = new();
        [Export] public Dictionary<Vector2I, Vector2I> CellSourceByCoord { get; set; } = new();

    }
}

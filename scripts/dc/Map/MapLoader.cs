using Godot;
using Godot.Collections;
using Resources;

namespace AutoMap{
    
    public partial class MapLoader : Node
    {
        [Export] public MapData MapData;
        [Export] private GridMap _gridMap;
        [Export] private int _meshItemId = 0;

        public override void _Ready()
        {
            if (MapData == null)
            {
                GD.PushError("MapLoader: No MapData resource assigned. Assign a .tres resource, not the MapData.cs script.");
                return;
            }

            if (_gridMap == null)
            {
                GD.PushError("MapLoader: No GridMap assigned.");
                return;
            }

            MeshLibrary meshLibrary = _gridMap.MeshLibrary;
            if (meshLibrary == null)
            {
                GD.PushError("MapLoader: GridMap has no MeshLibrary assigned.");
                return;
            }

            Array<Vector2I> usedCellData = MapData.UsedCellData;
            if (usedCellData == null || usedCellData.Count == 0)
            {
                GD.Print("MapLoader: MapData has no used cells.");
                return;
            }

            if (System.Array.IndexOf(meshLibrary.GetItemList(), _meshItemId) < 0)
            {
                GD.PushError($"MapLoader: Mesh item id {_meshItemId} not found in MeshLibrary.");
                return;
            }

            for (int i = 0; i < usedCellData.Count; i++)
            {
                Vector3 localpos;
                Vector3I meshPos = new Vector3I(usedCellData[i].X, -1, usedCellData[i].Y);
                localpos = _gridMap.MapToLocal(meshPos);
                Vector3 globalPos = _gridMap.ToGlobal(localpos);
                _gridMap.SetCellItem(meshPos, _meshItemId);
            }
        }

    }
}
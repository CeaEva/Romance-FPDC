using Godot;
using Godot.Collections;
using Resources;

public partial class MapDataCompiler : Node
{
    [Export] public TileMapLayer TileMapLayer;
    [Export] public string SavePath = "res://data/resources/map_data.tres";
    private Array<Vector2I> _cellData;
    private MapData _mapData = new();

    public override void _Ready()
    {
        // Fallback to a child TileMapLayer if the export field was not assigned in the editor.
        TileMapLayer ??= GetNodeOrNull<TileMapLayer>("TileMapLayer");
        if (TileMapLayer == null)
        {
            GD.PushError("MapDataCompiler: TileMapLayer is null. Assign the exported TileMapLayer field in the inspector.");
            return;
        }

        _cellData = TileMapLayer.GetUsedCells();
        _mapData.UsedCellData.Clear();
        _mapData.CellSourceByCoord.Clear();

        for (int i = 0; i < _cellData.Count; i++)
        {
            _mapData.UsedCellData.Add(_cellData[i]);
            _mapData.CellSourceByCoord[_cellData[i]] = TileMapLayer.GetCellAtlasCoords(_cellData[i]);
        }

        SaveMapData();
    }

    private void SaveMapData()
    {
        string directory = SavePath.GetBaseDir();
        Error dirError = DirAccess.MakeDirRecursiveAbsolute(directory);
        if (dirError != Error.Ok)
        {
            GD.PushError($"MapDataCompiler: Failed to create directory '{directory}'. Error: {dirError}");
            return;
        }

        Error saveError = ResourceSaver.Save(_mapData, SavePath);
        if (saveError != Error.Ok)
        {
            GD.PushError($"MapDataCompiler: Failed to save map data to '{SavePath}'. Error: {saveError}");
            return;
        }

        GD.Print($"MapDataCompiler: Saved map data to {SavePath}");
    }
}

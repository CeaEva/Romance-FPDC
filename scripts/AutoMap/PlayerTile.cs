using DC;
using Godot;

namespace AutoMap
{    
    public partial class PlayerTile : Area2D
    {
        [Signal]
        public delegate void SendProjectedPosEventHandler(Vector2I projectedPos);
        [Signal]
        public delegate void TurningSignalEventHandler(string direction);
        [Export]
        public TileMapLayer TileMap;
        [Export]
        public Sprite2D PlayerTileSprite;
        [Export]
        public NodePath RootPath;
        [Export] public float RotationOffset2D = 0f;
        public Vector2I ProjectedPos;
        private AutoMapRoot root;
        private bool isStepping;
        

        public override void _Ready()
        {
            StabilizeToClosestCell();
            root = GetNodeOrNull<AutoMapRoot>(RootPath);
            
        }

        public override void _PhysicsProcess(double delta)
        {
            PlayerMove();
        }


        public async void PlayerMove()
        {
            var player3D = root.Player3D;
            
            if (player3D.TurningTween?.IsRunning() == true || player3D.IsInEncounter) return;
            if (player3D.Grid == null || player3D.Node3DRoot == null) return;
            

            float projectedRotation;

            if (isStepping)
                return;

            if (TileMap == null || root?.Player3D == null)
                return;

            if (Input.IsActionPressed("StepForward"))
            {
                ProjectedPos = GetProjectedPos("StepForward");
                if (CollisionCheck(ProjectedPos))
                    return;
                await root.Player3D.MoveTween(4);
                UpdatePos(ProjectedPos);
                player3D.EncounterChance();
            }
            if (Input.IsActionPressed("TurnRight"))
            {
                projectedRotation = GetRotationDegrees("TurnRight");
                await root.Player3D.TurnTween(projectedRotation);
                UpdateRotationFrom3D();
            }
            if (Input.IsActionPressed("TurnLeft"))
            {
                projectedRotation = GetRotationDegrees("TurnLeft");
                await root.Player3D.TurnTween(projectedRotation);
                UpdateRotationFrom3D();
            }


            Vector2I GetProjectedPos(string key)
            {
                Vector2 currentLocal = TileMap.ToLocal(GlobalPosition);
                Vector2I currentCell = TileMap.LocalToMap(currentLocal);

                switch (key)
                {
                    case "StepForward":
                        float yaw = Mathf.Wrap(root.Player3D.GlobalRotationDegrees.Y, 0f, 360f);
                        int dirIndex = Mathf.PosMod(Mathf.RoundToInt(yaw / 90f), 4);
                        Vector2I forwardOffset = dirIndex switch
                        {
                            0 => Vector2I.Up,    // 0 deg: -Z (north)
                            1 => Vector2I.Left,  // 90 deg: -X (west)
                            2 => Vector2I.Down,  // 180 deg: +Z (south)
                            _ => Vector2I.Right, // 270 deg: +X (east)
                        };
                        return currentCell + forwardOffset;
                }
                return currentCell;

            }

            float GetRotationDegrees(string key)
            {
                var currentRotation = root.Player3D.GlobalRotationDegrees.Y;

                switch (key)
                {
                    case "TurnRight":
                        return currentRotation - 90f;
                    case "TurnLeft":
                        return currentRotation + 90f;
                }                
                return currentRotation;

            }

            void UpdatePos(Vector2I projectedPos)
            {
                Vector2 localPos = TileMap.MapToLocal(projectedPos);
                Vector2 globalPos = TileMap.ToGlobal(localPos);
                GlobalPosition = globalPos;
                EmitSignal(SignalName.SendProjectedPos, ProjectedPos);
                GD.Print($"Cell: {projectedPos} World: {GlobalPosition}");
                
            }

            bool CollisionCheck(Vector2I projectedPos)
            {
                var nextCell = TileMap.GetCellTileData(projectedPos);
                if (nextCell == null)
                    return true; // treat empty/off-map as blocked

                var collisionData = nextCell.GetCustomData("IsCollision");
                if (collisionData.VariantType == Variant.Type.Nil)
                    return false;

                return (bool)collisionData;
            }
        }

        private void StabilizeToClosestCell()
        {
            if (TileMap == null)
                return;

            Vector2 localPos = TileMap.ToLocal(GlobalPosition);
            Vector2I nearestCell = TileMap.LocalToMap(localPos);
            Vector2 snappedLocal = TileMap.MapToLocal(nearestCell);
            GlobalPosition = TileMap.ToGlobal(snappedLocal);
        }

        private void UpdateRotationFrom3D()
        {
            if (root?.Player3D == null)
                return;

            float yaw3D = Mathf.Wrap(root.Player3D.GlobalRotationDegrees.Y, 0f, 360f);
            float rotation2D = Mathf.Wrap(-yaw3D + RotationOffset2D, -180f, 180f);
            GlobalRotationDegrees = rotation2D;
        }

    }

}

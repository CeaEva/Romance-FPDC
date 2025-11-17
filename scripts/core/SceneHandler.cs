// SceneHandler.cs
using Godot;

namespace Core
{
    public partial class SceneHandler : Node
    {
        // Where to put the loaded scene. By default, this node (".").
        // If you later add a dedicated container (e.g., a sibling "SceneRoot"),
        // set this to "../SceneRoot" in the Inspector.
        [Export] public NodePath ContainerPath { get; set; } = ".";

        // Paths to your scenes (editable in the Inspector via a file picker).
        [Export(PropertyHint.File, "*.tscn")]
        public string OverworldPath { get; set; } = "res://scenes/overworld2d/Overworld2d.tscn";

        [Export(PropertyHint.File, "*.tscn")]
        public string DcEnvPath { get; set; } = "res://scenes/dc/DcEnv.tscn";

        private Node _container;

        public override void _Ready()
        {
            // Resolve the container (relative NodePath: "." means "this node").
            _container = GetNode<Node>(ContainerPath); // resolves in _Ready(), after entering the scene tree. :contentReference[oaicite:0]{index=0}

            // Expect the parent to be your Main node with the Game script attached.
            var game = GetParent() as Game;
            if (game == null)
            {
                GD.PushError("SceneHandler: Parent doesn't have the Game script. Attach Game to the Main node.");
                return;
            }

            // Load the correct scene based on the enum on Main.
            ApplyMode(game.CurrentMode);
        }

        /// <summary>Loads the scene that corresponds to the given GameMode.</summary>
        public void ApplyMode(Game.GameMode mode)
        {
            string path = mode == Game.GameMode.Overworld2d ? OverworldPath : DcEnvPath;
            ReplaceWith(path);
        }

        /// <summary>Clears the container and instances the scene at scenePath into it.</summary>
        public Node ReplaceWith(string scenePath)
        {
            var packed = ResourceLoader.Load<PackedScene>(scenePath);     // load .tscn as a resource. :contentReference[oaicite:1]{index=1}
            if (packed == null)
            {
                GD.PushError($"SceneHandler: Failed to load PackedScene at '{scenePath}'.");
                return null;
            }

            // Remove any existing children safely (deferred until end of frame).
            foreach (Node child in _container.GetChildren())
                child.QueueFree();                                        // safe deletion at frame end. :contentReference[oaicite:2]{index=2}

            // Turn the PackedScene into live nodes and add it to the tree.
            var inst = packed.Instantiate();                              // create node tree from the scene. :contentReference[oaicite:3]{index=3}
            _container.AddChild(inst);                                    // parent it under the container. :contentReference[oaicite:4]{index=4}
            return inst;
        }
        public override void _UnhandledInput(InputEvent e)
        {

            if (e.IsActionPressed("Dev_ToggleDebug"))
            {
                var game = GetParent() as Game;
                if (game.CurrentMode == Game.GameMode.Overworld2d)
                {
                    ApplyMode(Game.GameMode.DcEnv);
                }
                else
                {
                    ApplyMode(Game.GameMode.Overworld2d);
                }
            }
        }
    }
}
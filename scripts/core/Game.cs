using Godot;
using System;

namespace Core
{
    public partial class Game : Node
    {

        // private AnimationPlayer _animationPlayer;
        public override void _Ready()
        {
            // Preserve aspect ratio + internal resolution (uses your Project Settings base size).
            var root = GetTree().Root; // This is the main Window.
                                       //var camera = GetNode<AnimationPlayer>("/root/Main/Player/Camera3D");
            root.ContentScaleMode = Window.ContentScaleModeEnum.Viewport; // scale the whole viewport
            root.ContentScaleAspect = Window.ContentScaleAspectEnum.Keep;   // keep aspect; add letterboxing if needed

            int baseW = (int)ProjectSettings.GetSetting("display/window/size/viewport_width");
            int baseH = (int)ProjectSettings.GetSetting("display/window/size/viewport_height");
            root.ContentScaleSize = new Vector2I(baseW, baseH);             // internal/base resolution

            // Go fullscreen.
            GetWindow().Mode = Window.ModeEnum.Fullscreen;

            NewSave.I.ElleStats.MaxHp += 10;
            GD.Print(NewSave.I.ElleStats.MaxHp);
        }

        public enum GameMode
        {
            Overworld2d,
            DcEnv
        }

        [Export] public GameMode CurrentMode = GameMode.Overworld2d;

        // Optional: press F4 (action name: "fullscreen") to toggle fullscreen on/off.
        public override void _UnhandledInput(InputEvent e)
        {
            if (e.IsActionPressed("Fullscreen"))
            {
                var w = GetWindow();
                w.Mode = w.Mode == Window.ModeEnum.Fullscreen
                    ? Window.ModeEnum.Windowed
                    : Window.ModeEnum.Fullscreen;
            }
            if (e.IsActionPressed("Dev_ToggleDebug"))
            {
                CurrentMode = (CurrentMode == GameMode.Overworld2d) ? GameMode.DcEnv : GameMode.Overworld2d;
                GD.Print($"Mode is now: {CurrentMode}");
                GetViewport().SetInputAsHandled();
            }
            if (e is InputEventKey key
                   && key.Pressed
                   && !key.Echo
                   && key.Keycode == Key.Escape)
            {
                GetTree().Quit();
            }
        }

    }
}


    

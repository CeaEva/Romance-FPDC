using Godot;
using System;

public partial class ValueRef : Node
{
    [Export] public double[] TileSize2d = new double[2] { 128, 128 };
    [Export] public double[] GridSize3d = new double[3] { 4, 4, 4 };
    [Export] public double StepTime = 0.20;
    [Export] public double TurnTime = 0.15;
}

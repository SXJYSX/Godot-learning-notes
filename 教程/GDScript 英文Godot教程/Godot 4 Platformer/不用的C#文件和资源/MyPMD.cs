using Godot;
using System.Numerics;


[GlobalClass]
public partial class MyPMD : Resource
{
    [Export] public float Speed { get; set; } = 100.0f;         // 水平移速 (像素/秒)
    [Export] public float Acceleration { get; set; } = 800.0f;  // 加速度 (像素/秒²)
    [Export] public float Friction { get; set; } = 1000.0f;     // 摩擦力 (像素/秒²)
    [Export] public float JumpVelocity { get; set; } = -300.0f; // 跳跃速度，负值向上

    [Export] public float fff = -300.0f; // 跳跃速度，负值向上



    public float TestMT()
    {
        return JumpVelocity;
    }
}
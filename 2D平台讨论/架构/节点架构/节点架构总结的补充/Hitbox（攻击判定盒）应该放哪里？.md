笔记中写到了 `Hurtbox`（受击盒，在 `Omni` 下），但缺失了 `Hitbox`（主攻判定盒，如剑挥砍、怪物咬人）：

- **推荐归宿**：
    
    - **方式 A（动画驱动型）**：放在 **`Pivot/Hitbox`** 下。因为攻击框往往要贴合动画帧的挥剑轨迹，随 `Pivot` 翻转最省心（`Area2D` 不参与物理阻挡，放在 `Pivot` 翻转通常没问题）。
        
    - **方式 B（物理精确型）**：放在 **`Sensors/Directional/Hitbox`** 下，由 `AnimationPlayer` 仅控制其 `disabled` 属性的开关，方向由 `sensors.gd` 代码驱动。
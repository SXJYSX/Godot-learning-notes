在笔记中提到了 _“配合功能翻转，修改 Directional 的 scale.x”_：

- **隐患**：你在 Player 的 `Directional` 下挂载了 `InteractableCast`（可能包含 `ShapeCast2D`）。如果直接给 `Directional` 节点设置 `scale.x = -1`， Godot 4 物理引擎会在控制台弹出 `Global transform matrix has negative determinant` 警告，且可能导致形状判定失真。
    
- **修正建议**（在笔记中补充这一句）：
    
    > **`Sensors/Directional` 本身保持 `scale = (1, 1)`**。翻转时，由 `sensors.gd` 遍历 `Directional` 的子节点，用代码修改 `target_position.x = abs(x) * facing`（如果需要当场拿数据，紧跟一
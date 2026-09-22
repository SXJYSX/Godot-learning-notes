空间数学坐标在修改 `scale.x` 后会在内存中**微秒级即时更新**，但物理引擎**绝对不会自动重算碰撞检测**。

同帧内要同时获得“正确坐标”和“正确检测”，各检测节点在操作方式和安全性上存在本质差异：

|**节点类型**|**同帧坐标状态**|**同帧物理检测状态**|**刷新方法与安全避坑**|
|---|---|---|---|
|**Marker2D / 纯 Node2D**|⚡️ 瞬间正确|➖ 无检测属性|**完全安全**。随意修改 `scale.x = -1`，下两行代码直接取 `global_position`。|
|**RayCast2D**|⚡️ 瞬间正确|⚠️ 需要手动强刷|**安全**。改完方向后**必须调用 `force_raycast_update()`**，才能同帧拿到碰撞结果。|
|**ShapeCast2D**|⚡️ 瞬间正确|❌ **不建议改 scale**|**高危**。虽可调用 `force_shapecast_update()` 强刷，但负缩放会导致 `Negative Determinant` 警告及法线反转。**应改 `target_position.x` 而非 `scale`**。|
|**Area2D**|⚡️ 瞬间正确|⏳ **延迟一帧**|**受限**。物理引擎的 Area 重叠（Overlap）判定没有同帧强刷 API，`get_overlapping_bodies()` 无法当场拿到新重叠数据，必须等待物理步。|
|**CollisionShape2D**|⚡️ 瞬间正确|💥 **崩溃/穿墙**|**绝对禁止**。物理主体及物理形状不可负缩放，会导致 `move_and_slide()` 等底层算法失效。|

### 各核心节点的同帧处理规范

**1. RayCast2D（一维射线：支持 scale 翻转 + 手动强刷）**

GDScript

```
# 1. 改变方向（缩放或改 target_position）
raycast.scale.x = -1  # 或 raycast.target_position.x = -abs(raycast.target_position.x)

# 2. 强行拉取物理引擎计算
raycast.force_raycast_update()

# 3. 同帧读取：坐标与检测结果均绝对真实
var hit_point = raycast.get_collision_point() # 真实坐标
var is_hit = raycast.is_colliding()           # 真实检测结果
```

**2. ShapeCast2D（多维扫掠：严禁 scale 翻转，改 target_position + 手动强刷）**

GDScript

```
# ❌ 错误：shapecast.scale.x = -1（触发引擎 Warning 并导致法线错误）

# ✅ 正确：保持 scale=(1,1)，通过代码反转位移向量
shapecast.target_position.x = -abs(shapecast.target_position.x)

# 强行拉取物理引擎计算
shapecast.force_shapecast_update()

# 同帧读取：碰撞体与法线完全正确
if shapecast.is_colliding():
	var normal = shapecast.get_collision_normal(0)
```

**3. Area2D（重叠区域：同帧改坐标，物理检测延迟）**

GDScript

```
# 修改 Area2D 的位置/面向
area.scale.x = -1 

var pos = area.global_position # ⚡️ 坐标瞬间正确

# ⚠️ 注意：get_overlapping_bodies() 当场拿到的依然是修改前的旧重叠列表！
# Area2D 依赖 PhysicsServer2D 的帧末 Flush，无法在同一行代码里强制刷新重叠判定。
```

### 总结铁律

- **想当场拿物理检测结果**：必须在 `scale` 或 `target_position` 发生变动后的下一行，显式调用 `force_raycast_update()` 或 `force_shapecast_update()`。
    
- **ShapeCast2D & Area2D**：永远通过修改 `target_position.x` 或偏移量来切方向，保持 `scale` 恒为 `(1, 1)`，避免物理矩阵失效。
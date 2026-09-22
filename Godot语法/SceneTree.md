以这行代码为例：
var hearts = get_tree().get_nodes_in_group(GameGroups.HEARTS)

SceneTree是啥？get_tree()又是啥？
https://www.youtube.com/watch?v=KwRA6-E3OfI
----

`get_tree()` 是 `Node` 节点类提供的一个内置方法，它的作用是**获取当前游戏运行时的场景树管理者（`SceneTree` 实例）**。

在 Godot 中，所有挂载到场景里的节点，都生长在一棵统一的“大树”上。`get_tree()` 就是你向引擎索要**整棵树的总控制器**的接口。

### `SceneTree`（场景树）的 4 大核心能力

通过 `get_tree()` 拿到的 `SceneTree` 对象，是整个游戏运行时的总指挥部：

- **1. 场景与关卡切换**
    
    GDScript
    
    ```
    get_tree().change_scene_to_file("res://levels/level_02.tscn") # 切换场景
    get_tree().reload_current_scene()                             # 重新加载当前关卡
    get_tree().quit()                                             # 退出游戏
    ```
    
- **2. 全局分组搜寻与批量操作**
    
    GDScript
    
    ```
    var hearts = get_tree().get_nodes_in_group(&"Hearts") # 获取组内所有节点
    get_tree().call_group(&"Enemies", "die")             # 批量调用组内所有节点的方法
    ```
    
- **3. 游戏暂停与时间驱动**
    
    GDScript
    
    ```
    get_tree().paused = true # 暂停游戏（所有 process 和 physics 将停止运行）
    ```
    
- **4. 便捷工具（定时器与补间动画）**
    
    GDScript
    
    ```
    # 无需创建 Timer 节点，直接等待 1.5 秒
    await get_tree().create_timer(1.5).timeout
    ```

---
### **补充信息：**

**`get_tree()` 的返回类型是什么？** 它的返回类型就是 **`SceneTree`** 类的对象（实例）

---

### 在任何运行中的标准 Godot 游戏进程内部，**“SceneTree 类型的对象有且仅有一个”

_底层架构与边界条件拆解：
- **多窗口与多视口（SubViewport）**：在 Godot 4 中，即便开启多窗口或使用多个 `SubViewport` 渲染离屏画面，这些 `Window` 与 `Viewport` 节点依然全数挂载在同一个根节点（`root`）下方，受同一个 `SceneTree` 调度。
    
- **多线程（Multithreading）**：GDScript 中的 `Thread` 可以并行处理大量数值计算或后台资源加载，但场景树的节点状态更新、渲染与生命周期回调依然由主线程中的那唯一一个 `SceneTree` 统一管理。
    
- **多进程运行**：如果在编辑器中测试多人游戏同时启动了多个游戏窗口，这在操作系统层面属于**多个独立的 OS 进程**。每个进程各自拥有且仅有一个 `SceneTree`，彼此之间内存互不共享。
    
- **极罕见的 C++ 自定义底层**：在 C++ 引擎扩展层面，开发者理论上可以重写 `MainLoop` 实现一个完全不依赖节点系统的自定义主循环（此时 `SceneTree` 数量为 0）。但在所有标准的 Godot 游戏开发项目中，游戏进程必须且只能依赖一个 `SceneTree` 运作。

---
## 时间复杂度:**O(N)**

Q.  get_tree().get_nodes_in_group(&"Hearts") 这行代码的时间复杂度是多少？

A.  时间复杂度是 **O(N)**，其中 N 是**当前组内（即标记为 `"Hearts"`）的节点数量**，而不是整个场景树中的节点总数。

这行代码在 Godot 底层的具体执行过程分为两个阶段：

- **1. 查找目标组（Hash Lookup）——平均 O(1)** `SceneTree` 底层维护了一个哈希表（`HashMap<StringName, Group>`）。当你传入 `&"Hearts"` 时，引擎直接根据 `StringName` 的唯一内存哈希值进行精准定位，**完全不会去遍历整棵场景树**。
    
- **2. 封装并返回数组（Array Generation）—— O(N)** 定位到目标组后，Godot 需要将该组内保存的 N 个节点指针，逐个复制并封装进一个新的 GDScript `Array` 中返回。这一步的耗时严格与组内节点数 N 成正比。
    

**关键性能结论与优化建议**

- **不随场景规模膨胀**：假设你的关卡里有 10,000 个场景节点，但 `"Hearts"` 组里只有 5 个节点，这行代码的开销**仅与这 5 个节点相关**，执行速度极快。
    
- **为什么依然建议在 `_ready()` 中缓存**：虽然它非常高效，但每次调用都会在内存里**重新分配并创建一个新的数组对象**。如果写在 `_process()` 或 `_physics_process()` 这种每帧触发的代码块里，会带来不必要的内存分配与垃圾回收（GC）开销。
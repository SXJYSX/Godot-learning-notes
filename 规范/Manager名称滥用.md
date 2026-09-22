是否有命名规范 用以区分 局部 Manager 与 全局Manager ？

你看，LevelManager 和 SceneManager，level在中文的意思是关卡，scene在中文的意思是场景，很难看出这俩有啥区别

但在你之前的回答中，LevelManager 是每个关卡都有一个，SceneManager是全局单例 

---
在工业界开发中，单纯依赖 `XXXManager` 确实极易造成语义混淆。为了让团队成员或自己在一分钟内看懂代码作用域，业界通常不依靠词义差异（如 Level vs Scene），而是依靠**特定前缀/后缀命名约定**来强行区分。

### 1. 全局单例（Autoload）：改用 Service / System 或加 Global 前缀

全局单例是“后台基础设施”，它的生命周期与游戏进程同寿。为了将其与局部节点区分，通常采用以下命名规范：

- **用 `Service`（服务）或 `System`（系统）代替 `Manager`**：
    
    - `SceneService` / `SceneSystem`（比 `SceneManager` 更有“底层基础设施”的感觉）
        
    - `AudioService`（全局音频服务）
        
    - `SaveSystem`（全局存档系统）
        
- **显式添加 `Global` 或 `Game` 前缀**：
    
    - `GlobalAudioManager`
        
    - `GlobalSceneManager`
        
    - `GameService`
        

### 2. 局部管理者（场景内 Node）：改用 Controller / Handler / Tracker

局部管理者仅负责当前场景树下方一小块节点的协调，随场景销毁而销毁。

- **用 `Controller`（控制器）**：专职控制某个具体场景或 UI 窗口的生命周期（如 `LevelController`、`UIController`）。
    
- **用 `Tracker`（追踪器）/ `Handler`（处理程序）**：专职负责某种局部计数或事件响应（如 `HeartTracker`、`WaveHandler`）。
    
- **直接以主体节点命名（省略 Manager）**：如果一个脚本挂在 `level_01.tscn` 的根节点上，直接叫 `Level` 或 `LevelState` 即可，不需要硬加 `Manager` 后缀。
    

### 常见对比参考表

|职责范围|推荐后缀 / 前缀|经典命名示例|挂载方式|
|---|---|---|---|
|**全局服务**|`GlobalXXX` / `XXXService` / `XXXSystem`|`SceneService`<br><br>  <br><br>`AudioSystem`<br><br>  <br><br>`GlobalSaveManager`|Autoload 全局单例|
|**关卡局部**|`XXXController` / `XXXState` / 直接实体名|`LevelController`<br><br>  <br><br>`LevelState`<br><br>  <br><br>`Level`|关卡根节点|
|**组件局部**|`XXXHandler` / `XXXTracker`|`HeartTracker`<br><br>  <br><br>`EnemySpawner`<br><br>  <br><br>`BossPhaseController`|关卡内的子节点|

采用这种规范后，看到 `SceneService` 会立刻反应出“这是全局切关服务”，而看到 `LevelController` 则明确知道“这只是当前关卡内部的控制器”。
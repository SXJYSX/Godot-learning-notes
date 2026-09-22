https://www.youtube.com/watch?v=9lLdhJmvDyY
最基础的教程，但是没讲Global groups 与 Scene groups的区别


“场景组”与 “全局组”都不影响 运行游戏时的代码逻辑

他们的区别在于：在编辑器模式中，“场景组”仅在当前场景.tscn中显示

“全局组”在任何场景.tscn中 都能显示，且能在Project Settings中的Globals中的Groups中显示；

“场景组”**组名不会暴露给其他场景的编辑器面板，也不会污染项目全局设置”**。但当 Heart.tscn 被实例化拖入 Level_01.tscn 并运行游戏时，它**依然能被全局搜索到**！  
Gemini说能自动补全代码，但我没发现

↑

创建game_groups.gd作为【组名常量类】。

正因为 Godot 编辑器对字符串补全的不稳定性，正规项目开发中极少直接在代码里手敲 &"Hearts"。绝大多数团队会创建一个专门的**组名常量类**：

↑

const HEARTS := &"Hearts"

**:=**：告诉编辑器：“请自动推导右边 &"Hearts" 的类型（即 StringName），并**锁定** **HEARTS** **为** **StringName** **静态类型**”。

**&"Hearts"**：高性能 StringName 字面量。

const  默认且强制是类级别的



How to Use Groups in Godot 4 (Godot 4 Tutorial)
https://www.youtube.com/watch?v=6c52JHNXi3I



https://www.youtube.com/watch?v=wcZz7JCeRBI
这个教程应该不是零基础的，看不懂


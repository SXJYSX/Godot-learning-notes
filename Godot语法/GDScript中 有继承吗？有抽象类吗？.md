GDScript中 有继承吗？有抽象类吗？

**1. 有继承吗？** **有，而且非常核心。** GDScript 是面向对象的语言，继承是通过 `extends` 关键字实现的。

**2. 有抽象类吗？** **原生没有（没有 `abstract` 关键字）。** GDScript 目前不支持像 C#、Java 那样直接用 `abstract class` 或 `interface` 来强制约束接口，但社区有一套**标准变通方案**来完美模拟抽象类。

### 一、 GDScript 中的继承 (Inheritance)

在 GDScript 中，继承既可以继承**引擎内置节点**，也可以继承**你自己写的脚本/类**。

#### 1. 继承内置节点或自定义类

GDScript

```
# 继承内置节点
extends Area2D

# 或者继承你之前用 class_name 声明的自定义类
extends Hitbox

# 或者直接通过文件路径继承
extends "res://scripts/combat/hitbox.gd"
```

#### 2. 方法重写与调用父类（Godot 4 语法）

在 Godot 4 中，使用 `super` 关键字来调用父类同名方法或构造函数：

GDScript

```
# Enemy.gd (子节点)
extends CharacterBody2D

func _ready() -> void:
    # 调用父类的 _ready() 逻辑
    super._ready()
    # 执行子类特有的逻辑
    setup_enemy_stats()

# 重写父类自定义方法
func take_damage(amount: int) -> void:
    # 先执行父类的扣血逻辑
    super.take_damage(amount)
    # 子类扩展：播放闪红动画
    play_flash_animation()
```

### 二、 如何在 GDScript 中模拟“抽象类” (Abstract Class)

因为没有原生的 `abstract` 关键字，在 Godot 商业开发中，通常采用“运行时抛错/断言（Assert）”的方式来强制要求子类实现某个接口。

#### 模式：模拟抽象基类（例如：状态机基类 `State.gd`）

GDScript

```
# state.gd (模拟抽象基类)
class_name State
extends Node

# 1. 阻止基类被直接实例化（可选）
func _init() -> void:
	if get_script() == load("res://scripts/state_machine/state.gd"):
		assert(false, "State 是抽象基类，不能被直接实例化！请继承它。")

# 2. 模拟抽象方法：如果子类没重写，直接报致命断言错误
func enter() -> void:
	assert(false, "子类必须实现抽象方法 enter()")

func exit() -> void:
	assert(false, "子类必须实现抽象方法 exit()")

func update(_delta: float) -> void:
	assert(false, "子类必须实现抽象方法 update()")
```

当其他开发者（或者你自己）继承这个 `State` 脚本，却**忘记**写 `enter()` 方法时：

- 游戏运行到这里会**立刻中断并定位到报错行**，非常明确地提示：`子类必须实现抽象方法 enter()`。
    

### 💡 抽象类 vs 鸭子类型（Duck Typing）

Godot 的 GDScript 是一门动态弱类型（但支持渐进式强类型）语言，除了用“模拟抽象类”之外，GDScript 更常用 **鸭子类型（Duck Typing）** 来解决接口调用问题：

> _"如果它走起来像鸭子，叫起来像鸭子，那它就是鸭子。"_

在需要检查接口时，你不需要定义抽象接口，直接判断方法是否存在即可：

GDScript

```
# 只要对方有 receive_damage 方法，我就不管它是什么类型，直接调用
if target.has_method("receive_damage"):
    target.receive_damage(10)
```

**总结建议：**

- 架构层面的核心系统（如 **有限状态机 FSM**、**UI 页面基类 UIWindow**、**技能基类 Skill**），强烈建议用 `assert(false, "...")` 模拟抽象类。
    
- 普通的节点交互，优先使用 **信号 (Signals)** 或 **鸭子类型 `has_method()`** 保证代码灵活性。
    

想要看一个完整的“基于模拟抽象基类的 Godot 4 有限状态机（FSM）”架构示例吗？
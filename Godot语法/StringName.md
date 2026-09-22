
**What are StringNames, and are they worth it?**
[https://www.youtube.com/watch?v=dW4Vmz4Rdxo&t=172s](https://www.youtube.com/watch?v=dW4Vmz4Rdxo&t=172s)

虽然在很多情况下，String 与 StringNames可以互相自动转换——实参类型StringName，即使填入String也不会报错，逻辑也能正确执行
但是StringName还是与String不同

优点1：Godot中有很多内置方法 参数都是StringNames类型，
		传入StringName类型的实参 可以节省 自动转换类型的时间，提高执行效率。
		比如：get_tree().get_nodes_in_group(...)
		
优点2：StringName 之间做相等比较，速度极快



缺点：不可变（不可直接修改）
	如果一定要修改，必须先转换为String类型


用途：静态数据，常量（例如 ID、名称）；
在常量中使用；
避免在循环中使用；

作者实测，性能提升微乎其微，但是↓
作者自己的置顶评论：

*更正*
正如许多读者指出的那样，我的基准测试存在缺陷。因此，我已根据建议的改进重新运行了测试：
1. 遍历一些预先计算好的数组，而不是随机选取元素。这样可以消除生成随机数这一变量，而随机数很可能已经影响了结果的准确性。同时，通过在两次测试中使用相同的数组，我们还能确保测试条件完全一致。
2. 不要使用相同的字符串。Godot 中的字符串是引用计数的（这一点我之前不知道，谢谢指出！:0）。在新的基准测试中，我确保不会将同一个字符串引用与自身进行比较，而是对它们执行了 `string.reverse().reverse()` 操作。这样会创建一个新的字符串副本，其引用地址不同。其他任何操作也都可以正常工作，这只是我想到的第一个方法。
3. 使用更相似的字符串。这次完全是我的问题，我一开始还提到过，对于更相似的字符串，StringName 应该更快。

经过这些修改后，结果如下：  
基本保持不变。  
StringName 的性能始终比普通字符串快约 1 秒。

*CORRECTION* As many of you have pointed out, my benchmarks were flawed. So, I rerun them with the suggested improvements: 1. Iterate over some pre-computed Arrays instead of picking a random element on the fly. This removes the variable of generating random numbers, which likely have been polluting the results. It also enables us to ensure that circumstances are the exact same by using the same arrays for both tests. 2. Don't use the same Strings. Strings in Godot are Reference-counted (which I didn't know, thanks for pointing out! :0). In the new benchmarks, I made sure not to compare the same String references against themselves by doing `string.reverse().reverse()` on them. This creates a copy of the String that's not the same Reference. Any other operation should work fine too, this was just the first one that came to mind. 3. Use strings that are more similar. This one's completely on me. I even talked about how StringNames should be faster for more similar ones in the start. After these changes, the results were: Mostly the same. StringNames were consistently hovering around 1s faster than regular Strings, over
# P/Invokeで文字列バッファ→Stringするときのパフォーマンス比較
GetWindowTextやGetTempPathのように文字列バッファからSystem.Stringを作成する際のパフォーマンス比較を実施しました。
NewStringのやり方はnew stringの結果が書込み可能であるとは保証されず、おそらく危険です。

BenchmarkDotNetのテンプレートのコピペ元　→　[C#でTypeをキーにしたDictionaryのパフォーマンス比較と最速コードの実装 - Grani Engineering Blog](http://engineering.grani.jp/entry/2017/07/28/145035)

# 結果
±数十ms程度は測定のたびに誤差が出るっぽいです。

``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18362
Intel Core i7-2600K CPU 3.40GHz (Sandy Bridge), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100
  [Host]   : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT
  ShortRun : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
|                 Method |     Mean |     Error |   StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------------- |---------:|----------:|---------:|-------:|------:|------:|----------:|
|          StringBuilder | 625.0 ns | 354.39 ns | 19.43 ns | 0.1659 |     - |     - |     696 B |
|         StackAllocSpan | 388.4 ns |  40.02 ns |  2.19 ns | 0.1297 |     - |     - |     544 B |
| StackAllocSpanToString | 392.8 ns |  65.17 ns |  3.57 ns | 0.1297 |     - |     - |     544 B |
|      StackAllocPointer | 338.0 ns |  24.29 ns |  1.33 ns | 0.0229 |     - |     - |      96 B |
|       StackAllocCreate | 349.4 ns |  30.39 ns |  1.67 ns | 0.1297 |     - |     - |     544 B |
|     DangerousNewString | 352.5 ns |  89.41 ns |  4.90 ns | 0.1297 |     - |     - |     544 B |

# 結論
基本的にStringBuilderはStringBuilder自体のメモリを無駄に確保することになるため論外です。スタックを使ったValueStringBuilderも実装が進んでいるっぽいですが、現在は少なくともpublicにはなっていません。

おすすめできるのはポインタ使用です。unsafeを付ける必要はありますが、そもそもP/Invokeは危険なので覚悟決めてunsafeを使うのが良いかと思います。

次点でSpan<Char>で受けるのも速度差は誤差レベルで、カジュアルに使えるので便利です。他の処理の一部として出てくる場合にはポインタより取り回しが良いかもしれません。ただ、string化したときにnull文字までstringに含まれてしまうようなので、巨大なバッファを取る場合には注意が必要です。

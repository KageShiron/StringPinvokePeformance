# P/Invoke�ŕ�����o�b�t�@��String����Ƃ��̃p�t�H�[�}���X��r
GetWindowText��GetTempPath�̂悤�ɕ�����o�b�t�@����System.String���쐬����ۂ̃p�t�H�[�}���X��r�����{���܂����B
NewString�̂�����new string�̌��ʂ������݉\�ł���Ƃ͕ۏ؂��ꂸ�A�����炭�댯�ł��B

BenchmarkDotNet�̃e���v���[�g�̃R�s�y���@���@[C#��Type���L�[�ɂ���Dictionary�̃p�t�H�[�}���X��r�ƍő��R�[�h�̎��� - Grani Engineering Blog](http://engineering.grani.jp/entry/2017/07/28/145035)

# ����
�}���\ms���x�͑���̂��тɌ덷���o����ۂ��ł��B

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

# ���_
��{�I��StringBuilder��StringBuilder���̂̃������𖳑ʂɊm�ۂ��邱�ƂɂȂ邽�ߘ_�O�ł��B�X�^�b�N���g����ValueStringBuilder���������i��ł�����ۂ��ł����A���݂͏��Ȃ��Ƃ�public�ɂ͂Ȃ��Ă��܂���B

�������߂ł���̂̓|�C���^�g�p�ł��Bunsafe��t����K�v�͂���܂����A��������P/Invoke�͊댯�Ȃ̂Ŋo�匈�߂�unsafe���g���̂��ǂ����Ǝv���܂��B

���_��Span<Char>�Ŏ󂯂�̂����x���͌덷���x���ŁA�J�W���A���Ɏg����̂ŕ֗��ł��B���̏����̈ꕔ�Ƃ��ďo�Ă���ꍇ�ɂ̓|�C���^�����񂵂��ǂ���������܂���B�����Astring�������Ƃ���null�����܂�string�Ɋ܂܂�Ă��܂��悤�Ȃ̂ŁA����ȃo�b�t�@�����ꍇ�ɂ͒��ӂ��K�v�ł��B

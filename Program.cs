using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using System;
using System.Runtime.InteropServices;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        var switcher = new BenchmarkSwitcher(new[]
        {
            typeof(StringPinvokeBenchmark)
        });
        switcher.Run(); // 走らせる
    }
}

public class BenchmarkConfig : ManualConfig
{
    public BenchmarkConfig()
    {
        Add(MarkdownExporter.GitHub);
        Add(MemoryDiagnoser.Default);
        Add(Job.ShortRun);
    }
}

// ベンチマーク本体
[Config(typeof(BenchmarkConfig))]
public class StringPinvokeBenchmark
{
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetTempPathW")]
    static extern int GetTempPath1(uint nBufferLength, StringBuilder sb);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetTempPathW")]
    static extern int GetTempPath2(uint nBufferLength, ref char sb);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetTempPathW")]
    static extern unsafe int GetTempPath3(uint nBufferLength, char* sb);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetTempPathW")]
    static extern unsafe int GetTempPath4(uint nBufferLength, string sb);


    [Benchmark]
    public string StringBuilder()
    {
        StringBuilder sb = new StringBuilder(261);
        GetTempPath1(260, sb);
        return sb.ToString();

    }

    [Benchmark]
    public string StackAllocSpan()
    {
        Span<char> buff = stackalloc char[261];
        GetTempPath2(260, ref buff.GetPinnableReference());
        return new string(buff);
    }

    [Benchmark]
    public string StackAllocSpanToString()
    {
        Span<char> buff = stackalloc char[261];
        GetTempPath2(260, ref buff.GetPinnableReference());
        return buff.ToString();
    }

    [Benchmark]
    public unsafe string StackAllocPointer()
    {
        char* buff = stackalloc char[261];
        GetTempPath3(260, buff);
        return new string(buff);
    }

    [Benchmark]
    public string StackAllocCreate()
    {
        return string.Create(260, 0,
            (b, _) => { GetTempPath2(260, ref b.GetPinnableReference()); }
            );
    }

    [Benchmark]
    public string DangerousNewString()
    {
        // Dangerous
        string buff = new string('\0', 260);
        GetTempPath4(260, buff);
        return buff;
    }

}
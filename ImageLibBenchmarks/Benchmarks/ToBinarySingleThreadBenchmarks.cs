﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using ImagingLib;

namespace Benchmarks;

[MemoryDiagnoser]
// [SimpleJob(launchCount: 1, warmupCount: 1, iterationCount: 1, invocationCount: 1)]
[SimpleJob]
public class ToBinarySingleThreadBenchmarks : BenchmarkBase
{
    [Benchmark]
    public void SystemDrawing()
    {
        using var bin = SystemDrawingExtensions.ToBinary(Data);
    }


    [Benchmark]
    public void SkiaSharp()
    {
        using var bin = SkiaSharpExtensions.ToBinary(Data);
    }

    [Benchmark]
    public void LibTiff()
    {
        using var bin = LibTiffExtensions.ToBinary(Data);
    }

    [Benchmark]
    public void MagickNet()
    {
        using var bin = MagickNetExtensions.ToBinary(Data);
    }
}
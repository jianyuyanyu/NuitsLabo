﻿using System.IO;
using BenchmarkDotNet.Attributes;
using System.Windows.Media.Imaging;
using ImageMagick;
using ImagingLib;
using WebPBenchmark.Extensions;

namespace WebPBenchmark;

[MemoryDiagnoser]
[SimpleJob(launchCount: 1, warmupCount: 1, iterationCount: 1, invocationCount: 1)]
// [SimpleJob]
public class BinarizeByFixedThreshold : BaseBenchmark
{
    private readonly byte[] _data = File.ReadAllBytes("Color.jpg");

    /// <summary>
    /// 2値化しきい値
    /// </summary>
    private static readonly float Threshold = 0.75f;

    [Benchmark]
    public BitmapSource MagickNetFixedThreshold()
    {
        using var magickImage = new MagickImage(BitmapSource.ToBmpBytes());

        magickImage.Threshold(new Percentage(Threshold));
        magickImage.Depth = 1;

        // MagickImageからMemoryStreamに変換
        using var stream = new MemoryStream();
        magickImage.Write(stream, MagickFormat.Bmp);  // 一時的にBMP形式として出力
        stream.Position = 0;

        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.StreamSource = stream;
        bitmapImage.EndInit();
        bitmapImage.Freeze();

        return bitmapImage;
    }

    [Benchmark]
    public void BySystemDrawing()
    {
        using var bin = _data.BySystemDrawing(Threshold);
    }


    [Benchmark]
    public void BySkiaSharp()
    {
        using var bin = _data.BySkiaSharp(Threshold);
    }

#if NET8_0_OR_GREATER
    [Benchmark]
    public void ByImageSharp()
    {
        using var bin = _data.ByImageSharp(Threshold);
    }
#endif
}

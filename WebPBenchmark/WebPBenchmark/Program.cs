﻿using BenchmarkDotNet.Running;
using WebPBenchmark;

var summary = BenchmarkRunner.Run<BinarizeByFixedThreshold>();

//var jobs = new[] { Job.ShortRun, Job.Default };
//var jobName = Prompt.Select("Job", jobs.Select(x => x.ToString()));

//var job = jobs.Single(x => x.ToString() == jobName);

//var config =
//    job == Job.ShortRun
//        ? ManualConfig.Create(DefaultConfig.Instance)
//            .AddJob(job.WithRuntime(ClrRuntime.Net481))
//        : ManualConfig.Create(DefaultConfig.Instance)
//            .AddJob(job.WithToolchain(
//                CsProjCoreToolchain.From(
//                    new NetCoreAppSettings(
//                        targetFrameworkMoniker: "net8.0-windows",
//                        runtimeFrameworkVersion: string.Empty,
//                        name: ".NET 8.0"))))
//            .AddJob(job.WithRuntime(ClrRuntime.Net481));

//var switcher = new BenchmarkSwitcher(
//[
//    typeof(BinarizeByFixedThreshold),
//        typeof(BinarizeByOtsu),
//        typeof(BitmapSourceToBitmap),
//        typeof(BitmapToBitmapSource),
//        typeof(CreateThumbnail),
//        typeof(Crop),
//        typeof(Load),
//        typeof(LoadBitmap),
//        typeof(CalculateOtsuThreshold)
//]);

//switcher.Run(args, config);

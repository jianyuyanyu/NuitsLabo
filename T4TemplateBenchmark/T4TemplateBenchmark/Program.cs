﻿using System;
using BenchmarkDotNet.Running;

namespace T4TemplateBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            //new GenerateSource().CustomT4Template();
            var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
        }
    }
}

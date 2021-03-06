﻿using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Stringier;

namespace Benchmarks {
	[SimpleJob(RuntimeMoniker.NetCoreApp31)]
	[SimpleJob(RuntimeMoniker.CoreRt31)]
	[SimpleJob(RuntimeMoniker.Mono)]
	[MemoryDiagnoser]
	public class SearchBenchmarks {
		[Params("hello", "hello world", "The quick brown fox jumps over the lazy dog")]
		public String Source { get; set; }

		[Params("l", "he", "wo", "brown", "the")]
		public String Pattern { get; set; }

		[Benchmark(Baseline = true)]
		public void BruteForce() => Search.BruteForce(Source, Pattern);

		[Benchmark]
		public void Horspool() => Search.Horspool(Source, Pattern);

		[Benchmark]
		public void RabinKarp() => Search.RabinKarp(Source, Pattern);
	}
}

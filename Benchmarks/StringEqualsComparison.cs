﻿using System;
using BenchmarkDotNet.Attributes;

namespace Benchmarks {
	[ClrJob, CoreJob, CoreRtJob]
	[MemoryDiagnoser]
	public class StringEqualsComparison {
		
		[Params("Hello")]
		public String A { get; set; }

		[Params("Hello", "World", "Goodbye")]
		public String B { get; set; }

		[Benchmark(Baseline = true)]
		public Boolean NETEquals() => String.Equals(A, B);

		[Benchmark]
		public Boolean ForeachEquals() {
			if (A.Length != B.Length) { return false; }
			for (Int32 i = 0; i < A.Length; i++) {
				if (A[i] != B[i]) return false;
			}
			return true;
		}

		[Benchmark]
		public Boolean HashBeforeEquals() => A.GetHashCode() != B.GetHashCode() ? false : String.Equals(A, B);

	}
}
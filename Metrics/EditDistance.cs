﻿using System;

namespace Stringier {
	public static class Metrics {
		/// <summary>
		/// Calculates the Hamming edit-distance between the <paramref name="source"/> <see cref="String"/> and <paramref name="other"/> <see cref="String"/>.
		/// </summary>
		/// <param name="source">The source <see cref="String"/>.</param>
		/// <param name="other">The other <see cref="String"/>.</param>
		/// <returns>The number of edits to get from <paramref name="source"/> to <paramref name="other"/>.</returns>
		public static Int32 HammingDistance(String source, String other) => ReferenceEquals(source, other) ? 0 : HammingDistance(source.AsSpan(), other.AsSpan());

		/// <summary>
		/// Calculates the Hamming edit-distance between the <paramref name="source"/> <see cref="String"/> and <paramref name="other"/> <see cref="ReadOnlySpan{T}"/> of <see cref="Char"/>.
		/// </summary>
		/// <param name="source">The source <see cref="String"/>.</param>
		/// <param name="other">The other <see cref="ReadOnlySpan{T}"/> of <see cref="Char"/>.</param>
		/// <returns>The number of edits to get from <paramref name="source"/> to <paramref name="other"/>.</returns>
		public static Int32 HammingDistance(String source, ReadOnlySpan<Char> other) => HammingDistance(source.AsSpan(), other);

		/// <summary>
		/// Calculates the Hamming edit-distance between the <paramref name="source"/> <see cref="ReadOnlySpan{T}"/> of <see cref="Char"/> and <paramref name="other"/> <see cref="String"/>.
		/// </summary>
		/// <param name="source">The source <see cref="ReadOnlySpan{T}"/> of <see cref="Char"/>.</param>
		/// <param name="other">The other <see cref="String"/>.</param>
		/// <returns>The number of edits to get from <paramref name="source"/> to <paramref name="other"/>.</returns>
		public static Int32 HammingDistance(ReadOnlySpan<Char> source, String other) => HammingDistance(source, other.AsSpan());

		/// <summary>
		/// Calculates the Hamming edit-distance between the <paramref name="source"/> <see cref="ReadOnlySpan{T}"/> of <see cref="Char"/> and <paramref name="other"/> <see cref="ReadOnlySpan{T}"/> of <see cref="Char"/>.
		/// </summary>
		/// <param name="source">The source <see cref="ReadOnlySpan{T}"/> of <see cref="Char"/>.</param>
		/// <param name="other">The other <see cref="ReadOnlySpan{T}"/> of <see cref="Char"/>.</param>
		/// <returns>The number of edits to get from <paramref name="source"/> to <paramref name="other"/>.</returns>
		public static Int32 HammingDistance(ReadOnlySpan<Char> source, ReadOnlySpan<Char> other) {
			if (source.Length != other.Length) {
				throw new ArgumentException("Must be equal length");
			} else {
				Int32 d = 0;
				for (Int32 i = 0; i < source.Length; i++) {
					if (source[i] != other[i]) {
						d++;
					}
				}
				return d;
			}
		}

		/// <summary>
		/// Calculates the Levenshtein edit-distance between the <paramref name="source"/> <see cref="String"/> and <paramref name="other"/> <see cref="String"/>.
		/// </summary>
		/// <param name="source">The source <see cref="String"/>.</param>
		/// <param name="other">The other <see cref="String"/>.</param>
		/// <returns>The number of edits to get from <paramref name="source"/> to <paramref name="other"/>.</returns>
		public static Int32 LevenshteinDistance(String source, String other) => ReferenceEquals(source, other) ? 0 : LevenshteinDistance(source.AsSpan(), other.AsSpan());

		/// <summary>
		/// Calculates the Levenshtein edit-distance between the <paramref name="source"/> <see cref="String"/> and <paramref name="other"/> <see cref="ReadOnlySpan{T}"/> of <see cref="Char"/>.
		/// </summary>
		/// <param name="source">The source <see cref="String"/>.</param>
		/// <param name="other">The other <see cref="ReadOnlySpan{T}"/> of <see cref="Char"/>.</param>
		/// <returns>The number of edits to get from <paramref name="source"/> to <paramref name="other"/>.</returns>
		public static Int32 LevenshteinDistance(String source, ReadOnlySpan<Char> other) => LevenshteinDistance(source.AsSpan(), other);

		/// <summary>
		/// Calculates the Levenshtein edit-distance between the <paramref name="source"/> <see cref="ReadOnlySpan{T}"/> of <see cref="Char"/> and <paramref name="other"/> <see cref="String"/>.
		/// </summary>
		/// <param name="source">The source <see cref="ReadOnlySpan{T}"/> of <see cref="Char"/>.</param>
		/// <param name="other">The other <see cref="String"/>.</param>
		/// <returns>The number of edits to get from <paramref name="source"/> to <paramref name="other"/>.</returns>
		public static Int32 LevenshteinDistance(ReadOnlySpan<Char> source, String other) => LevenshteinDistance(source, other.AsSpan());

		/// <summary>
		/// Calculates the Levenshtein edit-distance between the <paramref name="source"/> <see cref="ReadOnlySpan{T}"/> of <see cref="Char"/> and <paramref name="other"/> <see cref="ReadOnlySpan{T}"/> of <see cref="Char"/>.
		/// </summary>
		/// <param name="source">The source <see cref="ReadOnlySpan{T}"/> of <see cref="Char"/>.</param>
		/// <param name="other">The other <see cref="ReadOnlySpan{T}"/> of <see cref="Char"/>.</param>
		/// <returns>The number of edits to get from <paramref name="source"/> to <paramref name="other"/>.</returns>
		public static Int32 LevenshteinDistance(ReadOnlySpan<Char> source, ReadOnlySpan<Char> other) {
			Int32 n = source.Length;
			Int32 m = other.Length;

			if (n == 0) {
				return m;
			} else if (m == 0) {
				return n;
			} else {
				Int32[,] d = new Int32[n + 1, m + 1];
				for (Int32 i = 0; i <= n; d[i, 0] = i++) { }
				for (Int32 j = 0; j <= m; d[0, j] = j++) { }

				Int32 c;
				for (Int32 i = 1; i <= n; i++) {
					for (Int32 j = 1; j <= m; j++) {
						c = (other[j - 1] == source[i - 1]) ? 0 : 1;
						d[i, j] = Math.Min(
							Math.Min(
								d[i - 1, j] + 1,
								d[i, j - 1] + 1),
							d[i - 1, j - 1] + c);
					}
				}
				return d[n, m];
			}
		}
	}
}
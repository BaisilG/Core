﻿using System;
using System.Text;
using Defender;

namespace Stringier {
	public static partial class StringierExtensions {
		/// <summary>
		/// Repeat the <paramref name="char"/> <paramref name="count"/> times.
		/// </summary>
		/// <param name="char">The <see cref="Char"/> to repeat.</param>
		/// <param name="count">The amount of times to repeat the <paramref name="char"/>.</param>
		/// <returns>A <see cref="String"/> containing the repeated <paramref name="char"/>.</returns>
		public static String Repeat(this Char @char, Int32 count) {
			Guard.GreaterThanOrEqualTo(count, nameof(count), 0);
			return new String(@char, count);
		}

		/// <summary>
		/// Repeat the <paramref name="rune"/> <paramref name="count"/> times.
		/// </summary>
		/// <param name="rune">The <see cref="Rune"/> to repeat.</param>
		/// <param name="count">The amount of times to repeat the <paramref name="rune"/>.</param>
		/// <returns>A <see cref="String"/> containing the repeated <paramref name="rune"/>.</returns>
		public static String Repeat(this Rune rune, Int32 count) {
			Guard.GreaterThanOrEqualTo(count, nameof(count), 0);
			Rune[] runes = new Rune[count];
			for (int i = 0; i < count; i++) {
				runes[i] = rune;
			}
			return runes.AsString();
		}

		/// <summary>
		/// Repeat the <paramref name="string"/> <paramref name="count"/> times.
		/// </summary>
		/// <param name="string">The <see cref="String"/> to repeat.</param>
		/// <param name="count">The amount of times to repeat the <paramref name="string"/>.</param>
		/// <returns>A <see cref="String"/> containing the repeated <paramref name="string"/>.</returns>
		public static String Repeat(this String @string, Int32 count) {
			Guard.NotNull(@string, nameof(@string));
			Char[] result = new Char[@string.Length * count];
			Int32 r = 0;
			for (Int32 i = 0; i < count; i++) {
				@string.CopyTo(0, result, r, @string.Length);
				r += @string.Length;
			}
			return new String(result);
		}

		/// <summary>
		/// Repeat the <paramref name="span"/> <paramref name="count"/> times.
		/// </summary>
		/// <param name="span">The <see cref="ReadOnlySpan{T}"/> of <see cref="Char"/> to repeat.</param>
		/// <param name="count">The amount of times to repeat the <paramref name="span"/>.</param>
		/// <returns>A <see cref="String"/> containing the repeated <paramref name="span"/>.</returns>
		public static String Repeat(this ReadOnlySpan<Char> span, Int32 count) {
			if (count <= 0) {
				throw new ArgumentOutOfRangeException(nameof(count), "Count must be a positive integer");
			}
			String @string = span.ToString();
			Char[] result = new Char[span.Length * count];
			Int32 r = 0;
			for (Int32 i = 0; i < count; i++) {
				@string.CopyTo(0, result, r, span.Length);
				r += @string.Length;
			}
			return new String(result);
		}
	}
}

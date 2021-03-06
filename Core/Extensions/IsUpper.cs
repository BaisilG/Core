﻿using System;
using System.Text;

namespace Stringier {
	public static partial class StringierExtensions {
		/// <summary>
		/// Indicates whether this <see cref="Char"/> is categorized as an uppercase letter.
		/// </summary>
		/// <param name="char">The Unicode character to evaluate.</param>
		/// <returns><see langword="true"/> if <paramref name="char"/> is an uppercase letter; otherwise, <see langword="false"/>.</returns>
		public static Boolean IsUpper(this Char @char) => Char.IsUpper(@char);

		/// <summary>
		/// Indicates whether this <see cref="Rune"/> is categorized as an uppercase letter.
		/// </summary>
		/// <param name="rune">The Unicode character to evaluate.</param>
		/// <returns><see langword="true"/> if <paramref name="rune"/> is an uppercase letter; otherwise, <see langword="false"/>.</returns>
		public static Boolean IsUpper(this Rune rune) => Rune.IsUpper(rune);
	}
}

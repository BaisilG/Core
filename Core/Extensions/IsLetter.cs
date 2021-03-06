﻿using System;
using System.Text;

namespace Stringier {
	public static partial class StringierExtensions {
		/// <summary>
		/// Indicates whether this <see cref="Char"/> is categorized as a Unicode letter.
		/// </summary>
		/// <param name="char">The Unicode character to evaluate.</param>
		/// <returns><see langword="true"/> if <paramref name="char"/> is a letter; otherwise, <see langword="false"/>.</returns>
		public static Boolean IsLetter(this Char @char) => Char.IsLetter(@char);

		/// <summary>
		/// Indicates whether this <see cref="Rune"/> is categorized as a Unicode letter.
		/// </summary>
		/// <param name="rune">The Unicode character to evaluate.</param>
		/// <returns><see langword="true"/> if <paramref name="rune"/> is a letter; otherwise, <see langword="false"/>.</returns>
		public static Boolean IsLetter(this Rune rune) => Rune.IsLetter(rune);
	}
}

﻿using System.Text.RegularExpressions;

namespace System {
	public static partial class StringierExtensions {
		/// <summary>
		/// Ensures the <paramref name="String"/> ends with the <paramref name="Required"/> string, adding it if necessary.
		/// </summary>
		/// <param name="String">The string to ensure.</param>
		/// <param name="Required">The required ending.</param>
		/// <returns>A string with the ensured ending.</returns>
		public static String EnsureEndsWith(this String String, String Required) {
			if (String is null || Required is null) {
				throw new ArgumentNullException(String is null ? nameof(String) : nameof(Required));
			}
			if (new Regex(Required + "$", RegexOptions.None).IsMatch(String)) {
				return String;
			}
			else {
				return String + Required;
			}
		}
	}
}

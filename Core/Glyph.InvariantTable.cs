using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Stringier {
	public readonly partial struct Glyph {
		//! Please do not use graphemes in this table; use the fully escaped codepoints. The graphemes would be visually identical (that's the point }), and would be remarkably difficult to find and fix errors.

		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();
		private static readonly Object � = new Object();

		/// <summary>
		/// Invariant equivalency table.
		/// </summary>
		/// <remarks>
		/// This intends to implement UAX#29 (https://unicode.org/reports/tr29/).
		/// </remarks>
		[SuppressMessage("Major Bug", "S3263:Static fields should appear in the order they must be initialized ", Justification = "It's almost like that's why this is at the bottom...")]
		internal static readonly Table InvariantTable = new Table(
			#region Latin-1 Supplement
			new KeyValuePair<String, Object>("\u00C0", �),
			new KeyValuePair<String, Object>("\u0041\u0300", �),
			new KeyValuePair<String, Object>("\u00C1", �),
			new KeyValuePair<String, Object>("\u0041\u0301", �),
			new KeyValuePair<String, Object>("\u00C2", �),
			new KeyValuePair<String, Object>("\u0041\u0302", �),
			new KeyValuePair<String, Object>("\u00C3", �),
			new KeyValuePair<String, Object>("\u0041\u0303", �),
			new KeyValuePair<String, Object>("\u00C4", �),
			new KeyValuePair<String, Object>("\u0041\u0308", �),
			new KeyValuePair<String, Object>("\u00C5", �),
			new KeyValuePair<String, Object>("\u0041\u030A", �),
			new KeyValuePair<String, Object>("\u00C6", �),
			new KeyValuePair<String, Object>("\u0041\u0045", �),
			new KeyValuePair<String, Object>("\u00C7", �),
			new KeyValuePair<String, Object>("\u0043\u0327", �),
			new KeyValuePair<String, Object>("\u00C8", �),
			new KeyValuePair<String, Object>("\u0045\u0300", �),
			new KeyValuePair<String, Object>("\u00C9", �),
			new KeyValuePair<String, Object>("\u0045\u0301", �),
			new KeyValuePair<String, Object>("\u00CA", �),
			new KeyValuePair<String, Object>("\u0045\u0302", �),
			new KeyValuePair<String, Object>("\u00CB", �),
			new KeyValuePair<String, Object>("\u0045\u0308", �),
			new KeyValuePair<String, Object>("\u00CC", �),
			new KeyValuePair<String, Object>("\u0049\u0300", �),
			new KeyValuePair<String, Object>("\u00CD", �),
			new KeyValuePair<String, Object>("\u0049\u0301", �),
			new KeyValuePair<String, Object>("\u00CE", �),
			new KeyValuePair<String, Object>("\u0049\u0302", �),
			new KeyValuePair<String, Object>("\u00CF", �),
			new KeyValuePair<String, Object>("\u0049\u0308", �),
			new KeyValuePair<String, Object>("\u00D1", �),
			new KeyValuePair<String, Object>("\u004E\u0303", �),
			new KeyValuePair<String, Object>("\u00D2", �),
			new KeyValuePair<String, Object>("\u004F\u0300", �),
			new KeyValuePair<String, Object>("\u00D3", �),
			new KeyValuePair<String, Object>("\u004F\u0301", �),
			new KeyValuePair<String, Object>("\u00D4", �),
			new KeyValuePair<String, Object>("\u004F\u0302", �),
			new KeyValuePair<String, Object>("\u00D5", �),
			new KeyValuePair<String, Object>("\u004F\u0303", �),
			new KeyValuePair<String, Object>("\u00D6", �),
			new KeyValuePair<String, Object>("\u004F\u0308", �),
			new KeyValuePair<String, Object>("\u00D9", �),
			new KeyValuePair<String, Object>("\u0055\u0300", �),
			new KeyValuePair<String, Object>("\u00DA", �),
			new KeyValuePair<String, Object>("\u0055\u0301", �),
			new KeyValuePair<String, Object>("\u00DB", �),
			new KeyValuePair<String, Object>("\u0055\u0302", �),
			new KeyValuePair<String, Object>("\u00DC", �),
			new KeyValuePair<String, Object>("\u0055\u0308", �),
			new KeyValuePair<String, Object>("\u00DD", �),
			new KeyValuePair<String, Object>("\u0059\u0301", �),
			new KeyValuePair<String, Object>("\u00E0", �),
			new KeyValuePair<String, Object>("\u0061\u0300", �),
			new KeyValuePair<String, Object>("\u00E1", �),
			new KeyValuePair<String, Object>("\u0061\u0301", �),
			new KeyValuePair<String, Object>("\u00E2", �),
			new KeyValuePair<String, Object>("\u0061\u0302", �),
			new KeyValuePair<String, Object>("\u00E3", �),
			new KeyValuePair<String, Object>("\u0061\u0303", �),
			new KeyValuePair<String, Object>("\u00E4", �),
			new KeyValuePair<String, Object>("\u0061\u0308", �),
			new KeyValuePair<String, Object>("\u00E5", �),
			new KeyValuePair<String, Object>("\u0061\u030A", �),
			new KeyValuePair<String, Object>("\u00E6", �),
			new KeyValuePair<String, Object>("\u0061\u0065", �),
			new KeyValuePair<String, Object>("\u00E7", �),
			new KeyValuePair<String, Object>("\u0063\u0327", �),
			new KeyValuePair<String, Object>("\u00E8", �),
			new KeyValuePair<String, Object>("\u0065\u0300", �),
			new KeyValuePair<String, Object>("\u00E9", �),
			new KeyValuePair<String, Object>("\u0065\u0301", �),
			new KeyValuePair<String, Object>("\u00EA", �),
			new KeyValuePair<String, Object>("\u0065\u0302", �),
			new KeyValuePair<String, Object>("\u00EB", �),
			new KeyValuePair<String, Object>("\u0065\u0308", �),
			new KeyValuePair<String, Object>("\u00EC", �),
			new KeyValuePair<String, Object>("\u0069\u0300", �),
			new KeyValuePair<String, Object>("\u00ED", �),
			new KeyValuePair<String, Object>("\u0069\u0301", �),
			new KeyValuePair<String, Object>("\u00EE", �),
			new KeyValuePair<String, Object>("\u0069\u0302", �),
			new KeyValuePair<String, Object>("\u00EF", �),
			new KeyValuePair<String, Object>("\u0069\u0308", �),
			new KeyValuePair<String, Object>("\u00F1", �),
			new KeyValuePair<String, Object>("\u006E\u0303", �),
			new KeyValuePair<String, Object>("\u00F2", �),
			new KeyValuePair<String, Object>("\u006F\u0300", �),
			new KeyValuePair<String, Object>("\u00F3", �),
			new KeyValuePair<String, Object>("\u006F\u0301", �),
			new KeyValuePair<String, Object>("\u00F4", �),
			new KeyValuePair<String, Object>("\u006F\u0302", �),
			new KeyValuePair<String, Object>("\u00F5", �),
			new KeyValuePair<String, Object>("\u006F\u0303", �),
			new KeyValuePair<String, Object>("\u00F6", �),
			new KeyValuePair<String, Object>("\u006F\u0308", �),
			new KeyValuePair<String, Object>("\u00F9", �),
			new KeyValuePair<String, Object>("\u0075\u0300", �),
			new KeyValuePair<String, Object>("\u00FA", �),
			new KeyValuePair<String, Object>("\u0075\u0301", �),
			new KeyValuePair<String, Object>("\u00FB", �),
			new KeyValuePair<String, Object>("\u0075\u0302", �),
			new KeyValuePair<String, Object>("\u00FC", �),
			new KeyValuePair<String, Object>("\u0075\u0308", �),
			new KeyValuePair<String, Object>("\u00FD", �),
			new KeyValuePair<String, Object>("\u0079\u0301", �),
			new KeyValuePair<String, Object>("\u00FE", �),
			new KeyValuePair<String, Object>("\u0079\u0308", �)
			#endregion
			);
	}
}

﻿using System;
using System.Collections.Generic;
using System.Text;

namespace System.Text.Patterns {
	/// <summary>
	/// Represents part of a <see cref="Pattern"/>
	/// </summary>
	internal abstract class Node : IEquatable<String> {

		/// <summary>
		/// Attempt to consume the <see cref="Pattern"/> from the <paramref name="Source"/>
		/// </summary>
		/// <param name="Source">The <see cref="String"/> to consume</param>
		/// <returns>A <see cref="Result"/> containing whether a match occured and the captured string</returns>
		public Result Consume(String Source) {
			Source source = new Source(Source);
			return Consume(ref source);
		}

		/// <summary>
		/// Attempt to consume the <see cref="Pattern"/> from the <paramref name="Source"/>, adjusting the position in the <paramref name="Source"/> as appropriate
		/// </summary>
		/// <param name="Source">The <see cref="Source"/> to consume</param>
		/// <returns>A <see cref="Result"/> containing whether a match occured and the captured string</returns>
		public abstract Result Consume(ref Source Source);

		public abstract override Boolean Equals(Object obj);

		public abstract Boolean Equals(String other);

		public abstract override Int32 GetHashCode();

		/// <summary>
		/// Attempt to consume from the <paramref name="Source"/> while neglecting the <paramref name="Pattern"/>
		/// </summary>
		/// <param name="Source">The <see cref="String"/> to consume</param>
		/// <returns>A <see cref="Result"/> containing whether a match occured and the captured string</returns>
		public Result Neglect(String Source) {
			Source source = new Source(Source);
			return Consume(ref source);
		}

		/// <summary>
		/// Attempt to consume from the <paramref name="Source"/> while neglecting the <see cref="Pattern"/>
		/// </summary>
		/// <param name="Source">The <see cref="Source"/> to consume</param>
		/// <returns>A <see cref="Result"/> containing whether a match occured and the captured string</returns>
		public abstract Result Neglect(ref Source Source);

		public abstract override String ToString();
	}
}

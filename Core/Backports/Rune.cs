﻿//! This exists to backport Rune to older runtimes, since we're going to take advantage of it throughout the entire project, so it must exist. By conditionally including it, and multitargeting both runtimes without and with the Rune type, it can be provided without colliding.
//! This (public) API must match the official one exactly. As such, copyright belongs to the .NET Foundation. The internals can be implemented using existing API's in Core.

#if NETSTANDARD2_0
using System.Buffers;
using System.Diagnostics;
using System.Globalization;
using Defender;
using Stringier;
using Stringier.Encodings;

namespace System.Text {
	/// <summary>
	/// Represents a Unicode scalar value ([ U+0000..U+D7FF ], inclusive; or [ U+E000..U+10FFFF ], inclusive).
	/// </summary>
	/// <remarks>
	/// This type's constructors and conversion operators validate the input, so consumers can call the APIs
	/// assuming that the underlying <see cref="Rune"/> instance is well-formed.
	/// </remarks>
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public readonly struct Rune : IComparable<Rune>, IEquatable<Rune> {
		/// <summary>
		/// The UNICODE Scalar Value.
		/// </summary>
		private readonly UInt32 value;

		/// <summary>
		/// Creates a <see cref="Rune"/> from the provided UTF-16 code unit.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// If <paramref name="ch"/> represents a UTF-16 surrogate code point
		/// U+D800..U+DFFF, inclusive.
		/// </exception>
		public Rune(Char ch) : this((UInt32)ch) { }

		/// <summary>
		/// Creates a <see cref="Rune"/> from the provided UTF-16 surrogate pair.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// If <paramref name="highSurrogate"/> does not represent a UTF-16 high surrogate code point
		/// or <paramref name="lowSurrogate"/> does not represent a UTF-16 low surrogate code point.
		/// </exception>
		public Rune(Char highSurrogate, Char lowSurrogate) : this(new SurrogatePair(new CodePoint(highSurrogate), new CodePoint(lowSurrogate)).CodePoint.Value) { }

		/// <summary>
		/// Creates a <see cref="Rune"/> from the provided Unicode scalar value.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// If <paramref name="value"/> does not represent a value Unicode scalar value.
		/// </exception>
		public Rune(Int32 value) : this((UInt32)value) { }

		/// <summary>
		/// Creates a <see cref="Rune"/> from the provided Unicode scalar value.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// If <paramref name="value"/> does not represent a value Unicode scalar value.
		/// </exception>
		[CLSCompliant(false)]
		public Rune(UInt32 value) {
			if (!Unsafe.IsScalarValue(value)) {
				throw new ArgumentOutOfRangeException(nameof(value), "The value is not a valid UNICODE Scalar Value");
			}
			this.value = value;
		}

		/// <summary>
		/// A <see cref="Rune"/> instance that represents the Unicode replacement character U+FFFD.
		/// </summary>
		public static Rune ReplacementChar { get; } = new Rune(0xFFFD);

		/// <summary>
		/// Returns true if and only if this scalar value is ASCII ([ U+0000..U+007F ])
		/// and therefore representable by a single UTF-8 code unit.
		/// </summary>
		public Boolean IsAscii => Unsafe.IsAscii(value);

		/// <summary>
		/// Returns true if and only if this scalar value is within the BMP ([ U+0000..U+FFFF ])
		/// and therefore representable by a single UTF-16 code unit.
		/// </summary>
		public Boolean IsBmp => Unsafe.IsBmp(value);

		/// <summary>
		/// Returns the Unicode plane (0 to 16, inclusive) which contains this scalar.
		/// </summary>
		public Int32 Plane => (Int32)Unsafe.Plane(value);

		/// <summary>
		/// Returns the length in code units (<see cref="char"/>) of the
		/// UTF-16 sequence required to represent this scalar value.
		/// </summary>
		/// <remarks>
		/// The return value will be 1 or 2.
		/// </remarks>
		public Int32 Utf16SequenceLength => (Int32)Unsafe.Utf16SequenceLength(value);

		/// <summary>
		/// Returns the length in code units of the
		/// UTF-8 sequence required to represent this scalar value.
		/// </summary>
		/// <remarks>
		/// The return value will be 1 through 4, inclusive.
		/// </remarks>
		public Int32 Utf8SequenceLength => (Int32)Unsafe.Utf8SequenceLength(value);

		/// <summary>
		/// Returns the Unicode scalar value as an integer.
		/// </summary>
		public Int32 Value => (Int32)value;

		// Displayed as "'<char>' (U+XXXX)"; e.g., "'e' (U+0065)"
		private String DebuggerDisplay => FormattableString.Invariant($"U+{value:X4} '{(IsValid(value) ? ToString() : "\uFFFD")}'");

		/// <summary>
		/// Decodes the <see cref="Rune"/> at the beginning of the provided UTF-16 source buffer.
		/// </summary>
		/// <returns>
		/// <para>
		/// If the source buffer begins with a valid UTF-16 encoded scalar value, returns <see cref="OperationStatus.Done"/>,
		/// and outs via <paramref name="result"/> the decoded <see cref="Rune"/> and via <paramref name="charsConsumed"/> the
		/// number of <see langword="char"/>s used in the input buffer to encode the <see cref="Rune"/>.
		/// </para>
		/// <para>
		/// If the source buffer is empty or contains only a standalone UTF-16 high surrogate character, returns <see cref="OperationStatus.NeedMoreData"/>,
		/// and outs via <paramref name="result"/> <see cref="ReplacementChar"/> and via <paramref name="charsConsumed"/> the length of the input buffer.
		/// </para>
		/// <para>
		/// If the source buffer begins with an ill-formed UTF-16 encoded scalar value, returns <see cref="OperationStatus.InvalidData"/>,
		/// and outs via <paramref name="result"/> <see cref="ReplacementChar"/> and via <paramref name="charsConsumed"/> the number of
		/// <see langword="char"/>s used in the input buffer to encode the ill-formed sequence.
		/// </para>
		/// </returns>
		/// <remarks>
		/// The general calling convention is to call this method in a loop, slicing the <paramref name="source"/> buffer by
		/// <paramref name="charsConsumed"/> elements on each iteration of the loop. On each iteration of the loop <paramref name="result"/>
		/// will contain the real scalar value if successfully decoded, or it will contain <see cref="ReplacementChar"/> if
		/// the data could not be successfully decoded. This pattern provides convenient automatic U+FFFD substitution of
		/// invalid sequences while iterating through the loop.
		/// </remarks>
		public static OperationStatus DecodeFromUtf16(ReadOnlySpan<Char> source, out Rune result, out Int32 charsConsumed) {
			if (!source.IsEmpty) {
				// First, check for the common case of a BMP scalar value.
				// If this is correct, return immediately.

				Char firstChar = source[0];
				if (TryCreate(firstChar, out result)) {
					charsConsumed = 1;
					return OperationStatus.Done;
				}

				// First thing we saw was a UTF-16 surrogate code point.
				// Let's optimistically assume for now it's a high surrogate and hope
				// that combining it with the next char yields useful results.

				if (1 < source.Length) {
					Char secondChar = source[1];
					if (TryCreate(firstChar, secondChar, out result)) {
						// Success! Formed a supplementary scalar value.
						charsConsumed = 2;
						return OperationStatus.Done;
					} else {
						// Either the first character was a low surrogate, or the second
						// character was not a low surrogate. This is an error.
						goto InvalidData;
					}
				} else if (!Char.IsHighSurrogate(firstChar)) {
					// Quick check to make sure we're not going to report NeedMoreData for
					// a single-element buffer where the data is a standalone low surrogate
					// character. Since no additional data will ever make this valid, we'll
					// report an error immediately.
					goto InvalidData;
				}
			}

			// If we got to this point, the input buffer was empty, or the buffer
			// was a single element in length and that element was a high surrogate char.

			charsConsumed = source.Length;
			result = ReplacementChar;
			return OperationStatus.NeedMoreData;

		InvalidData:

			charsConsumed = 1; // maximal invalid subsequence for UTF-16 is always a single code unit in length
			result = ReplacementChar;
			return OperationStatus.InvalidData;
		}

		/// <summary>
		/// Decodes the <see cref="Rune"/> at the beginning of the provided UTF-8 source buffer.
		/// </summary>
		/// <returns>
		/// <para>
		/// If the source buffer begins with a valid UTF-8 encoded scalar value, returns <see cref="OperationStatus.Done"/>,
		/// and outs via <paramref name="result"/> the decoded <see cref="Rune"/> and via <paramref name="bytesConsumed"/> the
		/// number of <see langword="byte"/>s used in the input buffer to encode the <see cref="Rune"/>.
		/// </para>
		/// <para>
		/// If the source buffer is empty or contains only a standalone UTF-8 high surrogate character, returns <see cref="OperationStatus.NeedMoreData"/>,
		/// and outs via <paramref name="result"/> <see cref="ReplacementChar"/> and via <paramref name="bytesConsumed"/> the length of the input buffer.
		/// </para>
		/// <para>
		/// If the source buffer begins with an ill-formed UTF-8 encoded scalar value, returns <see cref="OperationStatus.InvalidData"/>,
		/// and outs via <paramref name="result"/> <see cref="ReplacementChar"/> and via <paramref name="bytesConsumed"/> the number of
		/// <see langword="char"/>s used in the input buffer to encode the ill-formed sequence.
		/// </para>
		/// </returns>
		/// <remarks>
		/// The general calling convention is to call this method in a loop, slicing the <paramref name="source"/> buffer by
		/// <paramref name="bytesConsumed"/> elements on each iteration of the loop. On each iteration of the loop <paramref name="result"/>
		/// will contain the real scalar value if successfully decoded, or it will contain <see cref="ReplacementChar"/> if
		/// the data could not be successfully decoded. This pattern provides convenient automatic U+FFFD substitution of
		/// invalid sequences while iterating through the loop.
		/// </remarks>
		public static OperationStatus DecodeFromUtf8(ReadOnlySpan<Byte> source, out Rune result, out Int32 bytesConsumed) {
			// This method follows the Unicode Standard's recommendation for detecting
			// the maximal subpart of an ill-formed subsequence. See The Unicode Standard,
			// Ch. 3.9 for more details. In summary, when reporting an invalid subsequence,
			// it tries to consume as many code units as possible as long as those code
			// units constitute the beginning of a longer well-formed subsequence per Table 3-7.

			Int32 index = 0;

			// Try reading input[0].

			if ((UInt32)index >= (UInt32)source.Length) {
				goto NeedsMoreData;
			}

			UInt32 tempValue = source[index];
			if (!Unsafe.IsAscii(tempValue)) {
				goto NotAscii;
			}

		Finish:

			bytesConsumed = index + 1;
			Debug.Assert(1 <= bytesConsumed && bytesConsumed <= 4); // Valid subsequences are always length [1..4]
			result = new Rune(tempValue);
			return OperationStatus.Done;

		NotAscii:

			// Per Table 3-7, the beginning of a multibyte sequence must be a code unit in
			// the range [C2..F4]. If it's outside of that range, it's either a standalone
			// continuation byte, or it's an overlong two-byte sequence, or it's an out-of-range
			// four-byte sequence.

			if (!tempValue.Within(0xC2, 0xF4)) {
				goto FirstByteInvalid;
			}

			tempValue = (tempValue - 0xC2) << 6;

			// Try reading input[1].

			index++;
			if ((UInt32)index >= (UInt32)source.Length) {
				goto NeedsMoreData;
			}

			// Continuation bytes are of the form [10xxxxxx], which means that their two's
			// complement representation is in the range [-65..-128]. This allows us to
			// perform a single comparison to see if a byte is a continuation byte.

			Int32 thisByteSignExtended = (SByte)source[index];
			if (thisByteSignExtended >= -64) {
				goto Invalid;
			}

			tempValue += (UInt32)thisByteSignExtended;
			tempValue += 0x80; // remove the continuation byte marker
			tempValue += (0xC2 - 0xC0) << 6; // remove the leading byte marker

			if (tempValue < 0x0800) {
				Debug.Assert(tempValue.Within(0x0080, 0x07FF));
				goto Finish; // this is a valid 2-byte sequence
			}

			// This appears to be a 3- or 4-byte sequence. Since per Table 3-7 we now have
			// enough information (from just two code units) to detect overlong or surrogate
			// sequences, we need to perform these checks now.

			if (!tempValue.Within(((0xE0 - 0xC0) << 6) + (0xA0 - 0x80), ((0xF4 - 0xC0) << 6) + (0x8F - 0x80))) {
				// The first two bytes were not in the range [[E0 A0]..[F4 8F]].
				// This is an overlong 3-byte sequence or an out-of-range 4-byte sequence.
				goto Invalid;
			}

			if (tempValue.Within(((0xED - 0xC0) << 6) + (0xA0 - 0x80), ((0xED - 0xC0) << 6) + (0xBF - 0x80))) {
				// This is a UTF-16 surrogate code point, which is invalid in UTF-8.
				goto Invalid;
			}

			if (tempValue.Within(((0xF0 - 0xC0) << 6) + (0x80 - 0x80), ((0xF0 - 0xC0) << 6) + (0x8F - 0x80))) {
				// This is an overlong 4-byte sequence.
				goto Invalid;
			}

			// The first two bytes were just fine. We don't need to perform any other checks
			// on the remaining bytes other than to see that they're valid continuation bytes.

			// Try reading input[2].

			index++;
			if ((UInt32)index >= (UInt32)source.Length) {
				goto NeedsMoreData;
			}

			thisByteSignExtended = (SByte)source[index];
			if (thisByteSignExtended >= -64) {
				goto Invalid; // this byte is not a UTF-8 continuation byte
			}

			tempValue <<= 6;
			tempValue += (UInt32)thisByteSignExtended;
			tempValue += 0x80; // remove the continuation byte marker
			tempValue -= (0xE0 - 0xC0) << 12; // remove the leading byte marker

			if (tempValue <= 0xFFFF) {
				Debug.Assert(tempValue.Within(0x0800, 0xFFFF));
				goto Finish; // this is a valid 3-byte sequence
			}

			// Try reading input[3].

			index++;
			if ((UInt32)index >= (UInt32)source.Length) {
				goto NeedsMoreData;
			}

			thisByteSignExtended = (SByte)source[index];
			if (thisByteSignExtended >= -64) {
				goto Invalid; // this byte is not a UTF-8 continuation byte
			}

			tempValue <<= 6;
			tempValue += (UInt32)thisByteSignExtended;
			tempValue += 0x80; // remove the continuation byte marker
			tempValue -= (0xF0 - 0xE0) << 18; // remove the leading byte marker

			Debug.Assert(Unsafe.IsSmp(tempValue));
			goto Finish; // this is a valid 4-byte sequence

		FirstByteInvalid:

			index = 1; // Invalid subsequences are always at least length 1.

		Invalid:

			Debug.Assert(1 <= index && index <= 3); // Invalid subsequences are always length 1..3
			bytesConsumed = index;
			result = ReplacementChar;
			return OperationStatus.InvalidData;

		NeedsMoreData:

			Debug.Assert(0 <= index && index <= 3); // Incomplete subsequences are always length 0..3
			bytesConsumed = index;
			result = ReplacementChar;
			return OperationStatus.NeedMoreData;
		}

		/// <summary>
		/// Decodes the <see cref="Rune"/> at the end of the provided UTF-16 source buffer.
		/// </summary>
		/// <remarks>
		/// This method is very similar to <see cref="DecodeFromUtf16(ReadOnlySpan{Char}, out Rune, out Int32)"/>, but it allows
		/// the caller to loop backward instead of forward. The typical calling convention is that on each iteration
		/// of the loop, the caller should slice off the final <paramref name="charsConsumed"/> elements of
		/// the <paramref name="source"/> buffer.
		/// </remarks>
		public static OperationStatus DecodeLastFromUtf16(ReadOnlySpan<Char> source, out Rune result, out Int32 charsConsumed) {
			Int32 index = source.Length - 1;
			if ((UInt32)index < (UInt32)source.Length) {
				// First, check for the common case of a BMP scalar value.
				// If this is correct, return immediately.

				Char finalChar = source[index];
				if (TryCreate(finalChar, out result)) {
					charsConsumed = 1;
					return OperationStatus.Done;
				}

				if (Char.IsLowSurrogate(finalChar)) {
					// The final character was a UTF-16 low surrogate code point.
					// This must be preceded by a UTF-16 high surrogate code point, otherwise
					// we have a standalone low surrogate, which is always invalid.

					index--;
					if ((UInt32)index < (UInt32)source.Length) {
						Char penultimateChar = source[index];
						if (TryCreate(penultimateChar, finalChar, out result)) {
							// Success! Formed a supplementary scalar value.
							charsConsumed = 2;
							return OperationStatus.Done;
						}
					}

					// If we got to this point, we saw a standalone low surrogate
					// and must report an error.

					charsConsumed = 1; // standalone surrogate
					result = ReplacementChar;
					return OperationStatus.InvalidData;
				}
			}

			// If we got this far, the source buffer was empty, or the source buffer ended
			// with a UTF-16 high surrogate code point. These aren't errors since they could
			// be valid given more input data.

			charsConsumed = (Int32)((UInt32)(-source.Length) >> 31); // 0 -> 0, all other lengths -> 1
			result = ReplacementChar;
			return OperationStatus.NeedMoreData;
		}

		/// <summary>
		/// Decodes the <see cref="Rune"/> at the end of the provided UTF-8 source buffer.
		/// </summary>
		/// <remarks>
		/// This method is very similar to <see cref="DecodeFromUtf8(ReadOnlySpan{Byte}, out Rune, out Int32)"/>, but it allows
		/// the caller to loop backward instead of forward. The typical calling convention is that on each iteration
		/// of the loop, the caller should slice off the final <paramref name="bytesConsumed"/> elements of
		/// the <paramref name="source"/> buffer.
		/// </remarks>
		public static OperationStatus DecodeLastFromUtf8(ReadOnlySpan<Byte> source, out Rune value, out Int32 bytesConsumed) {
			Int32 index = source.Length - 1;
			if ((UInt32)index < (UInt32)source.Length) {
				// The buffer contains at least one byte. Let's check the fast case where the
				// buffer ends with an ASCII byte.

				UInt32 tempValue = source[index];
				if (Unsafe.IsAscii(tempValue)) {
					bytesConsumed = 1;
					value = new Rune(tempValue);
					return OperationStatus.Done;
				}

				// If the final byte is not an ASCII byte, we may be beginning or in the middle of
				// a UTF-8 multi-code unit sequence. We need to back up until we see the start of
				// the multi-code unit sequence; we can detect the leading byte because all multi-byte
				// sequences begin with a byte whose 0x40 bit is set. Since all multi-byte sequences
				// are no greater than 4 code units in length, we only need to search back a maximum
				// of four bytes.

				if (((Byte)tempValue & 0x40) != 0) {
					// This is a UTF-8 leading byte. We'll do a forward read from here.
					// It'll return invalid (if given C0, F5, etc.) or incomplete. Both are fine.

					return DecodeFromUtf8(source.Slice(index), out value, out bytesConsumed);
				}

				// If we got to this point, the final byte was a UTF-8 continuation byte.
				// Let's check the three bytes immediately preceding this, looking for the starting byte.

				for (Int32 i = 3; i > 0; i--) {
					index--;
					if ((UInt32)index >= (UInt32)source.Length) {
						goto Invalid; // out of data
					}

					// The check below will get hit for ASCII (values 00..7F) and for UTF-8 starting bytes
					// (bits 0xC0 set, values C0..FF). In two's complement this is the range [-64..127].
					// It's just a fast way for us to terminate the search.

					if ((SByte)source[index] >= -64) {
						goto ForwardDecode;
					}
				}

			Invalid:

				// If we got to this point, either:
				// - the last 4 bytes of the input buffer are continuation bytes;
				// - the entire input buffer (if fewer than 4 bytes) consists only of continuation bytes; or
				// - there's no UTF-8 leading byte between the final continuation byte of the buffer and
				//   the previous well-formed subsequence or maximal invalid subsequence.
				//
				// In all of these cases, the final byte must be a maximal invalid subsequence of length 1.
				// See comment near the end of this method for more information.

				value = ReplacementChar;
				bytesConsumed = 1;
				return OperationStatus.InvalidData;

			ForwardDecode:

				// If we got to this point, we found an ASCII byte or a UTF-8 starting byte at position source[index].
				// Technically this could also mean we found an invalid byte like C0 or F5 at this position, but that's
				// fine since it'll be handled by the forward read. From this position, we'll perform a forward read
				// and see if we consumed the entirety of the buffer.

				source = source.Slice(index);
				Debug.Assert(!source.IsEmpty, "Shouldn't reach this for empty inputs.");

				OperationStatus operationStatus = DecodeFromUtf8(source, out Rune tempRune, out Int32 tempBytesConsumed);
				if (tempBytesConsumed == source.Length) {
					// If this forward read consumed the entirety of the end of the input buffer, we can return it
					// as the result of this function. It could be well-formed, incomplete, or invalid. If it's
					// invalid and we consumed the remainder of the buffer, we know we've found the maximal invalid
					// subsequence, which is what we wanted anyway.

					bytesConsumed = tempBytesConsumed;
					value = tempRune;
					return operationStatus;
				}

				// If we got to this point, we know that the final continuation byte wasn't consumed by the forward
				// read that we just performed above. This means that the continuation byte has to be part of an
				// invalid subsequence since there's no UTF-8 leading byte between what we just consumed and the
				// continuation byte at the end of the input. Furthermore, since any maximal invalid subsequence
				// of length > 1 must have a UTF-8 leading byte as its first code unit, this implies that the
				// continuation byte at the end of the buffer is itself a maximal invalid subsequence of length 1.

				goto Invalid;
			} else {
				// Source buffer was empty.
				value = ReplacementChar;
				bytesConsumed = 0;
				return OperationStatus.NeedMoreData;
			}
		}

		public static explicit operator Rune(Char ch) => new Rune(ch);

		[CLSCompliant(false)]
		public static explicit operator Rune(UInt32 value) => new Rune(value);

		public static explicit operator Rune(Int32 value) => new Rune(value);

		/// <summary>
		/// Gets the <see cref="Rune"/> which begins at index <paramref name="index"/> in
		/// string <paramref name="input"/>.
		/// </summary>
		/// <remarks>
		/// Throws if <paramref name="input"/> is null, if <paramref name="index"/> is out of range, or
		/// if <paramref name="index"/> does not reference the start of a valid scalar value within <paramref name="input"/>.
		/// </remarks>
		public static Rune GetRuneAt(String input, Int32 index) {
			Int32 runeValue = ReadRuneFromString(input, index);
			if (runeValue < 0) {
				throw new ArgumentException("Cannot extract scalar value", nameof(index));
			}

			return new Rune((uint)runeValue);
		}

		/// <summary>
		/// Returns <see langword="true"/> iff <paramref name="value"/> is a valid Unicode scalar
		/// value, i.e., is in [ U+0000..U+D7FF ], inclusive; or [ U+E000..U+10FFFF ], inclusive.
		/// </summary>
		public static Boolean IsValid(Int32 value) => IsValid((UInt32)value);

		/// <summary>
		/// Returns <see langword="true"/> iff <paramref name="value"/> is a valid Unicode scalar
		/// value, i.e., is in [ U+0000..U+D7FF ], inclusive; or [ U+E000..U+10FFFF ], inclusive.
		/// </summary>
		[CLSCompliant(false)]
		public static Boolean IsValid(UInt32 value) => Unsafe.IsScalarValue(value);

		// returns a negative number on failure
		internal static int ReadFirstRuneFromUtf16Buffer(ReadOnlySpan<char> input) {
			if (input.IsEmpty) {
				return -1;
			}

			// Optimistically assume input is within BMP.

			uint returnValue = input[0];
			if (Utf16.IsSurrogate(returnValue)) {
				if (!Utf16.IsHighSurrogate(returnValue)) {
					return -1;
				}

				// Treat 'returnValue' as the high surrogate.

				if (1 >= (uint)input.Length) {
					return -1; // not an argument exception - just a "bad data" failure
				}

				uint potentialLowSurrogate = input[1];
				if (!Utf16.IsLowSurrogate(potentialLowSurrogate)) {
					return -1;
				}

				returnValue = Unsafe.Utf16Decode(returnValue, potentialLowSurrogate);
			}

			return (int)returnValue;
		}

		public static Boolean operator !=(Rune left, Rune right) => left.value != right.value;

		public static Boolean operator <(Rune left, Rune right) => left.value < right.value;

		public static Boolean operator <=(Rune left, Rune right) => left.value <= right.value;

		public static Boolean operator ==(Rune left, Rune right) => left.value == right.value;
		public static Boolean operator >(Rune left, Rune right) => left.value > right.value;

		public static Boolean operator >=(Rune left, Rune right) => left.value >= right.value;

		/// <summary>
		/// Attempts to create a <see cref="Rune"/> from the provided input value.
		/// </summary>
		public static Boolean TryCreate(Char ch, out Rune result) {
			UInt32 extendedValue = ch;
			if (!Utf16.IsSurrogate(extendedValue)) {
				result = new Rune(extendedValue);
				return true;
			} else {
				result = default;
				return false;
			}
		}

		/// <summary>
		/// Attempts to create a <see cref="Rune"/> from the provided UTF-16 surrogate pair.
		/// Returns <see langword="false"/> if the input values don't represent a well-formed UTF-16surrogate pair.
		/// </summary>
		public static Boolean TryCreate(Char highSurrogate, Char lowSurrogate, out Rune result) {
			// First, extend both to 32 bits, then calculate the offset of
			// each candidate surrogate char from the start of its range.

			UInt32 highSurrogateOffset = highSurrogate - 0xD800u;
			UInt32 lowSurrogateOffset = lowSurrogate - 0xDC00u;

			// This is a single comparison which allows us to check both for validity at once since
			// both the high surrogate range and the low surrogate range are the same length.
			// If the comparison fails, we call to a helper method to throw the correct exception message.

			if ((highSurrogateOffset | lowSurrogateOffset) <= 0x3FFu) {
				// The 0x40u << 10 below is to account for uuuuu = wwww + 1 in the surrogate encoding.
				result = new Rune((highSurrogateOffset << 10) + (lowSurrogate - 0xDC00u) + (0x40u << 10));
				return true;
			} else {
				// Didn't have a high surrogate followed by a low surrogate.
				result = default;
				return false;
			}
		}

		/// <summary>
		/// Attempts to create a <see cref="Rune"/> from the provided input value.
		/// </summary>
		public static Boolean TryCreate(Int32 value, out Rune result) => TryCreate((UInt32)value, out result);

		/// <summary>
		/// Attempts to create a <see cref="Rune"/> from the provided input value.
		/// </summary>
		[CLSCompliant(false)]
		public static Boolean TryCreate(UInt32 value, out Rune result) {
			if (Unsafe.IsScalarValue(value)) {
				result = new Rune(value);
				return true;
			} else {
				result = default;
				return false;
			}
		}

		public Int32 CompareTo(Rune other) => value.CompareTo(other.value);

		/// <summary>
		/// Encodes this <see cref="Rune"/> to a UTF-16 destination buffer.
		/// </summary>
		/// <param name="destination">The buffer to which to write this value as UTF-16.</param>
		/// <returns>The number of <see cref="Char"/>s written to <paramref name="destination"/>.</returns>
		/// <exception cref="ArgumentException">
		/// If <paramref name="destination"/> is not large enough to hold the output.
		/// </exception>
		public Int32 EncodeToUtf16(Span<Char> destination) {
			if (!TryEncodeToUtf16(destination, out Int32 charsWritten)) {
				throw new ArgumentException("Destination too short", nameof(destination));
			}
			return charsWritten;
		}

		/// <summary>
		/// Encodes this <see cref="Rune"/> to a UTF-8 destination buffer.
		/// </summary>
		/// <param name="destination">The buffer to which to write this value as UTF-8.</param>
		/// <returns>The number of <see cref="Byte"/>s written to <paramref name="destination"/>.</returns>
		/// <exception cref="ArgumentException">
		/// If <paramref name="destination"/> is not large enough to hold the output.
		/// </exception>
		public Int32 EncodeToUtf8(Span<Byte> destination) {
			if (!TryEncodeToUtf8(destination, out Int32 bytesWritten)) {
				throw new ArgumentException("Destination too short", nameof(destination));
			}
			return bytesWritten;
		}

		public override Boolean Equals(Object? obj) => (obj is Rune other) && Equals(other);

		public Boolean Equals(Rune other) => this == other;

		public override Int32 GetHashCode() => Value;

		/// <summary>
		/// Returns a <see cref="String"/> representation of this <see cref="Rune"/> instance.
		/// </summary>
		public override String ToString() {
			if (IsBmp) {
				return $"{(Char)value}";
			} else {
				Unsafe.Utf16Encode(value, out Char high, out Char low);
				return $"{high}{low}";
			}
		}

		/// <summary>
		/// Encodes this <see cref="Rune"/> to a UTF-16 destination buffer.
		/// </summary>
		/// <param name="destination">The buffer to which to write this value as UTF-16.</param>
		/// <param name="charsWritten">
		/// The number of <see cref="Char"/>s written to <paramref name="destination"/>,
		/// or 0 if the destination buffer is not large enough to contain the output.</param>
		/// <returns>True if the value was written to the buffer; otherwise, false.</returns>
		/// <remarks>
		/// The <see cref="Utf16SequenceLength"/> property can be queried ahead of time to determine
		/// the required size of the <paramref name="destination"/> buffer.
		/// </remarks>
		public Boolean TryEncodeToUtf16(Span<Char> destination, out Int32 charsWritten) {
			if (destination.Length >= 1) {
				if (new CodePoint(value).IsBmp) {
					destination[0] = (Char)value;
					charsWritten = 1;
					return true;
				} else if (destination.Length >= 2) {
					SurrogatePair surrogates = new SurrogatePair(new CodePoint(value));
					destination[0] = (Char)surrogates.High.Value;
					destination[1] = (Char)surrogates.Low.Value;
					charsWritten = 2;
					return true;
				}
			}

			// Destination buffer not large enough

			charsWritten = default;
			return false;
		}

		/// <summary>
		/// Encodes this <see cref="Rune"/> to a destination buffer as UTF-8 bytes.
		/// </summary>
		/// <param name="destination">The buffer to which to write this value as UTF-8.</param>
		/// <param name="bytesWritten">
		/// The number of <see cref="Byte"/>s written to <paramref name="destination"/>,
		/// or 0 if the destination buffer is not large enough to contain the output.</param>
		/// <returns>True if the value was written to the buffer; otherwise, false.</returns>
		/// <remarks>
		/// The <see cref="Utf8SequenceLength"/> property can be queried ahead of time to determine
		/// the required size of the <paramref name="destination"/> buffer.
		/// </remarks>
		public Boolean TryEncodeToUtf8(Span<Byte> destination, out Int32 bytesWritten) {
			// The bit patterns below come from the Unicode Standard, Table 3-6.

			if (destination.Length >= 1) {
				if (IsAscii) {
					destination[0] = (Byte)value;
					bytesWritten = 1;
					return true;
				}

				if (destination.Length >= 2) {
					if (value <= 0x7FFu) {
						// Scalar 00000yyy yyxxxxxx -> bytes [ 110yyyyy 10xxxxxx ]
						destination[0] = (Byte)((value + (0b110u << 11)) >> 6);
						destination[1] = (Byte)((value & 0x3Fu) + 0x80u);
						bytesWritten = 2;
						return true;
					}

					if (destination.Length >= 3) {
						if (value <= 0xFFFFu) {
							// Scalar zzzzyyyy yyxxxxxx -> bytes [ 1110zzzz 10yyyyyy 10xxxxxx ]
							destination[0] = (Byte)((value + (0b1110 << 16)) >> 12);
							destination[1] = (Byte)(((value & (0x3Fu << 6)) >> 6) + 0x80u);
							destination[2] = (Byte)((value & 0x3Fu) + 0x80u);
							bytesWritten = 3;
							return true;
						}

						if (destination.Length >= 4) {
							// Scalar 000uuuuu zzzzyyyy yyxxxxxx -> bytes [ 11110uuu 10uuzzzz 10yyyyyy 10xxxxxx ]
							destination[0] = (Byte)((value + (0b11110 << 21)) >> 18);
							destination[1] = (Byte)(((value & (0x3Fu << 12)) >> 12) + 0x80u);
							destination[2] = (Byte)(((value & (0x3Fu << 6)) >> 6) + 0x80u);
							destination[3] = (Byte)((value & 0x3Fu) + 0x80u);
							bytesWritten = 4;
							return true;
						}
					}
				}
			}

			// Destination buffer not large enough

			bytesWritten = default;
			return false;
		}

		/// <summary>
		/// Attempts to get the <see cref="Rune"/> which begins at index <paramref name="index"/> in
		/// string <paramref name="input"/>.
		/// </summary>
		/// <returns><see langword="true"/> if a scalar value was successfully extracted from the specified index,
		/// <see langword="false"/> if a value could not be extracted due to invalid data.</returns>
		/// <remarks>
		/// Throws only if <paramref name="input"/> is null or <paramref name="index"/> is out of range.
		/// </remarks>
		public static Boolean TryGetRuneAt(String input, Int32 index, out Rune value) {
			Int32 runeValue = ReadRuneFromString(input, index);
			if (runeValue >= 0) {
				value = new Rune((UInt32)runeValue);
				return true;
			} else {
				value = default;
				return false;
			}
		}

		private static Int32 ReadRuneFromString(String input, Int32 index) {
			if (input is null) {
				throw new ArgumentNullException(nameof(input));
			}

			if ((UInt32)index >= (UInt32)input!.Length) {
				throw new ArgumentOutOfRangeException(nameof(index));
			}

			// Optimistically assume input is within BMP.

			UInt32 returnValue = input[index];
			if (Utf16.IsSurrogate(returnValue)) {
				if (!Utf16.IsHighSurrogate(returnValue)) {
					return -1;
				}

				// Treat 'returnValue' as the high surrogate.
				//
				// If this becomes a hot code path, we can skip the below bounds check by reading
				// off the end of the string using unsafe code. Since strings are null-terminated,
				// we're guaranteed not to read a valid low surrogate, so we'll fail correctly if
				// the string terminates unexpectedly.

				index++;
				if ((UInt32)index >= (UInt32)input.Length) {
					return -1; // not an argument exception - just a "bad data" failure
				}

				UInt32 potentialLowSurrogate = input[index];
				if (!Utf16.IsLowSurrogate(potentialLowSurrogate)) {
					return -1;
				}

				returnValue = Unsafe.Utf16Decode(returnValue, potentialLowSurrogate);
			}

			return (Int32)returnValue;
		}

		public static Double GetNumericValue(Rune value) => CharUnicodeInfo.GetNumericValue(value.ToString(), 0);

		public static UnicodeCategory GetUnicodeCategory(Rune value) => CharUnicodeInfo.GetUnicodeCategory(value.ToString(), 0);

		public static Boolean IsControl(Rune value) => ((value.value + 1) & ~0x80u) <= 0x20u;

		public static Boolean IsDigit(Rune value) => GetUnicodeCategory(value) == UnicodeCategory.DecimalDigitNumber;

		public static Boolean IsLetter(Rune value) => ((UInt32)GetUnicodeCategory(value)).Within((UInt32)UnicodeCategory.UppercaseLetter, (UInt32)UnicodeCategory.OtherLetter);

		public static Boolean IsLetterOrDigit(Rune value) {
			UInt32 category = (UInt32)GetUnicodeCategory(value);
			return category.Within((UInt32)UnicodeCategory.UppercaseLetter, (UInt32)UnicodeCategory.OtherLetter) || (category == (UInt32)UnicodeCategory.DecimalDigitNumber);
		}

		public static Boolean IsLower(Rune value) => GetUnicodeCategory(value) == UnicodeCategory.LowercaseLetter;

		public static Boolean IsNumber(Rune value) => ((UInt32)GetUnicodeCategory(value)).Within((UInt32)UnicodeCategory.DecimalDigitNumber, (UInt32)UnicodeCategory.OtherNumber);

		public static Boolean IsPunctuation(Rune value) => ((UInt32)GetUnicodeCategory(value)).Within((UInt32)UnicodeCategory.ConnectorPunctuation, (UInt32)UnicodeCategory.OtherPunctuation);

		public static Boolean IsSeparator(Rune value) => ((UInt32)GetUnicodeCategory(value)).Within((UInt32)UnicodeCategory.SpaceSeparator, (UInt32)UnicodeCategory.ParagraphSeparator);

		public static Boolean IsSymbol(Rune value) => ((UInt32)GetUnicodeCategory(value)).Within((UInt32)UnicodeCategory.MathSymbol, (UInt32)UnicodeCategory.OtherSymbol);

		public static Boolean IsUpper(Rune value) => GetUnicodeCategory(value) == UnicodeCategory.UppercaseLetter;

		public static Boolean IsWhiteSpace(Rune value) => value.IsBmp && Char.IsWhiteSpace((Char)value.value); //BMP codepoints are always representable by a single Char, so this is safe.

		public static Rune ToLower(Rune value, CultureInfo culture) => GetRuneAt(value.ToString().ToLower(culture), 0);

		public static Rune ToLowerInvariant(Rune value) => GetRuneAt(value.ToString().ToLowerInvariant(), 0);

		public static Rune ToUpper(Rune value, CultureInfo culture) => GetRuneAt(value.ToString().ToUpper(culture), 0);

		public static Rune ToUpperInvariant(Rune value) => GetRuneAt(value.ToString().ToUpperInvariant(), 0);
	}
}
#endif
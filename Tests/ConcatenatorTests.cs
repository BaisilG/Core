﻿using System;
using System.Text.Patterns;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests {
	[TestClass]
	public class ConcatenatorTests {
		[TestMethod]
		public void Constructor() {
			Pattern _ = (Pattern)"Hello" & ' ' & "world";
		}

		[TestMethod]
		public void Consume() {
			Pattern HelloWorld = (Pattern)"Hello" & ' ' & "world";
			Pattern GoodbyeWorld = (Pattern)"Goodbye" & ' ' & "world";
			Result Result;

			Result = HelloWorld.Consume("Hello world");
			Assert.That.Succeeds(Result);
			Assert.That.Captures("Hello world", Result);

			Result = GoodbyeWorld.Consume("Goodbye world");
			Assert.That.Succeeds(Result);
			Assert.That.Captures("Goodbye world", Result);

			Result = HelloWorld.Consume("Hello everyone");
			Assert.That.Fails(Result);

			Result = GoodbyeWorld.Consume("Hello world");
			Assert.That.Fails(Result);
		}
	}
}

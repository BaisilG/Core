﻿namespace Tests.Patterns

open System
open Stringier
open Stringier.Patterns
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type StressTests() =

    [<TestMethod>]
    member _.``gibberish`` () =
        let mutable source = Source(Gibberish.Generate(128))
        let letter = Pattern.Check("letter", (fun (char) -> 'a' <= char && char <= 'z'))
        let word = span letter
        let space = span ' '
        let gibberish:Pattern = (span (word || space)) >> Pattern.EndOfSource
        ResultAssert.Succeeds(gibberish.Consume(&source))
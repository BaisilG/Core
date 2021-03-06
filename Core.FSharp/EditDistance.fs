﻿namespace Stringier

open System

[<AutoOpen>]
module EditDistance =
    type Binder =
        static member Hamming(source:string, other:string) = Metrics.HammingDistance(source, other)
        static member Hamming(source:char[], other:char[]) = Metrics.HammingDistance(source, other)
        static member Levenshtein(source:string, other:string) = Metrics.LevenshteinDistance(source, other)
        static member Levenshtein(source:char[], other:char[]) = Metrics.LevenshteinDistance(source, other)

    let inline private _hamming< ^t, ^a, ^b when (^t or ^a) : (static member Hamming : ^a * ^b -> int)> source other = ((^t or ^a) : (static member Hamming : ^a * ^b -> int)(source, other))

    let inline private _levenshtein< ^t, ^a, ^b when (^t or ^a) : (static member Levenshtein : ^a * ^b -> int)> source other = ((^t or ^a) : (static member Levenshtein : ^a * ^b -> int)(source, other))

    /// <summary>
    /// Calculates the Hamming edit-distance between source and other
    /// </summary>
    let inline hamming(source:^a)(other:^b):int32 = _hamming<Binder, ^a, ^b> source other

    /// <summary>
    /// Calculates the Levenshtein edit-distance between source and other
    /// </summary>
    let inline levenshtein(source:^a)(other:^b):int32 = _levenshtein<Binder, ^a, ^b> source other
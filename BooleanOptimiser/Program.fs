﻿open System

type ops = Ldarg of int | And | Or | Xor | Not | Dup | Pop | Ones | Zeros

[<EntryPoint>]
let main argv =

    /// Instructions understood and used by the optimiser
    let validinstructions = [|Ldarg 0; Ldarg 1; Ldarg 2; Ldarg 3;
                              And; Or; Xor; Dup; Pop; Not; Ones; Zeros|]

    /// Bit patterns for computing the truth table in parallel
    let args = [|0b1010101010101010; 0b1100110011001100;
                 0b1111000011110000; 0b1111111100000000|]


    /// Execute a single stack based instruction
    let exec = function
               | stk,Ldarg n   -> Some(args.[n]::stk)
               | x::y::stk,And -> Some((x &&& y)::stk)
               | x::y::stk,Or  -> Some((x ||| y)::stk)
               | x::y::stk,Xor -> Some((x ^^^ y)::stk)
               | x::stk,Not    -> Some((x ^^^ 0xffff)::stk)
               | x::stk,Dup    -> Some(x::x::stk)
               | x::stk,Pop    -> Some(stk)
               | stk,Ones      -> Some(0xffff::stk)
               | stk,Zeros     -> Some(0::stk)
               | _             -> None


    /// Calculate one truth table for each return value of a function
    let truthtables =
        List.fold (fun stk op ->
            stk |> Option.bind (fun s -> exec (s, op))) (Some([]))


    /// Increment a number in the form of a list of digits
    let rec increment max = function
        | x::xs when x = max -> 0::(increment max xs)
        | x::xs              -> (x + 1)::xs
        | _                  -> failwith "Empty list!"


    /// Create a list of all numbers of length and radix
    let makenumbers radix length =
        let rec loop number =
            seq { yield number
                  yield! loop (increment (radix - 1) number) }

        loop (List.replicate length 0)
        |> Seq.take (pown radix length)


    /// Populating the dictionary with the optimal functions,
    //  by testing the functions by increasing length from 0 to size
    //  the first function to generate a truth table is the shortest one.
    let createdictionary size =
        seq { 0 .. size }
        |> Seq.map (makenumbers validinstructions.Length)
        |> Seq.concat
        |> Seq.map (List.map (Array.get validinstructions))
        |> Seq.map (fun f -> truthtables f |> Option.map (fun t -> (t, f)))
        |> Seq.choose id
        |> Seq.distinctBy fst
        |> Map.ofSeq

    let optimalfunctions = createdictionary 5
    printfn "Number of optimal functions in dictionary: %A\n" optimalfunctions.Count


    /// Display results
    let show = function
        | prog,Some(result) -> if optimalfunctions.ContainsKey result
                               then printfn " Source:  %A" prog
                                    printfn "Optimal:  %A\n" optimalfunctions.[result]
                               else printfn "Not found:  %A\n" prog
        | prog,None         -> printfn "Invalid:  %A" prog


    /// Functions to be optimized
    let progs =
        [ [Ldarg 0; Ldarg 1; Or; Ldarg 0; Ldarg 1; And; Not; And]
          [Ldarg 0; Ldarg 0; Xor]
          [Ldarg 0; Pop]
          [Ldarg 0; Ldarg 1; And; Dup; Or]
          [Zeros; Not]
        ]

    // Perform optimizations and display the reults
    List.map truthtables progs
    |> List.zip progs
    |> List.iter show

    Console.ReadKey() |> ignore
    0
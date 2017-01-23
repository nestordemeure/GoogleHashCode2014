module GHC.Main

open FSharpx.Collections

open GHC.Extensions
open GHC.Extensions.Common
open GHC.Domain
open GHC.Import
open GHC.Solve
open GHC.Export

// missing :
// toseq, fromseq 
// mutable set
// scanf

//-------------------------------------------------------------------------------------------------
// SCORING

/// stores the final score for the solution
let mutable score = 0

/// takes a car and update the score
let scoreOfCar (visited : bool []) (graph : Graph) car =
   let rec visit path = 
      match path with 
      | [] | [_] -> visited
      | n1::n2::q -> 
         let street = Seq.find (fun s -> s.destination = n1) graph.[n2]
         if not visited.[street.id] then score <- street.score + score
         visited.[street.id] <- true
         visit (n2::q)
   visit car.path

/// compute and displays the score given all the cars
let computeScore streetNumber graph cars =
    score <- 0
    let visited = Array.create streetNumber false
    Array.fold (fun visi car -> scoreOfCar visi graph car) visited cars
    |> ignore
    printfn "score : %d" score

//-------------------------------------------------------------------------------------------------
// MAIN

[<EntryPoint>]
let main argv =
    // import
    let path = "../paris_54000.txt"
    let graph, streetNumber, timeMax, carNumber, startingPoint = import path
    let cars = createCars carNumber startingPoint
    // solve
    //let newCars = greedySolver timeMax streetNumber graph cars
    let newCars = dijSolver timeMax streetNumber graph cars
    // export
    computeScore streetNumber graph newCars
    let outPath = "../outPut.txt"
    export outPath newCars
    0 // return an integer exit code

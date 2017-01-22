module GHC.Main

open FSharpx.Collections

open GHC.Extensions
open GHC.Extensions.Common
open GHC.Domain
open GHC.Import
open GHC.Solve
open GHC.Export

//19h20
// toseq, fromseq visitedDatastruct
// greedy   13037
// dij      246485
// dij10    714622
// dij20    953999
// dij40    1191173
// dij80    1416819
// dij160   1612427
// dij320   1741722 -> 14ème
// dij360   1561408
// dij400   1732651
// dij500   1514378
// dij1000  1580493
//0. 1/400 1 1732398

//(**)      1827709
// /100     1833850
// /80  1834458 1838669 1842851





//-------------------------------------------------------------------------------------------------



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
    let pas = int argv.[0]
    let newCars = dijSolver pas timeMax streetNumber graph cars
    //export
    let outPath = "../outPut.txt"
    export outPath newCars
    printfn "%d" score
    0 // return an integer exit code

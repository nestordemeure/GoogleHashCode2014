module GHC.Solve

open FSharpx.Collections
open System

open GHC.Extensions
open GHC.Domain

let mutable score = 0

//-------------------------------------------------------------------------------------------------



//-------------------------------------------------------------------------------------------------
// SOLUTION

let rec greedySolverRec timeMax visited (graph : Graph) carQueue =
   match MPriorityQueue.popMin carQueue with 
   | None -> 
      carsOfQueue carQueue
   | Some (usedTime, car) when usedTime >= timeMax -> 
      MPriorityQueue.push usedTime car carQueue
      carsOfQueue carQueue
   | Some (usedTime, car) ->
      let streets = List.filter (fun s -> s.time + usedTime <= timeMax) graph.[car.position]
      match streets with 
      | [] ->
         MPriorityQueue.push Int32.MaxValue car carQueue
         greedySolverRec timeMax visited graph carQueue
      | _ -> 
         let street = List.maxBy (rentability visited) streets
         if not visited.[street.id] then score <- street.score + score
         visited.[street.id] <- true
         let newCar = { position = street.destination ; usedTime = usedTime + street.time ; path = street.destination::car.path }
         MPriorityQueue.push newCar.usedTime newCar carQueue
         greedySolverRec timeMax visited graph carQueue

let greedySolver timeMax streetNumber (graph : Graph) cars =
   let visited = Array.create streetNumber false
   let carQueue = 
      let q = MPriorityQueue.empty
      for car in cars do 
         MPriorityQueue.push car.usedTime car q
      q
   let cars = greedySolverRec timeMax visited (graph : Graph) carQueue
   cars

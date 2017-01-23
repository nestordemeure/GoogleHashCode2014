module GHC.Solve

open FSharpx.Collections
open System

open GHC.Extensions
open GHC.Extensions.Common
open GHC.Domain

//-------------------------------------------------------------------------------------------------

let dijstra timeMax (visited:bool array) (graph : Graph) car =
   let mutable bestCar = car 
   let visitedJunct = Array.create graph.Length false

   let carQueue = MPriorityQueue.empty
   MPriorityQueue.push 0 car carQueue

   let rng = System.Random()

   let rec dijstraRec carQueue = 
      match MPriorityQueue.popMin carQueue with 
      | None -> ()
      | Some (t, car) when (car.usedTime >= timeMax) (*|| visitedJunct.[car.position]*) -> dijstraRec carQueue
      | Some (t, car) ->
         if car.distance > bestCar.distance then bestCar <- car
         visitedJunct.[car.position] <- true
         let streets = List.filter (fun s -> (s.time + car.usedTime <= timeMax) && (not visitedJunct.[s.destination])) graph.[car.position]
         for street in streets do 
            let newDistance = if visited.[street.id] then car.distance else car.distance + street.score
            visited.[street.id] <- true
            let newCar = { 
               distance = newDistance ; 
               position = street.destination ; 
               usedTime = car.usedTime + street.time ; 
               path = street.destination::car.path }
            MPriorityQueue.push (newCar.usedTime + rng.Next(timeMax/80)) newCar carQueue
         dijstraRec carQueue
   
   dijstraRec carQueue
   bestCar

let visitsOfCar (visited : bool []) (graph : Graph) car =
   let rec visit path = 
      match path with 
      | [] | [_] -> ()
      | n1::n2::q -> 
         let street = Seq.find (fun s -> s.destination = n1) graph.[n2]
         visited.[street.id] <- true
         visit (n2::q)
   visit car.path

let greedyDij timeMax visited graph car =
      car |> dijstra timeMax (Array.copy visited) graph |-> visitsOfCar visited graph

let timeOfFloat timeMax x = int (x*(float timeMax))
let dijSolver timeMax streetNumber (graph : Graph) (cars:Car []) =
   let visited = Array.create streetNumber false
   let inline gDij x car = greedyDij (timeOfFloat timeMax x) visited graph car
   let fractions = [ 0. .. (1./400.) .. 1.]
   let rec compose fractions cars =
      match fractions with 
      | [] -> cars
      | x::q -> cars |> Array.map (gDij x) |> compose q
   let newCars = compose fractions cars
   newCars


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
         visited.[street.id] <- true
         let newCar = { distance = 0 ; position = street.destination ; usedTime = usedTime + street.time ; path = street.destination::car.path }
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

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

//-----

/// update an array of visited streets with a car's path
let updateVisitedWithCar (visited : bool []) (graph : Graph) car =
   let rec visit path = 
      match path with 
      | [] | [_] -> ()
      | n1::n2::q -> 
         let street = List.find (fun s -> s.destination = n1) graph.[n2]
         visited.[street.id] <- true
         visit (n2::q)
   visit car.path

//-----

let greedyDij timeMax visited graph car =
      car |> dijstra timeMax (Array.copy visited) graph |-> updateVisitedWithCar visited graph

let timeOfFloat timeMax x = int (x*(float timeMax))

//-----

/// greedy dijstra algorithm + fractionement + random disturbance
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

/// score/time
let rentability (visitedStreets : bool[]) street =
// could be computed to get as far as possible from the other car
      match visitedStreets.[street.id] with 
      | true -> 0.
      | false -> (float street.score) / (float street.time)

/// the cars take the best street one after the other until they cannot drive anymore
let rec greedySolverRec timeMax visited (graph : Graph) carQueue =
   match MPriorityQueue.popMin carQueue with 
   // no car in the queue, should not happend
   | None -> carQueue
   // no more car able to drive, time to stop
   | Some (usedTime, car) when usedTime >= timeMax -> 
      MPriorityQueue.push usedTime car carQueue
      carQueue
   // a valid car
   | Some (usedTime, car) ->
      /// all the streets reachable in the given time
      let streets = List.filter (fun s -> s.time + usedTime <= timeMax) graph.[car.position]
      match streets with 
      | [] ->
         // no street, that car cannot drive anymore
         MPriorityQueue.push Int32.MaxValue car carQueue
         greedySolverRec timeMax visited graph carQueue
      | _ -> 
         // take the best street on sight
         let street = List.maxBy (rentability visited) streets
         visited.[street.id] <- true
         let newCar = 
            { 
                  distance = car.distance + street.score
                  position = street.destination
                  usedTime = usedTime + street.time
                  path = street.destination :: car.path 
            }
         MPriorityQueue.push newCar.usedTime newCar carQueue
         greedySolverRec timeMax visited graph carQueue

//-----

/// greedy algorithm, the cars take the best street one after the other until they cannot drive anymore
let greedySolver timeMax streetNumber (graph : Graph) cars =
   let visited = Array.create streetNumber false
   cars
   |> Array.fold (fun q car -> q |-> MPriorityQueue.push car.usedTime car) MPriorityQueue.empty //carQueue
   |> greedySolverRec timeMax visited graph
   |> carsOfQueue

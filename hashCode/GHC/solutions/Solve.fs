module GHC.Solve

open FSharpx.Collections
open System

open GHC.Extensions
open GHC.Extensions.Common
open GHC.Domain

//-------------------------------------------------------------------------------------------------

/// takes a car and returns the car after it drove in a street (also update visited)
let carOfStreet (visited:bool[]) (originalCar:Car) street =
      let newDistance = 
         match visited.[street.id] with
         | true -> originalCar.distance
         | false -> originalCar.distance + street.score
      visited.[street.id] <- true
      { 
         distance = newDistance
         position = street.destination
         usedTime = originalCar.usedTime + street.time
         path = street.destination :: originalCar.path 
      }

/// searches for the best path a car could follow in the given time
let dijstra timeMax (visited:bool array) (graph : Graph) car =
   let perturbation = 
      let rng = System.Random()
      fun () -> rng.Next(timeMax/80)
   let visitedJunct = Array.create graph.Length false
   let carQueue = MPriorityQueue.empty |-> MPriorityQueue.push 0 car
   let mutable bestCar = car 
   /// search
   let rec dijstraRec carQueue = 
      match MPriorityQueue.popMin carQueue with 
      // no more car, time to stop
      | None -> ()
      // the top car is useless, try again
      | Some (time, car) when (car.usedTime >= timeMax) -> 
         dijstraRec carQueue
      // a useful car
      | Some (time, car) ->
         // saves a car if it is good, note that the junction has been visited
         if car.distance > bestCar.distance then bestCar <- car
         visitedJunct.[car.position] <- true
         // computes all legal path and saves them
         graph.[car.position] // streets
         |> List.filter (fun s -> (s.time + car.usedTime <= timeMax) && (not visitedJunct.[s.destination]) )
         |> List.map (carOfStreet visited car)
         |> List.iter (fun c -> MPriorityQueue.push (c.usedTime + perturbation()) c carQueue)
         // starts with the new queue
         dijstraRec carQueue
   dijstraRec carQueue ; bestCar

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

/// greedy dijstra algorithm + fractionement + random disturbance
let dijSolver timeMax streetNumber (graph : Graph) (cars:Car []) =
   let visited = Array.create streetNumber false
   let fraction = 400
   // cut the time into steps
   for step = 1 to fraction do 
      // greedy, optimise for the cars in order
      let localTimeMax = (step*timeMax) / fraction
      for c = 0 to cars.Length - 1 do 
         // runs dijstra for a car and update the visited streets
         cars.[c] <- dijstra localTimeMax (Array.copy visited) graph cars.[c]
         updateVisitedWithCar visited graph cars.[c]
   cars

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
   |> Seq.map (fun car -> car.usedTime, car)
   |> MPriorityQueue.fromSeq
   |> greedySolverRec timeMax visited graph
   |> carsOfQueue

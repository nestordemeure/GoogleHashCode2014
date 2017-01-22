module GHC.Domain

open FSharpx.Collections
open System.Collections.Generic

open GHC.Extensions
open GHC.Extensions.MPriorityQueue

//-------------------------------------------------------------------------------------------------

//type graph = Dictionary<'key,'Node>
type Junction = { score : int ; time : int ; destination : int ; id : int }

// could be computed to get as far as possible from the other car
let rentability (visited : bool[]) junction =
   if visited.[junction.id] then 0. else 
      (float junction.score) / (float junction.time)

type Graph = (Junction list) array

//-------------------------------------------------------------------------------------------------

type Car = { distance : int ; position : int ; usedTime : int ; path : int list }

let createCars carNumber startingPoint = 
   Array.create carNumber { distance = 0 ;  position = startingPoint ; usedTime = 0 ; path = [startingPoint] }

let carsOfQueue (q : MutablePriorityQueue<'K,'V>) =
   let result = Array.init (MPriorityQueue.size q) (fun i -> q.[i].v )
   result
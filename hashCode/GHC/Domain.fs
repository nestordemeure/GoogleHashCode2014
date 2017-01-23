module GHC.Domain

open FSharpx.Collections
open System.Collections.Generic

open GHC.Extensions
open GHC.Extensions.MPriorityQueue

//-------------------------------------------------------------------------------------------------

type Street = { score : int ; time : int ; destination : int ; id : int }

type Graph = (Street list) array

//-------------------------------------------------------------------------------------------------

type Car = { distance : int ; position : int ; usedTime : int ; path : int list }

/// create an array of empty cars
let createCars carNumber startingPoint = 
   Array.create carNumber { distance = 0 ;  position = startingPoint ; usedTime = 0 ; path = [startingPoint] }

/// takes a queue of cars and output an array of cars
let carsOfQueue (q : MutablePriorityQueue<'K,'V>) =
   Array.init (MPriorityQueue.size q) (fun i -> q.[i].v )

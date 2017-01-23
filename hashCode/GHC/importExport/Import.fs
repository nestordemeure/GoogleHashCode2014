module GHC.Import

open FSharpx.Collections
open System.IO

open GHC.Extensions
open GHC.Domain

//-------------------------------------------------------------------------------------------------



//-------------------------------------------------------------------------------------------------
// IMPORT

/// takes a path and outputs a graph*streetNumber*timeMax*carNumber*startingPoint
let import path =
   let text = 
      path
      |> File.ReadAllLines
      |> Array.map (fun (s:string) -> s.Split ' ')
   // the entete
   let entete = text.[0] |> Array.map int
   let junctionNum = entete.[0]
   let streetNum = entete.[1]
   let timeMax = entete.[2]
   let carNum = entete.[3]
   let startingPoint = entete.[4]
   // the final output for the importation
   let graph : Graph = Array.create junctionNum []
   // parse all the streets
   for i = junctionNum + 1 to text.Length - 1 do
      let id = i - junctionNum - 1
      let line = text.[i] |> Array.map int
      let fromJ = line.[0]
      let toJ = line.[1]
      let twoWay = line.[2] = 2
      let time = line.[3]
      let score = line.[4]
      let street = { score = score ; time = time ; destination = toJ ; id = id }
      graph.[fromJ] <- street::graph.[fromJ]
      if twoWay then graph.[toJ] <- {street with destination = fromJ}::graph.[toJ]
   graph, streetNum, timeMax, carNum, startingPoint
   
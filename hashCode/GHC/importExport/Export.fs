module GHC.Export

open FSharpx.Collections
open System.IO

open GHC.Extensions
open GHC.Domain

//-------------------------------------------------------------------------------------------------

/// takes a list and export a string of the elements separated by '\n'
let rec stringOfList l =
   match l with 
   | [] -> ""
   | _ -> List.reduce (fun acc s -> s + "\n" + acc) l

/// takes a car and output a string representation
let stringOfCar car = 
   let path = List.map string car.path
   sprintf "%d\n%s" (List.length car.path) (stringOfList path)

//-------------------------------------------------------------------------------------------------
// EXPORTATION

/// writes the array of cars to the path
let export path cars =
   let carNumber = Array.length cars |> string
   cars
   |> Array.map stringOfCar
   |> Array.fold (fun acc carStr -> acc + "\n" + carStr) carNumber
   |> fun txt -> File.WriteAllText(path, txt)
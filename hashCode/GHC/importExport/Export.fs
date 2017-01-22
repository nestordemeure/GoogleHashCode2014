module GHC.Export

open FSharpx.Collections
open System.IO

open GHC.Extensions
open GHC.Domain

//-------------------------------------------------------------------------------------------------

let rec stringOfList l =
   match l with 
   | [] -> ""
   | _ -> List.reduce (fun acc s -> s + "\n" + acc) l

let stringOfCar car = 
   let path = List.map string car.path
   sprintf "%d\n%s" (List.length car.path) (stringOfList path)


//-------------------------------------------------------------------------------------------------
// EXPORTATION

let export path cars =
   //File.WriteAllText(path, text)
   //File.WriteAllLines(path, lines)
   let carNumber = Array.length cars
   let mutable result = string carNumber 
   for car in cars do 
      result <- result + "\n" + (stringOfCar car)
   File.WriteAllText(path, result)
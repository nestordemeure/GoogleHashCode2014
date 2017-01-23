# Google Hash Code 2014

## Summary :

I tried the problem in conditions (4h to produce the best score possible).

My solutions and their scores were :

- simple greedy algorithm : **13037**

- each car, one after the other, uses dijstra's algorithm to find the best path in the given time (exploring the path with the smalest time) : **246485**

- each car search for the best path in 1/10th of the time and does it 10 times in a row to get a path that is the right length : **1714622**

- idem, 320 times in a row : **1741722**

- idem, dijskra algorithm that accept to use previously explored junctions (but will not add new ones to the exploration list) : **1827709**

- idem, disturbing dijskra algorithm by adding a random number between 0 and (timeMax/400)/80 to the priority : **1842851**

The last score is the best one, it is equivalent to a 9th rank in the competition (biaised because I add access to the subject before starting).

---

## Final algorithm :

Each car has timeMax to explore a maximum of new streets.

I divide timeMax in 400 steps. During each step :

Each car, one after the other, uses a variation of Dijstra's algorithm (perturbed by a random number < step/80) to find the best path doable during the step

---

## Notes :

The greedy algorithm was very weak.

Using the coordinates of the junctions might have been useful to avoid cars going in the same direction.

With a random generator and enough time, one can pump-up his score (plus, randomnes seems to genuinely help the algorithm)

It is possible to design an algorithm that consider all the cars at the same time, it might be the winning combinaison (given enough constraints)

---

## Solutions :

During the competition, the winning team used C++ and an "iterative optimisation algorithm" that's all I know.

Another [solution](https://github.com/jilljenn/hashcode2014/blob/master/haskell-le-langage-de-l-eternel/explication.md), in Haskell. The main difference with my approach is that they start by sending the cars to random junctions on the map. It would probably have improved my performances.

I found a post describing the optimal [solution](https://a3nm.net/blog/google_hashcode_2014.html) implemented after the competition. Their answer transforms the graph to make sure that an Eulerian path exists and then find a way to split it into 8, they later found out that the problem was a variant of the [k-mixed postman problem](https://en.wikipedia.org/wiki/Route_inspection_problem).

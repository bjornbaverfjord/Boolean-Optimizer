# Boolean-Optimizer
This program is a superoptimizer for bitwise functions with up to 4 parameters that works by creating a dictionary of the shortest function for each possible truth-table. The code to be optimized has its truth-table calculated and the optimal solution is found in the dictionary by using the truth-table as the key.

Only 65536 such functions exists so they can be precomputed and cached, given enough time and an efficient searcher.


This F# function was not optimised by the F# 3.0 compiler:

<code>let booltest a b  = (a ||| b) &&& (~~~ (a &&& b))</code>

It generated this CIL code:

<code>[Ldarg 0; Ldarg 1; Or; Ldarg 0; Ldarg 1; And; Not; And]</code>

Instead of the optimal code found by the optimizer:

<code>[Ldarg 0; Ldarg 1; Xor]</code>

---
It is 2018 and we have .net core 2.0, things have not improved...

  mov         edx,ebx  
  and         edx,esi  
  not         edx  
  mov         ecx,ebx  
  or          ecx,esi  
  and         edx,ecx

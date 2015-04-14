# Boolean-Optimizer
Superoptimizer for bitwise functions

This program is an optimizer that can generate optimal code
for any bitwise function with up to 4 parameters.

Only 65536 such functions exists so they can be precomputed
and cached, given enough time and an efficient searcher.


This F# function was not optimised by the F# 3.0 compiler:
<code>let booltest a b  = (a ||| b) &&& (~~~ (a &&& b))
rrrr</code>

It generated this CIL code:
<code>[Ldarg 0; Ldarg 1; Or; Ldarg 0; Ldarg 1; And; Not; And]</code>

Instead of the optimal code:
<code>[Ldarg 0; Ldarg 1; Xor]</code>

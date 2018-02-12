# hashcode-pizza

This program generates solutions for Google's Hashcode example pizza problem.
It's written in C# and accepts as command line arguments an input file with the
problem data to be solved. The second input argument is an output path including
a root name to which a score value is appended to create a unique name.
Example outputs can be found in in the "Example outputs" folder.
You'll need to download the example input files from Google to use as input for
this program. An example call of this program could look like this:

"Pizza Slicing.exe" C:\temp\Pizzas\example.in C:\temp\Pizzas\exampleSolutionScore

One of the generated output files is automatically named "exampleSolutionScore15"
(amongst others) and contains these lines:

3
0 3 2 4
0 2 2 2 
0 0 2 1

Sorry if the program isn't as pretty as it could be: I only gave myself 4 hours for the soulution.

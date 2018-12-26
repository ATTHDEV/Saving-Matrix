/*
SavingMatrix for c#
version 1.0
author atthawut phuangsiri
 
THIS LIBRARY WRITTED IN .NET FRAMEWORK 4.6.1
CREATE FOR SOLVE TRAVEL SALSEMAN PROBLEM 
FOR COST REDUCTION IN ROGISTICS 
 
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
API Referrence

Class

1 class Matrix 
  use for solve TSP and calculate path

2 class Matrix.Node 
  use for saving data for each target

Function 

1 Matrix(List<Node> nodes)
  use constructor for initial all data to saving matrix

2 List<List<int>> getDistanceMatrix()
  use for get distance matrix

3 List<List<int>> getSavingMatrix()
  use for get saving matrix

4 void calculate_path(int max_weight)
  use for find path for every way 
  param max_weight for set maximum weight to be transported in 1 round.

5 void route_every_path()
  use for route path every way

6 List<Node> to_nodes(List<int> order)
  use for convert order to node

7 List<List<int>> getPath()
  use for get every path

8 void print_path()
  use for print all path to console

9 void print_distance_matrix()
  use for print distance matrix to console

10 void print_saving_matrix()
   use for print saving matrix to console

11 static int CalculateDistance(List<Node> nodes)
   use for calculate distance from nodes

12 static void PrintPath(List<Node> nodes)
   use for print single path to console

13 static List<Node> Route(List<Node> nodes)
   use for route path single way (solve TSP)
 
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
 
Example
    //use namespace SavingMatrix;
    
    //create node.
    var nodes = new List<Matrix.Node>()
    {
        new Matrix.Node(0,0,0),
        new Matrix.Node(0,12,48),
        new Matrix.Node(6,5,36),
        new Matrix.Node(7,15,43),
        new Matrix.Node(9,12,92),
        new Matrix.Node(15,3,57),
        new Matrix.Node(20,0,16),
        new Matrix.Node(17,-2,56),
        new Matrix.Node(7,-4,30),
        new Matrix.Node(1,-6,57),
        new Matrix.Node(15,-6,47),
        new Matrix.Node(20,-7,91),
        new Matrix.Node(7,-9,55),
        new Matrix.Node(2,-15,38),
    };

    Matrix matrix = new Matrix(nodes); // add nodes to saving matrix.
    matrix.calculate_path(200); // calculate path for 200 weight.
    matrix.route_every_path();  // solve tsp for every path.
    matrix.print_path(); // show every path to console.

    //and also you can only solve tsp as shown in the example below.

    //create nodes.
    var nodes2 = new List<Matrix.Node>()
    {
        new Matrix.Node(20,-7),
        new Matrix.Node(20,0),
        new Matrix.Node(17,-2),
        new Matrix.Node(7,-4),
    };

    //use static method for route path(solve TSP).
    var best_path = Matrix.Route(nodes2); 

    //use static method for calculate distance.
    var best_distance = Matrix.CalculateDistance(best_path); 

    //use static method for show path to console.
    Matrix.PrintPath(best_path);

    //show distance to console
    Console.WriteLine("distance = " + best_distance);
 
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
 
good luck.
atthawut phuangsiri.
 
*/
using System;
using System.Collections.Generic;
using System.Linq;

namespace SavingMatrix
{
    class Matrix
    {
        public class Node
        {
            public int x, y, weight;

            public Node(int x, int y, int weigth)
            {
                this.x = x;
                this.y = y;
                this.weight = weigth;
            }

            public Node(int x, int y)
            {
                this.x = x;
                this.y = y;
                weight = -1;
            }

            public override string ToString()
            {
                if (weight == -1)
                    return String.Format("(x,y) = ({0},{1})", x, y);
                return String.Format("(x,y,w) = ({0},{1},{2})", x, y, weight);
            }
        }
        private List<Node> nodes;

        private class ThroughInt
        {
            public int value;
            public bool passed;
            public ThroughInt(int value)
            {
                this.value = value;
                passed = false;
            }
        }

        private class MatrixNode
        {
            public int i, j, value;

            public MatrixNode(int i, int j, int value)
            {
                this.i = i;
                this.j = j;
                this.value = value;
            }

            public override string ToString()
            {
                return String.Format("( i , j ) = ({0} , {1}) => value = {2} ", i, j, value);
            }
        };

        private class Path
        {
            public List<int> path_list = new List<int>();
            public int sum = 0;

            public void add(List<Node> nodes, int index)
            {
                foreach (var n in path_list)
                {
                    if (n == index)
                    {
                        return;
                    }
                }
                path_list.Add(index);
                sum += nodes[index].weight;
            }

            public int get_distance(List<Node> nodes)
            {
                double s = dist(nodes[0], nodes[path_list[0]]);
                for (int i = 1; i < path_list.Count; i++)
                {
                    s += dist(nodes[path_list[i - 1]], nodes[path_list[i]]);
                }
                s += dist(nodes[path_list[path_list.Count - 1]], nodes[0]);
                return (int)s;
            }
        }

        private List<List<int>> distance_list = new List<List<int>>(),
                                saving_list = new List<List<int>>();

        private List<Path> final_path = new List<Path>();

        private List<List<ThroughInt>> saving_through_list = new List<List<ThroughInt>>();

        public Matrix(List<Node> nodes)
        {
            this.nodes = nodes;
            calculate_distance_matrix();
            calculate_saving_matrix();
        }

        private static int dist(Node a, Node b)
        {
            int x = a.x - b.x;
            int y = a.y - b.y;
            return (int)Math.Round(Math.Sqrt(x * x + y * y));
        }

        private static void swap<T>(List<T> list, int indexA, int indexB)
        {
            var tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

        private void calculate_distance_matrix()
        {
            for (int i = 1; i < nodes.Count; i++)
            {
                var rows = new List<int>();
                for (int j = 0; j < i; j++)
                {
                    rows.Add(dist(nodes[i], nodes[j]));
                }
                distance_list.Add(rows);
            }
        }

        public List<List<int>> getDistanceMatrix()
        {
            return distance_list;
        }

        private void calculate_saving_matrix()
        {
            for (int i = 2; i < nodes.Count; i++)
            {
                var rows = new List<int>();
                var rows_passed = new List<ThroughInt>();
                for (int j = 1; j < i; j++)
                {
                    int d1 = dist(nodes[0], nodes[i]);
                    int d2 = dist(nodes[0], nodes[j]);
                    int d3 = dist(nodes[i], nodes[j]);
                    int s = d1 + d2 - d3;
                    rows_passed.Add(new ThroughInt(s));
                    rows.Add(s);
                }
                saving_through_list.Add(rows_passed);
                saving_list.Add(rows);
            }
        }

        public List<List<int>> getSavingMatrix()
        {
            return saving_list;
        }

        private void add_unique(ref List<int> list, int value)
        {
            foreach (var p in list)
            {
                if (p == value)
                    return;
            }
            list.Add(value);
        }

        public void calculate_path(int max_weight)
        {
            foreach(var node in nodes)
            {
                if (node.weight > max_weight)
                {
                    Console.WriteLine("warning ! some node weight over than max weight");
                    return;
                }
            }
            var tmp_path = new List<int>();
            var passed_path = new List<MatrixNode>();
            do
            {
                int max = int.MinValue;
                int _i = 0, _j = 0;
                for (int i = 0; i < saving_through_list.Count; i++)
                {
                    var rows = saving_through_list[i];
                    for (int j = 0; j < rows.Count; j++)
                    {
                        var s = rows[j].value;
                        var p = rows[j].passed;
                        if (s > max && !p)
                        {
                            max = s;
                            _i = i;
                            _j = j;
                        }
                    }
                }
                saving_through_list[_i][_j].passed = true;
                add_unique(ref tmp_path, _i + 2);
                add_unique(ref tmp_path, _j + 1);
                passed_path.Add(new MatrixNode(_j + 1, _i + 2, max));
            } while (tmp_path.Count < nodes.Count - 1);

            while (true)
            {
                var current_path = new Path();
                foreach (var p in passed_path)
                {
                    if (current_path.path_list.Count == 0)
                    {
                        if (current_path.sum + nodes[p.j].weight <= max_weight)
                            current_path.add(nodes, p.j);
                        if (current_path.sum + nodes[p.i].weight <= max_weight)
                            current_path.add(nodes, p.i);
                    }
                    else
                    {
                        for (int i = 0; i < current_path.path_list.Count; i++)
                        {
                            var c = current_path.path_list[i];
                            if (p.i == c && p.j != c && current_path.sum + nodes[p.j].weight <= max_weight)
                            {
                                current_path.add(nodes, p.j);
                            }
                            else if (p.i != c && p.j == c && current_path.sum + nodes[p.i].weight <= max_weight)
                            {
                                current_path.add(nodes, p.i);
                            }
                        }
                    }

                }
                for (int i = 0; i < passed_path.Count;)
                {
                    var p = passed_path[i];
                    bool updated = true;
                    foreach (var c in current_path.path_list)
                    {
                        if (passed_path.Count <= 0) break;
                        if (p.i == c || p.j == c)
                        {
                            passed_path.RemoveAt(i);
                            updated = false;
                            break;
                        }
                    }
                    if (updated)
                    {
                        i++;
                    }
                }
                final_path.Add(current_path);
                
                if (passed_path.Count == 0) break;
            }
          
            List<int> other_node = new List<int>();
            for (int i = 1; i <= nodes.Count - 1; i++)
            {
                other_node.Add(i);
            }
            foreach (var f in final_path)
            {
                foreach (var p in f.path_list)
                {
                    for (int i = 0; i < other_node.Count;)
                    {
                        if (p == other_node[i])
                        {
                            other_node.RemoveAt(i);
                        }
                        else
                        {
                            i++;
                        }
                    }
                }
            }

            Path other_path = new Path();
            foreach (var o in other_node)
            {
                bool appended = false;
                foreach (var f in final_path)
                {
                    if (f.sum + nodes[o].weight <= max_weight)
                    {
                        f.add(nodes, o);
                        appended = true;
                        break;
                    }
                }
                if (!appended)
                {
                    other_path.add(nodes, o);
                }
            }
            if (other_path.path_list.Count > 0)
            {
                final_path.Add(other_path);
            }
        }

        public void route_every_path()
        {
            foreach(var f in final_path)
                f.path_list = Route(f.path_list);
        }

        public void print_path()
        {
            if (final_path.Count == 0)
            {
                Console.WriteLine("you have not route path yet.");
                return;
            }
            foreach (var f in final_path)
            {
                int w = 0;
                foreach (var p in f.path_list)
                {
                    Console.WriteLine(String.Format("node {0} => {1} ", p, nodes[p]));
                    w += nodes[p].weight;
                }
                Console.WriteLine("-----------------------------");
                Console.WriteLine(String.Format("distance = {0}", f.get_distance(nodes)));
                Console.WriteLine(String.Format("weight = {0}", w));
                Console.WriteLine("=============================");
            }
            Console.WriteLine();
        }

        public List<Node> to_nodes(List<int> order)
        {
            List<Node> NODE = new List<Node>();
            foreach (var o in order)
                NODE.Add(nodes[o]);
            return NODE;
        }

        public List<List<int>> getPath()
        {
            List<List<int>> all_path = new List<List<int>>();
            foreach (var f in final_path)
                all_path.Add(f.path_list);
            return all_path;
        }

        public void print_distance_matrix()
        {
            foreach (var rows in getDistanceMatrix())
            {
                foreach (var node in rows)
                {
                    Console.Write(node + "||");
                }
                Console.Write("\n");
            }
            Console.WriteLine();
        }

        public void print_saving_matrix()
        {
            foreach (var rows in getSavingMatrix())
            {
                foreach (var node in rows)
                {
                    Console.Write(node + "||");
                }
                Console.Write("\n");
            }
            Console.WriteLine();
        }

        public static int CalculateDistance(List<Node> nodes)
        {
            var dc = new Node(0, 0, 0);
            int sum = dist(dc, nodes[0]);
            for (int i = 1; i < nodes.Count; i++)
            {
                sum += dist(nodes[i - 1], nodes[i]);
            }
            sum += dist(nodes[nodes.Count - 1], dc);
            return sum;
        }

        public static void PrintPath(List<Node> nodes)
        {
            foreach (var n in nodes)
            {
                Console.WriteLine(n);
            }
        }

        private List<int> Route(List<int> current_order)
        {
          
            List<int> order = new List<int>();

            List<Node> NODE = new List<Node>();

            for (int i = 0; i < current_order.Count; i++)
            {
                int o = current_order[i];
                order.Add(i);
                NODE.Add(nodes[o]);
            }
            var best_order = order.ToList();
            var best_distance = CalculateDistance(NODE);
            while (true)
            {
                var largestI = -1;
                for (var i = 0; i < order.Count - 1; i++)
                {
                    if (order[i] < order[i + 1])
                    {
                        largestI = i;
                    }
                }

                if (largestI == -1)
                {
                    break;
                }

                var largestJ = -1;
                for (var j = 0; j < order.Count; j++)
                {
                    if (order[largestI] < order[j])
                    {
                        largestJ = j;
                    }
                }

                swap(order, largestI, largestJ);

                var endArray = order.GetRange(largestI + 1, order.Count - largestI - 1);
                order.RemoveRange(largestI + 1, order.Count - largestI - 1);
                endArray.Reverse();
                order = order.Concat(endArray).ToList();

                var node = new List<Node>();
                foreach (var n in order)
                {
                    node.Add(NODE[n]);
                }

                var d = CalculateDistance(node);
                if (d < best_distance)
                {
                    best_distance = d;
                    best_order = order.ToList();
                }

            }

            for (int i = 0; i < best_order.Count;i++)
            {
                var b = best_order[i];
                best_order[i] = current_order[b];
            }
            return best_order;
        }

        public static List<Node> Route(List<Node> nodes)
        {
            var best_path = nodes.ToList();
            var best_distance = CalculateDistance(nodes);
     
            List<int> order = new List<int>();

            for (int i = 0; i < nodes.Count; i++)
            {
                order.Add(i);
            }

            while (true)
            {
                var largestI = -1;
                for (var i = 0; i < order.Count - 1; i++)
                {
                    if (order[i] < order[i + 1])
                    {
                        largestI = i;

                    }
                }

                if (largestI == -1)
                {
                    break;
                }

                var largestJ = -1;
                for (var j = 0; j < order.Count; j++)
                {
                    if (order[largestI] < order[j])
                    {
                        largestJ = j;
                    }
                }

                swap(order, largestI, largestJ);

                var endArray = order.GetRange(largestI + 1, order.Count - largestI - 1);
                order.RemoveRange(largestI + 1, order.Count - largestI - 1);
                endArray.Reverse();
                order = order.Concat(endArray).ToList();

                var node = new List<Node>();
                foreach (var n in order)
                {
                    node.Add(nodes[n]);
                }

                var d = CalculateDistance(node);
                if (d < best_distance)
                {
                    best_distance = d;
                    best_path = node.ToList();
                }

            }
            return best_path;
        }
    }
}

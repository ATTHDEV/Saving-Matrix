
using SavingMatrix;
using System;
using System.Collections.Generic;

namespace test_sort_circle
{
    class Program
    {

        static void Main(string[] args)
        {

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

            Matrix matrix = new Matrix(nodes);
            matrix.calculate_path(200);
            matrix.route_every_path();
            matrix.print_path();

            var nodes2 = new List<Matrix.Node>()
            {
                new Matrix.Node(20,-7),
                new Matrix.Node(20,0),
                new Matrix.Node(17,-2),
                new Matrix.Node(7,-4),
            };

            var best_path = Matrix.Route(nodes2);
            var best_distance = Matrix.CalculateDistance(best_path);

            Console.WriteLine("Single Path...");
            Matrix.PrintPath(best_path);
            Console.WriteLine("distance = " + best_distance);

            Console.ReadKey();
        }

        public static void swap<T>(List<T> point, int i, int j)
        {
            var temp = point[i];
            point[i] = point[j];
            point[j] = temp;
        }
    }
}


using System;
using System.Collections.Generic;

using Grasshopper.Kernel; // Libraries of GH 
using Rhino.Geometry; // Libraries of Rhino 

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_Evolve
{
    class Program
    {
        // open on Grasshopper
        /*
        static List<Circle> RedCircles = new List<Circle>();
        static List<Circle> BlueCircles = new List<Circle>();
        static List<Circle> GreenCircles = new List<Circle>();
        static List<Circle> WhiteCircles = new List<Circle>();   
        */

        // Comment Console.Write and Concole.WriteLine in Grasshopper

        //readonly static List<int> redInitialPoint = new List<int>();

        // these points are gonna be used for limiting the evaluation if it  starts to create some unuseful options
        static int[] redInitialPoints = new int[2] { 0, 0 }; //We use static because we use the variabels in a Class. Not with an object.
        static int[] greenInitialPoints = new int[2] { 0, 0 };
        static int[] blueInitialPoints = new int[2] { 0, 0 };
        static int[] yellowInitialPoints = new int[2] { 0, 0 };

        static void ShowMatrix(int[,] Arr) // A method to visualize the matrix in Console.App .All the Console commands needs to be transform sth else in GS.Print might work but it didnt.
        {
            Console.WriteLine("");
            for (int i = 0; i < Arr.GetLength(0); i++)
            {
                for (int j = 0; j < Arr.GetLength(1); j++)
                {
                    Console.Write(Arr[i, j].ToString() + " ");
                }
                Console.WriteLine("");
            }
            Console.WriteLine("   ");
            return;
        }
        static int[,] InitializeZeroMatrice(int width, int height) // initialize the matrix, using given arguments.
        {
            var areaArray = new int[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    areaArray[i, j] = 0;
                }
            }
            return areaArray;
        }
        /* The Color lists keep the colors in a seperate List in order to choose within them easily instead of searching where they are... */
        static List<int> redList = new List<int>();
        static List<int> greenList = new List<int>();
        static List<int> blueList = new List<int>();
        /**           These sizes are Spreading Limits for the spaces                     **/
        static int blueSize = 100;
        static int redSize = 100;
        static int greenSize = 100;
        /**           these counters are used for starting the límitations counts.         **/
        static int blueCounter = 1;
        static int redCounter = 1;
        static int greenCounter = 1;

        /**           Randomly Starts the color. Takes the Color lists as argumnts 
         * (We can delete these argumnents and it can get them as local referance
         * in our case it gives us a chance to choose what to initilize)  
         * 
         * **/

        static void StartColors(int[,] arr, List<int> RedList, List<int> greenList, List<int> blueList)
        {
            Random rand = new Random();
            //----------------------
            /* GetLength(0) and GetLength(1) might be set as variable */
            var a = rand.Next(0, arr.GetLength(0)); //Array.GetLength(0) gives the column lenght in other words height of an array
            var b = rand.Next(0, arr.GetLength(1)); //Array.GetLength(1) gives the row lenght in other words width of an array

            redInitialPoints[0] = a; redInitialPoints[1] = b;

            RedList.AddRange(new int[2] { a, b }); //AddRange ==> Adds multiple elements to a List . We use mod%2 to call the index of the point back instead of stacking them as an another List(or Array) in our list. 
            arr[a, b] = 1;// Red Starting Point
            //----------------------
            a = rand.Next(0, arr.GetLength(0));
            b = rand.Next(0, arr.GetLength(1));

            greenInitialPoints[0] = a; greenInitialPoints[1] = b;

            greenList.AddRange(new int[2] { a, b });
            arr[a, b] = 2; // Green Starting Point
            //----------------------
            a = rand.Next(0, arr.GetLength(0));
            b = rand.Next(0, arr.GetLength(1));

            blueInitialPoints[0] = a; blueInitialPoints[1] = b;

            blueList.AddRange(new int[2] { a, b });
            arr[a, b] = 3; // Blue Starting Point
            //----------------------
        }

        /* Calculates the distance between initial point in the color and the possible new point!
         *  (if the possible new point is too far from inital point eleminate it.) */
        static double distanceCalculate(int[] Initial, int x2, int y2)
        {
            var distance = Math.Sqrt((y2 - Initial[1]) * (y2 - Initial[1]) + (x2 * Initial[0]) * (x2 * Initial[0]));
            return distance;
        }

        static void Spread(int[,] arr, ref int spaceSize, List<int> spaceCoordList, ref int counter) //if we dont use ref Method will not update local variables.
        {

            /*ARGUMENTS 
             * (The main array which has rectangular form(will be 3d in short) ,
             * spaceSize = MAximum limit that can grow.  ,
             * spaceCoordList =   )
             */
            var rand = new Random();
            var directionList = new List<int>(); // keeps the possible move to choose them randomly.
            int Xcoord;
            int Ycoord;
            bool draw = false;
            int trycount = 0;


            while (draw == false && counter <= spaceSize)
            {
                if (counter <= spaceSize) // Probably Can be deleted. The condition is added to the While Loop Conditions.
                {
                    Console.WriteLine("Counter Number is : {0}\n", counter);
                    Console.WriteLine("Element number of the list {0}", spaceCoordList.Count / 2);
                    var randomCellIndex = rand.Next(spaceCoordList.Count / 2);

                    Xcoord = spaceCoordList[2 * randomCellIndex];
                    Ycoord = spaceCoordList[2 * randomCellIndex + 1];

                    Console.WriteLine("Coordinates if the new Cell is = {0}, {1}\n", Xcoord, Ycoord);

                    if (Xcoord < arr.GetLength(0) - 1 && Xcoord > 0 && Ycoord > 0 && Ycoord < arr.GetLength(1) - 1)
                    {
                        /* Check the conditions INSIDE borders */
                        if (arr[Xcoord + 1, Ycoord] == 0) // Check if possible new cell is still white or not!
                            directionList.AddRange(new int[2] { (Xcoord + 1), Ycoord }); //Store possible spread directions in a list.

                        if (arr[Xcoord - 1, Ycoord] == 0)
                            directionList.AddRange(new int[2] { (Xcoord - 1), Ycoord }); //Store possible spread directions in a list.

                        if (arr[Xcoord, Ycoord + 1] == 0)
                            directionList.AddRange(new int[2] { (Xcoord), Ycoord + 1 }); //Store possible spread directions in a list.

                        if (arr[Xcoord, Ycoord - 1] == 0)
                            directionList.AddRange(new int[2] { (Xcoord), (Ycoord - 1) }); //Store possible spread directions in a list.

                        if (directionList.Count > 0)
                        {
                            var randomIndex = rand.Next(directionList.Count / 2); // Since we stackted the positions consecutively, we need to get indices by  using Mod&2 (x+y doubles the lenght)

                            int a = directionList[2 * randomIndex];     // Index of Xn 
                            int b = directionList[2 * randomIndex + 1]; // Index of Yn

                            Console.WriteLine("Value of Xcoord and Ycoord: {0}, {1}", Xcoord, Ycoord);
                            Console.WriteLine("Value of new Coordinates: {0}, {1}", a, b);
                            Console.WriteLine("Value of initial value of Array[a, b]: {0}", arr[a, b]);

                            arr[a, b] = arr[Xcoord, Ycoord]; // Assign the new cell to the Array!
                            Console.WriteLine("Value of new Array[a, b]: {0}", arr[a, b]);

                            spaceCoordList.AddRange(new int[2] { a, b });

                            directionList.Clear();
                            directionList.TrimExcess();
                            draw = true;
                            counter++;
                        }

                        else // Important if all of the surroundins are full keep trying but STOP when you tried enough.// Keep the bugs away :)
                        {
                            trycount++;
                            if (trycount >= spaceCoordList.Count)
                            {
                                draw = true;
                                Console.WriteLine("trycountNumber is ={0}", trycount);
                                break;
                            }
                        }

                    }

                    /* EDGE CONDITIONS */
                    // Possile solution might be Exception/Try Catch... Instead of declaring each condition. May be tried. 
                    //BOTTOM
                    else if (Xcoord == (arr.GetLength(0) - 1) && Ycoord != 0 && Ycoord != (arr.GetLength(1) - 1))
                    {
                        if (arr[Xcoord - 1, Ycoord] == 0)
                        {
                            // Here we dont need Direction list anymore because we have only one direction
                            arr[Xcoord - 1, Ycoord] = arr[Xcoord, Ycoord];

                            int a = Xcoord - 1;
                            int b = Ycoord;

                            spaceCoordList.AddRange(new int[2] { a, b });

                            draw = true;
                            counter++;
                        }
                    }
                    //TOP
                    else if (Xcoord == 0 && Ycoord != 0 && Ycoord != (arr.GetLength(1) - 1))
                    {
                        if (arr[Xcoord + 1, Ycoord] == 0)
                        {
                            arr[Xcoord + 1, Ycoord] = arr[Xcoord, Ycoord];

                            int a = Xcoord + 1;
                            int b = Ycoord;

                            spaceCoordList.AddRange(new int[2] { a, b });

                            draw = true;
                            counter++;
                        }
                    }
                    //RIGHT
                    else if ((Ycoord == arr.GetLength(1) - 1) && Xcoord != 0 && Xcoord != (arr.GetLength(0) - 1))
                    {
                        if (arr[Xcoord, Ycoord - 1] == 0)
                        {
                            arr[Xcoord, Ycoord - 1] = arr[Xcoord, Ycoord];

                            int a = Xcoord;
                            int b = Ycoord - 1;

                            spaceCoordList.AddRange(new int[2] { a, b });

                            draw = true;
                            counter++;
                        }
                    }
                    //LEFT
                    else if (Ycoord == 0 && Xcoord != 0 && Xcoord != (arr.GetLength(0) - 1))
                    {
                        if (arr[Xcoord, Ycoord + 1] == 0)
                        {
                            arr[Xcoord, Ycoord + 1] = arr[Xcoord, Ycoord];

                            int a = Xcoord;
                            int b = Ycoord + 1;

                            arr[Xcoord, Ycoord + 1] = arr[Xcoord, Ycoord];

                            spaceCoordList.AddRange(new int[2] { a, b });

                            draw = true;
                            counter++;
                        }
                    }

                    //LEFT TOP
                    else if (Xcoord == 0 && Ycoord == 0)
                    {
                        if (arr[Xcoord, Ycoord + 1] == 0)
                            directionList.Add(Xcoord); directionList.Add(Ycoord + 1);

                        if (arr[Xcoord + 1, Ycoord] == 0)
                            directionList.Add(Xcoord + 1); directionList.Add(Ycoord);

                        if (directionList.Count > 0)
                        {
                            var randomIndex = rand.Next(directionList.Count / 2);

                            int a = directionList[2 * randomIndex];
                            int b = directionList[2 * randomIndex + 1];

                            arr[a, b] = arr[Xcoord, Ycoord];

                            spaceCoordList.AddRange(new int[2] { a, b });

                            directionList.Clear();
                            directionList.TrimExcess();
                            draw = true;

                            counter++;
                        }
                    }
                    //RIGHT TOP
                    else if (Xcoord == 0 && Ycoord == (arr.GetLength(1) - 1))
                    {
                        if (arr[Xcoord + 1, Ycoord] == 0)
                            directionList.Add(Xcoord + 1); directionList.Add(Ycoord);

                        if (arr[Xcoord, Ycoord - 1] == 0)
                            directionList.Add(Xcoord); directionList.Add(Ycoord - 1);

                        if (directionList.Count > 0)
                        {
                            var randomIndex = rand.Next(directionList.Count / 2);

                            int a = directionList[2 * randomIndex];
                            int b = directionList[2 * randomIndex + 1];

                            arr[a, b] = arr[Xcoord, Ycoord];

                            spaceCoordList.AddRange(new int[2] { a, b });

                            directionList.Clear();
                            directionList.TrimExcess();

                            draw = true;
                            counter++;
                        }
                    }
                    //RIGHT BOTTOM
                    else if (Xcoord == (arr.GetLength(0) - 1) && Ycoord == (arr.GetLength(1) - 1))
                    {
                        if (arr[Xcoord, Ycoord - 1] == 0)
                            directionList.Add(Xcoord); directionList.Add(Ycoord - 1);

                        if (arr[Xcoord - 1, Ycoord] == 0)
                            directionList.Add(Xcoord - 1); directionList.Add(Ycoord);

                        if (directionList.Count > 0)
                        {
                            var randomIndex = rand.Next(directionList.Count / 2);

                            int a = directionList[2 * randomIndex];
                            int b = directionList[2 * randomIndex + 1];

                            arr[a, b] = arr[Xcoord, Ycoord];

                            spaceCoordList.AddRange(new int[2] { a, b });

                            directionList.Clear();
                            directionList.TrimExcess();

                            draw = true;
                            counter++;
                        }
                    }
                    //LEFT BOTTOM
                    else if (Xcoord == (arr.GetLength(1) - 1) && Ycoord == 0)
                    {
                        if (arr[Xcoord, Ycoord + 1] == 0)
                            directionList.Add(Xcoord); directionList.Add(Ycoord - 1);

                        if (arr[Xcoord - 1, Ycoord] == 0)
                            directionList.Add(Xcoord - 1 - 1); directionList.Add(Ycoord);

                        if (directionList.Count > 0)
                        {
                            var randomIndex = rand.Next(directionList.Count / 2);

                            int a = directionList[2 * randomIndex];
                            int b = directionList[2 * randomIndex + 1];

                            arr[a, b] = arr[Xcoord, Ycoord];

                            spaceCoordList.AddRange(new int[2] { a, b });

                            directionList.Clear();
                            directionList.TrimExcess();

                            draw = true;
                            counter++;
                        }
                    }
                    /* EDGE CONDITIONS */
                }
            }

        }

        /*
        static void drawCircles(int[,] arr, double radius=5)
        {
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    if (arr[i, j] == 1)
                    {
                        Point3d center = new Point3d(j *radius*2, i * radius * 2, 0);
                        Circle c = new Circle(center, radius);
                        RedCircles.Add(c);
                    }
                    if (arr[i, j] == 2)
                    {
                        Point3d center = new Point3d(j * radius * 2, i * radius * 2, 0);
                        Circle c = new Circle(center, radius);
                        GreenCircles.Add(c);
                    }
                    if (arr[i, j] == 3)
                    {
                        Point3d center = new Point3d(j * radius * 2, i * radius * 2, 0);
                        Circle c = new Circle(center, radius);
                        BlueCircles.Add(c);
                    }
                    if (arr[i, j] == 0)
                    {
                        Point3d center = new Point3d(j * radius * 2, i * radius * 2, 0);
                        Circle c = new Circle(center, radius);
                        WhiteCircles.Add(c);
                    }
                }
            }

        }*/
        static void Main()
        {
            int width = 50; // Width of the Array
            int height = 25; // Height of  the Array

            int[,] areaArray = InitializeZeroMatrice(height, width);

            StartColors(areaArray, redList, greenList, blueList);

            for (int i = 0; i < 300; i++) // Calls the spreading functions in order and repettively as much as we want.
            {
                Spread(areaArray, ref redSize, redList, ref redCounter);
                Spread(areaArray, ref greenSize, greenList, ref greenCounter);
                Spread(areaArray, ref blueSize, blueList, ref blueCounter);

            }
            ShowMatrix(areaArray);



            //drawCircles(areaArray, 5.0); // open on grasshopper

            /* // open in Grasshopper
            A = areaArray;
            R = RedCircles;
            G = GreenCircles;
            B = BlueCircles;
            W = WhiteCircles;
            */
        }
    }
}

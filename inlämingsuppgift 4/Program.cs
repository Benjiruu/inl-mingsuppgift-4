using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Vaccination
{
    public class Program
    {

        private static int availableDoses = 0;
        private static bool vaccinateChildren = false;
        private static string inputFilePath = "C:\\Windows\\Temp\\People.csv";
        private static string outputFilePath = "C:\\Windows\\Temp\\Vaccinations.csv";

        public static void Main()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
           
            Console.WriteLine("Welcome to the Vaccination Program!");

            while (true)
            {
                Console.WriteLine("Availiable Doses " +  availableDoses);
                Console.WriteLine("Vacciantion under 18 " + vaccinateChildren);
                Console.WriteLine("Input File Path: " + inputFilePath);
                Console.WriteLine("Output File Path: " + outputFilePath);
                int choice = ShowMenu("Main Menu", new List<string>
                {
                    "Create Vaccination Priority Order",
                    "Change Available Doses",
                    "Change Vaccination Age Limit",
                    "Change Input File",
                    "Change Output File",
                    "Exit"
                });

                switch (choice)
                {
                    case 0:
                        // Create Vaccination Order
                        break;
                    case 1:
                        ChangeAvailableDoses();
                        break;
                    case 2:
                        ChangeVaccinationAgeLimit();
                        break;
                    case 3:
                        // Change Input File
                        break;
                    case 4:
                        // Change Output File
                        break;
                    case 5:
                        // Exit
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        public static int ShowMenu(string prompt, IEnumerable<string> options)
        {
            if (options == null || options.Count() == 0)
            {
                throw new ArgumentException("Cannot show a menu for an empty list of options.");
            }

            Console.WriteLine(prompt);

            // Hide the cursor that will blink after calling ReadKey.
            Console.CursorVisible = false;

            int selected = 0;
            int top = Console.CursorTop;
            for (int i = 0; i < options.Count(); i++)
            {
                if (i == selected)
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                Console.WriteLine($"- {options.ElementAt(i)}");
                Console.ResetColor();
            }

            Console.CursorTop = top;

            ConsoleKey? key = null;
            while (key != ConsoleKey.Enter)
            {
                key = Console.ReadKey(intercept: true).Key;

                if (key == ConsoleKey.DownArrow)
                {
                    selected = Math.Min(selected + 1, options.Count() - 1);
                }
                else if (key == ConsoleKey.UpArrow)
                {
                    selected = Math.Max(selected - 1, 0);
                }

                Console.CursorTop = top;

                for (int i = 0; i < options.Count(); i++)
                {
                    if (i == selected)
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    Console.WriteLine($"- {options.ElementAt(i)}");
                    Console.ResetColor();
                }

                Console.CursorTop = top;
            }

            Console.CursorVisible = true;
            return selected;
        }

        public static string[] CreateVaccinationOrder(string[] input, int doses, bool vaccinateChildren)
        {
            // Implement the logic to create the vaccination order based on the provided input data.
            // You will prioritize individuals, allocate doses, and generate output data here.
            // Return an array of strings representing the vaccination order.
            return new string[0]; // Replace with your logic.
        }

        public static void ChangeAvailableDoses()

        {
            Console.Clear();
            Console.WriteLine("Change the number of available vaccine doses.");
            string UserVaccineDoses = Console.ReadLine();

            if (int.TryParse(UserVaccineDoses, out int NewDoses))
            {
                if (NewDoses >=0)
                {
                    availableDoses = NewDoses;

                    Console.WriteLine("Available vaccine doses updated to " + availableDoses);
                   

                }

                else
                {
                    Console.WriteLine("Invalid input. Please enter a non-negative amount");
                }

            }

            Console.WriteLine("Press enter to return to the main menu");
            Console.ReadLine();
            Console.Clear();
        }

        public static void ChangeVaccinationAgeLimit()
        {
            Console.Clear();

            List<string> AgeLimitOptions = new List<string>()
            {
                "Yes",
                "No"

            };

            int choice = ShowMenu("Change Age Limit To Under 18", AgeLimitOptions);

            if (choice == 0)
            {
                vaccinateChildren = true;
                Console.WriteLine("Vaccinating people under 18 is now enabled.");
            }
            else if (choice == 1)
            {       
                vaccinateChildren= false;
                Console.WriteLine("Vaccinating people under 18 is now disabled");
            }

            Console.WriteLine("Press Enter to return to the main menu");
            Console.ReadLine();
            Console.Clear();
        }

        public static void ChangeInputFile()
        {
            // Implement the logic to change the input file path.
        }

        public static void ChangeOutputFile()
        {
            // Implement the logic to change the output file path.
        }
    }

    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public void ExampleTest()
        {
            // Implement test cases to verify the correctness of your program.
        }
    }
}

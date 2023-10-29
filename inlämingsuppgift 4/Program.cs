using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;

namespace Vaccination
{

    public class Person
    {
        public string PersonNumber { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public bool HealthNSocialCare { get; }
        public bool RiskGroup { get; }
        public bool InfectedAmount { get; }

        public Person(string personNumber, string firstname, string lastname, bool healthnsocialcare, bool riskgroup, bool infectedamount)
        {
            PersonNumber = personNumber;
            FirstName = firstname;
            LastName = lastname;
            HealthNSocialCare = healthnsocialcare;
            RiskGroup = riskgroup;
            InfectedAmount = infectedamount;
        }

        public DateTime GetBirthdate()
        {
            if (PersonNumber.Length >= 8)
            {
                // Try to parse the first 8 characters of PersonNumber as a date.
                if (DateTime.TryParseExact(PersonNumber.Substring(0, 8), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthdate))
                {
                    // Check if the date is valid.
                    if (IsValidDate(birthdate.Year, birthdate.Month, birthdate.Day))
                    {
                        return birthdate;
                    }
                }
            }

            // Handle errors or return DateTime.MinValue for an invalid PersonNumber format.
            return DateTime.MinValue;
        }

        private bool IsValidDate(int year, int month, int day)
        {
            try
            {
                new DateTime(year, month, day);
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                // The date is not valid.
                return false;
            }

        }

        public bool IsAdult()
        {
            if (int.TryParse(PersonNumber, out int year))
            {
                int currentYear = DateTime.Now.Year;
                return currentYear - year >= 18;
            }

            return false;
        }

        public int GetPriority(bool vaccinateChildren)
        {
            int priority = 0;

            if (vaccinateChildren || (!vaccinateChildren && IsAdult()))
            {
                if (HealthNSocialCare)
                {
                    priority = 1;
                }
                else if (RiskGroup)
                {
                    priority = 2;
                }
                else
                {
                    priority = 3;
                }
            }
            return priority;
        }
        public int GetDosesToAllocate(int availableDoses)
        {
            int dosesToAllocate = 0;

            if (availableDoses >= 2)
            {
                dosesToAllocate = 2;
            }
            else if (availableDoses == 1)
            {
                dosesToAllocate = 1;
            }

            return dosesToAllocate;
        }
    }


    public class Program
    {
        private static List<Person> people = new List<Person>();
        private static int availableDoses = 0;
        private static bool vaccinateChildren = false;
        private static string inputFilePath = "C:\\Programmering\\People.csv";
        private static string outputFilePath = "C:\\Programmering\\Vaccinations.csv";

        public static void Main()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            Console.WriteLine("Welcome to the Vaccination Program!");

            while (true)
            {
                Console.WriteLine("Availiable Doses " + availableDoses);
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
                        CreateVaccinationOrder();
                        break;
                    case 1:
                        ChangeAvailableDoses();
                        break;
                    case 2:
                        ChangeVaccinationAgeLimit();
                        break;
                    case 3:
                        ChangeInputFile();
                        break;
                    case 4:
                        ChangeOutputFile();
                        break;
                    case 5:
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

        public static void CreateVaccinationOrder()
        {
            Console.Clear();
            if (!File.Exists(inputFilePath))
            {
                Console.WriteLine("Input file not found. Please set the correct input file path.");
                return;
            }

            string[] input = File.ReadAllLines(inputFilePath);
            List<string> vaccinationOrder = new List<string>();

            foreach (string line in input)
            {
                string[] parts = line.Split(',');
                if (parts.Length == 6)
                {
                    string personNumber = parts[0];
                    string firstname = parts[1];
                    string lastname = parts[2];
                    bool healthnsocialcare = (parts[3] == "1");
                    bool riskgroup = (parts[4] == "1");
                    bool infectedamount = (parts[5] == "1");

                    try
                    {
                        Person newPerson = new Person(personNumber, firstname, lastname, healthnsocialcare, riskgroup, infectedamount);
                        DateTime birthdate = newPerson.GetBirthdate();
                        if (birthdate == DateTime.MinValue)
                        {
                            // Handle incorrect PersonNumber format by printing an error message.
                            Console.WriteLine($"Error: Invalid PersonNumber format in CSV file: {personNumber}");
                        }
                        else
                        {
                            // Add the person to the list only if their PersonNumber is valid.
                            people.Add(newPerson);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle other errors for invalid rows by printing an error message.
                        Console.WriteLine($"Error: Invalid row in CSV file: {line}");
                        Console.WriteLine($"Details: {ex.Message}");
                    }
                }
                else
                {
                    // Handle errors for lines with missing columns by printing an error message.
                    var missingColumns = new List<string>();
                    if (parts.Length < 1) missingColumns.Add("PersonNumber");
                    if (parts.Length < 2) missingColumns.Add("FirstName");
                    if (parts.Length < 3) missingColumns.Add("LastName");
                    if (parts.Length < 4) missingColumns.Add("HealthNSocialCare");
                    if (parts.Length < 5) missingColumns.Add("RiskGroup");
                    if (parts.Length < 6) missingColumns.Add("InfectedAmount");

                    Console.WriteLine($"Error: Invalid line in CSV file - Missing columns: {string.Join(", ", missingColumns)}");
                }
            }

            if (people.Count == 0)
            {
                // No valid entries, return to the main menu without creating a vaccination order.
                Console.WriteLine("No valid entries found. Returning to the main menu.");
                return;
            }

            people = people
                .OrderBy(p => p.GetPriority(vaccinateChildren))
                .ThenBy(p => p.GetBirthdate())
                .ToList();

            foreach (Person person in people)
            {
                int dosesToAllocate = person.GetDosesToAllocate(availableDoses);
                availableDoses -= dosesToAllocate;

                if (dosesToAllocate > 0)
                {
                    if (vaccinationOrder.All(order => !order.StartsWith(person.PersonNumber)))
                    {
                        vaccinationOrder.Add($"{person.PersonNumber},{person.FirstName},{person.LastName},{dosesToAllocate}");
                    }
                }
                if (availableDoses <= 0)
                {
                    break;
                }
            }

            File.WriteAllLines(outputFilePath, vaccinationOrder);
            Console.WriteLine("Vaccination order saved to the output file.");
        }

        public static void ChangeAvailableDoses()

        {
            Console.Clear();
            Console.WriteLine("Change the number of available vaccine doses.");
            string UserVaccineDoses = Console.ReadLine();

            if (int.TryParse(UserVaccineDoses, out int NewDoses))
            {
                if (NewDoses >= 0)
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
                vaccinateChildren = false;
                Console.WriteLine("Vaccinating people under 18 is now disabled");
            }

            Console.WriteLine("Press Enter to return to the main menu");
            Console.ReadLine();
            Console.Clear();
        }

        public static void ChangeInputFile()
        {

            Console.Clear();
            Console.WriteLine("Enter the new input file path or filename:");

            string newInputFilePath = Console.ReadLine();

            if (File.Exists(newInputFilePath))
            {
                // Input file exists; update the input file path.
                inputFilePath = newInputFilePath;
                Console.WriteLine("Input file path updated to: " + inputFilePath);
            }
            else
            {
                // Input file doesn't exist, check and create directory path if needed.
                string directoryPath = Path.GetDirectoryName(newInputFilePath);
                if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                using (File.Create(newInputFilePath)) { } // Create the input file.
                Console.WriteLine("Input file created at " + newInputFilePath);
            }

            Console.WriteLine("Press Enter to return to the main menu");
            Console.ReadLine();
            Console.Clear();


        }

        public static void ChangeOutputFile()
        {

            Console.Clear();
            Console.WriteLine("Enter the new output file path or filename:");

            string newOutputFilePath = Console.ReadLine();

            if (!string.IsNullOrEmpty(newOutputFilePath))
            {
                // Check and create directory path if needed.
                string directoryPath = Path.GetDirectoryName(newOutputFilePath);
                if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                outputFilePath = newOutputFilePath;
                Console.WriteLine("Output file path updated to: " + outputFilePath);
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid file path.");
            }

            Console.WriteLine("Press Enter to return to the main menu");
            Console.ReadLine();
            Console.Clear();
        }
    }

    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public void ExampleTest()
        {
            
        }
    }
}


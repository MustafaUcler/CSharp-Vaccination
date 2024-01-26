using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Vaccination
{
    public class VaccinationOrder
    {
        public string PersonalNumber;
        public string LastName;
        public string FirstName;
        public int HealthAndCare;
        public int OlderThan18;
        public int RiskGroup;
        public bool AlreadyInfected;
    }
    public class Program
    {
        private static int availableDoses = 0;
        private static bool vaccinateChildren = false;
        private static string inputFilePath = "D:\\Users\\Abdam\\Desktop\\People.csv";
        private static string outputFilePath = "D:\\Users\\Abdam\\Desktop\\Vaccinations.csv";
        public static void Main()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            bool running = true;

            while (running)
            {
                Console.Clear();
                Console.WriteLine("Huvudmeny");
                Console.WriteLine("---------");
                Console.WriteLine($"Antal tillgängliga vaccindoser: {availableDoses}");
                Console.WriteLine($"Vaccinering under 18 år: {(vaccinateChildren ? "ja" : "nej")}");
                Console.WriteLine($"Indatafil: {inputFilePath}");
                Console.WriteLine($"Utdatafil: {outputFilePath}");

                Console.WriteLine("\nVad vill du göra?");
                int choice = ShowMenu("Välj ett alternativ:", new List<string> {
                    "Skapa prioritetsordning",
                    "Ändra antal vaccindoser",
                    "Ändra åldersgräns",
                    "Ändra indatafil",
                    "Ändra utdatafil",
                    "Avsluta" });

                if (choice == 0)
                {
                    CreatePriorityOrder();
                }
                else if (choice == 1)
                {
                    ChangeDoses();
                }
                else if (choice == 2)
                {
                    ChangeAgeLimit();
                }
                else if (choice == 3)
                {
                    ChangeInputFile();
                }
                else if (choice == 4)
                {
                    ChangeOutputFile();
                }
                else if (choice == 5)
                {
                    running = false;
                    Console.WriteLine("Tack för ditt besök!");
                }
            }
        }
        private static void CreatePriorityOrder()
        {
            string[] input = File.ReadAllLines(inputFilePath);
            string[] output = CreateVaccinationOrder(input, availableDoses, vaccinateChildren);
            var choice = ShowMenu("Filen redan existerar. Vill du skriva över den?: ", new List<string>
            {
                "Ja",
                "Nej"
            });

            if (choice == 0)
            {
                File.WriteAllLines(outputFilePath, output);
                Console.WriteLine($"Resultatet har sparats i {outputFilePath}");
            }
            else if (choice == 1)
            {
                return;
            }

            Console.WriteLine("Tryck Enter för att återvända till huvudmenyn...");
            Console.ReadLine();
        }

        private static void ChangeDoses()
        {
            Console.WriteLine("Ändra antal vaccindoser");
            Console.WriteLine("-----------------------");
            Console.Write("Ange nytt antal doser: ");
            if (int.TryParse(Console.ReadLine(), out int newDoses) && newDoses >= 0)
            {
                availableDoses = newDoses;
                Console.WriteLine("Antal vaccindoser har ändrats.");
            }
            else
            {
                Console.WriteLine("Ogiltigt värde. Antal doser förblir oförändrat.");
            }
            Console.WriteLine("Tryck Enter för att återvända till huvudmenyn...");
            Console.ReadLine();
        }

        private static void ChangeAgeLimit()
        {
            int choice = ShowMenu("Ska personer under 18 vaccineras?", new List<string>
            {
                "Ja",
                "Nej"
            });

            if (choice == 0)
            {
                vaccinateChildren = true;
                Console.WriteLine("Personer under 18 kommer att vaccineras.");
            }
            else if (choice == 1)
            {
                vaccinateChildren = false;
                Console.WriteLine("Personer under 18 kommer inte att vaccineras.");
            }
            else
            {
                Console.WriteLine("Ogiltigt val. Åldersgräns förblir oförändrad.");
            }
            Console.WriteLine("Tryck Enter för att återvända till huvudmenyn...");
            Console.ReadLine();
        }

        private static void ChangeInputFile()
        {
            Console.WriteLine("Ändra indatafil");
            Console.WriteLine("---------------");
            Console.Write("Ange ny sökväg: ");
            string newPath = Console.ReadLine();
            if (File.Exists(newPath))
            {
                inputFilePath = newPath;
                Console.WriteLine("Indatafil har ändrats.");
            }
            else
            {
                Console.WriteLine("Filen finns inte. Sökvägen förblir oförändrad.");
            }
            Console.WriteLine("Tryck Enter för att återvända till huvudmenyn...");
            Console.ReadLine();
        }

        private static void ChangeOutputFile()
        {
            Console.WriteLine("Ändra utdatafil");
            Console.WriteLine("---------------");
            Console.Write("Ange ny sökväg: ");
            string newPath = Console.ReadLine();
            outputFilePath = newPath;
            Console.WriteLine("Utdatafil har ändrats.");
            Console.WriteLine("Tryck Enter för att återvända till huvudmenyn...");
            Console.ReadLine();
        }

        // Create the lines that should be saved to a CSV file after creating the vaccination order.
        //
        // Parameters:
        //
        // input: the lines from a CSV file containing population information
        // doses: the number of vaccine doses available
        // vaccinateChildren: whether to vaccinate people younger than 18
        public static string[] CreateVaccinationOrder(string[] input, int doses, bool vaccinateChildren)
        {
            List<VaccinationOrder> orders = new List<VaccinationOrder>();

            foreach (string personLine in input)
            {

                string[] personInfoPart = personLine.Split(',');

                if (personInfoPart.Length == 6)
                {
                    string personalNumber = personInfoPart[0];
                    if (personalNumber.Length == 10)
                    {
                        int yearPrefix = int.Parse(personalNumber.Substring(0, 2));
                        if (yearPrefix < 23)
                        {
                            personalNumber = "20" + personalNumber;
                        }
                        else
                        {
                            personalNumber = "19" + personalNumber;
                        }
                        personalNumber = personalNumber.Substring(0, 8) + "-" + personalNumber.Substring(8, 4);
                    }
                    else if (personalNumber.Length == 12)
                    {
                        personalNumber = personalNumber.Substring(0, 8) + "-" + personalNumber.Substring(8, 4);
                    }

                    int alreadyInfectedValue;
                    if (!int.TryParse(personInfoPart[5], out alreadyInfectedValue))
                    {
                        // Handle the case where parsing fails
                        Console.WriteLine($"Failed to parse 'AlreadyInfected' value for {personalNumber}. Defaulting to 0.");
                        alreadyInfectedValue = 0;
                    }

                    bool alreadyInfected = alreadyInfectedValue != 0;

                    VaccinationOrder order = new VaccinationOrder
                    {
                        PersonalNumber = personalNumber,
                        LastName = personInfoPart[1],
                        FirstName = personInfoPart[2],
                        HealthAndCare = int.Parse(personInfoPart[3]),
                        RiskGroup = int.Parse(personInfoPart[4]),
                        AlreadyInfected = personInfoPart[5] == "1"

                    };

                    orders.Add(order);
                }
            }

            // Sortera orders baserat på prioriteringsordningen
            var sortedOrders = orders
                .OrderByDescending(p => p.HealthAndCare)
                .ThenByDescending(p => p.OlderThan18)
                .ThenByDescending(p => p.RiskGroup) //Lagt till sortering för riskgruppen
                .ThenBy(p => p.PersonalNumber)
                .ToList();

            // Skapa en lista med utdatasträngar
            List<string> output = new List<string>();
            foreach (var order in sortedOrders)
            {
                int totalDoses = order.AlreadyInfected ? 1 : 2;

                if (totalDoses > doses)
                {
                    if (doses == 1 && totalDoses == 2)
                    {
                        totalDoses = 0;
                    }
                    else
                    {
                        totalDoses = doses;
                        doses = 0;
                    }
                }
                else
                {
                    doses -= totalDoses;
                }

                string outputLine = $"{order.PersonalNumber},{order.LastName},{order.FirstName},{totalDoses}";
                output.Add(outputLine);
            }
            return output.ToArray();
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

            // Calculate the width of the widest option so we can make them all the same width later.
            int width = options.Max(option => option.Length);

            int selected = 0;
            int top = Console.CursorTop;
            for (int i = 0; i < options.Count(); i++)
            {
                // Start by highlighting the first option.
                if (i == 0)
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                var option = options.ElementAt(i);
                // Pad every option to make them the same width, so the highlight is equally wide everywhere.
                Console.WriteLine("- " + option.PadRight(width));

                Console.ResetColor();
            }
            Console.CursorLeft = 0;
            Console.CursorTop = top - 1;

            ConsoleKey? key = null;
            while (key != ConsoleKey.Enter)
            {
                key = Console.ReadKey(intercept: true).Key;

                // First restore the previously selected option so it's not highlighted anymore.
                Console.CursorTop = top + selected;
                string oldOption = options.ElementAt(selected);
                Console.Write("- " + oldOption.PadRight(width));
                Console.CursorLeft = 0;
                Console.ResetColor();

                // Then find the new selected option.
                if (key == ConsoleKey.DownArrow)
                {
                    selected = Math.Min(selected + 1, options.Count() - 1);
                }
                else if (key == ConsoleKey.UpArrow)
                {
                    selected = Math.Max(selected - 1, 0);
                }

                // Finally highlight the new selected option.
                Console.CursorTop = top + selected;
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.White;
                string newOption = options.ElementAt(selected);
                Console.Write("- " + newOption.PadRight(width));
                Console.CursorLeft = 0;
                // Place the cursor one step above the new selected option so that we can scroll and also see the option above.
                Console.CursorTop = top + selected - 1;
                Console.ResetColor();
            }
            // Afterwards, place the cursor below the menu so we can see whatever comes next.
            Console.CursorTop = top + options.Count();

            // Show the cursor again and return the selected option.
            Console.CursorVisible = true;
            return selected;
        }
    }

    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public void ExampleTest()
        {
            // Arrange
            string[] input =
            {
                "19720906-1111,Elba,Idris,0,0,1",
                "8102032222,Efternamnsson,Eva,1,1,0"
            };
            int doses = 10;
            bool vaccinateChildren = false;

            // Act
            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            // Assert
            Assert.AreEqual(output.Length, 2);
            Assert.AreEqual("19810203-2222,Efternamnsson,Eva,2", output[0]);
            Assert.AreEqual("19720906-1111,Elba,Idris,1", output[1]);
        }
        [TestMethod]
        public void TestCreateVaccinationOrder()
        {
            // Arrange
            string[] input = new string[] { "123456-7890,Abdul,Ambdi,1,1,1", "234567-8901,Erik,Erik,0,1,0" };
            int doses = 2;
            bool vaccinateChildren = true;
            string[] expectedOutput = new string[] { "123456-7890,Abdul,Ambdi,1", "234567-8901,Erik,Erik,0" };

            // Act
            string[] actualOutput = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            // Assert
            CollectionAssert.AreEqual(expectedOutput, actualOutput);
        }
        [TestMethod]
        public void NoVaccinationOrderTest()
        {
            string[] input = { };
            int doses = 10;
            bool vaccinateChildren = false;
            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            Assert.AreEqual(output.Length, 0);
        }
        [TestMethod]
        public void EmptyInputTest()
        {
            string[] input = { };
            int doses = 10;
            bool vaccinateChildren = true;

            // Act
            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            // Assert
            Assert.AreEqual(output.Length, 0);
        }
        public void VaccinationOrderWithZeroDoses()
        {
            string[] input =
            {
            "19720906-1111,Elba,Idris,1,1,1",
            "8102032222,Efternamnsson,Eva,1,1,1",
            "9001011234,Doe,John,1,1,1"
            };
            int doses = 0;
            bool vaccinateChildren = true;

            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            Assert.AreEqual(output.Length, 3);
            Assert.AreEqual("19720906-1111,Elba,Idris,0", output[0]);
            Assert.AreEqual("19810203-2222,Efternamnsson,Eva,0", output[1]);
            Assert.AreEqual("19001011-234,Doe,John,0", output[2]);
        }
        [TestMethod]
        public void TestCreateVaccinationOrder_LargeNumberOfDoses()
        {
            // Arrange
            string[] input = new string[]
            {
                "19800101-1234,Test1,Person1,1,1,1",
                "19800202-5678,Test2,Person2,1,1,1",
                "19800303-9876,Test3,Person3,1,1,1"
            };
            int doses = 100;
            bool vaccinateChildren = true;
            string[] expectedOutput = new string[]
            {
                "19800101-1234,Test1,Person1,1",
                "19800202-5678,Test2,Person2,1",
                "19800303-9876,Test3,Person3,1"
            };
            // Act
            string[] actualOutput = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            // Assert
            CollectionAssert.AreEqual(expectedOutput, actualOutput);
        }
        [TestMethod]
        public void TestHasAlreadyBeenInfected()
        {
            // Arrange
            string[] input = { "123456-7890,Mustafa,Ucler,1,1,1" };
            int doses = 10;
            bool vaccinateChildren = true;

            // Act
            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            // Assert
            Assert.AreEqual(output.Length, 1);
            Assert.AreEqual("123456-7890,Mustafa,Ucler,1", output[0]);
        }
        [TestMethod]
        public void TestHasNotBeenInfected()
        {
            // Arrange
            string[] input = { "123456-7890,Said,KingOfDatabase,1,1,0" };
            int doses = 10;
            bool vaccinateChildren = true;

            // Act
            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            // Assert
            Assert.AreEqual(output.Length, 1);
            Assert.AreEqual("123456-7890,Said,KingOfDatabase,2", output[0]);
        }
        [TestMethod]
        public void TestSortByRiskGroup()
        {
            // Arrange
            string[] input =
            {
            "123456-7890,Abdul,Ambdi,1,1,1",
            "234567-8901,Mustafa,Ucler,1,2,0",
            "345678-9012,Said,KingOfDatabase,1,1,1"
            };
            int doses = 10;
            bool vaccinateChildren = true;

            // Act
            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            // Assert
            Assert.AreEqual(output.Length, 3);
            Assert.AreEqual("234567-8901,Mustafa,Ucler,2", output[0]);
            Assert.AreEqual("123456-7890,Abdul,Ambdi,1", output[1]);
            Assert.AreEqual("345678-9012,Said,KingOfDatabase,1", output[2]);
        }
    }
}

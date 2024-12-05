using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CW5._1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"Data Source=DESKTOP-6676I5B;Initial Catalog=countrys;Integrated Security=True;Connect Timeout=30;TrustServerCertificate=True;";

            List<Country> countries = Load(connectionString);


            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                (sender, certificate, chain, sslPolicyErrors) => true;

            while (true)
            {
                Console.WriteLine("\nChoose an option:");
                Console.WriteLine("1. Show all countries");
                Console.WriteLine("2. Show details of a country");
                Console.WriteLine("3. Add new country");
                Console.WriteLine("4. Show Top-3 countries by least population in each continent");
                Console.WriteLine("5. Show Top-3 countries by most population in each continent");
                Console.WriteLine("6. Show average population of countries");
                Console.WriteLine("7. Show countries starting with a letter");
                Console.WriteLine("8. Show capitals starting with a letter");
                Console.Write("Choose: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowAll(countries);
                        break;
                    case "2":
                        Console.Write("Enter country name: ");
                        string countryName = Console.ReadLine();
                        ShowDetails(countries, countryName);
                        break;
                    case "3":
                        countries = AddNew(connectionString, countries);
                        break;
                    case "4":
                        ShowTopByPopulation(countries, "least");
                        break;
                    case "5":
                        ShowTopByPopulation(countries, "most");
                        break;
                    case "6":
                        ShowAveragePopulation(countries);
                        break;
                    case "7":
                        Console.Write("Enter starting letter: ");
                        string startLetter = Console.ReadLine();
                        ShowWithLetter(countries, startLetter);
                        break;
                    case "8":
                        Console.Write("Enter starting letter: ");
                        string capitalLetter = Console.ReadLine();
                        ShowCapitalsWithLetter(countries, capitalLetter);
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }

        static List<Country> Load(string connectionString)
        {
            var countries = new List<Country>();
            string query = "SELECT * FROM Countries";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        countries.Add(new Country
                        {
                            Id = (int)reader["Id"],
                            Name = (string)reader["Name"],
                            Population = Convert.ToInt32(reader["Population"]),
                            GDP = Convert.ToInt32(reader["GDP"]),
                            Capital = (string)reader["Capital"]
                        });
                    }
                }
            }
            return countries;
        }

        static void ShowAll(List<Country> countries)
        {
            Console.WriteLine("\nList of countries:");
            foreach (var country in countries)
            {
                Console.WriteLine($"Name: {country.Name}, Population: {country.Population}, Capital: {country.Capital}");
            }
        }

        static void ShowDetails(List<Country> countries, string countryName)
        {
            var country = countries.FirstOrDefault(c => c.Name.Equals(countryName, StringComparison.OrdinalIgnoreCase));
            if (country != null)
            {
                Console.WriteLine($"Name: {country.Name}, Population: {country.Population}, Capital: {country.Capital}, GDP: {country.GDP}");
            }
            else
            {
                Console.WriteLine("Country not found.");
            }
        }

        static List<Country> AddNew(string connectionString, List<Country> countries)
        {
            Console.Write("Enter country name: ");
            string name = Console.ReadLine();
            Console.Write("Enter population: ");
            int population = int.Parse(Console.ReadLine());
            Console.Write("Enter GDP: ");
            int gdp = int.Parse(Console.ReadLine());
            Console.Write("Enter capital: ");
            string capital = Console.ReadLine();

            string query = "INSERT INTO Countries (Name, Population, GDP, Capital) VALUES (@Name, @Population, @GDP, @Capital)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Population", population);
                    command.Parameters.AddWithValue("@GDP", gdp);
                    command.Parameters.AddWithValue("@Capital", capital);
                    command.ExecuteNonQuery();

                    countries.Add(new Country { Name = name, Population = population, GDP = gdp, Capital = capital });
                }
            }

            return countries;
        }

        static void ShowTopByPopulation(List<Country> countries, string type)
        {
            var topCountries = (type == "least")
                ? countries.OrderBy(c => c.Population).Take(3)
                : countries.OrderByDescending(c => c.Population).Take(3);

            Console.WriteLine($"\nTop 3 countries by {type} population:");
            foreach (var country in topCountries)
            {
                Console.WriteLine($"- {country.Name}: {country.Population}");
            }
        }

        static void ShowAveragePopulation(List<Country> countries)
        {
            if (countries.Any())
            {
                double averagePopulation = countries.Average(c => c.Population);
                Console.WriteLine($"\nThe average population of countries is: {averagePopulation:F0}");
            }
            else
            {
                Console.WriteLine("No countries available.");
            }
        }

        static void ShowWithLetter(List<Country> countries, string letter)
        {
            var filteredCountries = countries.Where(c => c.Name.StartsWith(letter, StringComparison.OrdinalIgnoreCase)).ToList();
            if (filteredCountries.Any())
            {
                Console.WriteLine($"Countries starting with {letter}:");
                foreach (var country in filteredCountries)
                {
                    Console.WriteLine($"- {country.Name}");
                }
            }
            else
            {
                Console.WriteLine($"No countries found starting with {letter}.");
            }
        }

        static void ShowCapitalsWithLetter(List<Country> countries, string letter)
        {
            var filteredCapitals = countries.Where(c => c.Capital.StartsWith(letter, StringComparison.OrdinalIgnoreCase)).ToList();
            if (filteredCapitals.Any())
            {
                Console.WriteLine($"Capitals starting with {letter}:");
                foreach (var country in filteredCapitals)
                {
                    Console.WriteLine($"- {country.Capital}");
                }
            }
            else
            {
                Console.WriteLine($"No capitals found starting with {letter}.");
            }
        }
    }

    class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Population { get; set; }
        public int GDP { get; set; }
        public string Capital { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace InventorySystem
{
    // Marker Interface
    public interface IInventoryEntity
    {
        int Id { get; }
    }

    // Immutable Inventory Record
    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    // Generic Inventory Logger
    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private readonly List<T> _log = new();
        private readonly string _filePath;

        public InventoryLogger(string filePath)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        public void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (_log.Any(x => x.Id == item.Id))
                throw new InvalidOperationException($"Item with ID {item.Id} already exists");

            _log.Add(item);
            Console.WriteLine($"Added item: {item}");
        }

        public List<T> GetAll() => new(_log);

        public void SaveToFile()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(_log, options);

                using var writer = new StreamWriter(_filePath);
                writer.Write(json);

                Console.WriteLine($"Successfully saved {_log.Count} items to {_filePath}");
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
                throw;
            }
        }

        public void LoadFromFile()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    Console.WriteLine($"File not found: {_filePath}");
                    return;
                }

                using var reader = new StreamReader(_filePath);
                string json = reader.ReadToEnd();

                var items = JsonSerializer.Deserialize<List<T>>(json)
                    ?? throw new InvalidDataException("File contains invalid data");

                _log.Clear();
                _log.AddRange(items);

                Console.WriteLine($"Successfully loaded {items.Count} items from {_filePath}");
            }
            catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException)
            {
                Console.WriteLine($"Error loading from file: {ex.Message}");
                throw;
            }
        }
    }

    // Application Layer
    public class InventoryApp
    {
        private readonly InventoryLogger<InventoryItem> _logger;

        public InventoryApp(string filePath)
        {
            _logger = new InventoryLogger<InventoryItem>(filePath);
        }

        public void SeedSampleData()
        {
            Console.WriteLine("\nSeeding sample data...");

            try
            {
                _logger.Add(new InventoryItem(1, "Laptop", 15, DateTime.Now.AddDays(-10)));
                _logger.Add(new InventoryItem(2, "Monitor", 25, DateTime.Now.AddDays(-5)));
                _logger.Add(new InventoryItem(3, "Keyboard", 50, DateTime.Now.AddDays(-2)));
                _logger.Add(new InventoryItem(4, "Mouse", 60, DateTime.Now));
                _logger.Add(new InventoryItem(5, "Headphones", 30, DateTime.Now.AddDays(-1)));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding data: {ex.Message}");
            }
        }

        public void SaveData()
        {
            Console.WriteLine("\nSaving data to file...");
            _logger.SaveToFile();
        }

        public void LoadData()
        {
            Console.WriteLine("\nLoading data from file...");
            _logger.LoadFromFile();
        }

        public void PrintAllItems()
        {
            Console.WriteLine("\nCurrent Inventory:");
            Console.WriteLine("------------------");

            var items = _logger.GetAll();
            if (items.Count == 0)
            {
                Console.WriteLine("No items in inventory");
                return;
            }

            foreach (var item in items)
            {
                Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, " +
                                $"Qty: {item.Quantity}, Added: {item.DateAdded:yyyy-MM-dd}");
            }
        }
    }

    class Program
    {
        static void Main()
        {
            const string filePath = "inventory.json";

            Console.WriteLine("Inventory Management System");
            Console.WriteLine("===========================");

            try
            {
                // Create and run first session
                var app = new InventoryApp(filePath);
                app.SeedSampleData();
                app.SaveData();

                // Simulate new session
                Console.WriteLine("\nSimulating new session...");
                var newApp = new InventoryApp(filePath);
                newApp.LoadData();
                newApp.PrintAllItems();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal error: {ex.Message}");
            }
        }
    }
}
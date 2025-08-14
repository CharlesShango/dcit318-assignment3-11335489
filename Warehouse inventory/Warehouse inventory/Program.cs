using System;
using System.Collections.Generic;

namespace WarehouseInventorySystem
{
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Quantity = quantity;
            Brand = brand ?? throw new ArgumentNullException(nameof(brand));
            WarrantyMonths = warrantyMonths;
        }

        public override string ToString() =>
            $"[Electronic] ID: {Id}, {Name} ({Brand}), Qty: {Quantity}, Warranty: {WarrantyMonths} months";
    }

    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Quantity = quantity;
            ExpiryDate = expiryDate;
        }

        public override string ToString() =>
            $"[Grocery] ID: {Id}, {Name}, Qty: {Quantity}, Expires: {ExpiryDate:yyyy-MM-dd}";
    }

    public class InventoryRepository<T> where T : class, IInventoryItem
    {
        private readonly Dictionary<int, T> _items = new();

        public void AddItem(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (_items.ContainsKey(item.Id)) throw new Exception($"Item ID {item.Id} already exists!");

            _items.Add(item.Id, item);
            Console.WriteLine($"✅ Added: {item}");
        }

        public T? GetItemById(int id) => _items.TryGetValue(id, out var item) ? item : null;

        public void RemoveItem(int id)
        {
            if (!_items.Remove(id)) throw new Exception($"Item ID {id} not found!");
            Console.WriteLine($"🗑️ Removed item ID: {id}");
        }

        public List<T> GetAllItems() => new(_items.Values);

        public void UpdateQuantity(int id, int newQuantity)
        {
            var item = GetItemById(id) ?? throw new Exception($"Item ID {id} not found!");
            if (newQuantity < 0) throw new Exception("Quantity cannot be negative!");

            item.Quantity = newQuantity;
            Console.WriteLine($"🔄 Updated {item.Name} (ID: {id}) quantity to: {newQuantity}");
        }
    }

    public class WarehouseManager
    {
        private readonly InventoryRepository<ElectronicItem> _electronics = new();
        private readonly InventoryRepository<GroceryItem> _groceries = new();

        public void RunDemo()
        {
            Console.WriteLine("🏭 WAREHOUSE INVENTORY SYSTEM 🏭");
            Console.WriteLine("===============================\n");

            SeedInitialData();
            PrintInventory();
            TestOperations();
        }

        private void SeedInitialData()
        {
            Console.WriteLine("📦 Seeding initial data...\n");

            // Add sample electronics
            _electronics.AddItem(new ElectronicItem(1, "Smartphone", 50, "Samsung", 24));
            _electronics.AddItem(new ElectronicItem(2, "Laptop", 30, "Dell", 36));

            // Add sample groceries
            _groceries.AddItem(new GroceryItem(101, "Milk", 100, DateTime.Today.AddDays(7)));
            _groceries.AddItem(new GroceryItem(102, "Bread", 150, DateTime.Today.AddDays(3)));

            Console.WriteLine();
        }

        private void PrintInventory()
        {
            Console.WriteLine("📋 CURRENT INVENTORY");
            Console.WriteLine("====================");

            Console.WriteLine("\n💻 Electronics:");
            _electronics.GetAllItems().ForEach(Console.WriteLine);

            Console.WriteLine("\n🛒 Groceries:");
            _groceries.GetAllItems().ForEach(Console.WriteLine);

            Console.WriteLine();
        }

        private void TestOperations()
        {
            Console.WriteLine("⚙️ Testing Operations...\n");

            // Successful operations
            _electronics.UpdateQuantity(1, 45);
            _groceries.UpdateQuantity(101, 80);

            // Try error cases
            TryOperation("Add duplicate electronic", () =>
                _electronics.AddItem(new ElectronicItem(1, "Phone", 10, "Apple", 12)));

            TryOperation("Update non-existent item", () =>
                _electronics.UpdateQuantity(99, 10));

            TryOperation("Set negative quantity", () =>
                _groceries.UpdateQuantity(101, -5));

            TryOperation("Remove non-existent item", () =>
                _groceries.RemoveItem(999));

            // Final inventory
            Console.WriteLine("\n🔍 Final Inventory Status:");
            PrintInventory();
        }

        private void TryOperation(string description, Action action)
        {
            Console.WriteLine($"\n🔹 Testing: {description}");
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }
    }

    class Program
    {
        static void Main()
        {
            var manager = new WarehouseManager();
            manager.RunDemo();

            Console.WriteLine("\n🏁 System demo complete!");
        }
    }
}
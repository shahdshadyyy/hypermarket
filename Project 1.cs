using System;
using System.Collections.Generic;
using System.Linq;

namespace HyperMarket
{
    class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }

        public Product(string name, decimal price, string description)
        {
            Name = name;
            Price = price;
            Description = description;
        }
    }

    class Category
    {
        public string Name { get; set; }
        public List<Product> Products { get; } = new List<Product>();
        public List<Category> SubCategories { get; } = new List<Category>();

        public Category(string name)
        {
            Name = name;
        }

        public void AddProduct(string name, decimal price, string description)
        {
            Product product = new Product(name, price, description);
            Products.Add(product);
        }

        public bool RemoveProduct(string productName)
        {
            var product = Products.FirstOrDefault(p => p.Name.Equals(productName, StringComparison.OrdinalIgnoreCase));
            if (product != null)
            {
                Products.Remove(product);
                return true;
            }

            foreach (var subCategory in SubCategories)
            {
                if (subCategory.RemoveProduct(productName))
                {
                    return true;
                }
            }

            return false;
        }


        public bool UpdateProduct(string productName, decimal newPrice, string newDescription)
        {
            var product = Products.FirstOrDefault(p => p.Name.Equals(productName, StringComparison.OrdinalIgnoreCase));
            if (product != null)
            {
                product.Price = newPrice;
                product.Description = newDescription;
                return true;
            }
            return false;
        }

        public void AddSubCategory(Category subCategory)
        {
            SubCategories.Add(subCategory);
        }

        public decimal CalculateTotalPrice()
        {
            decimal totalPrice = Products.Sum(p => p.Price);
            foreach (var subCategory in SubCategories)
            {
                totalPrice += subCategory.CalculateTotalPrice();
            }
            return totalPrice;
        }

        public void DisplayCategoryHierarchy(int depth = 0)
        {
            string indent = new string(' ', depth * 4);
            Console.WriteLine($"{indent}{Name} (Total Price: {CalculateTotalPrice():C2})");
            foreach (var product in Products)
            {
                Console.WriteLine($"{indent}- {product.Name}, Price: {product.Price:C2}, Description: {product.Description}");
            }
            foreach (var subCategory in SubCategories)
            {
                subCategory.DisplayCategoryHierarchy(depth + 1);
            }
        }

        public Product SearchProduct(string productName)
        {
            foreach (var product in Products)
            {
                if (product.Name.Equals(productName, StringComparison.OrdinalIgnoreCase))
                {
                    return product;
                }
            }
            foreach (var subCategory in SubCategories)
            {
                var foundProduct = subCategory.SearchProduct(productName);
                if (foundProduct != null)
                {
                    return foundProduct;
                }
            }
            return null;
        }

        public void SortProducts()
        {
            Products.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
            foreach (var subCategory in SubCategories)
            {
                subCategory.SortProducts();
            }
        }
    }

    class Hypermarket
    {
        public Category RootCategory { get; }

        public Hypermarket()
        {
            RootCategory = new Category("Root");
        }

        public void InitializeSampleData()
        {
            var electronics = new Category("Electronics");
            electronics.AddProduct("Television", 299.99m, "HD TV");

            var fruits = new Category("Fruits");
            fruits.AddProduct("Apple", 1.20m, "Red Apple");
            fruits.AddProduct("Banana", 0.50m, "Yellow Banana");

            var vegetables = new Category("Vegetables");
            vegetables.AddProduct("Roca", 0.5m, "Green Roca");

            var dairies = new Category("Dairies");
            dairies.AddProduct("Dina Farm's Milk", 7m, "Full Fat Milk");

            var snacks = new Category("Snacks");
            snacks.AddProduct("Oreo", 1.7m, "Biscuit");

            var drinks = new Category("Drinks");
            drinks.AddProduct("RedBull", 4m, "Energy Drink");

            var household = new Category("Household");
            household.AddProduct("Tide", 18.99m, "Detergent");

            RootCategory.AddSubCategory(fruits);
            RootCategory.AddSubCategory(electronics);
            RootCategory.AddSubCategory(vegetables);
            RootCategory.AddSubCategory(dairies);
            RootCategory.AddSubCategory(snacks);
            RootCategory.AddSubCategory(drinks);
            RootCategory.AddSubCategory(household);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Hypermarket hypermarket = new Hypermarket();
            hypermarket.InitializeSampleData();
            string message = "Welcome To Your Grocery List";
            int screenWidth = Console.WindowWidth;
            int messageWidth = message.Length;
            int leftPadding = (screenWidth - messageWidth) / 2;
            string centeredMessage = new string(' ', leftPadding) + message;
            Console.WriteLine(centeredMessage);
            Console.ReadLine();

            while (true)
            {

                Console.WriteLine("\nMenu:");
                Console.WriteLine("1. Add Product");
                Console.WriteLine("2. Delete Product");
                Console.WriteLine("3. Update Product");
                Console.WriteLine("4. Search Product");
                Console.WriteLine("5. Sort Products");
                Console.WriteLine("6. Display All Products");
                Console.WriteLine("7. Calculate Total Price");
                Console.WriteLine("8. Display Category Hierarchy");
                Console.WriteLine("9. Exit");
                Console.Write("Enter your choice (1-9): ");

                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            AddProduct(hypermarket.RootCategory);
                            break;
                        case 2:
                            DeleteProduct(hypermarket.RootCategory);
                            break;
                        case 3:
                            UpdateProduct(hypermarket.RootCategory);
                            break;
                        case 4:
                            SearchProduct(hypermarket.RootCategory);
                            break;
                        case 5:
                            hypermarket.RootCategory.SortProducts();
                            Console.WriteLine("Products sorted.");
                            break;
                        case 6:
                            DisplayAllProducts(hypermarket.RootCategory);
                            break;
                        case 7:
                            decimal totalPrice = hypermarket.RootCategory.CalculateTotalPrice();
                            Console.WriteLine($"Total Price of All Products in Root Category: {totalPrice:C2}");
                            break;
                        case 8:
                            Console.WriteLine("\nCategory Hierarchy:");
                            hypermarket.RootCategory.DisplayCategoryHierarchy();
                            break;
                        case 9:
                            Environment.Exit(0);
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Please enter a valid option.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number (1-9).");
                }
            }
        }

        static void AddProduct(Category category)
        {
            Console.Write("Enter product name: ");
            string name = Console.ReadLine();
            Console.Write("Enter product price: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal price))
            {
                Console.Write("Enter product description: ");
                string description = Console.ReadLine();

                Console.WriteLine("Select a category to add the product to:");
                int categoryChoice = ChooseCategory(category);

                if (categoryChoice >= 0 && categoryChoice < category.SubCategories.Count)
                {
                    category.SubCategories[categoryChoice].AddProduct(name, price, description);
                    Console.WriteLine($"Product '{name}' added to '{category.SubCategories[categoryChoice].Name}' category successfully.");
                }
                else
                {
                    Console.WriteLine("Invalid category selection.");
                }
            }
            else
            {
                Console.WriteLine("Invalid price. Please enter a valid decimal number.");
            }
        }

        static int ChooseCategory(Category category)
        {
            for (int i = 0; i < category.SubCategories.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {category.SubCategories[i].Name}");
            }

            int categoryChoice;
            if (int.TryParse(Console.ReadLine(), out categoryChoice) && categoryChoice >= 1 && categoryChoice <= category.SubCategories.Count)
            {
                return categoryChoice - 1; // Adjust for 0-based index
            }

            return -1; // Invalid choice
        }

        static void DeleteProduct(Category category)
        {
            Console.Write("Enter product name to delete: ");
            string productName = Console.ReadLine();
            if (category.RemoveProduct(productName))
            {
                Console.WriteLine($"Product '{productName}' deleted successfully.");
            }
            else
            {
                Console.WriteLine($"Product '{productName}' not found.");
            }
        }

        static void UpdateProduct(Category category)
        {
            Console.Write("Enter product name to update: ");
            string productName = Console.ReadLine();
            Console.Write("Enter new price: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal newPrice))
            {
                Console.Write("Enter new description: ");
                string newDescription = Console.ReadLine();
                if (category.UpdateProduct(productName, newPrice, newDescription))
                {
                    Console.WriteLine($"Product '{productName}' updated successfully.");
                }
                else
                {
                    Console.WriteLine($"Product '{productName}' not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid price. Please enter a valid decimal number.");
            }
        }

        static void SearchProduct(Category category)
        {
            Console.Write("Enter product name to search: ");
            string productName = Console.ReadLine();
            Product foundProduct = category.SearchProduct(productName);
            if (foundProduct != null)
            {
                Console.WriteLine($"Product found: {foundProduct.Name}, Price: {foundProduct.Price:C2}, Description: {foundProduct.Description}");
            }
            else
            {
                Console.WriteLine($"Product '{productName}' not found.");
            }
        }

        static void DisplayAllProducts(Category category)
        {
            Console.WriteLine("\nAll Products:");
            DisplayAllProductsRecursive(category);
        }

        static void DisplayAllProductsRecursive(Category category, int depth = 0)
        {
            string indent = new string(' ', depth * 4);
            foreach (var product in category.Products)
            {
                Console.WriteLine($"{indent}- {product.Name}, Price: {product.Price:C2}, Description: {product.Description}");
            }

            foreach (var subCategory in category.SubCategories)
            {
                Console.WriteLine($"{indent}{subCategory.Name} (Total Price: {subCategory.CalculateTotalPrice():C2})");
                DisplayAllProductsRecursive(subCategory, depth + 1);
            }
        }
    }
}
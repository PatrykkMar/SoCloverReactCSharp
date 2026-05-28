using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SoClover.Server.Models;

namespace SoClover.Server.Context
{
    public static class CardSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, "Context", "cards.json");

            if (!File.Exists(filePath))
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "Context", "cards.json");
            }

            try
            {
                string json = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
                var cards = JsonSerializer.Deserialize<List<Card>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (cards != null)
                {
                    modelBuilder.Entity<Card>().HasData(cards);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during seeding: {ex.Message}");
            }
        }
    }
}
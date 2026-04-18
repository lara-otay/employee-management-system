using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = serviceProvider.GetRequiredService<AppDbContext>();

            if (context.Employees.Any())
            {
                return;   // DB has been seeded
            }

            context.Employees.AddRange(
                new Employee { Name = "Alice Johnson", Email = "alice.johnson@example.com", Phone = "555-0100", Department = "HR", IsActive = true },
                new Employee { Name = "Bob Smith", Email = "bob.smith@example.com", Phone = "555-0111", Department = "Engineering", IsActive = true },
                new Employee { Name = "Carol Williams", Email = "carol.williams@example.com", Phone = "555-0122", Department = "Sales", IsActive = false },
                new Employee { Name = "David Lee", Email = "david.lee@example.com", Phone = "555-0133", Department = "Marketing", IsActive = true },
                new Employee { Name = "Emma Brown", Email = "emma.brown@example.com", Phone = "555-0144", Department = "Finance", IsActive = true },
                new Employee { Name = "Test Employee", Email = "test.employee@example.com", Phone = "555-4324", Department = "HR", IsActive = false }
            );

            context.SaveChanges();
        }
    }
}
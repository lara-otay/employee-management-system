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
                new Employee { Name = "Alice Johnson", Email = "noor.ali@example.com", Phone = "0798349825", Department = "HR", IsActive = true },
                new Employee { Name = "Bob Smith", Email = "ahmad.alaa@example.com", Phone = "0770251849", Department = "Engineering", IsActive = true },
                new Employee { Name = "Carol Williams", Email = "laith.khalil@example.com", Phone = "0794278814", Department = "Sales", IsActive = false },
                new Employee { Name = "David Lee", Email = "zaid.ibrahim@example.com", Phone = "0771215509", Department = "Marketing", IsActive = true },
                new Employee { Name = "Emma Brown", Email = "sarah.khalid@example.com", Phone = "0775549811", Department = "Finance", IsActive = true },
                new Employee { Name = "Test Employee", Email = "mohammad.jaber@example.com", Phone = "0780094203", Department = "HR", IsActive = false }
            );

            context.SaveChanges();
        }
    }
}
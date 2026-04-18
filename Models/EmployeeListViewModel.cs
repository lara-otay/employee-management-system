using System.Collections.Generic;

namespace EmployeeManagement.Models
{
    public class EmployeeListViewModel
    {
        public IEnumerable<Employee> Employees { get; set; } = new List<Employee>();

        public string? SearchName { get; set; }
        public string? SearchEmail { get; set; }
        public string? Department { get; set; }
        public string? Status { get; set; }

        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }

        public string ViewMode { get; set; } = "list"; // or "card"
    }
}

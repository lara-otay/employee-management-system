namespace EmployeeManagement.Models
{
    public class EmployeeFormDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Department { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

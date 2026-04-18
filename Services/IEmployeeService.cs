using System.Collections.Generic;
using System.Threading.Tasks;
using EmployeeManagement.Models;

namespace EmployeeManagement.Services
{
    public interface IEmployeeService
    {
        Task<(IEnumerable<Employee> Items, int Total)> GetPagedAsync(string? searchName, string? searchEmail, string? department, string? status, int page, int pageSize);
        Task<IEnumerable<Employee>> GetFilteredAsync(string? searchName, string? searchEmail, string? department, string? status);
        Task<Employee?> GetByIdAsync(int id);
        Task<Employee> CreateAsync(EmployeeCreateDto dto);
        Task<bool> UpdateAsync(int id, EmployeeUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ToggleStatusAsync(int id);
    }
}
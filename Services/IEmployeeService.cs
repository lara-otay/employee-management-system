using System.Collections.Generic;
using System.Threading.Tasks;
using EmployeeManagement.Models;

namespace EmployeeManagement.Services
{
    public interface IEmployeeService
    {
        Task<(IEnumerable<EmployeeDto> Items, int Total)> GetPagedAsync(string? searchName, string? searchEmail, string? department, EmployeeManagement.Models.EmployeeStatus status, int page, int pageSize);
        Task<IEnumerable<EmployeeDto>> GetFilteredAsync(string? searchName, string? searchEmail, string? department, EmployeeManagement.Models.EmployeeStatus status);
        Task<EmployeeDto?> GetByIdAsync(int id);
        Task<EmployeeDto> CreateAsync(EmployeeCreateDto dto);
        Task<bool> UpdateAsync(int id, EmployeeUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ToggleStatusAsync(int id);
    }
}
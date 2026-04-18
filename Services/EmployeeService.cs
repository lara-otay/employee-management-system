using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Data;
using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _db;

        public EmployeeService(AppDbContext db)
        {
            _db = db;
        }

        private IQueryable<Employee> ApplyFilters(IQueryable<Employee> query, string? searchName, string? searchEmail, string? department, string? status)
        {
            if (!string.IsNullOrWhiteSpace(searchName))
            {
                query = query.Where(e => e.Name.Contains(searchName));
            }

            if (!string.IsNullOrWhiteSpace(searchEmail))
            {
                query = query.Where(e => e.Email.Contains(searchEmail));
            }

            if (!string.IsNullOrWhiteSpace(department))
            {
                query = query.Where(e => e.Department != null && e.Department.Contains(department));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (string.Equals(status, "Active", StringComparison.OrdinalIgnoreCase))
                    query = query.Where(e => e.IsActive);
                else if (string.Equals(status, "Inactive", StringComparison.OrdinalIgnoreCase))
                    query = query.Where(e => !e.IsActive);
            }

            return query;
        }

        public async Task<(IEnumerable<Employee> Items, int Total)> GetPagedAsync(string? searchName, string? searchEmail, string? department, string? status, int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 5;

            var query = _db.Employees.AsNoTracking().AsQueryable();
            query = ApplyFilters(query, searchName, searchEmail, department, status);

            var total = await query.CountAsync();
            var items = await query.OrderBy(e => e.Name)
                                   .Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return (items, total);
        }

        public async Task<IEnumerable<Employee>> GetFilteredAsync(string? searchName, string? searchEmail, string? department, string? status)
        {
            var query = _db.Employees.AsNoTracking().AsQueryable();
            query = ApplyFilters(query, searchName, searchEmail, department, status);
            return await query.OrderBy(e => e.Name).ToListAsync();
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _db.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Employee> CreateAsync(EmployeeCreateDto dto)
        {
            var employee = new Employee
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Department = dto.Department,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _db.Employees.Add(employee);
            await _db.SaveChangesAsync();
            return employee;
        }

        public async Task<bool> UpdateAsync(int id, EmployeeUpdateDto dto)
        {
            var existing = await _db.Employees.FindAsync(id);
            if (existing == null) return false;

            existing.Name = dto.Name;
            existing.Email = dto.Email;
            existing.Phone = dto.Phone;
            existing.Department = dto.Department;
            existing.IsActive = dto.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;

            _db.Employees.Update(existing);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _db.Employees.FindAsync(id);
            if (existing == null) return false;
            _db.Employees.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleStatusAsync(int id)
        {
            var existing = await _db.Employees.FindAsync(id);
            if (existing == null) return false;
            existing.IsActive = !existing.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;
            _db.Employees.Update(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}

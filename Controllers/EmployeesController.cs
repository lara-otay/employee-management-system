using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.Data;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly Microsoft.Extensions.Logging.ILogger<EmployeesController> _logger;

        public EmployeesController(AppDbContext context, Microsoft.Extensions.Logging.ILogger<EmployeesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Employees
        public async Task<IActionResult> Index(string? searchName, string? searchEmail, string? department, string? status, int page = 1, int pageSize = 10, string viewMode = "list")
        {
            _logger.LogInformation("Listing employees: name={searchName} email={searchEmail} department={department} status={status} page={page} size={pageSize}", searchName, searchEmail, department, status, page, pageSize);
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 5;

            var query = _context.Employees.AsNoTracking().AsQueryable();

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

            var total = await query.CountAsync();
            var items = await query.OrderBy(e => e.Name)
                                   .Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            var vm = new EmployeeListViewModel
            {
                Employees = items,
                Page = page,
                PageSize = pageSize,
                TotalItems = total,
                SearchName = searchName,
                SearchEmail = searchEmail,
                Department = department,
                Status = status,
                ViewMode = viewMode
            };

            return View(vm);
        }

        // GET: Employees/Export
        public async Task<IActionResult> Export(string? searchName, string? searchEmail, string? department, string? status)
        {
            _logger.LogInformation("Exporting employees CSV: name={searchName} email={searchEmail} department={department} status={status}", searchName, searchEmail, department, status);
            var query = _context.Employees.AsNoTracking().AsQueryable();

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

            var items = await query.OrderBy(e => e.Name).ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("Id,Name,Email,Phone,Department,Status,CreatedAt,UpdatedAt");
            foreach (var e in items)
            {
                string Escape(string? s) => string.IsNullOrEmpty(s) ? "" : '"' + s.Replace("\"", "\"\"") + '"';
                sb.AppendLine($"{e.Id},{Escape(e.Name)},{Escape(e.Email)},{Escape(e.Phone)},{Escape(e.Department)},{(e.IsActive ? "Active" : "Inactive")},{e.CreatedAt:u},{(e.UpdatedAt.HasValue ? e.UpdatedAt.Value.ToString("u") : "")}");
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", "employees.csv");
        }

        // POST: Employees/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();

            _logger.LogInformation("Toggling status for employee {Id} (current={IsActive})", id, employee.IsActive);
            employee.IsActive = !employee.IsActive;
            employee.UpdatedAt = DateTime.UtcNow;
            _context.Update(employee);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _context.Employees.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null) return NotFound();

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Email,Phone,Department,IsActive")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Creating employee {Email}", employee.Email);
                employee.CreatedAt = DateTime.UtcNow;
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Phone,Department,IsActive,CreatedAt")] Employee employee)
        {
            if (id != employee.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogInformation("Updating employee {Id}", id);
                    employee.UpdatedAt = DateTime.UtcNow;
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _context.Employees.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null) return NotFound();

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _logger.LogInformation("Deleting employee {Id}", id);
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}

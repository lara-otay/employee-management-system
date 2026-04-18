using System;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly EmployeeManagement.Services.IEmployeeService _service;
        private readonly Microsoft.Extensions.Logging.ILogger<EmployeesController> _logger;

        public EmployeesController(EmployeeManagement.Services.IEmployeeService service, Microsoft.Extensions.Logging.ILogger<EmployeesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET: Employees
        public async Task<IActionResult> Index(string? searchName, string? searchEmail, string? department, string? status, int page = 1, int pageSize = 5, string viewMode = "list")
        {
            _logger.LogInformation("Listing employees: name={searchName} email={searchEmail} department={department} status={status} page={page} size={pageSize}", searchName, searchEmail, department, status, page, pageSize);

            var statusEnum = ParseStatus(status);
            var (items, total) = await _service.GetPagedAsync(searchName, searchEmail, department, statusEnum, page, pageSize);

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
            var statusEnum = ParseStatus(status);
            var items = await _service.GetFilteredAsync(searchName, searchEmail, department, statusEnum);

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
            var ok = await _service.ToggleStatusAsync(id);
            if (!ok) return NotFound();
            return RedirectToAction(nameof(Index));
        }

        private EmployeeManagement.Models.EmployeeStatus ParseStatus(string? status)
        {
            if (string.IsNullOrWhiteSpace(status)) return EmployeeManagement.Models.EmployeeStatus.Any;
            if (string.Equals(status, "Active", StringComparison.OrdinalIgnoreCase)) return EmployeeManagement.Models.EmployeeStatus.Active;
            if (string.Equals(status, "Inactive", StringComparison.OrdinalIgnoreCase)) return EmployeeManagement.Models.EmployeeStatus.Inactive;
            return EmployeeManagement.Models.EmployeeStatus.Any;
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _service.GetByIdAsync(id.Value);
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
        public async Task<IActionResult> Create([Bind("Name,Email,Phone,Department,IsActive")] EmployeeCreateDto dto)
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Creating employee {Email}", dto.Email);
                await _service.CreateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _service.GetByIdAsync(id.Value);
            if (employee == null) return NotFound();

            var dto = new EmployeeUpdateDto
            {
                Name = employee.Name,
                Email = employee.Email,
                Phone = employee.Phone,
                Department = employee.Department,
                IsActive = employee.IsActive
            };

            ViewData["EmployeeId"] = employee.Id;
            return View(dto);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeUpdateDto dto)
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Updating employee {Id}", id);
                var ok = await _service.UpdateAsync(id, dto);
                if (!ok) return NotFound();
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _service.GetByIdAsync(id.Value);
            if (employee == null) return NotFound();

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();
            _logger.LogInformation("Deleting employee {Id}", id);
            return RedirectToAction(nameof(Index));
        }


    }
}

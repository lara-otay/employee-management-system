# EmployeeManagement

Simple ASP.NET Core MVC Employee Management demo using EF Core InMemory provider.

Run locally (PowerShell):

1. Restore & build

   dotnet restore
   dotnet build

2. Run

   dotnet run --project .\EmployeeManagement\EmployeeManagement.csproj

Open: https://localhost:5001/ or http://localhost:5000/ then navigate to /Employees

Prepare repository and push to GitHub (example steps):

1. Initialize git and commit

   cd C:\Users\larao\source\repos\EmployeeManagement
   git init
   git add .
   git commit -m "Initial: add EmployeeManagement app with EF InMemory and basic CRUD"

2. Create a GitHub repository (via web UI) and copy the remote URL, then:

   git remote add origin https://github.com/<your-username>/<repo-name>.git
   git branch -M main
   git push -u origin main

Or use the GitHub CLI:

   gh repo create <repo-name> --public --source=. --remote=origin --push

Notes
- The project uses EF Core InMemory for simplicity. For persistent storage, switch to SQLite or SQL Server and update Program.cs.
- Client-side validation uses jQuery Validate and unobtrusive scripts.

Suggested commits:
- "feat: add Employee model, DbContext and seed data"
- "feat: implement Employees controller and views (CRUD, search, paging, export)"
- "chore: enable logging and client-side validation"

If you want, I can create the initial local commit file or a PowerShell script that runs the git commands (you must provide the remote URL).
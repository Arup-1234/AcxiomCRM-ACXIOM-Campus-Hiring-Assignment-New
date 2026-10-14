# AcxiomCRM — ACXIOM Campus Hiring Assignment

An ASP.NET Core MVC CRM starter implementation aligned to the supplied functional requirements: ASP.NET Core Identity, EF Core/SQL Server, role-based authorization, Customers, Leads, Opportunities, Follow-ups, Activities, dashboard metrics and Chart.js, audit log, and a secured REST API.

## Requirements
- .NET 8 SDK
- SQL Server LocalDB (default connection string) or SQL Server Express/Developer
- VS Code with the C# Dev Kit extension (recommended)

## Run locally (Windows)
1. Extract the ZIP and open the `AcxiomCRM` folder in VS Code.
2. In the integrated terminal, run:
   ```powershell
   dotnet --info
   dotnet restore
   dotnet build
   dotnet run
   ```
3. Open the local URL printed in the terminal (launch profile uses `http://localhost:5080`).
4. The app creates the database using `EnsureCreated` on first startup and seeds roles plus a local administrator.

### First local administrator
- Email: `admin@acxiomcrm.local`
- Password: `ChangeMe123!`

**Change this password and the seed credentials before any demonstration on a shared machine.** They are for local development only. Set `SeedAdmin:Email` and `SeedAdmin:Password` in user secrets or environment variables rather than committing production credentials.

## SQL Server configuration
The default connection string uses LocalDB:
`Server=(localdb)\MSSQLLocalDB;Database=AcxiomCRM;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True`

If using SQL Express, replace it with e.g. `Server=.\SQLEXPRESS;Database=AcxiomCRM;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true`. Ensure the SQL Server instance is running.

## Included features
- ASP.NET Core Identity registration, login, logout, password hashing, unique email, lockout after five failed attempts, role support.
- Roles: `Admin`, `Manager`, `Sales Executive`; role management restricted to Admin; audit viewing restricted to Admin/Manager.
- Customer CRUD, searching by name/email/phone/company, unique email and phone checks.
- Lead create/read/update/delete, lead status validation and search/filter.
- Opportunity create/read/update/delete, customer/lead links, amount/probability/date validation and filters.
- Follow-up scheduling, date validation, completion action, customer/lead association.
- Activity create/list/search/edit and restricted delete.
- Dashboard KPIs and Chart.js charts for lead status, opportunity stages and monthly won sales.
- Manager/Admin reports for CRM totals, pipeline value, recent activity and audit events.
- Audit records for CRM create/update/delete actions, follow-up completion and role changes; Admin/Manager audit screen.
- Admin/Manager-only delete operations for CRM records; Admin-only user-role management and account activation/deactivation.
- Secured REST APIs: CRUD endpoints for `/api/customers`, `/api/leads`, and `/api/opportunities`; returns DTOs and standard HTTP status codes. All API routes require a signed-in CRM role.
- Anti-forgery tokens on MVC POST forms; DataAnnotations and server-side ModelState validation.

## API quick test
Sign in to the MVC app first. Use a browser or API client with the same authenticated session for:
- `GET http://localhost:5080/api/customers`
- `GET http://localhost:5080/api/customers?q=Acme`
- `GET http://localhost:5080/api/customers/1`

For POST/PUT JSON, send `Content-Type: application/json`. Example POST body:
```json
{
  "customerId": 0,
  "customerCode": "CUS-001",
  "customerName": "Aarav Sharma",
  "email": "aarav@example.com",
  "phone": "9876543210",
  "companyName": "Example Pvt Ltd",
  "status": "Active"
}
```

## Important notes before submission
- This project is a functional baseline; compile and test it in your local .NET/SQL Server environment. Environment-specific fixes may be needed.
- The current startup uses `EnsureCreated` for simple first-run setup. If you switch to EF migrations, create a migration in a clean database and remove/replace `EnsureCreated`; do not mix the two approaches.
- Audit entries cover MVC CRM create/update/delete actions, follow-up completion and role changes. Login success, failed login, lockout and logout events are also written to the audit log by the custom Identity pages.
- User assignment is stored as an identifier/email field in the CRM entities; this baseline does not enforce per-salesperson record-level data scoping. The assignment's role authorization is enforced at controller/API level.
- Chart.js and Bootstrap are loaded from CDNs, so those UI assets require internet access in the browser.
- For production, use secrets, HTTPS, migrations, stronger audit coverage, and tests; do not use the seeded development password.

## Recommended final verification checklist
- [ ] `dotnet restore` succeeds
- [ ] `dotnet build` succeeds with no errors
- [ ] Login and registration work
- [ ] Anonymous users are redirected from dashboard/CRM pages
- [ ] Admin can access Users & Roles and Audit Log
- [ ] Create/edit/delete customer, lead, and opportunity
- [ ] Duplicate customer email/phone is rejected
- [ ] Opportunity amount must be positive; probability 0–100; close date not in the past
- [ ] Follow-up cannot be scheduled in the past and must link to customer or lead
- [ ] Dashboard charts load
- [ ] Signed-out request to `/api/customers` is unauthorized/challenged; signed-in request returns JSON

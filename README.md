# Payment Ops — Enterprise Payment Processing & Reporting Simulation

A mock enterprise payment processing platform built to demonstrate a realistic full-stack architecture: a .NET 8 Web API backend, a SQL Server data layer with analytic reporting, and a React reporting dashboard.

The system simulates the core loop of a payments back office — accepting a transaction, running it through a mock authorization step, persisting it, and surfacing operational reporting on top of it (daily volume, moving averages, day-over-day change).

## Why this project

Most portfolio CRUD apps stop at "save a row, list the rows." This one is built around the part that actually matters in a payments/banking context: the reporting layer. The SQL script includes a stored procedure built with CTEs and window functions (`AVG() OVER`, `SUM() OVER`, `LAG()`) to compute rolling metrics directly in the database, the same approach used in real financial reporting pipelines.

## Architecture

```
React (Vite)  --->  ASP.NET Core 8 Web API  --->  SQL Server
 dashboard          Dapper + ADO.NET               stored procedures,
 (port 5173)        (port 5142)                    CTE / window functions
```

- **Backend**: minimal, controller-based ASP.NET Core Web API. Dapper is used instead of an ORM for direct control over the SQL that hits the database — closer to how reporting-heavy financial systems are usually built.
- **Reporting**: the daily volume report is not computed in C#. It's a single SQL Server stored procedure (`usp_GetDailyVolumeReport`) using a CTE to aggregate per-day totals and window functions to compute a 7-day moving average, a running total, and day-over-day % change.
- **Frontend**: React dashboard with a transaction form, a filterable transaction table, and a chart (bar + line) rendered with Recharts.

## Tech stack

| Layer      | Technology                          |
|------------|--------------------------------------|
| Frontend   | React 18, Vite, Recharts             |
| Backend    | ASP.NET Core 8 Web API, Dapper       |
| Database   | Microsoft SQL Server                 |
| API docs   | Swagger / OpenAPI                    |

## Project structure

```
kurumsal-odeme-sistemi/
├── database/
│   ├── 01_create_schema.sql      # tables, constraints, indexes, stored procedure
│   └── 02_seed_data.sql          # ~45 days of mock transactions
├── backend/
│   └── PaymentSystem.Api/
│       ├── Controllers/          # PaymentsController, ReportsController
│       ├── Models/                # Payment, CreatePaymentRequest, DailyVolumeRow
│       ├── Data/                  # SQL connection factory
│       ├── Services/              # PaymentService (mock auth + persistence)
│       └── Program.cs
└── frontend/
    └── payment-dashboard/
        └── src/
            ├── api/                # fetch client
            └── components/         # PaymentForm, PaymentTable, VolumeChart, Dashboard
```

## Getting started

### 1. Database

```bash
sqlcmd -S localhost -U sa -P <your-password> -Q "CREATE DATABASE PaymentSystemDb"
sqlcmd -S localhost -U sa -P <your-password> -d PaymentSystemDb -i database/01_create_schema.sql
sqlcmd -S localhost -U sa -P <your-password> -d PaymentSystemDb -i database/02_seed_data.sql
```

Update the connection string in `backend/PaymentSystem.Api/appsettings.json` to match your local SQL Server instance.

### 2. Backend

```bash
cd backend/PaymentSystem.Api
dotnet restore
dotnet run
```

API runs at `http://localhost:5142` by default. Swagger UI is available at `/swagger` in development.

### 3. Frontend

```bash
cd frontend/payment-dashboard
npm install
npm run dev
```

Dashboard runs at `http://localhost:5173`.

## API endpoints

| Method | Route                          | Description                              |
|--------|---------------------------------|-------------------------------------------|
| POST   | `/api/payments`                 | Submit a mock payment                    |
| GET    | `/api/payments?status=&take=`   | List recent transactions, optional filter |
| GET    | `/api/payments/{id}`            | Get a single transaction                 |
| GET    | `/api/reports/daily-volume`     | Daily volume + moving average report     |

## Notes

- Payment authorization is fully mocked — no real card data is processed or stored. Only the last 4 digits are kept, and the "authorization" outcome is randomized to simulate success/decline rates seen in real gateways.
- This is a portfolio / demo project and is not intended for production use as-is (no auth layer, no rate limiting, no PCI-scoped handling).

## License

MIT

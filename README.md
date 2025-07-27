# Rental Car API

This project is a RESTful API built with C# and ASP.NET Core for managing a rental car system. It allows users to view available cars, make reservations, and manage customer data.

## Features
- List available cars and filter by brand, model, or availability
- Customer registration and rental history tracking
- Create, update, and cancel reservations
- Validate rental periods and car availability
- Admin access for managing the car fleet

## Tech Stack
- C# (.NET 8)
- ASP.NET Core Web API
- Entity Framework Core (Code First)
- SQL Server / SQLite
- Swagger (API documentation)

## Endpoints Overview
| Method | Endpoint                  | Description                        |
|--------|---------------------------|------------------------------------|
| GET    | `/api/cars`               | Get list of available cars         |
| POST   | `/api/reservations`       | Create a new reservation           |
| DELETE | `/api/reservations/{id}`  | Cancel a reservation               |
| GET    | `/api/customers/{id}`     | Get customer and rental history    |

## Getting Started
1. Clone the repo
2. Update `appsettings.json` with your DB connection
3. Run migrations: `dotnet ef database update`
4. Run the project: `dotnet run`
5. Open Swagger at `https://localhost:xxxx/swagger`

## Future Improvements
- Authentication with JWT
- Role-based access (admin/customer)
- Payment integration (Stripe, PayPal)

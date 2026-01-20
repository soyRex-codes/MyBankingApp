# MyBankingApp
# Demo: https://www.youtube.com/watch?v=4wIPxtGRTtM

A modern, full-stack banking application built with **Clean Architecture**, **CQRS**, **.NET 10**, and **React + TypeScript**.

## Technology Stack

| Layer | Technology |
|-------|------------|
| **Backend Runtime** | .NET 10 |
| **Backend Framework** | ASP.NET Core Web API |
| **Architecture** | Clean Architecture + CQRS |
| **ORM** | Entity Framework Core |
| **Database** | SQLite (dev) / SQL Server (prod) |
| **Frontend** | React 19 + TypeScript + Vite |
| **Styling** | Tailwind CSS |
| **State Management** | React Context + TanStack Query |
| **Logging** | Serilog |
| **Testing** | xUnit + FluentAssertions + Moq |
| **API Docs** | Scalar (OpenAPI) |

## Project Structure

```
MyBankingApp/
├── src/
│   ├── MyBankingApp.Domain/         # Core business logic, entities, value objects
│   ├── MyBankingApp.Application/    # Use cases, CQRS commands/queries
│   ├── MyBankingApp.Infrastructure/ # EF Core, repositories, external services
│   └── MyBankingApp.API/            # Controllers, middleware, DI
├── client/                          # React frontend
│   ├── src/
│   │   ├── components/              # Reusable UI components
│   │   ├── pages/                   # Dashboard, Login, Register, Transfer
│   │   ├── context/                 # User authentication context
│   │   └── services/                # API client
│   └── package.json
└── tests/
    ├── MyBankingApp.Domain.Tests/
    ├── MyBankingApp.Application.Tests/
    └── MyBankingApp.API.Tests/
```

## Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)

## Run the API
```bash
cd MyBankingApp
dotnet restore
dotnet build
dotnet run --project src/MyBankingApp.API
```
#backend will be available on http://localhost:5114/scalar/v1

## Run the Client
```bash
to run client:
cd MyBankingApp/client
npm install && npm run dev
```
#client will be available on http://localhost:5173/


## View API Documentation
Navigate to: `http://localhost:5114/scalar/v1`

## Run Tests
```bash
dotnet test
```

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/accounts/{id}` | Get account details |
| POST | `/api/accounts` | Create new account |
| POST | `/api/accounts/{id}/deposit` | Deposit funds |
| POST | `/api/accounts/{id}/withdraw` | Withdraw funds |
| GET | `/api/accounts/{id}/transactions` | Get transaction history |
| POST | `/api/transactions/transfer` | Transfer funds |

## Key Features

### Clean Architecture
- **Domain Layer**: Entities, value objects, domain events, exceptions
- **Application Layer**: CQRS commands/queries, DTOs, validators
- **Infrastructure Layer**: EF Core, repositories, services
- **API Layer**: Controllers, middleware

### Rich Domain Model
```csharp
public class Account
{
    public Transaction Withdraw(Money amount)
    {
        if (amount > Balance)
            throw new InsufficientFundsException(Id, amount, Balance);
        
        Balance -= amount;
        AddDomainEvent(new FundsWithdrawnEvent(Id, amount));
        return transaction;
    }
}
```

### CQRS Pattern
- Commands for state changes: `CreateAccountCommand`, `DepositCommand`, `TransferCommand`
- Queries for reads: `GetAccountQuery`, `GetTransactionsQuery`

### Value Objects
```csharp
public record Money(decimal Amount, string Currency = "USD")
{
    public static Money operator +(Money a, Money b) => 
        new(a.Amount + b.Amount, a.Currency);
}
```

## Architecture Diagram

```
┌─────────────────┐     ┌──────────────────┐     ┌─────────────────┐
│   API Layer     │────▶│ Application Layer│────▶│  Domain Layer   │
│  (Controllers)  │     │ (CQRS Handlers)  │     │   (Entities)    │
└─────────────────┘     └──────────────────┘     └─────────────────┘
         │                       │
         │                       ▼
         │              ┌──────────────────┐
         └─────────────▶│Infrastructure    │
                        │(EF Core, Repos)  │
                        └──────────────────┘
```

## License

MIT

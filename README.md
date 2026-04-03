# ⛷️ Skinet — Ski Equipment E-Commerce API

A RESTful API built with **ASP.NET Core** for an online ski equipment store. Skinet provides a full e-commerce backend including product catalog management, user authentication, shopping cart, order processing, and payment integration.

---
- Live: [Skinet](http://skishop.runasp.net/)


## 🚀 Features

- Product catalog with categories and filtering
- User registration and authentication with JWT
- Shopping cart management
- Order creation and history
- Payment processing integration (Stripe)
- Admin product management
- Pagination, sorting, and searching
- Error handling and validation

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core |
| Language | C# |
| Database | SQL Server / PostgreSQL |
| ORM | Entity Framework Core |
| Caching | Redis |
| Authentication | JWT Bearer Tokens |
| Payments | Stripe API |
| API Docs | Swagger / OpenAPI |

---

## 📁 Project Structure

```
skinet/
├── API/                  # Controllers, middleware, extensions
├── Core/                 # Entities, interfaces, specifications
├── Infrastructure/       # Data access, repositories, services
├── skinet.sln            # Solution file
└── README.md
```

---

## ⚙️ Getting Started

### Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server) or [PostgreSQL](https://www.postgresql.org/)
- [Redis](https://redis.io/)
- [Stripe Account](https://stripe.com/) (for payments)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/muhmdramah/skinet.git
   cd skinet
   ```

2. **Set up configuration**

   Update `appsettings.json` or use user secrets:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "your-db-connection-string"
     },
     "Token": {
       "Key": "your-jwt-secret-key",
       "Issuer": "your-issuer"
     },
     "StripeSettings": {
       "PublishableKey": "your-stripe-publishable-key",
       "SecretKey": "your-stripe-secret-key"
     },
     "Redis": "localhost"
   }
   ```

3. **Apply database migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run --project API
   ```

5. **Access the API**
   ```
   https://localhost:5001/swagger
   ```

---

## 📬 API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/products` | Get all products |
| GET | `/api/products/{id}` | Get product by ID |
| POST | `/api/account/register` | Register a new user |
| POST | `/api/account/login` | Login and receive JWT |
| GET | `/api/basket` | Get shopping cart |
| POST | `/api/orders` | Create an order |
| POST | `/api/payments` | Create a payment intent |

> Full API documentation available via Swagger at `/swagger`

---

## 🧪 Running Tests

```bash
dotnet test
```

---

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/your-feature`)
3. Commit your changes (`git commit -m 'Add your feature'`)
4. Push to the branch (`git push origin feature/your-feature`)
5. Open a Pull Request

---

## 📄 License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

## 👤 Author

**Mohammed Rammah**
- GitHub: [@muhmdramah](https://github.com/muhmdramah)
- LinkedIn: [muhmdramah](https://linkedin.com/in/muhmdramah)

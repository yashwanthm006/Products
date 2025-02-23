# ProductAPI - ASP.NET Core REST API

This is a REST API for managing products with unique 6-digit IDs, using ASP.NET Core and EF Core

## 🚀 Features
✅ CRUD Operations for Products 
✅ Unique 6-digit Product ID generation (Supports Distributed Systems) 
✅ Stock Management (Increment & Decrement) 
✅ EF Core Code-First Migrations 
✅ Unit Tests with xUnit & Moq 


## 🛠️ Tech Stack
- **.NET 8**
- **Entity Framework Core**
- **SQL Server**
- **Moq & xUnit (Testing)**

## 🏗️ Prerequisites
- Install **.NET SDK** (https://dotnet.microsoft.com/)
- Install **SQL Server** or **Use Docker** for DB
- Install **Git** (https://git-scm.com/)

## 🔧 Setup & Run the API Locally
### **1️⃣ Clone the Repository**

git clone https://github.com/yashwanthm006/Products.git


### **2️⃣ Configure Database Connection**
Edit `appsettings.json`:
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=ProductDB;Trusted_Connection=True;TrustServerCertificate=True;"
}

### **3️⃣ Run Database Migrations**s
Select Default Project As ProductApi.Data
Then
In Package Manager Console
 1. Add-Migration "MigrationName"
 2. Update-Database

then run the unit tests and run the api

## API Endpoints

| Method | Endpoint                          | Description               |
|--------|----------------------------------|---------------------------|
| `POST` | `/api/products`                 | Create a new product      |
| `GET`  | `/api/products`                 | Get all products          |
| `GET`  | `/api/products/{id}`            | Get product by ID         |
| `PUT`  | `/api/products/{id}`            | Update product            |
| `DELETE` | `/api/products/{id}`          | Delete product            |
| `PUT` | `/api/products/decrement-stock/{id}/{quantity}` | Decrease stock |
| `PUT` | `/api/products/add-to-stock/{id}/{quantity}` | Increase stock |


## 🎯 Author
- **Yashwanth M**
- **Email:** yashuyashwanth004@gmail.com/myashwanthg004@gmail.com
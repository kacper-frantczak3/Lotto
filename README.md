# 🎲 Lotto Generator API & Web App

A modern, secure, and production-ready Web API built with **.NET** (Minimal API) that generates randomized lottery numbers (Lotto, Mini Lotto, Eurojackpot) and maintains a persistent history using **Entity Framework Core** and **SQLite**, paired with a clean, minimalist Apple-inspired frontend.

---

## 🚀 Tech Stack & Features
* **Backend:** .NET (C#) / Minimal API
* **Database & ORM:** SQLite with Entity Framework Core
* **Validation:** FluentValidation for input payload protection
* **Security:** Cryptographically secure random number generation (`RandomNumberGenerator`)
* **Testing:** Comprehensive unit tests written with **xUnit**
* **Frontend:** HTML5, Vanilla JavaScript, and Tailwind CSS (served as static files)

---

## ⚡ API Endpoints
* **`POST /api/lotto/generate`** – Generates randomized numbers, validates input, and saves the ticket to SQLite.
* **`GET /api/lotto/history`** – Retrieves the history of generated tickets.
* **`DELETE /api/lotto/history`** – Clears all saved ticket history from the database.

---

## 🛠️ How to Run Locally

### 1. Clone & Navigate
```bash
git clone [https://github.com/kacper-frantczak3/LottoGenerator.git](https://github.com/kacper-frantczak3/LottoGenerator.git)
cd LottoGenerator/LottoGenerator.Api

### 2. Run the Application
`dotnet run`

### 3. Run Unit Tests
Open a separate terminal in the root folder and run:
`dotnet test`

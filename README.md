# SENG_iproject3
Third Invididual Project - Backend with API and DB

## 🎯 Project Objective

This project is an **ASP.NET Core 10.0 Web API** designed to serve as the secure backend for video game data management. It implements a Clean Architecture pattern, utilizes Entity Framework Core for database interaction, and secures all sensitive endpoints using **JWT (JSON Web Tokens) Authentication**.

The API manages a persistent catalog of `FighterCharacter` entities, supporting full CRUD operations and secure user management.

## 🏗️ Architecture and Project Structure

The solution follows a standard Clean Architecture approach to ensure maintainability, testability, and separation of concerns.

| Project | Responsibility | Key Components |
| :--- | :--- | :--- |
| **`GameAPI.API`** | **Presentation Layer** (Entry Point) | Controllers (`FightersController`, `AuthController`), Program/Startup, DTOs, JWT Configuration. |
| **`GameAPI.Core`** | **Domain Layer** | Entity Models (`FighterCharacter`), Repository Interfaces (`IFighterRepository`). |
| **`GameAPI.Infrastructure`** | **Data Access Layer** | `GameDbContext`, EF Core Migrations, Repository Implementation (`FighterRepository`). |
| **`GameAPI.Tests`** | **Testing Layer** | Unit Tests for Controllers and Services (using NUnit and Moq). |

## 🚀 Getting Started

Follow these steps to set up and run the API locally using **Docker Compose**. This method provides a consistent environment with a dedicated SQL Server container.

### Prerequisites

1.  **.NET 10.0 SDK** (or newer)
2.  **Docker Desktop** (running)
3.  A code editor (Visual Studio 2024 or VS Code)

### Setup Instructions

1.  **Clone the Repository:**
    ```bash
    git clone [Your-Repo-URL]
    cd GameDataManagerAPI
    ```

2.  **Verify Configuration:**
    Ensure your local setup respects the environment variables defined in the `docker-compose.yml` file, especially the database connection settings.

3.  **Build and Run Containers:**
    Run this command from the solution root directory to build the API image and start both the API and the local SQL Server database container:

    ```bash
    docker compose up --build -d
    ```
    *Note: The API will be accessible at `http://localhost:5000`.*

4.  **Run EF Core Migrations (Crucial!):**
    After the containers are running, you must apply the database schema. This command connects to the running SQL Server container (`sqlserver_db`) and builds all necessary tables.

    ```bash
    # Ensure you run this from the solution root
    dotnet ef database update --project GameAPI.Infrastructure --startup-project GameAPI.API
    ```

## 🔐 API Documentation and Usage

The API includes Swagger documentation for interactive testing and is secured using JWT.

### Access Documentation

Open your browser and navigate to the Swagger UI:



> **[http://localhost:5000/swagger](http://localhost:5000/swagger)**

### Authentication Flow (Obtaining a JWT)

To access any secured endpoint (`/api/fighters`), you must first log in to receive a valid token.

1.  **Register a User (Optional):**
    * **Endpoint:** `POST /api/auth/register`
    * **Body:** `{"username": "testuser", "email": "user@example.com", "password": "Password123!"}`

2.  **Login to Get Token:**
    * **Endpoint:** `POST /api/auth/login`
    * **Body:** `{"email": "user@example.com", "password": "Password123!"}`
    * **Response:** Copy the `token` string from the successful response.

3.  **Authorize in Swagger:**
    * Click the **Authorize** button (the green padlock icon) in the Swagger UI.
    * Enter the token in the format: `Bearer [your_copied_token]`
    * Click **Authorize**.

### Core Endpoints (CRUD)

Once authorized, you can test the following endpoints:

| Method | Endpoint | Description | Security |
| :--- | :--- | :--- | :--- |
| **POST** | `/api/fighters` | Creates a new `FighterCharacter`. | **Secured** |
| **GET** | `/api/fighters` | Retrieves a list of all fighters. | **Secured** |
| **GET** | `/api/fighters/{id}` | Retrieves a single fighter by ID. | **Secured** |
| **PUT** | `/api/fighters/{id}` | Updates an existing fighter resource. | **Secured** |
| **DELETE** | `/api/fighters/{id}` | Deletes a fighter by ID. | **Secured** |

## 🧪 Running Unit Tests

The solution includes a dedicated `GameAPI.Tests` project. You can run all NUnit tests using the .NET CLI:

```bash
dotnet test
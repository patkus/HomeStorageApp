# HomeStorageApp (MVP)

A mobile application for managing a home medicine cabinet, designed to track medications, expiration dates, locations, and dosages within a single household.

## Table of Contents

- [Project Description](#project-description)
- [Tech Stack](#tech-stack)
- [Getting Started Locally](#getting-started-locally)
    - [Prerequisites](#prerequisites)
    - [Backend Setup](#backend-setup)
    - [Frontend Setup](#frontend-setup)
- [Available Scripts](#available-scripts)
- [Project Scope](#project-scope)
    - [Key Features (MVP)](#key-features-mvp)
    - [Out of Scope (MVP)](#out-of-scope-mvp)
- [Project Status](#project-status)
- [License](#license)

## Project Description

HomeStorageApp is a mobile solution designed to solve the common problems of managing a home pharmacy. Users often struggle with tracking expiration dates (leading to waste and risk), locating specific medicines, remembering dosage schedules for different household members, and knowing the current stock levels.

This application centralizes all medication information, helps reduce waste through timely alerts, and improves safety and regularity with reminders for doses and expirations.

## Tech Stack

The project is built using the .NET ecosystem for both backend and frontend components.

### Backend
* **Architecture:** Modular Monolith, Domain-Driven Design (DDD), Vertical Slices, REST API
* **Technology:** .NET 9 Web Api, Minimal Api
* **Libraries:** Entity Framework Core (EF Core)
* **Persistence:** PostgreSQL

### Frontend
* **Technology:** .NET MAUI

## Getting Started Locally

Follow these instructions to get a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

* [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
* [.NET MAUI Workload](https://learn.microsoft.com/en-us/dotnet/maui/get-started/installation)
* [PostgreSQL](https://www.postgresql.org/download/) (or a Docker instance)
* A code editor (e.g., Visual Studio 2022, VS Code)

### Backend Setup

1.  **Clone the repository:**
    ```sh
    git clone [https://github.com/your-username/homestorageapp.git](https://github.com/your-username/homestorageapp.git)
    cd homestorageapp/backend
    ```

2.  **Configure application settings:**
    * Rename or create `appsettings.Development.json`.
    * Add your PostgreSQL connection string:
    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Host=localhost;Database=HomeStorageDb;Username=your_user;Password=your_password"
      }
    }
    ```

3.  **Apply database migrations:**
    * Ensure the EF Core CLI tools are installed (`dotnet tool install --global dotnet-ef`).
    * Run the following command from the project directory containing the `DbContext`:
    ```sh
    dotnet ef database update
    ```

4.  **Run the backend:**
    ```sh
    dotnet run
    ```
    The API should now be running, typically on `https://localhost:7001` or `http://localhost:5001`.

### Frontend Setup

1.  **Navigate to the frontend project:**
    ```sh
    cd ../frontend
    ```

2.  **Restore dependencies:**
    ```sh
    dotnet restore
    ```

3.  **Run the application:**
    * You can run the .NET MAUI application by selecting your target (e.g., Android Emulator, Windows Machine) from your IDE (like Visual Studio).
    * Alternatively, using the CLI:
    ```sh
    # Example for running on Windows
    dotnet build -t:Run -f net9.0-windows10.0.19041.0

    # Example for running on Android
    dotnet build -t:Run -f net9.0-android
    ```

## Available Scripts

This project uses standard `dotnet` CLI commands.

* **Run Backend:** `dotnet run` (from the backend project directory)
* **Run Frontend:** `dotnet build -t:Run -f <TARGET_FRAMEWORK>` (from the frontend project directory)
* **Add Migration:** `dotnet ef migrations add <MigrationName>`
* **Update Database:** `dotnet ef database update`
* **Run Tests:** `dotnet test`

## Project Scope

This section outlines the functional scope of the Minimum Viable Product (MVP).

### Key Features (MVP)

* **Authentication:** Simple email/password authentication for a single "Head of Household" account.
* **Drug Management:** Full CRUD for drug definitions, including archiving.
* **Unit Management:** Ability to define a *main unit* (e.g., 'tablet', 'ml') for a drug and multiple *derived units* (e.g., 'blister', 'box') with conversion rates (e.g., 1 blister = 10 tablets).
* **Batch Tracking:** All inventory is managed in *batches* (e.g., individual packages), each with its own expiration date and quantity.
* **Location Management:** Create and manage an unlimited hierarchical (tree) structure for storage locations (e.g., Cabinet -> Shelf -> Box).
* **Location Identification:** Assign a unique text "symbol" (e.g., "BATH_CAB") to a location, which can be used to generate an app-specific QR code for quick scanning.
* **Household Profiles:** Create simple profiles for household members (e.g., "Tomek") to assign dosage schedules.
* **Dosing Schedules:** Define simple schedules (daily, multiple times per day, specific weekdays) and link them to a member and a drug.
* **Stock Management:** Record consumption (which auto-reduces stock from the batch with the nearest expiration date), manual adjustments, and disposal.
* **Barcode Scanning:**
    * Link a barcode/QR code to a drug definition.
    * Scan an *existing* drug's barcode to automatically add a new batch (in its default purchase unit) to the inventory.
* **Notifications & Dashboard:**
    * Push notifications for upcoming doses and expiration dates (at 30, 7, and 0 days).
    * A main dashboard showing upcoming doses, expiration alerts, and quick actions.
* **Inventory (Stock-Take):** A feature to select a location, view all expected items, and manually confirm or correct their quantities.
* **Technical:** The app requires an internet connection for all write operations (add, edit, consume). Data is cached for offline reading.

### Out of Scope (MVP)

* Multi-household accounts or data separation.
* Invitation or household sharing systems.
* Integration with external/public drug databases.
* User-configurable alert timings (hard-coded to 30/7/0 days).
* Advanced permissions or roles (beyond the single "Household Head").
* Full offline mode with write capabilities and conflict synchronization.
* Advanced reporting or data export.

## Project Status

**Status:** In Development (MVP)

This project is currently in the Minimum Viable Product (MVP) development phase. The features listed in the scope are the primary focus.

## License

This project is licensed under the MIT License. See the `LICENSE` file for details.
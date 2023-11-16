# MPFT (Med Tracker) Medication Tracking System

## Introduction

The Medication Tracking System is a web application designed to help healthcare providers and patients manage medication information efficiently. This README provides an overview of the project structure, key components, and instructions on how to run the application.

## Table of Contents

- [Technologies Used](#technologies-used)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Usage](#usage)
- [Contributing](#contributing)

## Technologies Used

- **ASP.NET MVC**: The web framework used for developing the application.
- **Entity Framework**: ORM (Object-Relational Mapping) tool for database operations.
- **Hangfire**: Used for background job processing, particularly for scheduling medication notifications.
- **SMTP (Simple Mail Transfer Protocol)**: Used for sending confirmation emails during user registration.
- **Bootstrap**: Front-end framework for designing responsive and mobile-first websites.

## Project Structure

- **Models**: Contains the data models used in the application (e.g., `Patient`, `Medication`, `Provider`).
- **Controllers**: Controllers for handling user requests and interacting with the database.
- **Views**: User interface templates and layouts.
- **AuthorizeUserAccess Attribute**: Custom attribute for user authorization.
- **BackgroundTaskManager**: Class for scheduling background tasks using Hangfire.
- **MyDbContext**: Database context class for Entity Framework.

## Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) installed.
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) for database operations.

### Installation

1. Clone the repository
2. Navigate to the project directory
3. Restore dependencies: `dotnet restore`

## Configuration

### Database Connection

1. Open `Web.config`.
2. Update the connection string in the `DefaultConnection` section.

### SMTP Configuration

1. Open `PatientController.cs`.
2. Update SMTP client settings in the `SendConfirmationEmail` method.

## Usage

1. Build the application: `dotnet build`
2. Run migrations
3. Run the application: `dotnet run`

The application will be accessible at `https://localhost:44350` (or another specified port).




---

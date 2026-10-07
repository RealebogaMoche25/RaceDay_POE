# RaceDay

## Project Overview

RaceDay is a web-based event management system designed for the South African road running, walking, and cycling community.

The system is designed to allow event organisers to manage sporting events, event categories, participant enrolments, routes, and race results. Participants can create accounts, view available events, enrol in events, manage their profiles, and view their personal race results.

The project is being developed progressively using ASP.NET Core, C#, Entity Framework Core, SQL Server, RESTful APIs, and GitHub.

The RaceDay system consists of two main user roles:

### Organiser

The Organiser is responsible for managing sporting events. Organisers can:

* Create and manage events.
* Update and delete their own events.
* Create and manage categories for their own events.
* Create and manage routes for their own events.
* View participants enrolled in their events.
* Record participant race results.
* Update results for their events.
* View results for their events.

### Participant

The Participant is a user who takes part in sporting events. Participants can:

* Register and log into the system.
* Manage their own profile.
* View available events.
* View event details and categories.
* View available routes.
* Enrol in events.
* View their own enrolments.
* View their personal race results.

## Part 1 – Planning and Database Design

Part 1 focuses on the planning and design of the RaceDay system.

The project documentation includes:

* Entity Relationship Diagram (ERD)
* Database design
* SQL database script
* API Endpoint Plan
* Project documentation

All required Part 1 documentation is stored in the Docs folder of this repository.

### Part 1 Documentation

The following files are included:

* Docs/erdprog.drawio.png – RaceDay Entity Relationship Diagram
* Docs/PROG6212 PART 1.pdf – Part 1 planning and project documentation
* Docs/RACEDAY_POE.sql – RaceDay SQL database script

## Database Design

The RaceDay database is designed to support users, events, event categories, participant enrolments, routes, and race results.

The main entities include:

* USER – Stores system user information and user roles.
* EVENT – Stores information about sporting events.
* ROUTE – Stores route information associated with events.
* CATEGORY – Stores participation categories for events.
* ENROLMENT – Records participants enrolled in events.
* RESULT – Stores participant finishing times and finishing positions.

The relationships between these entities are represented in the RaceDay ERD.

The SQL database script is provided in:
Docs/RACEDAY_POE.sql

## API Endpoint Plan

The Part 1 API Endpoint Plan defines the RESTful API that will be developed for RaceDay.

The plan identifies:

* HTTP method
* API route
* Endpoint purpose
* Required user role
* Request body
* Expected response and HTTP status codes

The API includes endpoints covering:

* Authentication
* User profiles
* Events
* Event categories
* Participant enrolments
* Race routes
* Race results

The complete API Endpoint Plan is included in:
Docs/PROG6212 PART 1.pdf

## Repository Structure

The repository is organised to keep the project documentation, Part 2 projects, automated tests, and GitHub Actions workflow clearly separated.

* RaceDay_POE/
* │
* ├── .github/
* │   └── workflows/
* │       └── repository-check.yml
* │
* ├── Docs/
* │   ├── erdprog.drawio.png
* │   ├── PROG6212 PART 1.pdf
* │   └── RACEDAY_POE.sql
* │
* ├── PART2/
* │   ├── RACEDAY_API/
* │   └── RACEDAY_API.API.Test/
* │
* ├── README.md
* └── RaceDay_POE.slnx

The .github/workflows directory contains the GitHub Actions workflow used to validate, build, and test the RaceDay project.

The Docs directory contains the required Part 1 documentation.

The PART2 directory contains the RaceDay ASP.NET Core Web API and automated unit testing project.

## Setup and Run Instructions

### Prerequisites

The following software is required to run the RaceDay project:

* Visual Studio
* .NET 8 SDK
* SQL Server
* SQL Server Management Studio (SSMS)
* Git, if cloning the repository

### 1. Clone or Download the Repository

Clone the RaceDay repository from GitHub or download the repository as a ZIP file.

### 2. Open the Solution

Open the following solution in Visual Studio:

RaceDay_POE.slnx

The solution contains:

* RACEDAY_API – ASP.NET Core Web API project.
* RACEDAY_API.API.Test – Automated unit testing project.

The RACEDAY_API project is the startup project.

### 3. Set Up the Database

Open SQL Server Management Studio (SSMS).

Create or use the SQL Server instance configured for the project.

The RaceDay Part 2 database is named:

RACEDAY_POE_PART2

The database connection is configured in:

PART2/RACEDAY_API/appsettings.json

The project uses SQL Server with a local Windows/Trusted connection.

The Part 1 SQL database script is available in the Docs folder.

### 4. Restore NuGet Packages

When the solution is opened in Visual Studio, restore the required NuGet packages.

The API uses packages including:

* Microsoft.EntityFrameworkCore.SqlServer
* Microsoft.EntityFrameworkCore.Tools
* Microsoft.EntityFrameworkCore.Design
* BCrypt.Net-Next
* Swashbuckle.AspNetCore

### 5. Build the Project

Build the solution in Visual Studio using:

Build → Build Solution

The solution should build successfully.

### 6. Run the API

Set RACEDAY_API as the startup project and run the project using Visual Studio.

The API will open in the browser with Swagger.

Swagger can be used to view and test the available RaceDay API endpoints.

### 7. Run the Automated Tests

The automated tests are located in:

PART2/RACEDAY_API.API.Test

The tests can be run using:

Test → Test Explorer → Run All Tests

The automated tests verify functionality including:

* User registration.
* Duplicate email registration.
* Successful login.
* Invalid login credentials.
* Organiser event creation.
* Event role authorization.
* Participant enrolment.
* Enrolment role authorization.

All eight automated tests pass successfully.

## Project Roles and Authorization

The RaceDay API uses role-based authorization to control access to protected functionality.

### Organiser

Organisers can:

* Create events.
* Update and delete their own events.
* Create, update, and delete categories belonging to their events.
* Create, update, and delete routes belonging to their events.
* View participants enrolled in their events.
* Create and update race results for their events.
* View race results for their events.

Organisers cannot modify resources belonging to another organiser.

### Participant

Participants can:

* Register and log into the system.
* View available events.
* View event details and categories.
* View available routes.
* Manage their own profile.
* Enrol in events.
* View their own enrolments.
* View their own race results.

Participants cannot create or manage events, categories, routes, or race results.

## Automated Testing

The RaceDay API includes automated unit tests using xUnit and Entity Framework Core InMemory.

The tests use isolated in-memory databases so that individual tests do not depend on the SQL Server database.

The automated test suite currently contains eight tests covering:

* Registration success.
* Duplicate email registration.
* Successful login.
* Incorrect password login failure.
* Organiser event creation.
* Event authorization.
* Participant enrolment.
* Enrolment authorization.

All eight automated tests pass successfully.

## GitHub and Version Control

GitHub is used to manage the RaceDay project and track the development process.

The repository contains meaningful commits documenting the development and progression of the project.

Version control is used to:

* Track changes to the project.
* Maintain the project history.
* Store the required documentation.
* Manage the GitHub Actions workflow.
* Provide evidence of the development process.

## CI/CD and GitHub Actions

GitHub Actions is used to automatically validate, build, and test the RaceDay project.

The workflow is located at:

.github/workflows/repository-check.yml

The workflow runs automatically when changes are pushed to the repository or when a pull request is created.

The workflow:

* Checks out the repository.
* Sets up .NET 8.
* Validates the required repository structure and documentation.
* Builds the RaceDay solution in Release configuration.
* Runs the automated unit tests.

### Successful Green Build

A successful green build was completed using GitHub Actions.

The screenshot below provides evidence of the successful build and automated test execution.

**[INSERT NEW GREEN GITHUB ACTIONS BUILD SCREENSHOT HERE]**

The successful green build confirms that the RaceDay solution builds successfully and that the automated tests pass in GitHub Actions.

## Project Demonstration Video

Demonstration videos are provided for both Part 1 and Part 2 of the RaceDay project.

### Part 1 YouTube Video

https://youtu.be/PDVLrLdDqpQ?feature=shared

The Part 1 video demonstrates the project documentation, database design, API Endpoint Plan, GitHub repository structure, version control, and GitHub Actions workflow.

### Part 2 YouTube Video

**[INSERT PART 2 UNLISTED YOUTUBE VIDEO LINK HERE]**

The Part 2 video demonstrates the RaceDay ASP.NET Core Web API, authentication and authorization, organiser and participant functionality, API testing, automated unit tests, and the successful GitHub Actions build.

## Project Status

### Part 1

Part 1 includes:

* Project planning
* System requirements
* User roles
* Database design
* Entity Relationship Diagram
* SQL database script
* API Endpoint Plan
* GitHub repository
* Version control
* GitHub Actions repository validation
* Project documentation
* Demonstration video

### Part 2

Part 2 includes:

* ASP.NET Core Web API
* Entity Framework Core
* SQL Server database integration
* User registration and login
* Cookie authentication
* Role-based authorization
* Organiser functionality
* Participant functionality
* Event management
* Category management
* Route management
* Participant enrolments
* Race results
* Swagger API documentation
* Automated unit tests
* GitHub Actions continuous integration


### Future Development
The planned later stages of RaceDay will involve implementing the RESTful API and developing the MVC front-end.
The API implementation will build on the endpoint plan created during Part 1.



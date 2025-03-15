Employee Chat Application
Overview
The Employee Chat Application is a web-based platform designed to facilitate communication and collaboration among employees within an organization. This application leverages ASP.NET Core Razor Pages for its front-end and back-end, providing a robust and scalable solution for internal communication.
Features
•	User Registration and Authentication: Users can register, log in, and log out securely.
•	Role Management: Admins can manage user roles, ensuring appropriate access control.
•	Email Confirmation: Users must confirm their email addresses to activate their accounts.
•	Password Management: Users can reset their passwords if they forget them.
•	Real-Time Chat: Employees can communicate in real-time using SignalR.
•	User and Role Management: Admins can view and edit user roles.

Technologies Used
•	ASP.NET Core 9.0: The primary framework for building the web application.
•	Razor Pages: Used for building the user interface.
•	Entity Framework Core: An ORM (Object-Relational Mapper) for database operations.
•	SQL Server: The database management system used to store application data.
•	Identity Framework: Provides authentication and authorization features.
•	SignalR: Used for real-time web functionality, such as chat.
•	Azure SignalR Service: A managed service for adding real-time functionality to applications.
•	Bootstrap: A front-end framework for responsive web design.
•	Microsoft.Extensions.DependencyInjection: For dependency injection.
•	Microsoft.AspNetCore.Identity.UI.Services: For email services.
•	Microsoft.EntityFrameworkCore: For database context and migrations.
•	Microsoft.AspNetCore.SignalR: For SignalR hubs.
•	Microsoft.Azure.SignalR: For integrating Azure SignalR Service.
•	C# 13.0: The programming language used for the application.

Project Structure
The project is structured as follows:
•	Controllers: Contains the AccountController which handles user authentication, registration, and role management.
•	Models: Contains the data models used throughout the application, such as RegisterModel, LoginModel, ForgotPasswordModel, etc.
•	Views: Contains the Razor views for the various pages in the application.
•	Data: Contains the database context and migration files.
•	Hubs: Contains the SignalR hubs for real-time communication.
Getting Started
Prerequisites
•	.NET 9.0 SDK
•	SQL Server

Installation
1.	Clone the repository:
       git clone https://github.com/kipkiruiedmond/Employee-Chat/employee-chat.git
    cd employee-chat
2.	Update the database connection string in appsettings.json:
	    "ConnectionStrings": {
        "DefaultConnection": "Server=your_server;Database=EmployeeChat;Trusted_Connection=True;MultipleActiveResultSets=true"
    }
3.	Apply database migrations:
       dotnet ef database update
4.	Run the application:
       dotnet run
Usage
•	Navigate to http://localhost:5000 to access the application.
•	Register a new user and confirm the email.
•	Log in with the registered user credentials.
•	Admin users can manage roles and users through the admin panel.
Contributing
Contributions are welcome! Please fork the repository and submit a pull request for any enhancements or bug fixes.

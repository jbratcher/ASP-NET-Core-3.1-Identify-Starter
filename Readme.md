# Documentation  

This document shows the development process and steps required to create an application with the following features:  
* User authentication
* User profile with custom fields (occupation, industry, home location)


## Overview  

A basic setup for an ASP.NET Core 3 MVC app with pre-configured user authentication via Identity.

[Github Repo](https://github.com/jbratcher/)

## Getting Started  

```
git clone https://github.com/jbratcher/
cd GoThere
```

## Buidling from scratch

Outlined below is the development process used to create this app.

Dependencies:

* EF Core 3.1
* EF Core SQL Server 3.1
* ASP. NET Core Identity 3.1

### How to get the app up and running

* Using Visual Studio, from the start screen, select Create a new project. 
* From the Create a new project screen, select ASP.NET Core Web Application and click Next.
* From the Conigure project screen, add a name for the project and click Create.
* From the Create a new ASP.NET Core Web Application screen, select Wep Application (Model-View-Controller) 
* Under Authentication, choose Change => select Individual User Account and click OK
* Click Create

This will create a basic MVC app with user authentication The application with be created and opened in Visual Studio.

### Install Entity Core Framework  

Tools => NuGet Package Manager => Package Manager Console  
`Install-Package Microsoft.EntityFrameworkCore.SqlServer` 

This project uses Microsoft SQL Server.  SQLite is the default.  Others are supported.

### Add Local User Authentication

This project uses local users and the ASP.NET Core Identity module for user authentication.

[ASP.NET Core Identity Module](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/add-user-data?view=aspnetcore-3.1&tabs=visual-studio#add-custom-user-data-to-the-identity-db)

#### Scaffold Identity

* From Solution Explorer, right-click on the project => Add => New Scaffolded Item.
* From the left pane of the Add Scaffold dialog, select Identity > Add.
* In the Add Identity dialog, the following options:  
    * Select the existing layout file ~/Pages/Shared/_Layout.cshtml
    * Select the ApplicationDBContext for the context
    * Select Add.
* Manually create Data folder in Areas/Identity and add User class (GoThereUser.cs)
* From the Package Manager Console, add a migration and update the database

#### Update Scaffolded items to use custom user class

###### Several items must be updated to get the custom user class and fields working after the initial scaffolding

##### DB Context

In ApplicationDBContext.cs, create a DBSet of Users (list of users):
```
public DbSet<GoThereUser> GoThereUsers { get; set; }
```

##### Startup.Configure and Startup.ConfigureServices

In Startup.Configure Services, add/replace the current AddIdentity method with the following:
```
services.AddIdentity<GoThereUser, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDbContext>();
```

From `Startup.Configue`, set standard practice user authentication options including password strength requirements, validation, lockouts, and cookies:
```
services.Configure<IdentityOptions>(options =>
    {
        // Password settings.
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 6;
        options.Password.RequiredUniqueChars = 1;

        // Lockout settings.
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;

        // User settings.
        options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        options.User.RequireUniqueEmail = false;
    });

    services.ConfigureApplicationCookie(options =>
    {
        // Cookie settings
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

        options.LoginPath = "/Identity/Account/Login";
        options.AccessDeniedPath = "/Identity/Account/AccessDenied";
        options.SlidingExpiration = true;
    });
```

##### Views

###### Login Partial View

* In Shared/_LoginPartial.cshtml:
```
@using GoThere.Areas.Identity.Data;
@inject SignInManager<GoThereUser> SignInManager
@inject UserManager<GoThereUser> UserManager
```
This global shared partial view, requires the sign in and user manager services from the custom user class

###### Login, Register, Logout Views  

* In the Login, Register, and Logout Models (ex. login.cshtml.cs), update the generic identity user class to the custom user class.  

###### Manage Navigation page

* In the _ManageNav page, update the user class to the custom user class.  
###### Account/Manage view

In Identity/Pages/Account/Manage/Index.cshtml.cs:  
* add local variables with default get/set to the class for each custom field
```
public string Username { get; set; }
public string FirstName { get; set; }
public string LastName { get; set; }
public string Occupation { get; set; }
public string Industry { get; set; }
public string PostalCode { get; set; }
```

* Set the input model fields to display the user's current info for editing
```
Input = new InputModel
{
    PhoneNumber = phoneNumber,
    FirstName = user.FirstName,
    LastName = user.LastName,
    Occupation = user.Occupation,
    Industry = user.Industry,
    PostalCode = user.PostalCode,
};
```

Add the new fields as input to the page's form to allowing editing and submission.

#### Create a model  

[Create a model](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/adding-model?view=aspnetcore-3.1&tabs=visual-studio) 

#### Create scaffold CRUD pages with EFCore  

Add new controller => MVC EFCore with Pages => enter model => enter context => check generate pages

In Shared/_Layout.cshtml, add a link to the new controller

####  Migration (sync model with database)  

From the NuGet Package Manager Console: 
`Add-Migration InitialCreate`  // The name [InitialCreate] can be whatever
`Update-Database`  

At this point the application should be minimally functional allowing user actions and CRUD operations for the primary model (Location)

#### Add a new field

* Navigate to the Model.cs file of the model that the new field will be added to  
* Inside the model class, add a variable with the type and name of the new field  
* Attributes can be added like [Display()], [RegularExpression()], or [Required] by prepending the new field variable
* Update any views that used the field
* Add a migration
* Update database

#### Form validation

* Form validation is provdied by the associated model's field attributes
* [RegularExpression()], [Required], and [DataType()] are commonly used

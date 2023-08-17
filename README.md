# HR_Magnament

### A simple  project that simulates a HR Management System application using a set of technologies that are used in our group’s supported systems.  a simple sample project that simulates a HR Management System application using a set of technologies that are used in our group’s supported systems.  

How run the project
#### Create a Migration: Run the dotnet ef migrations add command followed by the migration name. Replace YourMigrationName with a descriptive name for your migration:
```
dotnet ef migrations add YourMigrationName
```

### Apply the Migration: 
To apply the migration and update the database schema, run the following command:
```
dotnet ef database update
```

### Run the Project: 
Execute the following command to run ASP.NET Core project:
```
dotnet run
```

After executing the command, the ASP.NET Core application will start and listen on a specific port (usually 5000 or 5001). Open your web browser and navigate to http://localhost:5000 or https://localhost:5001 to access the application.

If you want to specify a different port, you can do so by using the `--urls` option. For example:
```
dotnet run --urls=http://localhost:8080
```

Go to:
```
https://localhost:{ip-specify}/swagger/index.html
```

To view the endpoint documentation
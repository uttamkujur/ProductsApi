This project contains Api to support CRUD operations for Products.

For code first migration approach.
Ensure no Database name ProductDb in present. 
Step 1 : open package manager console and run the command 
	 "dotnet ef migrations add InitialCreate --project Products.Infrastructure --startup-project Products.Api".
Step 2: 
  "run command "dotnet ef database update --startup-project Products.Api --project Products.Infrastructure" .

  

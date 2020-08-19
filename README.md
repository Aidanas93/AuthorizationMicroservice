# 1. Prerequisite

- .Net Core SDK 3.x
- DotNet runtime 3.x
- AspNet (AspDotNet) runtime 3.x
- Any environmental to run .Net Core SDK 3.x (e.g. Visual Studio 2019, Visual Code ect..)
- MongoDB 4.2.x

# 2. Before you start

Helpful articles
- [SOLID](https://docs.microsoft.com/en-us/archive/msdn-magazine/2014/may/csharp-best-practices-dangers-of-violating-solid-principles-in-csharp)
- [Onion Architecture](https://www.c-sharpcorner.com/article/onion-architecture-in-asp-net-core-mvc/)
- [Restful API](https://docs.microsoft.com/en-us/aspnet/web-api/overview/older-versions/build-restful-apis-with-aspnet-web-api)
- [Repositories and Unit of Work](https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application)
- [CQRS](https://medium.com/@ducmeit/net-core-using-cqrs-pattern-with-mediatr-part-1-55557e90931b)
- [Mongo Database](https://www.mongodb.com/blog/post/quick-start-c-sharp-and-mongodb-starting-and-setup)
- [Jwt Tokens](https://www.c-sharpcorner.com/article/jwt-json-web-token-authentication-in-asp-net-core/)

# 3. Running the projcet
 
- Clone this repository
- Build the solution using Visual Studio, or on the command line with **dotnet build**.
- Run the AuthorizationMicroservice.API project. The API will start up on http://localhost:3002 with **dotnet run**.
- Use an HTTP client like Postman or Fiddler to GET http://localhost:3002.

# 4. Architecture overview

AuthorizationMicroservice project uses onion architecture [more about here](https://www.codeguru.com/csharp/csharp/cs_misc/designtechniques/understanding-onion-architecture.html#:~:text=Onion%20Architecture%20is%20based%20on,on%20the%20actual%20domain%20models.). 
Project structure:


![Untitled](https://res.cloudinary.com/practicaldev/image/fetch/s--sWdyI1q4--/c_limit%2Cf_auto%2Cfl_progressive%2Cq_auto%2Cw_880/https://dev-to-uploads.s3.amazonaws.com/i/dhti2v0e1smn055tages.png)

Components within the layered architecture pattern are organized into layers, each layer performing a specific role within the application (e.g., **business logic**). Although the layered architecture pattern does not specify the number and types of layers that must exist in the pattern, most layered architectures consist of four standard layers: presentation (**API**), business(**APPLICATION**), persistence, and database(**DOMAIN**). In some cases, the business layer and persistence layer are combined into a single business layer, particularly when the persistence logic (e.g., **SQL** or **NoSQL**) is embedded within the business layer components. Thus, smaller applications may have only three layers, whereas larger and more complex business applications may contain five or more layers.

One of the powerful features of the layered architecture pattern is the separation of concerns among components. Components within a specific layer deal only with logic that pertains to that layer. For example, components in the presentation layer deal only with presentation logic, whereas components residing in the business layer deal only with business logic. This type of component classification makes it easy to build effective roles and responsibility models into your architecture, and also makes it easy to develop, test, govern, and maintain applications using this architecture pattern due to well-defined component interfaces and limited component scope.

## API layer

This layer contains all configuration, startup, controller files. In controllers AuthorizationMicroservice uses CQRS.
**CQRS** is an architectural **pattern**. CQRS pattern states that the data read operation and write operation should be separated. This makes our controllers clean and easy to maintain because all required logic is handled by **Queries**, **Commands** and **Handlers** these classes are located in the **Application layer**. Controller e.g.:
```csharp
 public async Task<ActionResult<AccessTokenDto>> Login([FromBody] Login.Command command)
 {
     return await unitOfWork.Mediator.Send(command);
 }
```
As you can see request body information is mapped to the `Login.Command` class and it is passed to the appropriate handler by using `Send` command.

## Application layer

**Business Logic Layer (BLL)**: Application processing. Coordinates data between the API and DAL. In general, this code will likely fall into one of the following categories:

-   Processing commands and queries
-   Coordinating workflow
-   Maintaining application state
-   Accessing data (from the DAL)
-   Making logical decisions
-   Performing calculations
-   Persisting data (to the DAL)
-   Sending data (to the API layer)

This layers contains **CryptographyService**, **Dto**, **HintService**, **Intrastructure**, **MediatorNotifications**, **UserService**.

This layer handles main logic behind CQRS pattern. Each controller has its own handler for example Login controller is handled by the `UserService.Login` class and these classes consist of 2 nested classes. If request is safe (**HTTP GET**) then we create **Query** and **Handler** classes, if request is not safe (HTTP POST, PUT, DELETE) **Command** and **Handler** is created. To help implement CQRS application layer uses [Mediatr](https://dotnetcoretutorials.com/2019/04/30/the-mediator-pattern-part-3-mediatr-library/) library. Query or Command contains request properties and Handler handles these properties.

```csharp
public class Login
    {
        public class Command : IRequest<RETURNTYPE>
        {
            public string Property{ get; set; }
            public string Property{ get; set; }
        }
        
        public class Handler : IRequestHandler<Command, RETURNTYPE>
        {
            public async Task<RETURNTYPE> Handle(Command request, CancellationToken cancellationToken)
            {

            }
        }
    }
```

To get all dependencies from one place we use UnitOfWork pattern described [here](https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application) 

## Persistance layer

Persistence layer will perform as CRUD (Create, Read, Update, Delete) operations. This layer contains **connection to the database** and our **repositories**.

For **database** we use **MongoDb**. MongoDB is a cross-platform document-oriented database program. Classified as a NoSQL database program, MongoDB uses JSON-like documents. 

```csharp
    public class ApplicationDbContext<T> : IApplicationDbContext<T> where T : class
    {
        private readonly IMongoDatabase _database = null;
        private readonly string _table;
        public ApplicationDbContext(IOptions<Settings> settings)
        {
            var client = new MongoClient();
            _table = settings.Value.Table;
            if (client != null)
                _database = client.GetDatabase("AuthorizationMicroservice");
        }

        public IMongoCollection<T> Collection
        {
            get
            {
                return _database.GetCollection<T>(_table);
            }
        }
    }
```


Two main properties need to be initialized in order to run the database. 

## Domain

The **domain layer** is a collection of entity objects and related business logic that is designed to represent the enterprise business model.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Products.Application;
using Products.Domain;
using Products.Domain.Repositories;
using Products.Infrastructure;
using Products.Infrastructure.Repositories;


namespace Products.Api;


public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }


    public IConfiguration Configuration { get; }


    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddDbContext<ProductDbContext>(options =>
            options.UseSqlServer(Configuration["ConnectionStrings:ProductDbConnection"],
            b => b.MigrationsAssembly("Products.Infrastructure")));
        Console.WriteLine("Connection----------------------------: " + Configuration.GetConnectionString("ProductDbConnection"));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductService, ProductService>();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }


    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        //app.UseMiddleware<ExceptionMiddleware>();




        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Products API V1");
                c.RoutePrefix = ""; // Makes Swagger UI available at the root URL
            });
        }


        app.UseRouting();
        //app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
};










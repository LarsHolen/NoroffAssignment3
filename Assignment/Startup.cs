using Assignment.Models;
using Assignment.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;


namespace Assignment
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            // Adding Automapper
            services.AddAutoMapper(typeof(Startup));
            // Setting up DBcontext, giving it connectionstring from appsettings.json
            services.AddDbContext<AssignmentDbContext>(opt =>
            opt.UseSqlServer(Configuration.GetConnectionString("AzureConnection"))); // DefaultConnection or AzureConnection
            // Adding my IServices(Character, Franchise and Movie)
            services.AddScoped(typeof(ICharacterService), typeof(CharacterService));
            services.AddScoped(typeof(IFranchiseService), typeof(FranchiseService));
            services.AddScoped(typeof(IMovieService), typeof(MovieService));
            // Adding Docs
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { 
                    Title = "Assignment", 
                    Version = "v1",
                    Description = "An assignment from Noroff Accelerate Backend with .Net.  Setting up EF core, automapper, swagger docs and so on.",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Lars Holen",
                        Email = "larsholen6@hotmail.com",
                        Url = new Uri("https://larsholen.com/"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under MIT",
                        Url = new Uri("https://opensource.org/licenses/MIT"),
                    }

                });
                // Set the comments path for the Swagger JSON and UI. Remeber to edit project file!
                // no warn for missing docs and  <GenerateDocumentationFile>true..
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
            }

            // Moving these two lines outside the env.IsDevelopment, to get the docs in production
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Assignment v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

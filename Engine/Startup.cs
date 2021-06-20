using Engine.DAL.Contexts;
using Engine.DAL.Repositories.Users;
using Engine.DAL.Repositories.Logs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Engine.DAL.Repositories.Companies;
using System;
using Engine.Services.WorkingServices.Users;
using Engine.Services.WorkingServices.Companies;
using Engine.Services.LogsServices.Logs;
using Engine.Services.Resolvers;
using Engine.Services.LogsServices;

namespace Engine
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
			services.AddOptions().Configure<Settings>(Configuration.GetSection("Settings")).AddSingleton(Configuration);

			services.AddMemoryCache();
			services.AddScoped<IUsersRepository, UsersRepository>();
			services.AddScoped<ILogsRepository, LogsRepository>();
            services.AddScoped<ICompaniesRepository, CompaniesRepository>();
            services.AddScoped<CompaniesService>();
            services.AddScoped<UsersService>();
            services.AddScoped<ILogsService, LogsService>();

            var settings = Configuration.GetSection(nameof(Settings)).Get<Settings>();
            services.AddDbContext<EngineContext>(options =>
			{
				options.UseSqlite(settings.ConnnectionStrings.Sqlite);
			});
			services.AddControllers().AddXmlSerializerFormatters().AddXmlDataContractSerializerFormatters();

            services.AddScoped<WorkingServiceResolver>(sp => table =>
            {
                switch (table)
                {
                    case Tables.Companies:
                        return sp.GetService<CompaniesService>();
                    case Tables.Users:
                        return sp.GetService<UsersService>();
                    default:
                        throw new ArgumentException($"Unsupported service type for {table} table.");
                }
            });

            services.AddScoped<ServiceResolver>(sp => table =>
            {
                switch (table)
                {
                    case Tables.Companies:
                        return sp.GetService<CompaniesService>();
                    case Tables.Users:
                        return sp.GetService<UsersService>();
                    case Tables.Logs:
                        return sp.GetService<LogsService>();
                    default:
                        throw new ArgumentException($"Unsupported service type for {table} table.");
                }
            });

            

            services.AddSwaggerGen();
        }

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

        }
    }
}

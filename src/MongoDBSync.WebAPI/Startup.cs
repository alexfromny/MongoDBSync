using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDBSync.WebAPI.App_Start;
using MongoDBSync.WebAPI.App_Start.Settings;
using MongoDBSync.WebAPI.Domain;
using MongoDBSync.WebAPI.Helpers;
using MongoDBSync.WebAPI.Services;

namespace MongoDBSync.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = new AppSettings(Configuration.GetSection("AppSettings"));
            var dbSettings = MongoDBHelper.GetMongoDBSettings(appSettings.MongoDBConnectionString);
            InitMongoDB(dbSettings, appSettings.MongoDBName);

            var serviceCollection = DependencyFactory.PopulateContainer(services, appSettings, dbSettings);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            serviceProvider.GetService<IMongoDBCommandService>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private void InitMongoDB(MongoClientSettings dbSettings, String dbName)
        {
            MongoClient dbClient = new MongoClient(dbSettings);
            var db = dbClient.GetDatabase(dbName);
            
            db.DropCollection(DBCollections.Users.ToString());
            db.DropCollection(DBCollections.CommandsLog.ToString());

            db.CreateCollection(DBCollections.Users.ToString());
            db.CreateCollection(DBCollections.CommandsLog.ToString());
        }
    }
}

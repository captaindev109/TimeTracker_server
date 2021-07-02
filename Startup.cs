using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;

using UserApi.Models;
using ProjectApi.Models;
using TeamApi.Models;
using CompanyApi.Models;
using ResourceTypeApi.Models;
using TaskItemApi.Models;
using TaskTypeApi.Models;
using TimeTableApi.Models;
using UserAclApi.Models;

namespace TimeTracker_server
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
      services.AddCors();

      // Add framework services.
      services.AddDbContext<UserContext>(options =>
      options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

      services.AddDbContext<ProjectContext>(options =>
      options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

      services.AddDbContext<TeamContext>(options =>
      options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

      services.AddDbContext<CompanyContext>(options =>
      options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

      services.AddDbContext<ResourceTypeContext>(options =>
      options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

      services.AddDbContext<TimeTableContext>(options =>
      options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

      services.AddDbContext<TaskItemContext>(options =>
      options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

      services.AddDbContext<TaskTypeContext>(options =>
      options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

      services.AddDbContext<UserAclContext>(options =>
      options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

      // services.AddControllers(config =>
      // {
      //   var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
      //   config.Filters.Add(new AuthorizeFilter(policy));
      // });

      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "TimeTracker_server", Version = "v1" });
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TimeTracker_server v1"));
      }
      else
      {
        app.UseHsts();
      }

      app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

      // app.UseAuthentication();
      // app.UseAuthorization();

      app.UseHttpsRedirection();

      app.UseRouting();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}

using Bazar_Eshop.Models;
using Bazar_Eshop.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazar_Eshop
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
            services.AddControllersWithViews();
            services.AddScoped<ICategoryRepository, SQlCategoryRepository>();
           
            services.AddIdentity<ApplicationUser, IdentityRole>(
                options=>
                {
                    
                    options.Password.RequiredUniqueChars = 0;
                    options.Password.RequireNonAlphanumeric = false;
                    
                    options.Password.RequireDigit = false;
                }

                    ).AddEntityFrameworkStores<AppDbContext>();

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(10000);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Administration/AccessDenied");
            });
            services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = "575479208858-re1sjlqgiqum2sfekn0j69pfsgtbkout.apps.googleusercontent.com";
                options.ClientSecret = "8RimB8e13TcsQguwzpdfY1NR";
            });
            services.AddAuthentication().AddFacebook(options =>
            {
                options.AppId = "495982888274377";
                options.AppSecret = "7242986092a30330f1f1ffbdd76f8f20";
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("DeleteRolePolicy",
                    policy => policy.RequireClaim("Delete Role", "true"));
                options.AddPolicy("CreateRolePolicy",
                    policy => policy.RequireClaim("Create Role","true"));

                options.AddPolicy("EditRolePolicy",
                   policy => policy.RequireClaim("Edit Role","true"));




                options.AddPolicy("NonAdminDeleteClaimPolicy",
                    policy => policy.RequireAssertion(context =>
                    context.User.IsInRole("Super Admin") ||
                    context.User.HasClaim(claim => claim.Type== "Delete Role" && claim.Value =="true")));


                options.AddPolicy("NonAdminEditClaimPolicy",
                   policy => policy.RequireAssertion(context =>
                   context.User.IsInRole("Super Admin") ||
                   context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true")));


                options.AddPolicy("NonAdminCreateClaimPolicy",
                   policy => policy.RequireAssertion(context =>
                   context.User.IsInRole("Super Admin") ||
                   context.User.HasClaim(claim => claim.Type == "Create Role" && claim.Value == "true")));




                options.AddPolicy("AdminEditRolePolicy",
                    policy => policy.Requirements.Add(new ManageAdminRolesAndClaimsRequirement()));
            });
            
            services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();
            services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("BazarEshopDbConnection")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

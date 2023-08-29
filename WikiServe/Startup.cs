namespace WikiServe;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllersWithViews();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseStatusCodePagesWithReExecute("/Home/NotFound", "?code={0}&path={1}");
        
        app.UseRouting();
        app.UseEndpoints(end =>
        {
            end.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );
            end.MapControllerRoute(
                name: "wiki",
                pattern: "{wiki}",
                defaults: new { controller = "Page", action = "ViewWiki" }
            );
            end.MapControllerRoute(
                name: "page",
                pattern: "{wiki}/{*page}",
                defaults: new { controller = "Page", action = "ViewPage" }
            );
            
        });
    }
}
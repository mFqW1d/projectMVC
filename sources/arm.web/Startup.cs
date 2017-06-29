using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(arm_repairs_project.Startup))]
namespace arm_repairs_project
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

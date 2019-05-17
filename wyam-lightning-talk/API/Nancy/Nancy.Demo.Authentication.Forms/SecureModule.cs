namespace Nancy.Demo.Authentication.Forms
{
    using Nancy;
    using Nancy.Demo.Authentication.Forms.Models;
    using Nancy.Security;

    public class SecureModule : NancyModule
    {
        public SecureModule() : base("/secure")
        {
            this.RequiresAuthentication();

            Get["/"] = x => {
                var model = new UserModel(this.Context.CurrentUser.UserName);
                return View["secure.cshtml", model];
            };
        }
    }
}
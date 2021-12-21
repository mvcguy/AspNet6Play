namespace PlayWebApp.Services.Database.Model
{
    public class Customer : EntityBase
    {
        public string Name { get; set; }

        public bool Active { get; set; }
    }
}
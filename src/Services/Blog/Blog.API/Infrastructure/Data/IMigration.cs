namespace Blog.API.Infrastructure.Data
{
    public interface IMigration
    {
        void Up();
        void Down();
    }
}
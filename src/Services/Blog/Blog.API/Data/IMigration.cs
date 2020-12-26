namespace Blog.API.Data
{
    public interface IMigration
    {
        void Up();
        void Down();
    }
}
namespace Blogging.API.Infrastructure.Data
{
  public interface IMigration
  {
    void Up();
    void Down();
  }
}
namespace Blogging.API.Dtos
{
  public class BlogCreateDto
  {
    public string Name { get; set; }
    public string Title { get; set; }
    public string Heading { get; set; }
    public string Footer { get; set; }
    public string Uri { get; set; }
    public string Path { get; set; }
  }
}
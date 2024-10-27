namespace Application.DTOs
{
    public class DogQueryParameters
    {
        public string Attribute { get; set; } = "name";  // sorting by name by default
        public string Order { get; set; } = "asc";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

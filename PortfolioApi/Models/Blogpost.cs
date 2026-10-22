namespace PortfolioApi.Models;

public class Blogpost
{
    public int Id { get; set; }

    public string Titel { get; set; } = string.Empty;
    
    public string Inhooud { get; set; } = string.Empty;

     public DateTime PublicatieDatum { get; set; }

}
    
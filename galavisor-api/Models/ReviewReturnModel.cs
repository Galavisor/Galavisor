namespace GalavisorApi.Models;

public class ReviewReturnModel
{
    public int ReviewId { get; set; }
    public int? PlanetId { get; set; }

    public int? UserId { get; set; }

    public string? PlanetName { get; set; }

    public string? UserName { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }
}

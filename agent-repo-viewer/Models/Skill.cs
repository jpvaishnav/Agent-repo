namespace AgentRepoViewer.Models;

public class Skill
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Version { get; set; }
    public string Author { get; set; }
    public string Entrypoint { get; set; }
    public string Language { get; set; }
    public string Usage { get; set; }
    public List<string> Tags { get; set; } = new();
    public string Path { get; set; }
    public string FullContent { get; set; }
}

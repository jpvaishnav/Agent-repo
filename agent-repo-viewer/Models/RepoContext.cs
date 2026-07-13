namespace AgentRepoViewer.Models;

public class RepoContext
{
    public string AgentsMdContent { get; set; }
    public List<Skill> Skills { get; set; } = new();
    public List<Instruction> Instructions { get; set; } = new();
}

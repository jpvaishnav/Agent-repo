using System.Text.RegularExpressions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using AgentRepoViewer.Models;

namespace AgentRepoViewer.Services;

public class RepoAnalyzerService
{
    private readonly string _repoRoot;
    private readonly IDeserializer _deserializer;

    public RepoAnalyzerService(string repoRoot)
    {
        _repoRoot = repoRoot;
        _deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
    }

    public async Task<RepoContext> AnalyzeRepoAsync()
    {
        var context = new RepoContext();

        // Load AGENTS.md
        var agentsMdPath = Path.Combine(_repoRoot, "AGENTS.md");
        if (File.Exists(agentsMdPath))
        {
            context.AgentsMdContent = await File.ReadAllTextAsync(agentsMdPath);
        }

        // Load Skills
        var skillsDir = Path.Combine(_repoRoot, ".claude", "skills");
        if (Directory.Exists(skillsDir))
        {
            foreach (var skillFolder in Directory.GetDirectories(skillsDir))
            {
                var skill = await LoadSkillAsync(skillFolder);
                if (skill != null)
                {
                    context.Skills.Add(skill);
                }
            }
        }

        // Load Instructions
        var claudeInstructionsDir = Path.Combine(_repoRoot, ".claude", "instructions");
        if (Directory.Exists(claudeInstructionsDir))
        {
            foreach (var instrFile in Directory.GetFiles(claudeInstructionsDir, "*.md", SearchOption.AllDirectories))
            {
                var instr = await LoadInstructionAsync(instrFile);
                if (instr != null)
                {
                    context.Instructions.Add(instr);
                }
            }
        }

        var githubInstructionsDir = Path.Combine(_repoRoot, ".github", "instructions");
        if (Directory.Exists(githubInstructionsDir))
        {
            foreach (var instrFile in Directory.GetFiles(githubInstructionsDir, "*.md", SearchOption.AllDirectories))
            {
                var instr = await LoadInstructionAsync(instrFile);
                if (instr != null)
                {
                    context.Instructions.Add(instr);
                }
            }
        }

        return context;
    }

    private async Task<Skill?> LoadSkillAsync(string skillFolder)
    {
        var skillMdPath = Path.Combine(skillFolder, "skill.md");
        if (!File.Exists(skillMdPath))
            return null;

        var content = await File.ReadAllTextAsync(skillMdPath);
        var (frontmatter, markdown) = ExtractFrontmatter(content);

        if (frontmatter == null)
            return null;

        var skill = new Skill
        {
            Path = skillFolder,
            FullContent = content
        };

        try
        {
            var yaml = _deserializer.Deserialize<Dictionary<object, object>>(frontmatter);
            skill.Id = yaml?.GetValueOrDefault("name")?.ToString() ?? Path.GetFileName(skillFolder);
            skill.Name = yaml?.GetValueOrDefault("name")?.ToString() ?? skill.Id;
            skill.Description = yaml?.GetValueOrDefault("description")?.ToString() ?? "";
            skill.Version = yaml?.GetValueOrDefault("version")?.ToString() ?? "1.0.0";
            skill.Author = yaml?.GetValueOrDefault("author")?.ToString() ?? "";
            skill.Entrypoint = yaml?.GetValueOrDefault("entrypoint")?.ToString() ?? "";
            skill.Language = yaml?.GetValueOrDefault("language")?.ToString() ?? "";
            skill.Usage = yaml?.GetValueOrDefault("usage")?.ToString() ?? "";

            if (yaml?.GetValueOrDefault("tags") is List<object> tags)
            {
                skill.Tags = tags.Select(t => t.ToString()).ToList();
            }
        }
        catch { }

        return skill;
    }

    private async Task<Instruction?> LoadInstructionAsync(string instrFile)
    {
        var content = await File.ReadAllTextAsync(instrFile);
        var (frontmatter, markdown) = ExtractFrontmatter(content);

        if (frontmatter == null)
            return null;

        var instr = new Instruction
        {
            Path = instrFile,
            FullContent = content
        };

        try
        {
            var yaml = _deserializer.Deserialize<Dictionary<object, object>>(frontmatter);
            instr.Name = yaml?.GetValueOrDefault("name")?.ToString() ?? Path.GetFileNameWithoutExtension(instrFile);
            instr.Description = yaml?.GetValueOrDefault("description")?.ToString() ?? "";
            instr.Version = yaml?.GetValueOrDefault("version")?.ToString() ?? "1.0.0";
            instr.Author = yaml?.GetValueOrDefault("author")?.ToString() ?? "";

            if (yaml?.GetValueOrDefault("tags") is List<object> tags)
            {
                instr.Tags = tags.Select(t => t.ToString()).ToList();
            }
        }
        catch { }

        return instr;
    }

    private (string?, string?) ExtractFrontmatter(string content)
    {
        var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        
        if (lines.Length < 2 || lines[0].Trim() != "---")
            return (null, null);

        var endIdx = -1;
        for (int i = 1; i < lines.Length; i++)
        {
            if (lines[i].Trim() == "---")
            {
                endIdx = i;
                break;
            }
        }

        if (endIdx == -1)
            return (null, null);

        var frontmatter = string.Join("\n", lines.Skip(1).Take(endIdx - 1));
        var markdown = string.Join("\n", lines.Skip(endIdx + 1));

        return (frontmatter, markdown);
    }
}

# Agent Repo Viewer

A modern ASP.NET Core web application that provides a **human-readable view** of an agent-friendly repository. It displays all available skills, instructions, and repository context (AGENTS.md).

## Features

- 🎨 **Beautiful UI** - Modern, responsive web interface
- 🛠️ **Skills Discovery** - Lists all skills from `.claude/skills/`
- 📚 **Instructions Display** - Shows all instructions from `.claude/instructions/` and `.github/instructions/`
- 📋 **Repository Context** - Displays AGENTS.md global context
- ⚡ **Real-time Loading** - Scans and loads repo content on startup
- 📱 **Responsive Design** - Works on desktop and mobile devices

## Tech Stack

- **Framework**: ASP.NET Core 10.0
- **Frontend**: HTML5, CSS3, vanilla JavaScript
- **YAML Parsing**: YamlDotNet
- **Architecture**: Minimal API + Service Layer

## Getting Started

### Prerequisites

- .NET 10.0 SDK or later
- Windows, macOS, or Linux

### Build

```bash
cd agent-repo-viewer
dotnet build -c Release
```

### Run

```bash
dotnet run
```

The application will start on `https://localhost:5001` or `http://localhost:5000`.

Open your browser and navigate to the URL shown in the console.

### From Repository Root

```bash
cd Agent-repo/agent-repo-viewer
dotnet run
```

## Project Structure

```
agent-repo-viewer/
├── Models/              # Data models
│   ├── Skill.cs        # Skill model
│   ├── Instruction.cs  # Instruction model
│   └── RepoContext.cs  # Combined context
├── Services/
│   └── RepoAnalyzerService.cs  # Scans and parses repo
├── Controllers/
│   └── RepoController.cs       # API endpoint
├── wwwroot/
│   └── index.html             # Frontend UI
├── Program.cs          # ASP.NET Core setup
└── AgentRepoViewer.csproj
```

## How It Works

1. **Startup**: RepoAnalyzerService scans the repository root
2. **Discovery**:
   - Reads `AGENTS.md` for global context
   - Scans `.claude/skills/*/skill.md` for skills
   - Scans `.claude/instructions/` and `.github/instructions/` for instructions
3. **Parsing**: YAML frontmatter extracted and parsed using YamlDotNet
4. **API**: `/api/repo` endpoint returns JSON with all discovered content
5. **UI**: Frontend renders skills, instructions, and stats in a card-based layout

## API Endpoint

### GET /api/repo

Returns complete repository context:

```json
{
  "agentsMdContent": "...",
  "skills": [
    {
      "id": "generate-pr-diagram",
      "name": "Code Change Diagram",
      "description": "...",
      "version": "0.1.0",
      "author": "Copilot CLI",
      "entrypoint": "generate_diagram.py",
      "language": "python",
      "usage": "...",
      "tags": ["diagram", "visualization"],
      "path": "...",
      "fullContent": "..."
    }
  ],
  "instructions": [...]
}
```

## Development

### Add New Features

1. **New Model**: Add to `Models/`
2. **New Analysis**: Update `RepoAnalyzerService`
3. **New Endpoint**: Add to `Controllers/RepoController`
4. **UI Updates**: Edit `wwwroot/index.html`

### Local Testing

```bash
# Terminal 1: Run the app
dotnet run

# Terminal 2: Test API
curl https://localhost:5001/api/repo
```

## Configuration

The app automatically discovers the repository root by traversing up from the application directory. To customize:

Edit `Program.cs`:
```csharp
var repoRoot = Path.Combine(Directory.GetCurrentDirectory(), "..");  // Adjust as needed
```

## License

MIT

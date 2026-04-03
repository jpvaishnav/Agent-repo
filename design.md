repo-root/
│
├── AGENTS.md                          # Copilot + Claude — always-on, cascades in subdirs
├── CLAUDE.md                          # Claude Code — always-on, cascades in subdirs
├── .cursorrules                       # Cursor — always-on
├── .windsurfrules                     # Windsurf — always-on
├── .clinerules                        # Cline — always-on
├── .mcp.json                          # Claude Code — MCP servers
│
├── .github/
│   ├── copilot-instructions.md        # Copilot — always-on workspace instructions
│   ├── instructions/
│   │   ├── *.instructions.md          # Copilot — scoped by applyTo or description
│   ├── agents/
│   │   └── *.agent.md                 # Copilot — custom agents with tool restrictions
│   ├── prompts/
│   │   └── *.prompt.md                # Copilot — reusable slash commands
│   ├── skills/
│   │   └── <name>/
│   │       ├── SKILL.md               # Copilot — skill definition
│   │       ├── scripts/               # Executable code
│   │       ├── references/            # Docs loaded on demand
│   │       └── assets/                # Templates, boilerplate
│   └── hooks/
│       └── *.json                     # Copilot — lifecycle hooks
│
├── .claude/
│   ├── settings.json                  # Claude — permissions, hooks
│   ├── settings.local.json            # Claude — local overrides (gitignored)
│   └── skills/
│       └── <name>/
│           └── SKILL.md               # Claude/Copilot — shared skills
│
├── .cursor/
│   └── rules/
│       └── *.mdc                      # Cursor — scoped rules by glob
│
├── .vscode/
│   └── mcp.json                       # Copilot/Cursor — MCP servers
│
├── .agents/
│   └── skills/
│       └── <name>/
│           └── SKILL.md               # Copilot — alt skill location
│
└── docs/                              # Referenced by instructions (link, don't embed)
    ├── architecture.md
    └── conventions.md

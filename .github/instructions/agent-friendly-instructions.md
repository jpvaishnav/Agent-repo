---
name: agent-friendly-repo
description: Repository-wide instructions to make this repo agent-friendly for Claude/Copilot agents.
version: 0.1.0
author: Copilot CLI
tags:
  - instructions
  - agents
  - repository
---

This repository is agent-friendly. Follow these guidelines derived from agent docs (Claude, Copilot):

- Organize skills under `.claude/skills/<skill-name>/` with `skill.md` frontmatter and a clear entrypoint.
- Keep per-skill outputs in `output/` inside the skill directory to avoid global state.
- Provide a `.claude/skills.json` manifest for explicit registration when needed.
- Provide human- and agent-readable `skill.md` with name, description, entrypoint, usage, and tags.
- Place repository-wide instructions in `.github/instructions/` so agents that scan the repo can discover them.
- Store transient session artifacts in a dedicated session or workspace folder; avoid committing secrets.
- Structure code and documentation to make context obvious (README, AGENTS.md, CLAUDE.md).
- Use clear filenames and top-level directories so components map easily to agent analysis.
- Provide memory/context hints where helpful (e.g., intent, component boundaries, important files).
- Keep documentation concise and machine-friendly (frontmatter, short paragraphs, bullet lists).

Agents operating on this repo should respect these guidelines when adding skills, instructions, or modifying structure.

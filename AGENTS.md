# Agents Global Context

This file defines the repository-wide context for agents operating in this repository.

- This is the global context for agents: treat this repository as agent-friendly.
- Load skills from: `.claude/skills/` (each skill in its own subfolder with `skill.md`).
- Load instructions from: `.claude/instructions/` and `.github/instructions/`.
- Use `.claude/skills.json` manifest for explicit skill registration when present.
- Keep per-skill outputs inside each skill's `output/` directory to avoid global state.

Agents should respect these conventions when adding skills, reading instructions, or operating on code in this repository.

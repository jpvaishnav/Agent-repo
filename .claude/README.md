# Claude Skills

Custom skills for the Copilot agent.

## Available Skills

### generate-pr-diagram
- **Location**: `skills/generate-pr-diagram/`
- **Description**: Generate a visual system diagram showing components affected by a code change
- **Usage**: `python skills/generate-pr-diagram/generate_diagram.py [commit-ish]`

## Adding New Skills

1. Create a new directory: `skills/your-skill-name/`
2. Add `skill.md` with frontmatter metadata
3. Add implementation file (entrypoint from skill.md)
4. Update `skills.json` with the new skill reference

## Discovery

- Skills are loaded from `.claude/skills/*/skill.md`
- Manifest is in `.claude/skills.json`

---
name: code-change-diagram
description: Generate a visual system diagram for a code change, showing which components are modified and the nature of the change.
version: 0.1.0
author: Copilot CLI
entrypoint: generate_diagram.py
language: python
usage: |
  Analyze a commit or PR and generate a Graphviz diagram showing:
  - The change (commit message)
  - Components affected (top-level directories)
  - Files modified in each component
  
  Usage: generate_diagram [commit-ish]
  Default: HEAD
  
  Output: .claude/output/diagram_<commit>.dot (and .png if Graphviz available)
tags:
  - diagram
  - visualization
  - code-review
  - architecture
---

# Code Change Diagram Skill

Automatically generates a **visual system diagram** for any code change (commit/PR) by analyzing the source tree.

## What it does

1. **Parses the commit** - extracts message and changed files
2. **Maps to components** - groups files by top-level directory/module
3. **Generates diagram** - creates a Graphviz DOT file showing:
   - **Change box** (yellow): commit message
   - **Component boxes** (blue): system components affected
   - **Arrows**: flow from change to affected components
4. **Renders PNG** - if Graphviz (`dot`) is installed

## Output

- **Diagram**: `.claude/output/diagram_<commit>.dot`
- **PNG**: `.claude/output/diagram_<commit>.png` (if dot available)

## Example

```bash
python .claude/generate_diagram.py HEAD
```

Generates a visual showing exactly which parts of the system this commit touches.

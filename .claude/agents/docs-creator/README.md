docs-creator agent

Purpose:
- Generate structured documentation for this repository: High-Level Design (HLD), Low-Level Design (LLD), and architecture diagrams.

Usage:
- CLI: python scripts/generate_docs.py [--rev REV] [--output-dir PATH]
- Outputs markdown files and DOT diagrams into .claude/agents/docs-creator/output/ by default.

Notes:
- Graphviz (dot) is optional; DOT files are generated regardless.
- This agent is intended as a starting point; tune analysis heuristics for complex repositories.

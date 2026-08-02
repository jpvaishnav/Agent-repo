#!/usr/bin/env python3
"""
Generate HLD and LLD documentation for the repository.
Usage: python generate_docs.py [--rev REV] [--output-dir PATH]

This is a starter script that collects basic repository structure and writes
markdown HLD/LLD stubs into the output directory. Extend analysis heuristics
for more in-depth documentation.
"""
import argparse
import os
from pathlib import Path
import subprocess

parser = argparse.ArgumentParser()
parser.add_argument('--rev', default='HEAD')
parser.add_argument('--output-dir', default=None)
args = parser.parse_args()

repo_root = Path(__file__).resolve().parents[3]
out_dir = Path(args.output_dir) if args.output_dir else repo_root / '.claude' / 'agents' / 'docs-creator' / 'output'
out_dir.mkdir(parents=True, exist_ok=True)

# Basic repo scan
def list_top_dirs():
    return [p.name for p in repo_root.iterdir() if p.is_dir() and not p.name.startswith('.')]

hld = out_dir / f'HLD_{args.rev}.md'
lld = out_dir / f'LLD_{args.rev}.md'

with open(hld, 'w', encoding='utf-8') as f:
    f.write('# High-Level Design\n\n')
    f.write('## Components\n\n')
    for d in list_top_dirs():
        f.write(f'- {d}\n')
    f.write('\n## Data Flow\n\nDescribe major data flows here.\n')

with open(lld, 'w', encoding='utf-8') as f:
    f.write('# Low-Level Design\n\n')
    f.write('## Modules and Responsibilities\n\n')
    for d in list_top_dirs():
        f.write(f'### {d}\n\nFiles and notable classes/functions.\n')

print(f'Wrote HLD: {hld}\nWrote LLD: {lld}')

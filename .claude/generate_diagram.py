#!/usr/bin/env python3
"""
Simple diagram generator for a code change.
Usage: python generate_diagram.py [commit-ish]
Outputs a DOT file in .claude/output and attempts to render PNG if 'dot' available.
"""
import os
import subprocess
import sys
from pathlib import Path

repo_root = Path(__file__).resolve().parents[1]
out_dir = repo_root / '.claude' / 'output'
out_dir.mkdir(parents=True, exist_ok=True)

rev = sys.argv[1] if len(sys.argv) > 1 else 'HEAD'

# Try to get commit message and changed files
def run(cmd):
    return subprocess.check_output(cmd, shell=True, cwd=str(repo_root), text=True).strip()

commit_msg = ''
changed_files = []
try:
    # If rev is a commit, get its message
    commit_msg = run(f'git show -s --format=%s {rev}')
    # Try to list files changed in that commit (rev^..rev)
    try:
        changed_files = run(f'git diff --name-only {rev}^ {rev}').splitlines()
    except subprocess.CalledProcessError:
        # Fallback: list files in tree for the rev
        changed_files = run(f'git ls-tree -r --name-only {rev}').splitlines()
except Exception as e:
    commit_msg = f'[{rev}]'
    try:
        changed_files = run(f'git ls-files').splitlines()
    except Exception:
        changed_files = []

if not changed_files:
    changed_files = []

# Map files to components (top-level directory or file)
components = {}
for f in changed_files:
    # Normalize separators
    p = f.replace('\\', '/').lstrip('/')
    top = p.split('/')[0] if p else '(root)'
    components.setdefault(top, []).append(p)

# Create DOT
safe_rev = rev.replace('/', '_')
dot_path = out_dir / f'diagram_{safe_rev}.dot'
png_path = out_dir / f'diagram_{safe_rev}.png'
with open(dot_path, 'w', encoding='utf-8') as dot:
    dot.write('digraph G {\n')
    dot.write('  rankdir=LR;\n')
    dot.write('  node [shape=record, fontname="Helvetica"];\n')

    # Change node
    change_label = commit_msg.replace('"', '\\"')
    dot.write(f'  change [label="{{Change|{change_label}}}", style=filled, fillcolor=lightgoldenrodyellow];\n')

    # Component nodes
    for i, (comp, files) in enumerate(sorted(components.items())):
        file_list = '\\n'.join(files[:5])
        if len(files) > 5:
            file_list += '\\n...'
        comp_label = comp.replace('"', '\\"')
        label = f'{{{comp_label}|{file_list}}}' if file_list else f'{{{comp_label}}}'
        dot.write(f'  comp{i} [label="{label}", shape=record, style=filled, fillcolor=lightblue];\n')
        dot.write(f'  change -> comp{i};\n')

    if not components:
        dot.write('  note [label="No changed files found", shape=note];\n')

    dot.write('}\n')

print(f"DOT written to: {dot_path}")

# Try to render PNG using dot
try:
    subprocess.check_call(f'dot -V', shell=True, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
    subprocess.check_call(f'dot -Tpng "{dot_path}" -o "{png_path}"', shell=True)
    print(f"Rendered PNG: {png_path}")
except Exception:
    print("Graphviz not available or render failed. Install graphviz to render PNG. DOT file is ready.")

print('Done.')

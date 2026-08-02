Hooks in .claude/hooks are convenience scripts for local testing.

agent-repo-viewer-localhost: a hook that runs .claude/scripts/check_localhost.py to verify a server is listening on https://localhost:PORT (default port 8443). Use the .sh or .ps1 wrapper depending on your OS.

Examples:
  bash .claude/hooks/agent-repo-viewer-localhost.sh 8443
  powershell -File .claude/hooks/agent-repo-viewer-localhost.ps1 -Port 8443

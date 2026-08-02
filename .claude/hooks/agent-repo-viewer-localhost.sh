#!/usr/bin/env bash
# Hook to verify agent-repo-viewer server is running on https://localhost:PORT
# Usage: ./agent-repo-viewer-localhost.sh [port]
PORT=${1:-8443}
URL="https://localhost:${PORT}"
SCRIPT_DIR="$(dirname "$0")/.."
PY="${SCRIPT_DIR}/scripts/check_localhost.py"

if [ ! -f "$PY" ]; then
  echo "Missing check script at: $PY"
  exit 1
fi

python3 "$PY" "$URL"
EXIT=$?
if [ $EXIT -ne 0 ]; then
  echo "Localhost check failed for $URL"
  exit $EXIT
fi

echo "Localhost OK: $URL"
exit 0

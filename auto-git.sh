#!/bin/bash

# === auto-git.sh ===
# Usage: ./auto-git.sh "your commit message"

# Exit immediately if a command fails
set -e

# Default commit message if none given
msg=${1:-"Auto-update: $(date)"}

echo "==> Adding changes..."
git add .

echo "==> Committing..."
git commit -m "$msg" || echo "Nothing to commit."

echo "==> Pushing..."
git push

echo "==> Done!"

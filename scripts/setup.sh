#!/bin/bash

usage() {
    echo "Usage: $0 --session <session>"
    echo "  session: 01-sequential-pattern, 02-concurrent-pattern, 03-handoff-pattern, 04-group-chat-pattern"
    exit 1
}

SESSION=""
while [[ $# -gt 0 ]]; do
    case "$1" in
        --session) SESSION="$2"; shift 2 ;;
        *) usage ;;
    esac
done

case "$SESSION" in
    01-sequential-pattern|02-concurrent-pattern|03-handoff-pattern|04-group-chat-pattern) ;;
    *) usage ;;
esac

REPOSITORY_ROOT=$(git rev-parse --show-toplevel)
SAMPLE_DIR="$REPOSITORY_ROOT/samples/$SESSION/start"
WORKSHOP_DIR="$REPOSITORY_ROOT/workshop"

if [ -d "$WORKSHOP_DIR" ]; then
    echo "The 'workshop' directory already exists. Rename or remove it before running this script."
    exit 1
fi

echo "Setting up the workshop directory for session: $SESSION"

mkdir -p "$WORKSHOP_DIR"
cp -a "$SAMPLE_DIR/." "$WORKSHOP_DIR/"

find "$WORKSHOP_DIR" -name "post-deploy.*" -type f | while read -r file; do
    sed -i.bak -E "s|samples/$SESSION/start|workshop|g" "$file"
    rm -f "$file.bak"
done

echo "Workshop directory is ready at: $WORKSHOP_DIR"
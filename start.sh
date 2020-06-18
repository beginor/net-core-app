#!/bin/bash -e
if ! [ -x "$(command -v concurrently)" ]; then
  echo 'Warning: concurrently is not installed, install it with npm now .' >&2
  npm i -g concurrently
fi

concurrently "cd client && npm run start" \
  "cd server/src/GisHub.Entry && dotnet watch run"

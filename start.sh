#!/bin/bash -e
concurrently "cd client && npm run start" "cd server/src/NetCoreApp.Api && dotnet watch run"

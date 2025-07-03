#!/bin/bash

# Script to run E2E tests using Playwright server in Docker

set -e

PLAYWRIGHT_PORT=3000
CONTAINER_NAME="playwright-server"

# Function to cleanup
cleanup() {
    echo "Cleaning up..."
    docker stop $CONTAINER_NAME 2>/dev/null || true
    docker rm $CONTAINER_NAME 2>/dev/null || true
}

# Set trap to cleanup on exit
trap cleanup EXIT

echo "Building client..."
cd client && npm run build && cd ..

echo "Starting Playwright server in Docker..."
docker run -d \
    --name $CONTAINER_NAME \
    --add-host=hostmachine:host-gateway \
    -p $PLAYWRIGHT_PORT:$PLAYWRIGHT_PORT \
    --rm \
    --init \
    --workdir /home/pwuser \
    --user pwuser \
    mcr.microsoft.com/playwright:v1.53.0-noble \
    /bin/sh -c "npx -y playwright@1.53.0 run-server --port $PLAYWRIGHT_PORT --host 0.0.0.0"

echo "Waiting for Playwright server to start..."
sleep 5

# Check if server is running
if ! curl -s http://localhost:$PLAYWRIGHT_PORT/health > /dev/null; then
    echo "Playwright server failed to start. Checking logs..."
    docker logs $CONTAINER_NAME
    exit 1
fi

echo "Playwright server is running. Running E2E tests..."

# Set environment variable to connect to remote Playwright server
export PW_TEST_CONNECT_WS_ENDPOINT="ws://127.0.0.1:$PLAYWRIGHT_PORT/"

# Run the tests
echo "Choose test runner:"
echo "1) NUnit (E2E tests only) - recommended"
echo "2) TUnit (all tests including E2E) - may have Playwright version issues"
echo ""

# Check if we have a command line argument
if [ "$1" = "tunit" ]; then
    CHOICE="2"
elif [ "$1" = "nunit" ] || [ "$1" = "" ]; then
    CHOICE="1"
else
    echo "Invalid option. Use 'tunit' or 'nunit' or run without arguments for TUnit"
    exit 1
fi

if [ "$CHOICE" = "1" ]; then
    echo "Running NUnit E2E tests only..."
    cd server/tests-e2e
    if dotnet test; then
        echo "E2E tests completed successfully!"
    else
        echo "E2E tests failed!"
        exit 1
    fi
else
    echo "Running TUnit tests (all tests including E2E)..."
    cd server/tests
    if dotnet run -c Release; then
        echo "E2E tests completed successfully!"
    else
        echo "E2E tests failed!"
        exit 1
    fi
fi

echo "E2E tests completed. Check server/tests/test-output/ for screenshots and artifacts."
# 🧪 E2E Testing Setup Guide

This guide covers how to run E2E tests in your development environment, especially with JetBrains Rider.

## 🎯 Overview

You now have **3 different approaches** to run E2E tests:

1. **🚀 Bash Script** - Fully automated (recommended for CI/CD)
2. **🔧 Manual Setup + IDE** - Fastest for development iteration  
3. **🤖 Auto-Docker** - Convenient but slower per test

## 📋 Option 1: Bash Script (Fully Automated)

**Best for**: CI/CD, one-off testing, demo purposes

```bash
# Run with NUnit (E2E only):
./run-e2e-with-docker.sh

# Run with TUnit (all tests):
./run-e2e-with-docker.sh tunit
```

**Pros**: Zero setup, handles everything  
**Cons**: Slower, can't run individual tests easily

---

## ⚡ Option 2: Manual Setup + IDE (Recommended for Development)

**Best for**: Development, debugging individual tests, Rider integration

### 🔧 One-Time Setup:

1. **Build React app:**
   ```bash
   cd client && npm run build && cd ..
   ```

2. **Start Playwright server:**
   ```bash
   docker run -d --name playwright-server \
     --add-host=hostmachine:host-gateway \
     -p 3000:3000 --rm --init \
     --workdir /home/pwuser --user pwuser \
     mcr.microsoft.com/playwright:v1.53.0-noble \
     /bin/sh -c "npx -y playwright@1.53.0 run-server --port 3000 --host 0.0.0.0"
   ```

3. **Configure Rider:**
   - Open Run/Debug Configurations
   - Edit the TUnit configuration for your test project
   - Add Environment Variable: `PW_TEST_CONNECT_WS_ENDPOINT=ws://127.0.0.1:3000/`

### 🎮 Usage:
- Run individual E2E tests from Rider's test explorer
- Debug tests with breakpoints
- Fast iteration - no Docker startup delay per test

### 🧹 Cleanup:
```bash
docker stop playwright-server
```

**Pros**: Fast iteration, IDE integration, debugging support  
**Cons**: Manual setup required

---

## 🤖 Option 3: Auto-Docker (Fallback)

**Best for**: Convenience when you don't want manual setup

### Usage:
Just run tests normally - they'll auto-start Docker if no manual server is detected.

**Pros**: No manual setup  
**Cons**: Slower (Docker startup per test session)

---

## 🏗️ Test Classes Available

| Test Class | Base Class | Purpose |
|------------|------------|---------|
| `IdeE2eTest` | `IdeE2eTestBase` | IDE-friendly, auto/manual Docker |
| `DockerManagedE2eTest` | `DockerManagedE2eTestBase` | Full auto-Docker management |
| `BasicE2eTest` (old) | `E2eTestBase` | Original implementation |

## 🎯 Recommended Workflow for Development

### For Daily Development:
1. **Morning setup** (once):
   ```bash
   cd client && npm run build
   docker run -d --name playwright-server --add-host=hostmachine:host-gateway -p 3000:3000 --rm --init --workdir /home/pwuser --user pwuser mcr.microsoft.com/playwright:v1.53.0-noble /bin/sh -c "npx -y playwright@1.53.0 run-server --port 3000 --host 0.0.0.0"
   ```

2. **Set Rider environment variable**: `PW_TEST_CONNECT_WS_ENDPOINT=ws://127.0.0.1:3000/`

3. **Develop & test**: Run individual `IdeE2eTest` tests from Rider

4. **End of day cleanup**:
   ```bash
   docker stop playwright-server
   ```

### For CI/CD:
```bash
# E2E tests only:
./run-e2e-with-docker.sh nunit

# All tests (API + E2E):  
./run-e2e-with-docker.sh tunit
```

## ⚠️ TUnit Filtering Limitation

**Important**: TUnit's `--treenode-filter` has reliability issues with wildcards and doesn't support category filtering. 

**For E2E-only tests**: Use the **NUnit option** in the script:
```bash
./run-e2e-with-docker.sh nunit  # E2E tests only
```

**For all tests including E2E**: Use the **TUnit option**:
```bash
./run-e2e-with-docker.sh tunit  # All tests (slower but comprehensive)
```

## 🛠️ Adding New E2E Tests

Create new tests by extending `IdeE2eTestBase`:

```csharp
public class MyFeatureE2eTest : IdeE2eTestBase
{
    [Test]
    public async Task CanTestMyFeature()
    {
        // Navigate to your feature
        await Page.GotoAsync($"{BaseUrl}/my-feature");
        
        // Interact with UI
        await Page.ClickAsync("button[data-testid='my-button']");
        
        // Assert results
        var result = await Page.TextContentAsync("[data-testid='result']");
        if (result != "Expected Value")
            throw new Exception($"Expected 'Expected Value' but got '{result}'");
    }
}
```

## 🐛 Troubleshooting

### Playwright Server Not Starting:
```bash
# Check if container is running:
docker ps | grep playwright

# Check logs:
docker logs playwright-server

# Restart:
docker stop playwright-server
# Then run the docker run command again
```

### Tests Can't Connect:
- Verify environment variable: `PW_TEST_CONNECT_WS_ENDPOINT=ws://127.0.0.1:3000/`
- Check port 3000 is not used by another service: `lsof -i :3000`

### React App Not Loading:
- Ensure client is built: `cd client && npm run build`
- Check for build errors

### TUnit Filter Issues:
- Use Rider's test explorer instead of command line filters
- Or run all tests: `dotnet run -c Release`

## 📊 Performance Comparison

| Method | Setup Time | Per-Test Time | IDE Integration |
|--------|------------|---------------|-----------------|
| Bash Script | ~10s | ~8s | ❌ |
| Manual + IDE | ~10s (once) | ~3s | ✅ |
| Auto-Docker | 0s | ~8s | ✅ |

**Recommendation**: Use Manual + IDE for development, Bash Script for CI/CD.
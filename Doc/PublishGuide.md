# IOTWave NuGet 包发布指南

## 发布前检查清单

- [ ] 确认所有测试通过
- [ ] 验证版本号正确
- [ ] 确认 README 和文档是最新的
- [ ] 检查包依赖项
- [ ] 验证目标框架兼容性

## 版本号管理

当前版本: 1.0.0

版本号格式: 主版本号.次版本号.修订号

## 构建和验证包

### 构建命令
```bash
dotnet build IOTWave/IOTWave.csproj -c Release
```

### 打包命令
```bash
dotnet pack IOTWave/IOTWave.csproj -c Release -o ./nupkgs
```

### 验证包内容
生成的包位于 `IOTWave/bin/Release/IOTWave.1.0.0.nupkg`

验证包内容:
1. 解压.nupkg文件（实际上是ZIP格式）
2. 检查 lib/net8.0/ 和 lib/net10.0/ 目录下的DLL文件
3. 确认包含 README.md 和 LICENSE 文件
4. 验证包的依赖项正确

## 发布到 NuGet.org

### 1. 获取 API 密钥
访问 https://www.nuget.org/account/apikeys 获取API密钥

### 2. 发布命令
```bash
dotnet nuget push "IOTWave/bin/Release/IOTWave.1.0.0.nupkg" --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
```

### 3. 或者使用 NuGet CLI
```bash
nuget push "IOTWave/bin/Release/IOTWave.1.0.0.nupkg" -ApiKey YOUR_API_KEY -Source https://api.nuget.org/v3/index.json
```

## 发布到私有源

如果发布到私有NuGet源:
```bash
dotnet nuget push "IOTWave/bin/Release/IOTWave.1.0.0.nupkg" --api-key YOUR_PRIVATE_SOURCE_KEY --source YOUR_PRIVATE_SOURCE_URL
```

## 验证发布

发布完成后，可以通过以下方式验证:
- 访问 https://www.nuget.org/packages/IOTWave/
- 搜索包名称确认可见
- 检查版本号和描述是否正确
- 尝试安装测试: `dotnet add package IOTWave --version 1.0.0`

## 回滚发布

如果需要撤销发布的包（24小时内）:
1. 登录 NuGet.org
2. 进入 "Manage Packages" 页面
3. 找到对应包，点击 "Unlist" 按钮

注意: 无法永久删除已发布的包版本，只能取消列出。

## 自动化发布脚本示例

```bash
#!/bin/bash

VERSION="1.0.0"
PACKAGE_PATH="IOTWave/bin/Release/IOTWave.$VERSION.nupkg"

echo "Building IOTWave version $VERSION..."
dotnet build IOTWave/IOTWave.csproj -c Release

echo "Packing IOTWave version $VERSION..."
dotnet pack IOTWave/IOTWave.csproj -c Release

if [ -f "$PACKAGE_PATH" ]; then
    echo "Package created successfully at $PACKAGE_PATH"
    echo "Verifying package contents..."

    # Create temp directory and extract package to verify contents
    TEMP_DIR=$(mktemp -d)
    unzip -q "$PACKAGE_PATH" -d "$TEMP_DIR"

    echo "Package contents:"
    find "$TEMP_DIR" -type f | sort

    # Optional: Validate package metadata
    echo "Package verification complete."

    # Uncomment the following lines to actually publish
    # echo "Publishing to NuGet.org..."
    # dotnet nuget push "$PACKAGE_PATH" --api-key $NUGET_API_KEY --source https://api.nuget.org/v3/index.json

    rm -rf "$TEMP_DIR"
else
    echo "ERROR: Package file not found at $PACKAGE_PATH"
    exit 1
fi
```

## CI/CD 集成

### GitHub Actions 示例
```yaml
name: Publish NuGet Package

on:
  release:
    types: [published]

jobs:
  publish:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3

    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'

    - name: Restore dependencies
      run: dotnet restore

    - name: Build
      run: dotnet build --no-restore -c Release

    - name: Pack
      run: dotnet pack --no-build -c Release -o ./nupkgs

    - name: Push to NuGet
      run: dotnet nuget push "./nupkgs/*.nupkg" --api-key ${{ secrets.NUGET_API_KEY }} --source https://api.nuget.org/v3/index.json
```

## 发布后任务

- [ ] 在GitHub上创建Release
- [ ] 更新项目README中的版本信息
- [ ] 在社交媒体或相关社区宣传
- [ ] 监控包的下载统计
- [ ] 处理用户反馈和问题报告

## 常见问题

### 包大小限制
NuGet.org对包大小有限制（目前为250MB），确保包文件不超过此限制。

### 符号包
如需发布符号包(.snupkg)以支持调试:
```bash
dotnet pack IOTWave/IOTWave.csproj -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
```

### 包验证
发布前可以使用NuGet验证工具验证包:
```bash
nuget verify IOTWave.1.0.0.nupkg
```
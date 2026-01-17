#!/bin/bash

echo "🐳 Docker Desktop 直接安装脚本"
echo "================================"
echo ""

# 检测Mac芯片类型
ARCH=$(uname -m)
if [ "$ARCH" = "arm64" ]; then
    echo "✅ 检测到 Apple Silicon (M1/M2/M3) 芯片"
    DOCKER_URL="https://desktop.docker.com/mac/main/arm64/Docker.dmg"
    CHIP_TYPE="Apple Silicon"
else
    echo "✅ 检测到 Intel 芯片"
    DOCKER_URL="https://desktop.docker.com/mac/main/amd64/Docker.dmg"
    CHIP_TYPE="Intel"
fi

echo ""
echo "📥 正在下载 Docker Desktop for Mac ($CHIP_TYPE)..."
echo "下载地址: $DOCKER_URL"
echo ""

# 下载Docker Desktop
curl -L -o ~/Downloads/Docker.dmg "$DOCKER_URL"

if [ $? -eq 0 ]; then
    echo ""
    echo "✅ 下载完成！"
    echo ""
    echo "📦 安装步骤:"
    echo "1. 打开 Finder"
    echo "2. 进入 '下载' 文件夹"
    echo "3. 双击 'Docker.dmg' 文件"
    echo "4. 将 Docker 图标拖到 Applications 文件夹"
    echo "5. 从 Applications 文件夹启动 Docker"
    echo "6. 首次启动需要输入密码授权"
    echo ""
    echo "或者运行以下命令自动打开安装程序:"
    echo "open ~/Downloads/Docker.dmg"
    echo ""
else
    echo ""
    echo "❌ 下载失败"
    echo ""
    echo "请手动下载:"
    echo "1. 打开浏览器访问: https://www.docker.com/products/docker-desktop/"
    echo "2. 点击 'Download for Mac'"
    echo "3. 选择适合你芯片的版本 ($CHIP_TYPE)"
    echo ""
fi

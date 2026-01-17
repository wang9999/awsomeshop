#!/bin/bash

echo "=========================================="
echo "AWSomeShop 部署检查工具"
echo "=========================================="
echo ""

# 颜色定义
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# 检查 GitHub 仓库
echo "📦 检查 GitHub 仓库..."
if git remote -v | grep -q "wang9999/awsomeshop"; then
    echo -e "${GREEN}✅ GitHub 仓库已配置${NC}"
    echo "   仓库地址: https://github.com/wang9999/awsomeshop"
else
    echo -e "${RED}❌ GitHub 仓库未配置${NC}"
fi
echo ""

# 检查本地服务
echo "🔍 检查本地服务状态..."

# 检查 Docker
if command -v docker &> /dev/null; then
    echo -e "${GREEN}✅ Docker 已安装${NC}"
    
    # 检查 MySQL
    if docker ps | grep -q "awsomeshop-mysql"; then
        echo -e "${GREEN}✅ MySQL 容器运行中${NC}"
    else
        echo -e "${YELLOW}⚠️  MySQL 容器未运行${NC}"
    fi
    
    # 检查 Redis
    if docker ps | grep -q "awsomeshop-redis"; then
        echo -e "${GREEN}✅ Redis 容器运行中${NC}"
    else
        echo -e "${YELLOW}⚠️  Redis 容器未运行${NC}"
    fi
else
    echo -e "${YELLOW}⚠️  Docker 未安装${NC}"
fi
echo ""

# 检查部署文档
echo "📚 检查部署文档..."
docs=(
    "START_HERE.md"
    "QUICK_REFERENCE.md"
    "NEXT_STEPS.md"
    "railway-env-vars.txt"
    "vercel-env-vars.txt"
    "railway-config.txt"
    "vercel-config.txt"
)

for doc in "${docs[@]}"; do
    if [ -f "$doc" ]; then
        echo -e "${GREEN}✅ $doc${NC}"
    else
        echo -e "${RED}❌ $doc 缺失${NC}"
    fi
done
echo ""

# 部署清单
echo "=========================================="
echo "📋 部署清单"
echo "=========================================="
echo ""
echo "第一阶段：GitHub 推送"
echo -e "${GREEN}✅ 已完成${NC}"
echo ""
echo "第二阶段：Railway 部署（30分钟）"
echo "⏳ 待完成"
echo "   1. 访问 https://railway.app"
echo "   2. 创建项目，选择 wang9999/awsomeshop"
echo "   3. 添加 MySQL 和 Redis"
echo "   4. 配置后端服务（参考 railway-config.txt）"
echo "   5. 配置环境变量（参考 railway-env-vars.txt）"
echo "   6. 部署并获取 URL"
echo ""
echo "第三阶段：Vercel 部署（10分钟）"
echo "⏳ 待完成"
echo "   1. 访问 https://vercel.com"
echo "   2. 导入 wang9999/awsomeshop"
echo "   3. 配置构建设置（参考 vercel-config.txt）"
echo "   4. 配置环境变量（参考 vercel-env-vars.txt）"
echo "   5. 部署并获取 URL"
echo ""
echo "第四阶段：最终配置（5分钟）"
echo "⏳ 待完成"
echo "   1. 更新 Railway CORS 配置"
echo "   2. 测试访问"
echo ""

# 快速链接
echo "=========================================="
echo "🔗 快速链接"
echo "=========================================="
echo ""
echo "GitHub 仓库: https://github.com/wang9999/awsomeshop"
echo "Railway: https://railway.app"
echo "Vercel: https://vercel.com"
echo ""

# 推荐文档
echo "=========================================="
echo "📖 推荐文档"
echo "=========================================="
echo ""
echo "1. START_HERE.md - 快速开始指南"
echo "2. QUICK_REFERENCE.md - 快速参考卡片"
echo "3. NEXT_STEPS.md - 详细步骤说明"
echo ""

echo "=========================================="
echo "✨ 准备就绪！"
echo "=========================================="
echo ""
echo "下一步："
echo "1. 打开浏览器"
echo "2. 访问 https://railway.app"
echo "3. 按照 START_HERE.md 的指引操作"
echo ""

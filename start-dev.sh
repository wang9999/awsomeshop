#!/bin/bash

# AWSomeShop 开发环境启动脚本

echo "🚀 AWSomeShop 开发环境启动脚本"
echo "================================"
echo ""

# 检查Docker是否安装
if ! command -v docker &> /dev/null; then
    echo "❌ Docker未安装"
    echo ""
    echo "请先安装Docker Desktop:"
    echo "1. 访问 https://www.docker.com/products/docker-desktop/"
    echo "2. 下载并安装适合你系统的版本"
    echo "3. 启动Docker Desktop"
    echo ""
    echo "详细说明请查看 DOCKER_SETUP.md"
    exit 1
fi

echo "✅ Docker已安装"

# 检查Docker是否运行
if ! docker info &> /dev/null; then
    echo "❌ Docker未运行"
    echo ""
    echo "请启动Docker Desktop，然后重新运行此脚本"
    exit 1
fi

echo "✅ Docker正在运行"
echo ""

# 启动MySQL和Redis
echo "📦 启动MySQL和Redis..."
docker-compose up -d

# 等待服务就绪
echo ""
echo "⏳ 等待服务启动..."
sleep 5

# 检查服务状态
echo ""
echo "📊 服务状态:"
docker-compose ps

# 检查MySQL健康状态
echo ""
echo "🔍 检查MySQL连接..."
if docker exec awsomeshop-mysql mysqladmin ping -h localhost -u root -pyour_password &> /dev/null; then
    echo "✅ MySQL连接成功"
else
    echo "⚠️  MySQL还在启动中，请稍等片刻"
fi

# 检查Redis健康状态
echo ""
echo "🔍 检查Redis连接..."
if docker exec awsomeshop-redis redis-cli ping &> /dev/null; then
    echo "✅ Redis连接成功"
else
    echo "⚠️  Redis还在启动中，请稍等片刻"
fi

echo ""
echo "================================"
echo "✅ 数据库服务已启动！"
echo ""
echo "📝 下一步操作:"
echo ""
echo "1. 启动后端服务:"
echo "   cd backend/src/AWSomeShop.API"
echo "   ~/.dotnet/dotnet run"
echo ""
echo "2. 启动前端服务 (新终端窗口):"
echo "   cd frontend"
echo "   npm run dev"
echo ""
echo "3. 访问应用:"
echo "   前端: http://localhost:5173"
echo "   后端: https://localhost:5001"
echo "   Swagger: https://localhost:5001/swagger"
echo ""
echo "4. 测试账号:"
echo "   员工: employee1@awsome.com / Employee@123"
echo "   管理员: superadmin / Admin@123"
echo ""
echo "💡 提示:"
echo "   - 查看日志: docker-compose logs -f"
echo "   - 停止服务: docker-compose stop"
echo "   - 重启服务: docker-compose restart"
echo ""

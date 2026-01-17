#!/bin/bash

echo "=========================================="
echo "AWSomeShop - ngrok 快速部署"
echo "=========================================="
echo ""

# 检查ngrok是否安装
if ! command -v ngrok &> /dev/null; then
    echo "❌ ngrok未安装"
    echo ""
    echo "请先安装ngrok："
    echo "  macOS: brew install ngrok"
    echo "  或访问: https://ngrok.com/download"
    echo ""
    exit 1
fi

echo "✅ ngrok已安装"
echo ""

# 检查服务是否运行
echo "📊 检查服务状态..."
echo ""

# 检查后端
if curl -s http://localhost:5144/api/products > /dev/null 2>&1; then
    echo "✅ 后端服务运行中 (http://localhost:5144)"
else
    echo "❌ 后端服务未运行"
    echo "请先启动后端："
    echo "  cd backend/src/AWSomeShop.API"
    echo "  ~/.dotnet/dotnet run"
    echo ""
    exit 1
fi

# 检查前端
if curl -s http://localhost:5173 > /dev/null 2>&1; then
    echo "✅ 前端服务运行中 (http://localhost:5173)"
else
    echo "❌ 前端服务未运行"
    echo "请先启动前端："
    echo "  cd frontend"
    echo "  npm run dev"
    echo ""
    exit 1
fi

echo ""
echo "=========================================="
echo "🚀 开始部署到公网"
echo "=========================================="
echo ""

# 创建临时目录存储ngrok URL
mkdir -p .ngrok-temp

# 启动后端ngrok（后台运行）
echo "1️⃣ 启动后端公网访问..."
ngrok http 5144 --log=stdout > .ngrok-temp/backend.log 2>&1 &
BACKEND_PID=$!
echo "   后端ngrok进程ID: $BACKEND_PID"

# 等待ngrok启动
sleep 3

# 获取后端公网URL
BACKEND_URL=$(curl -s http://localhost:4040/api/tunnels | grep -o '"public_url":"https://[^"]*' | grep -o 'https://[^"]*' | head -1)

if [ -z "$BACKEND_URL" ]; then
    echo "❌ 无法获取后端公网URL"
    echo "请检查ngrok是否正确配置"
    kill $BACKEND_PID 2>/dev/null
    exit 1
fi

echo "   ✅ 后端公网地址: $BACKEND_URL"
echo ""

# 更新前端环境变量
echo "2️⃣ 更新前端配置..."
cat > frontend/.env.development.local << EOF
VITE_API_BASE_URL=${BACKEND_URL}/api
EOF
echo "   ✅ 前端配置已更新"
echo ""

# 提示重启前端
echo "3️⃣ 请重启前端服务以应用新配置："
echo ""
echo "   在前端终端按 Ctrl+C 停止服务，然后运行："
echo "   npm run dev"
echo ""
echo "   按回车键继续..."
read

# 启动前端ngrok（后台运行）
echo "4️⃣ 启动前端公网访问..."
ngrok http 5173 --log=stdout > .ngrok-temp/frontend.log 2>&1 &
FRONTEND_PID=$!
echo "   前端ngrok进程ID: $FRONTEND_PID"

# 等待ngrok启动
sleep 3

# 获取前端公网URL（使用不同的端口获取）
FRONTEND_URL=$(curl -s http://localhost:4041/api/tunnels | grep -o '"public_url":"https://[^"]*' | grep -o 'https://[^"]*' | head -1)

if [ -z "$FRONTEND_URL" ]; then
    echo "❌ 无法获取前端公网URL"
    echo "尝试手动查看ngrok控制台"
    FRONTEND_URL="请访问 http://localhost:4041 查看"
fi

echo "   ✅ 前端公网地址: $FRONTEND_URL"
echo ""

# 保存进程ID
echo "$BACKEND_PID" > .ngrok-temp/backend.pid
echo "$FRONTEND_PID" > .ngrok-temp/frontend.pid

# 保存URL
echo "$BACKEND_URL" > .ngrok-temp/backend.url
echo "$FRONTEND_URL" > .ngrok-temp/frontend.url

echo "=========================================="
echo "🎉 部署完成！"
echo "=========================================="
echo ""
echo "📱 访问地址："
echo "   前端: $FRONTEND_URL"
echo "   后端: $BACKEND_URL"
echo ""
echo "🔐 测试账号："
echo "   员工: employee1@awsome.com / Employee@123"
echo "   管理员: superadmin / Admin@123"
echo ""
echo "📊 ngrok控制台："
echo "   后端: http://localhost:4040"
echo "   前端: http://localhost:4041"
echo ""
echo "⚠️  注意事项："
echo "   1. 保持此终端窗口打开"
echo "   2. 保持后端和前端服务运行"
echo "   3. 免费版ngrok有连接数限制"
echo "   4. URL在重启后会变化"
echo ""
echo "🛑 停止服务："
echo "   运行: ./stop-ngrok.sh"
echo ""
echo "按 Ctrl+C 停止ngrok服务"
echo ""

# 等待用户中断
wait

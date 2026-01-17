#!/bin/bash

echo "=========================================="
echo "停止 ngrok 服务"
echo "=========================================="
echo ""

# 检查是否有保存的进程ID
if [ ! -d ".ngrok-temp" ]; then
    echo "❌ 未找到ngrok进程信息"
    echo "尝试手动停止所有ngrok进程..."
    killall ngrok 2>/dev/null
    echo "✅ 已尝试停止所有ngrok进程"
    exit 0
fi

# 停止后端ngrok
if [ -f ".ngrok-temp/backend.pid" ]; then
    BACKEND_PID=$(cat .ngrok-temp/backend.pid)
    echo "停止后端ngrok (PID: $BACKEND_PID)..."
    kill $BACKEND_PID 2>/dev/null
    echo "✅ 后端ngrok已停止"
fi

# 停止前端ngrok
if [ -f ".ngrok-temp/frontend.pid" ]; then
    FRONTEND_PID=$(cat .ngrok-temp/frontend.pid)
    echo "停止前端ngrok (PID: $FRONTEND_PID)..."
    kill $FRONTEND_PID 2>/dev/null
    echo "✅ 前端ngrok已停止"
fi

# 清理临时文件
echo ""
echo "清理临时文件..."
rm -rf .ngrok-temp
rm -f frontend/.env.development.local

echo "✅ 清理完成"
echo ""
echo "=========================================="
echo "🎉 ngrok服务已全部停止"
echo "=========================================="
echo ""
echo "💡 提示："
echo "   - 后端和前端服务仍在运行"
echo "   - 如需重新部署，运行: ./deploy-ngrok.sh"
echo ""

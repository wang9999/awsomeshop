#!/bin/bash

echo "=========================================="
echo "推送代码到GitHub"
echo "=========================================="
echo ""

# 检查是否已经是git仓库
if [ -d ".git" ]; then
    echo "✅ 已经是Git仓库"
else
    echo "📦 初始化Git仓库..."
    git init
    git branch -M main
fi

# 配置Git用户信息
echo ""
echo "📝 配置Git用户信息"
echo "请输入你的GitHub用户名："
read github_username

echo "请输入你的GitHub邮箱："
read github_email

git config --global user.name "$github_username"
git config --global user.email "$github_email"

echo "✅ Git用户信息已配置"
echo ""

# 添加.gitignore
echo "📝 创建.gitignore文件..."
cat > .gitignore << 'EOF'
# .NET
bin/
obj/
*.user
*.suo
*.cache
*.dll
*.exe
*.pdb
*.log

# Node
node_modules/
dist/
.env.local
.env.development.local
.env.test.local
.env.production.local

# IDE
.vscode/
.idea/
*.swp
*.swo

# OS
.DS_Store
Thumbs.db

# Temporary files
.ngrok-temp/
*.tmp

# Docker
.docker/
EOF

echo "✅ .gitignore已创建"
echo ""

# 添加所有文件
echo "📦 添加文件到Git..."
git add .

# 提交
echo "💾 提交代码..."
git commit -m "Initial commit: AWSomeShop employee rewards platform"

echo ""
echo "=========================================="
echo "🔗 连接到GitHub仓库"
echo "=========================================="
echo ""
echo "请输入你的GitHub仓库URL"
echo "格式: https://github.com/用户名/awsomeshop.git"
echo ""
read repo_url

# 添加远程仓库
git remote add origin "$repo_url"

echo ""
echo "=========================================="
echo "🚀 推送代码到GitHub"
echo "=========================================="
echo ""
echo "⚠️  重要提示："
echo "当提示输入密码时，请输入你的Personal Access Token"
echo "（不是GitHub密码！）"
echo ""
echo "按回车键继续..."
read

# 推送代码
git push -u origin main

if [ $? -eq 0 ]; then
    echo ""
    echo "=========================================="
    echo "🎉 成功推送到GitHub！"
    echo "=========================================="
    echo ""
    echo "📱 访问你的仓库："
    echo "   $repo_url"
    echo ""
    echo "🎯 下一步："
    echo "   1. 访问 https://railway.app"
    echo "   2. 点击 'New Project'"
    echo "   3. 选择 'Deploy from GitHub repo'"
    echo "   4. 选择 awsomeshop 仓库"
    echo ""
else
    echo ""
    echo "❌ 推送失败"
    echo ""
    echo "💡 常见问题："
    echo "   1. 确认Personal Access Token是否正确"
    echo "   2. 确认仓库URL是否正确"
    echo "   3. 确认token有repo权限"
    echo ""
    echo "🔄 重新运行脚本："
    echo "   ./push-to-github.sh"
fi

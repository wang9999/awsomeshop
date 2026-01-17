# 🚀 快速参考卡片

## 📦 GitHub 信息
- **仓库**: https://github.com/wang9999/awsomeshop
- **用户名**: wang9999
- **Token**: (已保存，不在此显示)

---

## 🚂 Railway 配置

### 服务配置
```
Root Directory: backend/src/AWSomeShop.API
Build Command: dotnet publish -c Release -o out
Start Command: dotnet out/AWSomeShop.API.dll
```

### 环境变量（复制粘贴）
```bash
ConnectionStrings__DefaultConnection=Server=${{MySQL.MYSQLHOST}};Port=${{MySQL.MYSQLPORT}};Database=${{MySQL.MYSQLDATABASE}};User=${{MySQL.MYSQLUSER}};Password=${{MySQL.MYSQLPASSWORD}}

Jwt__SecretKey=AWSomeShop2024SecretKeyForJWTTokenGeneration32CharactersLong
Jwt__Issuer=AWSomeShop
Jwt__Audience=AWSomeShop

Redis__Configuration=${{Redis.REDIS_URL}}

CORS__Origins=https://your-app.vercel.app

ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:$PORT
```

---

## 🎨 Vercel 配置

### 构建设置
```
Framework: Vite
Root Directory: frontend
Build Command: npm run build
Output Directory: dist
```

### 环境变量
```bash
VITE_API_BASE_URL=https://你的railway后端URL/api
```

---

## 🔐 测试账号

### 员工账号
```
邮箱: employee1@awsome.com
密码: Employee@123
```

### 管理员账号
```
用户名: superadmin
密码: Admin@123
```

---

## 📋 部署顺序

1. **Railway** (30分钟)
   - 创建项目 → 添加 MySQL → 添加 Redis
   - 配置服务 → 配置环境变量 → 部署
   - 生成域名 → 验证 /swagger

2. **Vercel** (10分钟)
   - 导入仓库 → 配置构建
   - 配置环境变量 → 部署
   - 获取域名

3. **更新 CORS** (5分钟)
   - Railway → Variables → CORS__Origins
   - 更新为 Vercel 域名 → 重新部署

4. **测试** (5分钟)
   - 访问 Vercel URL
   - 测试登录和功能

---

## 🔗 快速链接

- Railway: https://railway.app
- Vercel: https://vercel.com
- GitHub: https://github.com/wang9999/awsomeshop

---

## 💡 常见问题

### Railway 部署失败？
- 检查环境变量格式
- 查看部署日志
- 确认 MySQL 和 Redis 已创建

### 前端无法连接后端？
- 检查 VITE_API_BASE_URL
- 检查 CORS 配置
- 查看浏览器控制台

### 数据库连接失败？
- 确认 MySQL 服务已创建
- 检查连接字符串
- 查看 Railway 数据库状态

---

**提示**: 保存这个文件，部署时随时查看！

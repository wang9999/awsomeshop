# 🚀 Railway + Vercel 部署检查清单

## ✅ 准备阶段（10分钟）

### 1. 注册账号
- [ ] GitHub账号：https://github.com
- [ ] Railway账号：https://railway.app
- [ ] Vercel账号：https://vercel.com

### 2. 推送代码到GitHub
```bash
git init
git add .
git commit -m "Initial commit"
git remote add origin https://github.com/你的用户名/awsomeshop.git
git branch -M main
git push -u origin main
```

---

## 🚂 Railway部署（30分钟）

### 步骤1：创建Railway项目
- [ ] 访问 https://railway.app
- [ ] 点击 "New Project"
- [ ] 选择 "Deploy from GitHub repo"
- [ ] 选择 awsomeshop 仓库

### 步骤2：添加MySQL数据库
- [ ] 点击 "+ New" → "Database" → "Add MySQL"
- [ ] 等待创建完成
- [ ] 记录数据库连接信息

### 步骤3：添加Redis缓存
- [ ] 点击 "+ New" → "Database" → "Add Redis"
- [ ] 等待创建完成

### 步骤4：配置后端服务
- [ ] 进入后端服务 → "Settings"
- [ ] Root Directory: `backend/src/AWSomeShop.API`
- [ ] Build Command: `dotnet publish -c Release -o out`
- [ ] Start Command: `dotnet out/AWSomeShop.API.dll`

### 步骤5：配置环境变量
在 "Variables" 标签添加：

```bash
# 数据库（使用Railway内部变量）
ConnectionStrings__DefaultConnection=Server=${{MySQL.MYSQLHOST}};Port=${{MySQL.MYSQLPORT}};Database=${{MySQL.MYSQLDATABASE}};User=${{MySQL.MYSQLUSER}};Password=${{MySQL.MYSQLPASSWORD}}

# JWT（请修改密钥！）
Jwt__SecretKey=your-super-secret-key-at-least-32-characters-long
Jwt__Issuer=AWSomeShop
Jwt__Audience=AWSomeShop

# Redis
Redis__Configuration=${{Redis.REDIS_URL}}

# CORS（稍后更新）
CORS__Origins=https://your-app.vercel.app

# 环境
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:$PORT
```

### 步骤6：部署并获取URL
- [ ] 点击 "Deploy"
- [ ] 等待部署完成（3-5分钟）
- [ ] Settings → Networking → Generate Domain
- [ ] 复制后端URL：`https://______.up.railway.app`

### 步骤7：验证后端
- [ ] 访问：`https://你的railway域名/swagger`
- [ ] 确认API文档页面正常显示

---

## 🎨 Vercel部署（15分钟）

### 步骤1：创建Vercel项目
- [ ] 访问 https://vercel.com
- [ ] 点击 "Add New" → "Project"
- [ ] 导入 awsomeshop 仓库

### 步骤2：配置构建设置
- [ ] Framework Preset: Vite
- [ ] Root Directory: `frontend`
- [ ] Build Command: `npm run build`
- [ ] Output Directory: `dist`

### 步骤3：配置环境变量
```bash
VITE_API_BASE_URL=https://你的railway后端URL/api
```

### 步骤4：部署
- [ ] 点击 "Deploy"
- [ ] 等待部署完成（2-3分钟）
- [ ] 复制前端URL：`https://______.vercel.app`

---

## 🔗 最后配置（5分钟）

### 更新CORS
- [ ] 回到Railway后端服务
- [ ] Variables → 找到 `CORS__Origins`
- [ ] 更新为：`https://你的vercel域名.vercel.app`
- [ ] 保存并等待重新部署

---

## ✅ 测试验证（5分钟）

### 访问应用
- [ ] 打开：`https://你的vercel域名.vercel.app`
- [ ] 页面正常加载

### 测试登录
- [ ] 员工登录：employee1@awsome.com / Employee@123
- [ ] 管理员登录：superadmin / Admin@123

### 验证功能
- [ ] 浏览产品列表
- [ ] 查看产品详情
- [ ] 添加到购物车
- [ ] 查看积分明细
- [ ] 查看兑换历史

---

## 🎉 完成！

### 你的应用地址
- **前端**：https://______.vercel.app
- **后端**：https://______.up.railway.app

### 分享给别人
把前端URL分享给任何人都可以访问！

---

## 📝 注意事项

### Railway免费额度
- $5/月免费额度
- 预计使用：$3-5/月
- 第1-2个月完全免费

### 如何查看使用量
1. Railway控制台 → 项目
2. 查看 "Usage" 标签
3. 监控费用使用情况

### 如果超出免费额度
1. 绑定信用卡继续使用（$5/月）
2. 或迁移到Render（完全免费但会休眠）

---

## 🆘 遇到问题？

### 后端部署失败
- 检查环境变量是否正确
- 查看Railway部署日志
- 确认代码已推送到GitHub

### 前端无法连接后端
- 检查VITE_API_BASE_URL是否正确
- 检查CORS配置是否包含Vercel域名
- 查看浏览器控制台错误信息

### 数据库连接失败
- 确认MySQL服务已创建
- 检查连接字符串格式
- 查看Railway数据库状态

---

## 📚 详细文档

完整步骤请查看：`RAILWAY_VERCEL_DEPLOY.md`

---

**预计总时间**：60分钟  
**难度**：⭐⭐ 中等  
**成本**：第1-2个月免费，之后可能$0-5/月

# 🚀 Railway + Vercel 部署指南（电脑可以休眠）

## 为什么选择这个方案？

✅ **完全免费**（或$5/月，有免费额度）  
✅ **24/7在线**（电脑可以关机、休眠）  
✅ **自动HTTPS**（安全访问）  
✅ **专业可靠**（适合作品集展示）  
✅ **部署一次，永久访问**

---

## 📋 准备工作（10分钟）

### 1. 注册账号

需要注册以下3个免费账号：

1. **GitHub** - https://github.com
   - 用于托管代码
   
2. **Railway** - https://railway.app
   - 用于部署后端+数据库
   - 提供$5免费额度（够用1-2个月）
   
3. **Vercel** - https://vercel.com
   - 用于部署前端
   - 完全免费

### 2. 推送代码到GitHub

```bash
# 在项目根目录执行
git init
git add .
git commit -m "Initial commit"

# 在GitHub创建新仓库后
git remote add origin https://github.com/你的用户名/awsomeshop.git
git branch -M main
git push -u origin main
```

---

## 🎯 第一步：部署后端到Railway（20分钟）

### 1.1 创建Railway项目

1. 访问 https://railway.app
2. 点击 **"New Project"**
3. 选择 **"Deploy from GitHub repo"**
4. 授权GitHub访问
5. 选择你的 **awsomeshop** 仓库

### 1.2 添加MySQL数据库

1. 在Railway项目页面，点击 **"+ New"**
2. 选择 **"Database"** → **"Add MySQL"**
3. 等待数据库创建完成（约1分钟）
4. 点击MySQL服务，进入 **"Variables"** 标签
5. 复制以下变量值（稍后会用到）：
   - `MYSQLHOST`
   - `MYSQLPORT`
   - `MYSQLDATABASE`
   - `MYSQLUSER`
   - `MYSQLPASSWORD`

### 1.3 添加Redis缓存

1. 在Railway项目页面，点击 **"+ New"**
2. 选择 **"Database"** → **"Add Redis"**
3. 等待Redis创建完成（约1分钟）
4. 点击Redis服务，进入 **"Variables"** 标签
5. 复制 `REDIS_URL` 的值

### 1.4 配置后端服务

1. 点击你的后端服务（awsomeshop仓库）
2. 进入 **"Settings"** 标签
3. 找到 **"Root Directory"**，设置为：
   ```
   backend/src/AWSomeShop.API
   ```

4. 找到 **"Build Command"**，设置为：
   ```
   dotnet publish -c Release -o out
   ```

5. 找到 **"Start Command"**，设置为：
   ```
   dotnet out/AWSomeShop.API.dll
   ```

### 1.5 配置环境变量

在后端服务的 **"Variables"** 标签，添加以下变量：

```bash
# 数据库连接（使用Railway提供的内部地址）
ConnectionStrings__DefaultConnection=Server=${{MySQL.MYSQLHOST}};Port=${{MySQL.MYSQLPORT}};Database=${{MySQL.MYSQLDATABASE}};User=${{MySQL.MYSQLUSER}};Password=${{MySQL.MYSQLPASSWORD}}

# JWT配置（重要：请修改为你自己的密钥）
Jwt__SecretKey=your-super-secret-key-at-least-32-characters-long-change-this
Jwt__Issuer=AWSomeShop
Jwt__Audience=AWSomeShop

# Redis配置
Redis__Configuration=${{Redis.REDIS_URL}}

# CORS配置（稍后会更新）
CORS__Origins=https://your-app.vercel.app

# 环境
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:$PORT
```

**重要提示**：
- `Jwt__SecretKey` 必须至少32个字符，请修改为你自己的密钥
- `CORS__Origins` 稍后会更新为Vercel提供的域名

### 1.6 部署后端

1. 点击 **"Deploy"** 按钮
2. 等待部署完成（约3-5分钟）
3. 部署成功后，点击 **"Settings"** → **"Networking"**
4. 点击 **"Generate Domain"** 生成公网域名
5. 复制生成的域名，例如：
   ```
   https://awsomeshop-production.up.railway.app
   ```

### 1.7 验证后端

访问：`https://你的railway域名/swagger`

应该能看到API文档页面。

---

## 🎨 第二步：部署前端到Vercel（10分钟）

### 2.1 创建Vercel项目

1. 访问 https://vercel.com
2. 点击 **"Add New"** → **"Project"**
3. 点击 **"Import Git Repository"**
4. 选择你的 **awsomeshop** 仓库
5. 点击 **"Import"**

### 2.2 配置构建设置

在配置页面：

1. **Framework Preset**: 选择 **Vite**
2. **Root Directory**: 点击 **"Edit"**，输入 `frontend`
3. **Build Command**: 保持默认 `npm run build`
4. **Output Directory**: 保持默认 `dist`

### 2.3 配置环境变量

在 **"Environment Variables"** 部分，添加：

```bash
VITE_API_BASE_URL=https://你的railway后端域名/api
```

例如：
```bash
VITE_API_BASE_URL=https://awsomeshop-production.up.railway.app/api
```

### 2.4 部署前端

1. 点击 **"Deploy"** 按钮
2. 等待部署完成（约2-3分钟）
3. 部署成功后，Vercel会提供一个域名，例如：
   ```
   https://awsomeshop.vercel.app
   ```

---

## 🔗 第三步：更新CORS配置（5分钟）

### 3.1 更新Railway后端环境变量

1. 回到Railway项目
2. 点击后端服务
3. 进入 **"Variables"** 标签
4. 找到 `CORS__Origins` 变量
5. 更新为你的Vercel域名：
   ```
   https://awsomeshop.vercel.app
   ```

### 3.2 重新部署后端

1. 点击 **"Deploy"** 按钮
2. 等待重新部署完成（约2分钟）

---

## ✅ 第四步：测试访问（5分钟）

### 4.1 访问应用

打开浏览器，访问你的Vercel域名：
```
https://awsomeshop.vercel.app
```

### 4.2 测试登录

使用测试账号登录：

**员工账号**：
- 邮箱：employee1@awsome.com
- 密码：Employee@123

**管理员账号**：
- 用户名：superadmin
- 密码：Admin@123

### 4.3 验证功能

- ✅ 浏览产品列表
- ✅ 查看产品详情
- ✅ 添加到购物车
- ✅ 查看积分明细
- ✅ 查看兑换历史

---

## 🎉 完成！

现在你的应用已经部署到云端了！

### 📱 分享链接

你可以把这个链接分享给任何人：
```
https://awsomeshop.vercel.app
```

### ✨ 优势

- ✅ **24/7在线** - 电脑可以关机、休眠
- ✅ **自动HTTPS** - 安全访问
- ✅ **全球CDN** - 访问速度快
- ✅ **自动部署** - 推送代码自动更新
- ✅ **免费或低成本** - Railway $5免费额度

---

## 🔄 如何更新代码？

### 方法1：通过Git推送（推荐）

```bash
# 修改代码后
git add .
git commit -m "更新说明"
git push

# Railway和Vercel会自动检测并重新部署
```

### 方法2：手动触发部署

在Railway或Vercel控制台点击 **"Redeploy"** 按钮

---

## 💰 成本说明

### Railway
- **免费额度**：$5/月
- **使用情况**：
  - 后端服务：~$3/月
  - MySQL数据库：~$1/月
  - Redis缓存：~$1/月
- **总计**：约$5/月（在免费额度内）

### Vercel
- **完全免费**
- 无限制的前端托管

### 总成本
- **第一个月**：免费（使用Railway $5额度）
- **之后**：$5/月（或继续免费，如果用量小）

---

## 🆘 常见问题

### Q: Railway免费额度用完了怎么办？
A: 
1. 可以绑定信用卡继续使用（$5/月）
2. 或者迁移到其他免费平台（Render、Fly.io）

### Q: 如何查看日志？
A: 
- Railway：点击服务 → "Deployments" → 查看日志
- Vercel：点击项目 → "Deployments" → 查看日志

### Q: 数据库数据会丢失吗？
A: 
- Railway的数据库是持久化的，不会丢失
- 建议定期备份重要数据

### Q: 可以绑定自定义域名吗？
A: 
- Vercel：免费支持自定义域名
- Railway：付费版支持自定义域名

### Q: 部署失败怎么办？
A: 
1. 检查环境变量是否正确
2. 查看部署日志找到错误信息
3. 确保代码已推送到GitHub

---

## 📚 相关资源

- [Railway文档](https://docs.railway.app)
- [Vercel文档](https://vercel.com/docs)
- [Railway社区](https://railway.app/discord)
- [Vercel社区](https://vercel.com/discord)

---

## 🎯 下一步优化

部署完成后，你可以：

1. **绑定自定义域名**（可选）
2. **配置CI/CD自动测试**（可选）
3. **添加监控和告警**（可选）
4. **优化性能和缓存**（可选）

---

**创建时间**: 2026-01-16  
**预计部署时间**: 45分钟  
**难度**: ⭐⭐ 中等  
**推荐指数**: ⭐⭐⭐⭐⭐

# 🚀 下一步：Railway + Vercel 部署

## ✅ 已完成
- GitHub 仓库创建：https://github.com/wang9999/awsomeshop
- 代码已推送（224个文件）

---

## 📋 接下来的步骤（约45分钟）

### 步骤 1：部署后端到 Railway（30分钟）

#### 1.1 创建 Railway 项目（5分钟）
1. 访问：https://railway.app
2. 使用 GitHub 登录
3. 点击 **"New Project"**
4. 选择 **"Deploy from GitHub repo"**
5. 选择 **wang9999/awsomeshop** 仓库

#### 1.2 添加数据库（5分钟）
1. 点击 **"+ New"** → **"Database"** → **"Add MySQL"**
2. 等待 MySQL 创建完成
3. 点击 **"+ New"** → **"Database"** → **"Add Redis"**
4. 等待 Redis 创建完成

#### 1.3 配置后端服务（10分钟）
1. 点击后端服务（awsomeshop）
2. 进入 **"Settings"** 标签
3. 配置以下内容：

**Root Directory**:
```
backend/src/AWSomeShop.API
```

**Build Command**:
```
dotnet publish -c Release -o out
```

**Start Command**:
```
dotnet out/AWSomeShop.API.dll
```

#### 1.4 配置环境变量（10分钟）
进入 **"Variables"** 标签，添加以下变量：

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

**注意**：
- `CORS__Origins` 稍后会更新为 Vercel 域名
- `Jwt__SecretKey` 已设置为安全的32字符密钥

#### 1.5 部署并获取 URL
1. 点击 **"Deploy"**
2. 等待部署完成（3-5分钟）
3. 进入 **"Settings"** → **"Networking"**
4. 点击 **"Generate Domain"**
5. **复制后端 URL**（例如：`https://awsomeshop-production.up.railway.app`）

#### 1.6 验证后端
访问：`https://你的railway域名/swagger`

应该能看到 API 文档页面。

---

### 步骤 2：部署前端到 Vercel（10分钟）

#### 2.1 创建 Vercel 项目（3分钟）
1. 访问：https://vercel.com
2. 使用 GitHub 登录
3. 点击 **"Add New"** → **"Project"**
4. 选择 **wang9999/awsomeshop** 仓库
5. 点击 **"Import"**

#### 2.2 配置构建设置（2分钟）
- **Framework Preset**: Vite
- **Root Directory**: `frontend`
- **Build Command**: `npm run build`
- **Output Directory**: `dist`

#### 2.3 配置环境变量（2分钟）
在 **"Environment Variables"** 部分添加：

```bash
VITE_API_BASE_URL=https://你的railway后端URL/api
```

例如：
```bash
VITE_API_BASE_URL=https://awsomeshop-production.up.railway.app/api
```

#### 2.4 部署
1. 点击 **"Deploy"**
2. 等待部署完成（2-3分钟）
3. **复制前端 URL**（例如：`https://awsomeshop.vercel.app`）

---

### 步骤 3：更新 CORS 配置（5分钟）

#### 3.1 更新 Railway 环境变量
1. 回到 Railway 项目
2. 点击后端服务
3. 进入 **"Variables"** 标签
4. 找到 `CORS__Origins`
5. 更新为你的 Vercel 域名：
   ```
   https://awsomeshop.vercel.app
   ```
6. 保存

#### 3.2 重新部署
1. 点击 **"Deploy"** 或等待自动重新部署
2. 等待完成（约2分钟）

---

### 步骤 4：测试访问（5分钟）

#### 4.1 访问应用
打开浏览器，访问你的 Vercel URL：
```
https://awsomeshop.vercel.app
```

#### 4.2 测试登录
**员工账号**：
- 邮箱：employee1@awsome.com
- 密码：Employee@123

**管理员账号**：
- 用户名：superadmin
- 密码：Admin@123

#### 4.3 验证功能
- ✅ 浏览产品列表
- ✅ 查看产品详情
- ✅ 添加到购物车
- ✅ 查看积分明细
- ✅ 查看兑换历史

---

## 🎉 完成！

部署成功后，你会得到：

1. **GitHub 仓库**：https://github.com/wang9999/awsomeshop
2. **Railway 后端**：https://______.up.railway.app
3. **Vercel 前端**：https://______.vercel.app

可以把 Vercel URL 分享给任何人访问！

---

## 💰 费用说明

- **GitHub**：完全免费
- **Vercel**：完全免费
- **Railway**：
  - 第1-2个月：免费（$5额度）
  - 之后：约$3-5/月

---

## 🆘 遇到问题？

### Railway 部署失败
- 检查环境变量是否正确
- 查看 Railway 部署日志
- 确认 MySQL 和 Redis 已创建

### 前端无法连接后端
- 检查 `VITE_API_BASE_URL` 是否正确
- 检查 CORS 配置是否包含 Vercel 域名
- 查看浏览器控制台错误信息

### 数据库连接失败
- 确认 MySQL 服务已创建
- 检查连接字符串格式
- 查看 Railway 数据库状态

---

## 📚 详细文档

- `RAILWAY_VERCEL_DEPLOY.md` - 完整部署指南
- `DEPLOYMENT_CHECKLIST.md` - 检查清单
- `GITHUB_SETUP_COMPLETE.md` - GitHub 设置记录

---

**创建时间**: 2026-01-17  
**预计时间**: 45分钟  
**难度**: ⭐⭐ 中等

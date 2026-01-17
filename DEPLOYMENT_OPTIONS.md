# AWSomeShop 部署方案指南

## 🎯 方案选择

### 方案1：免费快速演示（推荐新手）⭐

**总成本**：完全免费  
**部署时间**：30-60分钟  
**适合场景**：展示、演示、测试

#### 架构
```
前端 (Vercel) → 后端 (Railway) → 数据库 (Railway MySQL)
                                  → 缓存 (Railway Redis)
```

#### 步骤概览
1. 前端部署到 Vercel（免费）
2. 后端+数据库部署到 Railway（$5免费额度）
3. 配置环境变量
4. 测试访问

---

### 方案2：本地网络穿透（最简单）⭐⭐

**总成本**：免费  
**部署时间**：5分钟  
**适合场景**：临时展示、内网穿透

使用 **ngrok** 或 **localtunnel** 将本地服务暴露到公网

#### 优点
- ✅ 最简单，无需部署
- ✅ 完全免费
- ✅ 保持本地开发环境

#### 缺点
- ⚠️ 只能临时使用
- ⚠️ 需要保持电脑运行
- ⚠️ URL会变化

---

### 方案3：云服务器完整部署

**总成本**：¥100-300/月  
**部署时间**：2-4小时  
**适合场景**：生产环境、长期运行

---

## 🚀 方案1详细步骤：免费快速演示

### 准备工作

1. **注册账号**
   - [Vercel](https://vercel.com) - 前端托管
   - [Railway](https://railway.app) - 后端+数据库托管
   - GitHub账号（用于代码托管）

2. **推送代码到GitHub**
   ```bash
   git init
   git add .
   git commit -m "Initial commit"
   git remote add origin https://github.com/你的用户名/awsomeshop.git
   git push -u origin main
   ```

---

### 步骤1：部署后端到Railway

#### 1.1 创建Railway项目
1. 访问 https://railway.app
2. 点击 "New Project"
3. 选择 "Deploy from GitHub repo"
4. 选择你的 AWSomeShop 仓库

#### 1.2 添加MySQL数据库
1. 在项目中点击 "+ New"
2. 选择 "Database" → "MySQL"
3. 等待数据库创建完成
4. 记录数据库连接信息

#### 1.3 添加Redis缓存
1. 在项目中点击 "+ New"
2. 选择 "Database" → "Redis"
3. 等待Redis创建完成

#### 1.4 配置后端环境变量
在Railway后端服务中添加以下环境变量：

```bash
# 数据库配置（从Railway MySQL获取）
ConnectionStrings__DefaultConnection=Server=mysql.railway.internal;Port=3306;Database=railway;User=root;Password=你的密码

# JWT配置
Jwt__SecretKey=你的超长密钥至少32个字符
Jwt__Issuer=AWSomeShop
Jwt__Audience=AWSomeShop

# Redis配置（从Railway Redis获取）
Redis__Configuration=redis.railway.internal:6379

# CORS配置
CORS__Origins=https://你的vercel域名.vercel.app

# 环境
ASPNETCORE_ENVIRONMENT=Production
```

#### 1.5 配置构建命令
在Railway设置中：
- **Root Directory**: `backend/src/AWSomeShop.API`
- **Build Command**: `dotnet publish -c Release -o out`
- **Start Command**: `dotnet out/AWSomeShop.API.dll`

#### 1.6 获取后端URL
部署完成后，Railway会提供一个URL，例如：
```
https://awsomeshop-production.up.railway.app
```

---

### 步骤2：部署前端到Vercel

#### 2.1 创建Vercel项目
1. 访问 https://vercel.com
2. 点击 "Add New" → "Project"
3. 导入你的GitHub仓库
4. 选择 "frontend" 目录作为根目录

#### 2.2 配置构建设置
- **Framework Preset**: Vite
- **Root Directory**: `frontend`
- **Build Command**: `npm run build`
- **Output Directory**: `dist`

#### 2.3 配置环境变量
在Vercel项目设置中添加：

```bash
VITE_API_BASE_URL=https://你的railway后端URL.up.railway.app/api
```

#### 2.4 部署
点击 "Deploy"，等待部署完成

#### 2.5 获取前端URL
部署完成后，Vercel会提供一个URL，例如：
```
https://awsomeshop.vercel.app
```

---

### 步骤3：更新CORS配置

回到Railway后端，更新环境变量：
```bash
CORS__Origins=https://awsomeshop.vercel.app
```

重新部署后端服务。

---

### 步骤4：测试访问

1. 访问你的Vercel URL
2. 使用测试账号登录：
   - 员工：employee1@awsome.com / Employee@123
   - 管理员：superadmin / Admin@123

---

## 🔧 方案2详细步骤：本地网络穿透（最简单）

### 使用ngrok

#### 1. 安装ngrok
```bash
# macOS
brew install ngrok

# 或下载：https://ngrok.com/download
```

#### 2. 注册并获取token
1. 访问 https://ngrok.com
2. 注册账号
3. 获取 authtoken

#### 3. 配置ngrok
```bash
ngrok config add-authtoken 你的token
```

#### 4. 启动后端穿透
```bash
# 在新终端窗口运行
ngrok http 5144
```

会得到一个公网URL，例如：
```
https://abc123.ngrok.io
```

#### 5. 更新前端配置
修改 `frontend/.env.development`：
```bash
VITE_API_BASE_URL=https://abc123.ngrok.io/api
```

#### 6. 重启前端
```bash
cd frontend
npm run dev
```

#### 7. 启动前端穿透
```bash
# 在另一个新终端窗口运行
ngrok http 5173
```

会得到另一个公网URL，例如：
```
https://xyz789.ngrok.io
```

#### 8. 分享链接
将前端的ngrok URL分享给别人即可访问！

**注意**：
- 免费版ngrok每次重启URL会变化
- 需要保持电脑和服务运行
- 适合临时演示

---

## 📊 方案对比

| 方案 | 成本 | 难度 | 稳定性 | 适合场景 |
|------|------|------|--------|----------|
| 本地穿透(ngrok) | 免费 | ⭐ | ⭐⭐ | 临时演示 |
| Railway+Vercel | 免费/$5 | ⭐⭐ | ⭐⭐⭐⭐ | 展示/测试 |
| 云服务器 | ¥100+/月 | ⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ | 生产环境 |

---

## 🎯 推荐选择

### 如果你想：
- **快速展示给朋友看** → 使用 ngrok（5分钟搞定）
- **做作品集展示** → 使用 Railway + Vercel（免费，专业）
- **实际生产使用** → 租用云服务器（稳定可靠）

---

## 💡 下一步

我可以帮你：
1. 创建详细的ngrok部署脚本（最快）
2. 创建Railway+Vercel部署指南（最专业）
3. 创建Docker部署配置（最灵活）

你想选择哪个方案？我可以提供详细的操作步骤！

---

## 📚 相关资源

- [Vercel文档](https://vercel.com/docs)
- [Railway文档](https://docs.railway.app)
- [ngrok文档](https://ngrok.com/docs)
- [Docker文档](https://docs.docker.com)

---

**创建时间**: 2026-01-16  
**项目**: AWSomeShop 员工积分兑换平台

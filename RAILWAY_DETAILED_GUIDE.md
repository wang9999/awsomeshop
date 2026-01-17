# 🚂 Railway 详细配置指南

## 📍 配置位置说明

### 第一步：找到你的后端服务

1. 登录 Railway：https://railway.app
2. 进入你的项目（awsomeshop）
3. 你会看到 3 个服务：
   - **awsomeshop**（后端服务）⭐ - 这是我们要配置的
   - **MySQL**（数据库）
   - **Redis**（缓存）

### 第二步：进入后端服务设置

点击 **awsomeshop** 服务，你会看到顶部有几个标签：
- Deployments
- **Settings** ⭐ - 在这里配置构建命令
- **Variables** ⭐ - 在这里配置环境变量
- Metrics
- Logs

---

## ⚙️ Settings 标签配置

### 位置
```
点击 awsomeshop 服务 → Settings 标签
```

### 需要配置的内容

#### 1. Service Name（可选）
保持默认或改为 `awsomeshop-backend`

#### 2. Root Directory ⭐ 重要
```
backend/src/AWSomeShop.API
```

**说明**：告诉 Railway 从哪个目录开始构建

#### 3. Build Command ⭐ 重要
```
dotnet publish -c Release -o out
```

**说明**：如何构建 .NET 项目

#### 4. Start Command ⭐ 重要
```
dotnet out/AWSomeShop.API.dll
```

**说明**：如何启动应用

#### 5. Watch Paths（可选）
```
backend/**
```

**说明**：监控哪些文件变化时自动重新部署

#### 6. Custom Build Command（通常不需要）
留空即可

---

## 🔧 Variables 标签配置

### 位置
```
点击 awsomeshop 服务 → Variables 标签
```

### 需要配置的内容

点击 **"New Variable"** 或 **"Raw Editor"**，然后添加以下环境变量：

#### 方式1：使用 Raw Editor（推荐）⭐

1. 点击 **"Raw Editor"** 按钮
2. 复制 `railway-env-vars.txt` 的全部内容
3. 粘贴到编辑器中
4. 点击 **"Update Variables"**

#### 方式2：逐个添加

逐个点击 **"New Variable"**，添加以下变量：

```bash
# 1. 数据库连接
Variable: ConnectionStrings__DefaultConnection
Value: Server=${{MySQL.MYSQLHOST}};Port=${{MySQL.MYSQLPORT}};Database=${{MySQL.MYSQLDATABASE}};User=${{MySQL.MYSQLUSER}};Password=${{MySQL.MYSQLPASSWORD}}

# 2. JWT 密钥
Variable: Jwt__SecretKey
Value: AWSomeShop2024SecretKeyForJWTTokenGeneration32CharactersLong

# 3. JWT Issuer
Variable: Jwt__Issuer
Value: AWSomeShop

# 4. JWT Audience
Variable: Jwt__Audience
Value: AWSomeShop

# 5. Redis 配置
Variable: Redis__Configuration
Value: ${{Redis.REDIS_URL}}

# 6. CORS 配置（稍后更新）
Variable: CORS__Origins
Value: https://your-app.vercel.app

# 7. 环境
Variable: ASPNETCORE_ENVIRONMENT
Value: Production

# 8. URLs
Variable: ASPNETCORE_URLS
Value: http://0.0.0.0:$PORT
```

---

## 🎯 完整操作流程

### 步骤1：配置 Settings

1. 点击 **awsomeshop** 服务
2. 点击 **Settings** 标签
3. 向下滚动找到 **Build & Deploy** 部分
4. 填写：
   - Root Directory: `backend/src/AWSomeShop.API`
   - Build Command: `dotnet publish -c Release -o out`
   - Start Command: `dotnet out/AWSomeShop.API.dll`
5. 点击页面底部的 **"Save"** 或 **"Update"**

### 步骤2：配置 Variables

1. 点击 **Variables** 标签
2. 点击 **"Raw Editor"** 按钮
3. 打开 `railway-env-vars.txt` 文件
4. 复制所有内容
5. 粘贴到 Railway 的 Raw Editor
6. 点击 **"Update Variables"**

### 步骤3：部署

1. 点击 **Deployments** 标签
2. 点击 **"Deploy"** 按钮（或等待自动部署）
3. 等待 3-5 分钟
4. 查看部署日志，确认成功

### 步骤4：获取 URL

1. 点击 **Settings** 标签
2. 向下滚动找到 **Networking** 部分
3. 点击 **"Generate Domain"**
4. 复制生成的 URL（例如：`https://awsomeshop-production.up.railway.app`）

### 步骤5：验证

访问：`https://你的railway域名/swagger`

应该能看到 API 文档页面。

---

## 💡 重要提示

### ✅ 使用 Railway 内部变量

注意这些特殊的变量格式：
- `${{MySQL.MYSQLHOST}}` - Railway 会自动替换为 MySQL 主机地址
- `${{MySQL.MYSQLPORT}}` - Railway 会自动替换为 MySQL 端口
- `${{Redis.REDIS_URL}}` - Railway 会自动替换为 Redis URL
- `$PORT` - Railway 会自动分配端口

**不要**手动填写这些值，保持原样即可！

### ✅ CORS 配置

`CORS__Origins` 的值 `https://your-app.vercel.app` 是占位符。

在 Vercel 部署完成后，你需要：
1. 回到 Railway
2. 进入 Variables 标签
3. 找到 `CORS__Origins`
4. 更新为你的 Vercel 域名（例如：`https://awsomeshop.vercel.app`）
5. 保存

### ✅ 双下划线

注意环境变量名中的双下划线 `__`：
- `ConnectionStrings__DefaultConnection` ✅
- `Jwt__SecretKey` ✅
- `ASPNETCORE_ENVIRONMENT` ✅（单下划线）

这是 .NET 的配置约定，不要改成单下划线！

---

## 🆘 常见问题

### Q1: 找不到 Settings 标签？
**A**: 确保你点击的是 **awsomeshop 服务**（后端），而不是 MySQL 或 Redis。

### Q2: 部署失败？
**A**: 
1. 检查 Root Directory 是否正确：`backend/src/AWSomeShop.API`
2. 检查 Build Command 是否正确：`dotnet publish -c Release -o out`
3. 查看 Logs 标签的错误信息

### Q3: 环境变量太多，一个个添加太麻烦？
**A**: 使用 **Raw Editor** 功能，可以一次性粘贴所有变量！

### Q4: 数据库连接失败？
**A**: 
1. 确认 MySQL 服务已创建
2. 确认环境变量中使用了 `${{MySQL.MYSQLHOST}}` 等内部变量
3. 不要手动填写数据库地址

---

## 📸 界面参考

### Settings 标签界面
```
┌─────────────────────────────────────┐
│ awsomeshop                          │
├─────────────────────────────────────┤
│ Deployments | Settings | Variables  │ ← 点击 Settings
├─────────────────────────────────────┤
│                                     │
│ Service Name                        │
│ [awsomeshop-backend]                │
│                                     │
│ Root Directory                      │
│ [backend/src/AWSomeShop.API]        │ ← 填写这里
│                                     │
│ Build Command                       │
│ [dotnet publish -c Release -o out] │ ← 填写这里
│                                     │
│ Start Command                       │
│ [dotnet out/AWSomeShop.API.dll]    │ ← 填写这里
│                                     │
│ Watch Paths                         │
│ [backend/**]                        │ ← 可选
│                                     │
└─────────────────────────────────────┘
```

### Variables 标签界面
```
┌─────────────────────────────────────┐
│ awsomeshop                          │
├─────────────────────────────────────┤
│ Deployments | Settings | Variables  │ ← 点击 Variables
├─────────────────────────────────────┤
│                                     │
│ [New Variable] [Raw Editor]         │ ← 点击 Raw Editor
│                                     │
│ ┌─────────────────────────────────┐ │
│ │ ConnectionStrings__Default...   │ │
│ │ Jwt__SecretKey=AWSomeShop...    │ │ ← 粘贴所有变量
│ │ Jwt__Issuer=AWSomeShop          │ │
│ │ ...                             │ │
│ └─────────────────────────────────┘ │
│                                     │
│ [Update Variables]                  │ ← 点击保存
│                                     │
└─────────────────────────────────────┘
```

---

## ✅ 检查清单

配置完成后，确认以下内容：

- [ ] Settings 标签：
  - [ ] Root Directory: `backend/src/AWSomeShop.API`
  - [ ] Build Command: `dotnet publish -c Release -o out`
  - [ ] Start Command: `dotnet out/AWSomeShop.API.dll`

- [ ] Variables 标签：
  - [ ] ConnectionStrings__DefaultConnection
  - [ ] Jwt__SecretKey
  - [ ] Jwt__Issuer
  - [ ] Jwt__Audience
  - [ ] Redis__Configuration
  - [ ] CORS__Origins
  - [ ] ASPNETCORE_ENVIRONMENT
  - [ ] ASPNETCORE_URLS

- [ ] 部署：
  - [ ] 点击 Deploy 或等待自动部署
  - [ ] 查看 Logs 确认成功
  - [ ] 生成公网域名
  - [ ] 访问 /swagger 验证

---

**配置完成后，继续下一步：Vercel 部署！** 🚀

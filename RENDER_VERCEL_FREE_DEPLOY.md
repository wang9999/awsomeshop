# 🆓 Render + Vercel 完全免费部署指南

## ✨ 为什么选择这个方案？

✅ **完全免费** - 永久免费，无需信用卡  
✅ **24/7在线** - 电脑可以关机、休眠  
✅ **自动HTTPS** - 安全访问  
✅ **适合作品集** - 专业可靠  

## ⚠️ 注意事项

**Render免费版限制**：
- 15分钟无活动会自动休眠
- 休眠后首次访问需要等待30-60秒唤醒
- 适合作品集展示，不适合高频使用

**解决方案**：
- 可以使用定时ping服务保持唤醒（可选）
- 或者接受首次访问稍慢的情况

---

## 📋 准备工作（10分钟）

### 1. 注册账号

需要注册以下3个免费账号：

1. **GitHub** - https://github.com
   - 用于托管代码
   
2. **Render** - https://render.com
   - 用于部署后端+数据库
   - 完全免费，无需信用卡
   
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

## 🎯 第一步：部署数据库到Render（10分钟）

### 1.1 创建PostgreSQL数据库

⚠️ **重要**：Render免费版只支持PostgreSQL，不支持MySQL

1. 访问 https://render.com
2. 点击 **"New +"** → **"PostgreSQL"**
3. 填写信息：
   - **Name**: `awsomeshop-db`
   - **Database**: `awsomeshop`
   - **User**: `awsomeshop`
   - **Region**: 选择离你最近的（Singapore或Oregon）
   - **PostgreSQL Version**: 选择最新版本
   - **Plan**: 选择 **Free**

4. 点击 **"Create Database"**
5. 等待数据库创建完成（约2-3分钟）

### 1.2 获取数据库连接信息

数据库创建完成后，在数据库详情页面找到：
- **Internal Database URL**（用于后端连接）
- **External Database URL**（用于本地测试）

复制 **Internal Database URL**，格式类似：
```
postgres://awsomeshop:密码@dpg-xxx-a.oregon-postgres.render.com/awsomeshop
```

---

## 🔧 第二步：修改代码支持PostgreSQL（15分钟）

由于Render免费版只支持PostgreSQL，我们需要修改代码。

### 2.1 更新NuGet包

修改 `backend/src/AWSomeShop.API/AWSomeShop.API.csproj`：

```xml
<!-- 移除MySQL包 -->
<!-- <PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="8.0.2" /> -->

<!-- 添加PostgreSQL包 -->
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.2" />
```

### 2.2 更新DbContext配置

修改 `backend/src/AWSomeShop.API/Extensions/ServiceCollectionExtensions.cs`：

找到这一行：
```csharp
options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
```

替换为：
```csharp
options.UseNpgsql(connectionString);
```

### 2.3 更新数据库初始化

修改 `backend/src/AWSomeShop.API/Program.cs`：

找到：
```csharp
await context.Database.EnsureCreatedAsync();
```

替换为：
```csharp
await context.Database.MigrateAsync();
```

### 2.4 创建数据库迁移

```bash
cd backend/src/AWSomeShop.API

# 安装EF Core工具（如果还没安装）
dotnet tool install --global dotnet-ef

# 创建初始迁移
dotnet ef migrations add InitialCreate

# 这会在项目中创建 Migrations 文件夹
```

### 2.5 提交代码更改

```bash
git add .
git commit -m "Switch from MySQL to PostgreSQL for Render deployment"
git push
```

---

## 🚀 第三步：部署后端到Render（15分钟）

### 3.1 创建Web Service

1. 在Render控制台，点击 **"New +"** → **"Web Service"**
2. 选择 **"Build and deploy from a Git repository"**
3. 点击 **"Connect GitHub"** 并授权
4. 选择你的 **awsomeshop** 仓库
5. 点击 **"Connect"**

### 3.2 配置Web Service

填写以下信息：

**Basic Settings**:
- **Name**: `awsomeshop-api`
- **Region**: 选择与数据库相同的区域
- **Branch**: `main`
- **Root Directory**: `backend/src/AWSomeShop.API`
- **Runtime**: `.NET`
- **Build Command**: 
  ```bash
  dotnet publish -c Release -o out
  ```
- **Start Command**: 
  ```bash
  dotnet out/AWSomeShop.API.dll
  ```

**Plan**: 选择 **Free**

### 3.3 配置环境变量

在 **"Environment"** 标签，添加以下变量：

```bash
# 数据库连接（使用Render提供的Internal URL）
ConnectionStrings__DefaultConnection=postgres://awsomeshop:密码@dpg-xxx-a.oregon-postgres.render.com/awsomeshop

# JWT配置（重要：请修改为你自己的密钥）
Jwt__SecretKey=your-super-secret-key-at-least-32-characters-long-change-this-now
Jwt__Issuer=AWSomeShop
Jwt__Audience=AWSomeShop

# Redis配置（暂时禁用，Render免费版不支持Redis）
Redis__Configuration=

# CORS配置（稍后会更新）
CORS__Origins=https://your-app.vercel.app

# 环境
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:$PORT
```

**重要提示**：
- 将数据库URL替换为你的实际URL
- `Jwt__SecretKey` 必须至少32个字符

### 3.4 部署后端

1. 点击 **"Create Web Service"**
2. 等待部署完成（约5-10分钟）
3. 部署成功后，你会得到一个URL，例如：
   ```
   https://awsomeshop-api.onrender.com
   ```

### 3.5 验证后端

访问：`https://awsomeshop-api.onrender.com/swagger`

⚠️ **注意**：首次访问可能需要等待30-60秒（服务唤醒）

---

## 🎨 第四步：部署前端到Vercel（10分钟）

### 4.1 创建Vercel项目

1. 访问 https://vercel.com
2. 点击 **"Add New"** → **"Project"**
3. 点击 **"Import Git Repository"**
4. 选择你的 **awsomeshop** 仓库
5. 点击 **"Import"**

### 4.2 配置构建设置

在配置页面：

1. **Framework Preset**: 选择 **Vite**
2. **Root Directory**: 点击 **"Edit"**，输入 `frontend`
3. **Build Command**: 保持默认 `npm run build`
4. **Output Directory**: 保持默认 `dist`

### 4.3 配置环境变量

在 **"Environment Variables"** 部分，添加：

```bash
VITE_API_BASE_URL=https://awsomeshop-api.onrender.com/api
```

### 4.4 部署前端

1. 点击 **"Deploy"** 按钮
2. 等待部署完成（约2-3分钟）
3. 部署成功后，Vercel会提供一个域名，例如：
   ```
   https://awsomeshop.vercel.app
   ```

---

## 🔗 第五步：更新CORS配置（5分钟）

### 5.1 更新Render后端环境变量

1. 回到Render控制台
2. 进入你的Web Service
3. 点击 **"Environment"** 标签
4. 找到 `CORS__Origins` 变量
5. 更新为你的Vercel域名：
   ```
   https://awsomeshop.vercel.app
   ```
6. 点击 **"Save Changes"**

### 5.2 重新部署

Render会自动重新部署服务（约3-5分钟）

---

## ✅ 第六步：测试访问（5分钟）

### 6.1 访问应用

打开浏览器，访问你的Vercel域名：
```
https://awsomeshop.vercel.app
```

⚠️ **首次访问提示**：
- 如果后端休眠了，首次访问可能需要等待30-60秒
- 页面可能显示"加载中"或"网络错误"
- 等待一会儿，刷新页面即可

### 6.2 测试登录

使用测试账号登录：

**员工账号**：
- 邮箱：employee1@awsome.com
- 密码：Employee@123

**管理员账号**：
- 用户名：superadmin
- 密码：Admin@123

### 6.3 验证功能

- ✅ 浏览产品列表
- ✅ 查看产品详情
- ✅ 添加到购物车
- ✅ 查看积分明细
- ✅ 查看兑换历史

---

## 🎉 完成！

现在你的应用已经完全免费部署到云端了！

### 📱 分享链接

你可以把这个链接分享给任何人：
```
https://awsomeshop.vercel.app
```

### ✨ 优势

- ✅ **完全免费** - 永久免费
- ✅ **24/7在线** - 电脑可以关机
- ✅ **自动HTTPS** - 安全访问
- ✅ **全球CDN** - 访问速度快
- ✅ **自动部署** - 推送代码自动更新

---

## 🔄 如何更新代码？

### 方法1：通过Git推送（推荐）

```bash
# 修改代码后
git add .
git commit -m "更新说明"
git push

# Render和Vercel会自动检测并重新部署
```

### 方法2：手动触发部署

在Render或Vercel控制台点击 **"Manual Deploy"** 按钮

---

## 💡 优化建议

### 1. 保持服务唤醒（可选）

使用免费的定时ping服务，每10分钟访问一次你的API：

**推荐服务**：
- **UptimeRobot** - https://uptimerobot.com（免费）
- **Cron-job.org** - https://cron-job.org（免费）

**设置方法**：
1. 注册UptimeRobot
2. 添加监控：`https://awsomeshop-api.onrender.com/api/products`
3. 设置间隔：10分钟
4. 这样服务就不会休眠了

### 2. 自定义域名（可选）

Vercel免费支持自定义域名：
1. 在Vercel项目设置中添加域名
2. 按照提示配置DNS
3. 自动获得HTTPS证书

---

## 🆘 常见问题

### Q: 首次访问很慢怎么办？
A: 
- Render免费版15分钟无活动会休眠
- 首次访问需要30-60秒唤醒
- 可以使用UptimeRobot保持唤醒

### Q: Redis缓存怎么办？
A: 
- Render免费版不支持Redis
- 可以暂时禁用Redis功能
- 或者使用Redis Labs免费版（需要额外配置）

### Q: 数据库数据会丢失吗？
A: 
- Render免费PostgreSQL数据会保留
- 但90天不活动会被删除
- 建议定期备份重要数据

### Q: 可以升级到付费版吗？
A: 
- 可以随时升级
- Render付费版：$7/月起
- 付费版无休眠，性能更好

### Q: 部署失败怎么办？
A: 
1. 检查环境变量是否正确
2. 查看Render部署日志
3. 确保代码已推送到GitHub
4. 检查PostgreSQL连接字符串

---

## 📊 性能对比

| 指标 | Render免费版 | Railway免费版 | 云服务器 |
|------|-------------|--------------|---------|
| 成本 | 完全免费 | $5/月额度 | ¥100+/月 |
| 休眠 | 15分钟后休眠 | 不休眠 | 不休眠 |
| 唤醒时间 | 30-60秒 | - | - |
| 数据库 | PostgreSQL | MySQL/PostgreSQL | 任意 |
| Redis | 不支持 | 支持 | 支持 |
| 适合场景 | 作品集展示 | 中度使用 | 生产环境 |

---

## 📚 相关资源

- [Render文档](https://render.com/docs)
- [Vercel文档](https://vercel.com/docs)
- [PostgreSQL文档](https://www.postgresql.org/docs/)
- [Npgsql文档](https://www.npgsql.org/doc/)

---

## 🎯 下一步

部署完成后，你可以：

1. **添加定时ping** - 保持服务唤醒
2. **绑定自定义域名** - 更专业
3. **添加Redis缓存** - 使用Redis Labs免费版
4. **优化性能** - 添加CDN、压缩等

---

**创建时间**: 2026-01-16  
**预计部署时间**: 60分钟  
**难度**: ⭐⭐⭐ 中等  
**成本**: 🆓 完全免费  
**推荐指数**: ⭐⭐⭐⭐

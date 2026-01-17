# AWSomeShop 快速启动指南

## 当前状态

✅ **前端服务**: 正在运行 (http://localhost:5173/)  
✅ **后端代码**: 所有编译错误已修复  
❌ **数据库服务**: 需要安装Docker  

## 选项1：完整功能（推荐）- 需要Docker

### 第1步：安装Docker Desktop

1. **下载Docker Desktop**
   - 打开浏览器访问：https://www.docker.com/products/docker-desktop/
   - 点击 "Download for Mac"
   - 根据你的Mac芯片选择版本：
     - 如果是M1/M2/M3芯片：选择 "Mac with Apple chip"
     - 如果是Intel芯片：选择 "Mac with Intel chip"

2. **安装Docker Desktop**
   - 下载完成后，打开 `.dmg` 文件
   - 将Docker图标拖到Applications文件夹
   - 从Applications文件夹打开Docker
   - 首次启动会要求输入密码，输入你的Mac密码
   - 等待Docker启动完成（顶部菜单栏会显示Docker图标）

3. **验证安装**
   ```bash
   docker --version
   docker-compose --version
   ```

### 第2步：启动数据库服务

在项目根目录执行：

```bash
# 方式1：使用启动脚本（推荐）
./start-dev.sh

# 方式2：手动启动
docker-compose up -d
```

等待几秒钟，让MySQL和Redis完全启动。

### 第3步：启动后端服务

打开新的终端窗口：

```bash
cd backend/src/AWSomeShop.API
~/.dotnet/dotnet run
```

后端会自动：
- 连接MySQL数据库
- 创建所有数据表
- 填充测试数据（产品、员工、管理员）
- 启动API服务

### 第4步：登录测试

前端已经在运行，访问：http://localhost:5173/

**员工登录**
- 邮箱：employee1@awsome.com
- 密码：Employee@123
- 积分：2000

**管理员登录**
- 用户名：superadmin
- 密码：Admin@123
- 权限：超级管理员

---

## 选项2：仅前端预览（临时方案）

如果暂时不想安装Docker，可以先体验前端UI：

### 当前可用功能

✅ 查看所有页面布局和设计  
✅ 测试响应式设计（调整浏览器窗口大小）  
✅ 查看UI组件和交互效果  
❌ 无法登录（需要后端API）  
❌ 无法加载真实数据  

### 如何预览

1. 前端已在运行：http://localhost:5173/
2. 可以访问的页面：
   - 登录页面：http://localhost:5173/login
   - 管理员登录：http://localhost:5173/admin/login
   - 产品列表：http://localhost:5173/products（会显示加载状态）

---

## 选项3：使用Homebrew安装（不推荐）

如果不想使用Docker，可以手动安装MySQL和Redis：

### 安装Homebrew（如果还没有）

```bash
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
```

### 安装MySQL

```bash
brew install mysql
brew services start mysql

# 设置root密码
mysql_secure_installation

# 创建数据库
mysql -uroot -p
CREATE DATABASE awsomeshop_dev CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
EXIT;
```

### 安装Redis

```bash
brew install redis
brew services start redis
```

### 更新配置

编辑 `backend/src/AWSomeShop.API/appsettings.Development.json`，确保密码正确：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=awsomeshop_dev;User=root;Password=你设置的密码;CharSet=utf8mb4;",
    "Redis": "localhost:6379"
  }
}
```

### 启动后端

```bash
cd backend/src/AWSomeShop.API
~/.dotnet/dotnet run
```

---

## 故障排除

### Docker相关

**问题：Docker命令找不到**
```bash
# 确认Docker Desktop已启动
# 查看顶部菜单栏是否有Docker图标
```

**问题：端口被占用**
```bash
# 查看占用端口的进程
lsof -i :3306  # MySQL
lsof -i :6379  # Redis

# 停止占用端口的进程
kill -9 <PID>
```

**问题：容器启动失败**
```bash
# 查看详细日志
docker-compose logs mysql
docker-compose logs redis

# 重新创建容器
docker-compose down
docker-compose up -d
```

### 后端相关

**问题：无法连接数据库**
- 确认MySQL容器正在运行：`docker-compose ps`
- 检查密码是否正确：`appsettings.Development.json`
- 查看后端日志中的错误信息

**问题：编译错误**
- 所有编译错误已修复
- 如果还有问题，尝试：`dotnet clean && dotnet restore && dotnet build`

### 前端相关

**问题：登录失败**
- 确认后端服务正在运行
- 检查浏览器控制台的网络请求
- 确认后端地址：https://localhost:5001

---

## 推荐的开发流程

1. ✅ **安装Docker Desktop**（一次性，约10分钟）
2. ✅ **启动数据库服务**（`./start-dev.sh`，约30秒）
3. ✅ **启动后端服务**（`dotnet run`，约10秒）
4. ✅ **访问前端应用**（http://localhost:5173/）
5. ✅ **开始开发和测试**

---

## 有用的命令

### Docker管理

```bash
# 查看服务状态
docker-compose ps

# 查看日志
docker-compose logs -f

# 停止服务
docker-compose stop

# 启动服务
docker-compose start

# 重启服务
docker-compose restart

# 停止并删除容器（保留数据）
docker-compose down

# 停止并删除所有数据
docker-compose down -v
```

### 后端管理

```bash
# 启动后端
cd backend/src/AWSomeShop.API
~/.dotnet/dotnet run

# 热重载模式
~/.dotnet/dotnet watch run

# 查看Swagger文档
# 浏览器访问：https://localhost:5001/swagger
```

### 前端管理

```bash
# 启动前端（已在运行）
cd frontend
npm run dev

# 构建生产版本
npm run build

# 预览生产版本
npm run preview
```

---

## 下一步

选择你想要的方式：

1. **推荐**：安装Docker Desktop，获得完整功能
2. **临时**：先预览前端UI，稍后安装Docker
3. **手动**：使用Homebrew安装MySQL和Redis

无论选择哪种方式，我都可以继续帮助你！

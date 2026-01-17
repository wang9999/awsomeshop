# AWSomeShop - 员工福利电商平台

一个功能完整的内部员工积分兑换系统，员工可以使用"AWSome积分"兑换产品。

## 📋 项目概述

AWSomeShop是一个基于积分的员工福利电商平台，提供完整的产品浏览、购物车、订单管理和积分系统功能。

### 核心功能

**员工端**
- 🔐 员工登录认证
- 🛍️ 产品浏览、搜索和筛选
- 🛒 购物车管理
- 📦 订单创建和管理
- 💰 积分余额查询
- 📍 收货地址管理
- 🔔 站内通知

**管理员端**
- 👨‍💼 管理员登录（RBAC权限控制）
- 📦 产品管理（CRUD、上下架）
- 💎 积分管理（发放、扣除）
- 📋 订单管理
- 📊 数据统计分析
- 📝 操作审计日志

**系统功能**
- ⏰ 自动积分发放（每月、入职、生日）
- ⏳ 积分过期处理（1年有效期）
- 🔔 订单状态通知
- 🛡️ 安全防护（验证码、限流、防重复提交）

## 🏗️ 技术栈

### 后端
- **框架**: ASP.NET Core 8.0
- **数据库**: MySQL 8.0 + Entity Framework Core
- **缓存**: Redis
- **认证**: JWT Bearer Token
- **密码加密**: BCrypt

### 前端
- **框架**: React 19.2.0 + TypeScript 5.9.3
- **构建工具**: Vite 7.2.4
- **UI库**: Ant Design 6.2.0
- **状态管理**: Zustand 5.0.10
- **路由**: React Router DOM 7.12.0
- **HTTP客户端**: Axios 1.13.2

## 📁 项目结构

```
AWSomeShop/
├── backend/                    # 后端项目
│   └── src/
│       └── AWSomeShop.API/
│           ├── Controllers/    # API控制器
│           ├── Services/       # 业务逻辑
│           ├── Repositories/   # 数据访问
│           ├── Models/         # 数据模型
│           ├── Infrastructure/ # 基础设施
│           └── Middleware/     # 中间件
├── frontend/                   # 前端项目
│   └── src/
│       ├── pages/             # 页面组件
│       ├── components/        # UI组件
│       ├── services/          # API服务
│       ├── store/             # 状态管理
│       └── utils/             # 工具函数
└── .kiro/                     # 项目文档
    ├── specs/                 # 规格文档
    └── steering/              # 指导文档
```

## 🚀 快速开始

### 环境要求

- .NET 8.0 SDK
- Node.js 18+
- Docker Desktop（推荐）或 MySQL 8.0+ 和 Redis 6.0+

### 方式1：使用Docker（推荐）

这是最简单的启动方式，无需手动安装MySQL和Redis。

#### 1. 安装Docker Desktop

- 访问 https://www.docker.com/products/docker-desktop/
- 下载并安装适合你系统的版本
- 启动Docker Desktop

详细安装指南请查看 `DOCKER_SETUP.md`

#### 2. 启动数据库服务

在项目根目录执行：

```bash
# 启动MySQL和Redis
docker-compose up -d

# 查看服务状态
docker-compose ps

# 查看日志
docker-compose logs -f
```

#### 3. 启动后端

```bash
cd backend/src/AWSomeShop.API
~/.dotnet/dotnet run
```

后端将在 `https://localhost:5001` 启动，并自动：
- 连接数据库
- 创建表结构
- 填充测试数据

#### 4. 启动前端

```bash
cd frontend
npm install
npm run dev
```

前端将在 `http://localhost:5173` 启动

### 方式2：手动安装（不使用Docker）

如果不使用Docker，需要手动安装MySQL和Redis。

### 方式2：手动安装（不使用Docker）

如果不使用Docker，需要手动安装MySQL和Redis。

#### 安装MySQL

```bash
# macOS (使用Homebrew)
brew install mysql
brew services start mysql

# 创建数据库
mysql -uroot -p
CREATE DATABASE awsomeshop_dev CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

#### 安装Redis

```bash
# macOS (使用Homebrew)
brew install redis
brew services start redis
```

### 后端启动

```bash
# 进入后端目录
cd backend/src/AWSomeShop.API

# 还原依赖
dotnet restore

# 更新数据库连接字符串
# 编辑 appsettings.Development.json

# 运行项目
dotnet run

# 或使用热重载
dotnet watch run
```

后端将在 `https://localhost:5001` 启动

### 前端启动

```bash
# 进入前端目录
cd frontend

# 安装依赖
npm install

# 启动开发服务器
npm run dev
```

前端将在 `http://localhost:5173` 启动

## 📝 配置说明

### 后端配置 (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;database=awsomeshop;user=root;password=your_password",
    "Redis": "localhost:6379"
  },
  "Jwt": {
    "SecretKey": "your-secret-key-at-least-32-characters",
    "Issuer": "AWSomeShop",
    "Audience": "AWSomeShop-Users"
  }
}
```

### 前端配置 (.env.development)

```env
VITE_API_BASE_URL=https://localhost:5001/api
```

## 👥 测试账号

系统会在开发模式下自动创建测试账号：

**员工账号**
- 员工1: `employee1@awsome.com` / `Employee@123` (积分: 2000)
- 员工2: `employee2@awsome.com` / `Employee@123` (积分: 1500)
- 员工3: `employee3@awsome.com` / `Employee@123` (积分: 3000)

**管理员账号**
- 超级管理员: `superadmin` / `Admin@123`
- 产品管理员: `productadmin` / `Admin@123`
- 积分管理员: `pointsadmin` / `Admin@123`

## 📚 API文档

启动后端后访问 Swagger UI:
```
https://localhost:5001/swagger
```

## 🎯 核心业务规则

### 积分规则
- 积分有效期：1年
- 每月1号自动发放：500积分
- 入职时发放：1000积分
- 生日当天发放：200积分

### 兑换规则
- 每月最多兑换：5次
- 同一产品限兑：1次
- 取消订单：24小时内且待发货状态

### 订单状态流转
```
待确认 → 待发货 → 已发货 → 已完成
   ↓        ↓
 已取消   已取消
```

## 🔐 安全特性

- ✅ JWT认证和授权
- ✅ BCrypt密码加密
- ✅ RBAC权限控制
- ✅ 验证码验证
- ✅ 防重复提交
- ✅ API限流（60次/分钟）
- ✅ 审计日志

## 📊 数据统计

管理员可以查看：
- 概览统计（订单、员工、积分）
- 热门产品排行
- 活跃员工排行
- 积分趋势分析

## 🧪 测试

```bash
# 后端测试
cd backend/src/AWSomeShop.API
dotnet test

# 前端测试
cd frontend
npm run test
```

## 📦 部署

### Docker部署（推荐）

项目包含完整的Docker配置：

```bash
# 启动所有服务（MySQL + Redis）
docker-compose up -d

# 查看服务状态
docker-compose ps

# 停止服务
docker-compose stop

# 停止并删除容器（保留数据）
docker-compose down

# 停止并删除所有数据
docker-compose down -v
```

详细的Docker使用说明请查看 `DOCKER_SETUP.md`

### 生产环境部署

1. 构建后端
```bash
cd backend/src/AWSomeShop.API
dotnet publish -c Release -o ./publish
```

2. 构建前端
```bash
cd frontend
npm run build
```

3. 配置Nginx反向代理
4. 配置HTTPS证书
5. 启动服务

## 📖 文档

详细文档位于 `.kiro/specs/` 目录：

- `requirements_plan.md` - 需求规划
- `design.md` - 系统设计
- `tasks.md` - 任务分解
- `final-summary.md` - 项目总结

## 🤝 贡献

欢迎提交Issue和Pull Request！

## 📄 许可证

MIT License

## 👨‍💻 作者

AWSomeShop Team

## 📞 联系方式

如有问题，请提交Issue或联系开发团队。

---

**注意**: 这是一个演示项目，生产环境部署前请：
1. 修改所有默认密码和密钥
2. 启用HTTPS
3. 配置生产环境数据库
4. 进行安全审计
5. 进行性能测试

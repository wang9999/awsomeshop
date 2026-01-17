# AWSomeShop 设计文档 (Design Document)

## 概述

AWSomeShop 是一个基于 React + .NET + MySQL 技术栈的内部员工福利电商平台。系统采用前后端分离架构，支持员工使用积分兑换产品，管理员管理产品和积分。

### 设计目标

1. **可扩展性**: 支持1000并发用户和1000个产品
2. **安全性**: HTTPS传输、密码加密、数据脱敏
3. **性能**: 页面响应时间 < 3秒
4. **可维护性**: 清晰的分层架构，便于团队协作
5. **可测试性**: 所有核心业务逻辑都可进行属性测试

### 技术选型

- **前端**: React 18+ + TypeScript + Ant Design
- **后端**: .NET 8 + ASP.NET Core Web API
- **数据库**: MySQL 8.0
- **缓存**: Redis (用于会话管理和热点数据)
- **文件存储**: 对象存储服务 (用于产品图片)
- **部署**: Docker + Kubernetes

---

## 架构设计

### 系统架构图

```
┌─────────────────────────────────────────────────────────────┐
│                         客户端层                              │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │  员工Web端   │  │  管理员Web端  │  │  移动端H5    │      │
│  │   (React)    │  │   (React)    │  │  (React)     │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└─────────────────────────────────────────────────────────────┘
                            │
                            │ HTTPS
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                         网关层                                │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  API Gateway (Nginx / .NET Gateway)                  │   │
│  │  - 路由转发  - 负载均衡  - SSL终止  - 限流          │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                      应用服务层                               │
│  ┌────────────┐  ┌────────────┐  ┌────────────┐            │
│  │ 员工服务   │  │ 产品服务   │  │ 订单服务   │            │
│  │ Employee   │  │ Product    │  │ Order      │            │
│  │ Service    │  │ Service    │  │ Service    │            │
│  └────────────┘  └────────────┘  └────────────┘            │
│  ┌────────────┐  ┌────────────┐  ┌────────────┐            │
│  │ 积分服务   │  │ 通知服务   │  │ 管理服务   │            │
│  │ Points     │  │ Notification│ │ Admin      │            │
│  │ Service    │  │ Service    │  │ Service    │            │
│  └────────────┘  └────────────┘  └────────────┘            │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                      数据访问层                               │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  Repository Pattern + Entity Framework Core          │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                      数据存储层                               │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐                  │
│  │  MySQL   │  │  Redis   │  │ 对象存储  │                  │
│  │  (主库)  │  │  (缓存)  │  │ (图片)   │                  │
│  └──────────┘  └──────────┘  └──────────┘                  │
└─────────────────────────────────────────────────────────────┘
```

### 架构说明

1. **前后端分离**: 前端React应用通过RESTful API与后端通信
2. **微服务化**: 后端按业务领域划分为多个服务，便于独立开发和部署
3. **分层架构**: 每个服务内部采用Controller → Service → Repository三层架构
4. **缓存策略**: 使用Redis缓存热点数据（产品列表、用户会话）
5. **数据库设计**: MySQL存储业务数据，支持事务和复杂查询

---

## 组件和接口设计

### 前端组件结构

```
src/
├── components/          # 通用组件
│   ├── Layout/         # 布局组件
│   ├── ProductCard/    # 产品卡片
│   ├── ShoppingCart/   # 购物车
│   └── Notification/   # 通知组件
├── pages/              # 页面组件
│   ├── employee/       # 员工端页面
│   │   ├── Login/
│   │   ├── ProductList/
│   │   ├── ProductDetail/
│   │   ├── Cart/
│   │   ├── OrderHistory/
│   │   └── Profile/
│   └── admin/          # 管理端页面
│       ├── Login/
│       ├── ProductManagement/
│       ├── PointsManagement/
│       ├── OrderManagement/
│       └── Dashboard/
├── services/           # API服务
│   ├── authService.ts
│   ├── productService.ts
│   ├── orderService.ts
│   └── pointsService.ts
├── store/              # 状态管理 (Redux/Zustand)
│   ├── authSlice.ts
│   ├── cartSlice.ts
│   └── productSlice.ts
└── utils/              # 工具函数
    ├── request.ts      # HTTP请求封装
    ├── auth.ts         # 认证工具
    └── validators.ts   # 表单验证
```

### 后端服务结构

```
AWSomeShop.API/
├── Controllers/        # API控制器
│   ├── AuthController.cs
│   ├── ProductController.cs
│   ├── OrderController.cs
│   ├── PointsController.cs
│   └── AdminController.cs
├── Services/           # 业务逻辑层
│   ├── IAuthService.cs / AuthService.cs
│   ├── IProductService.cs / ProductService.cs
│   ├── IOrderService.cs / OrderService.cs
│   ├── IPointsService.cs / PointsService.cs
│   └── INotificationService.cs / NotificationService.cs
├── Repositories/       # 数据访问层
│   ├── IEmployeeRepository.cs / EmployeeRepository.cs
│   ├── IProductRepository.cs / ProductRepository.cs
│   ├── IOrderRepository.cs / OrderRepository.cs
│   └── IPointsTransactionRepository.cs / PointsTransactionRepository.cs
├── Models/             # 数据模型
│   ├── Entities/       # 数据库实体
│   ├── DTOs/           # 数据传输对象
│   └── ViewModels/     # 视图模型
├── Middleware/         # 中间件
│   ├── AuthenticationMiddleware.cs
│   ├── ExceptionHandlingMiddleware.cs
│   └── RateLimitingMiddleware.cs
└── Infrastructure/     # 基础设施
    ├── DbContext/
    ├── Cache/
    └── FileStorage/
```

### 核心API接口设计

#### 1. 认证接口

```csharp
// POST /api/auth/employee/login
// 员工登录
Request: {
  "email": "string",
  "password": "string"
}
Response: {
  "token": "string",
  "employeeId": "string",
  "name": "string",
  "points": number
}

// POST /api/auth/admin/login
// 管理员登录
Request: {
  "username": "string",
  "password": "string"
}
Response: {
  "token": "string",
  "adminId": "string",
  "role": "string"
}
```

#### 2. 产品接口

```csharp
// GET /api/products
// 获取产品列表
Query: {
  "category": "string?",
  "keyword": "string?",
  "minPoints": "number?",
  "maxPoints": "number?",
  "page": "number",
  "pageSize": "number"
}
Response: {
  "items": [
    {
      "id": "string",
      "name": "string",
      "description": "string",
      "images": ["string"],
      "points": number,
      "stock": number,
      "category": "string",
      "status": "string"
    }
  ],
  "total": number,
  "page": number,
  "pageSize": number
}

// GET /api/products/{id}
// 获取产品详情
Response: {
  "id": "string",
  "name": "string",
  "description": "string",
  "images": ["string"],
  "points": number,
  "stock": number,
  "category": "string",
  "tags": ["string"],
  "status": "string"
}
```

#### 3. 购物车接口

```csharp
// POST /api/cart/items
// 添加到购物车
Request: {
  "productId": "string",
  "quantity": number
}
Response: {
  "success": boolean,
  "message": "string"
}

// GET /api/cart
// 获取购物车
Response: {
  "items": [
    {
      "productId": "string",
      "productName": "string",
      "productImage": "string",
      "points": number,
      "quantity": number,
      "stock": number
    }
  ],
  "totalPoints": number
}
```

#### 4. 订单接口

```csharp
// POST /api/orders
// 创建订单
Request: {
  "items": [
    {
      "productId": "string",
      "quantity": number
    }
  ],
  "addressId": "string",
  "captcha": "string"
}
Response: {
  "orderId": "string",
  "status": "string",
  "totalPoints": number
}

// GET /api/orders
// 获取订单列表
Query: {
  "status": "string?",
  "startDate": "date?",
  "endDate": "date?",
  "page": "number",
  "pageSize": "number"
}
Response: {
  "items": [
    {
      "id": "string",
      "orderNumber": "string",
      "products": ["string"],
      "totalPoints": number,
      "status": "string",
      "createdAt": "datetime"
    }
  ],
  "total": number
}

// PUT /api/orders/{id}/cancel
// 取消订单
Response: {
  "success": boolean,
  "message": "string"
}
```

#### 5. 积分接口

```csharp
// GET /api/points/balance
// 获取积分余额
Response: {
  "balance": number,
  "expiringSoon": number,
  "expiryDate": "date?"
}

// GET /api/points/transactions
// 获取积分历史
Query: {
  "startDate": "date?",
  "endDate": "date?",
  "page": "number",
  "pageSize": "number"
}
Response: {
  "items": [
    {
      "id": "string",
      "amount": number,
      "type": "string",
      "reason": "string",
      "operator": "string",
      "balanceAfter": number,
      "createdAt": "datetime"
    }
  ],
  "total": number
}

// POST /api/admin/points/grant
// 发放积分
Request: {
  "employeeIds": ["string"],
  "amount": number,
  "reason": "string"
}
Response: {
  "success": boolean,
  "affectedCount": number
}
```

#### 6. 管理员接口

```csharp
// POST /api/admin/products
// 创建产品
Request: {
  "name": "string",
  "description": "string",
  "images": ["string"],
  "points": number,
  "stock": number,
  "category": "string",
  "tags": ["string"]
}
Response: {
  "productId": "string"
}

// PUT /api/admin/products/{id}
// 更新产品
Request: {
  "name": "string?",
  "description": "string?",
  "points": "number?",
  "stock": "number?",
  "status": "string?"
}
Response: {
  "success": boolean
}

// GET /api/admin/orders
// 管理员查看订单
Query: {
  "status": "string?",
  "employeeName": "string?",
  "page": "number",
  "pageSize": "number"
}
Response: {
  "items": [
    {
      "id": "string",
      "orderNumber": "string",
      "employeeName": "string",
      "products": ["string"],
      "totalPoints": number,
      "status": "string",
      "address": "string",
      "createdAt": "datetime"
    }
  ],
  "total": number
}

// PUT /api/admin/orders/{id}/status
// 更新订单状态
Request: {
  "status": "string",
  "trackingNumber": "string?"
}
Response: {
  "success": boolean
}
```

---

## 数据模型设计

### 数据库表结构

#### 1. 员工表 (Employees)

```sql
CREATE TABLE Employees (
    Id VARCHAR(36) PRIMARY KEY,
    Email VARCHAR(255) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Name VARCHAR(100) NOT NULL,
    EmployeeNumber VARCHAR(50) NOT NULL UNIQUE,
    Department VARCHAR(100),
    Birthday DATE,
    PointsBalance INT DEFAULT 0,
    MonthlyRedeemCount INT DEFAULT 0,
    LastRedeemResetDate DATE,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    IsActive BOOLEAN DEFAULT TRUE,
    INDEX idx_email (Email),
    INDEX idx_employee_number (EmployeeNumber)
);
```

#### 2. 管理员表 (Administrators)

```sql
CREATE TABLE Administrators (
    Id VARCHAR(36) PRIMARY KEY,
    Username VARCHAR(100) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Name VARCHAR(100) NOT NULL,
    Role ENUM('SuperAdmin', 'ProductAdmin', 'PointsAdmin') NOT NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    IsActive BOOLEAN DEFAULT TRUE,
    INDEX idx_username (Username)
);
```

#### 3. 产品表 (Products)

```sql
CREATE TABLE Products (
    Id VARCHAR(36) PRIMARY KEY,
    Name VARCHAR(200) NOT NULL,
    Description TEXT,
    Points INT NOT NULL,
    Stock INT NOT NULL DEFAULT 0,
    Category ENUM('电子产品', '生活用品', '礼品卡', '图书文具') NOT NULL,
    Status ENUM('上架', '下架') DEFAULT '下架',
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(36),
    INDEX idx_category (Category),
    INDEX idx_status (Status),
    INDEX idx_points (Points),
    FULLTEXT idx_name_desc (Name, Description)
);
```

#### 4. 产品图片表 (ProductImages)

```sql
CREATE TABLE ProductImages (
    Id VARCHAR(36) PRIMARY KEY,
    ProductId VARCHAR(36) NOT NULL,
    ImageUrl VARCHAR(500) NOT NULL,
    DisplayOrder INT DEFAULT 0,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE,
    INDEX idx_product_id (ProductId)
);
```

#### 5. 产品标签表 (ProductTags)

```sql
CREATE TABLE ProductTags (
    Id VARCHAR(36) PRIMARY KEY,
    ProductId VARCHAR(36) NOT NULL,
    Tag VARCHAR(50) NOT NULL,
    FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE,
    INDEX idx_product_id (ProductId),
    INDEX idx_tag (Tag)
);
```

#### 6. 购物车表 (ShoppingCarts)

```sql
CREATE TABLE ShoppingCarts (
    Id VARCHAR(36) PRIMARY KEY,
    EmployeeId VARCHAR(36) NOT NULL,
    ProductId VARCHAR(36) NOT NULL,
    Quantity INT NOT NULL DEFAULT 1,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id) ON DELETE CASCADE,
    FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE,
    UNIQUE KEY uk_employee_product (EmployeeId, ProductId),
    INDEX idx_employee_id (EmployeeId)
);
```

#### 7. 订单表 (Orders)

```sql
CREATE TABLE Orders (
    Id VARCHAR(36) PRIMARY KEY,
    OrderNumber VARCHAR(50) NOT NULL UNIQUE,
    EmployeeId VARCHAR(36) NOT NULL,
    TotalPoints INT NOT NULL,
    Status ENUM('待确认', '待发货', '已发货', '已完成', '已取消') DEFAULT '待确认',
    AddressId VARCHAR(36),
    TrackingNumber VARCHAR(100),
    CancelReason VARCHAR(500),
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id),
    INDEX idx_employee_id (EmployeeId),
    INDEX idx_order_number (OrderNumber),
    INDEX idx_status (Status),
    INDEX idx_created_at (CreatedAt)
);
```

#### 8. 订单明细表 (OrderItems)

```sql
CREATE TABLE OrderItems (
    Id VARCHAR(36) PRIMARY KEY,
    OrderId VARCHAR(36) NOT NULL,
    ProductId VARCHAR(36) NOT NULL,
    ProductName VARCHAR(200) NOT NULL,
    ProductImage VARCHAR(500),
    Points INT NOT NULL,
    Quantity INT NOT NULL DEFAULT 1,
    FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
    INDEX idx_order_id (OrderId)
);
```

#### 9. 收货地址表 (Addresses)

```sql
CREATE TABLE Addresses (
    Id VARCHAR(36) PRIMARY KEY,
    EmployeeId VARCHAR(36) NOT NULL,
    ReceiverName VARCHAR(100) NOT NULL,
    ReceiverPhone VARCHAR(20) NOT NULL,
    Province VARCHAR(50),
    City VARCHAR(50),
    District VARCHAR(50),
    DetailAddress VARCHAR(500) NOT NULL,
    IsDefault BOOLEAN DEFAULT FALSE,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id) ON DELETE CASCADE,
    INDEX idx_employee_id (EmployeeId)
);
```

#### 10. 积分交易表 (PointsTransactions)

```sql
CREATE TABLE PointsTransactions (
    Id VARCHAR(36) PRIMARY KEY,
    EmployeeId VARCHAR(36) NOT NULL,
    Amount INT NOT NULL,
    Type ENUM('发放', '扣除', '消费', '退回', '过期') NOT NULL,
    Reason VARCHAR(500),
    OperatorId VARCHAR(36),
    OperatorType ENUM('System', 'Admin') DEFAULT 'System',
    BalanceBefore INT NOT NULL,
    BalanceAfter INT NOT NULL,
    RelatedOrderId VARCHAR(36),
    ExpiryDate DATE,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id),
    INDEX idx_employee_id (EmployeeId),
    INDEX idx_type (Type),
    INDEX idx_created_at (CreatedAt),
    INDEX idx_expiry_date (ExpiryDate)
);
```

#### 11. 通知表 (Notifications)

```sql
CREATE TABLE Notifications (
    Id VARCHAR(36) PRIMARY KEY,
    EmployeeId VARCHAR(36) NOT NULL,
    Title VARCHAR(200) NOT NULL,
    Content TEXT NOT NULL,
    Type ENUM('积分变动', '订单状态', '产品上新', '系统通知') NOT NULL,
    RelatedId VARCHAR(36),
    IsRead BOOLEAN DEFAULT FALSE,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id) ON DELETE CASCADE,
    INDEX idx_employee_id (EmployeeId),
    INDEX idx_is_read (IsRead),
    INDEX idx_created_at (CreatedAt)
);
```

#### 12. 操作日志表 (AuditLogs)

```sql
CREATE TABLE AuditLogs (
    Id VARCHAR(36) PRIMARY KEY,
    AdminId VARCHAR(36) NOT NULL,
    AdminName VARCHAR(100) NOT NULL,
    Action VARCHAR(100) NOT NULL,
    EntityType VARCHAR(50) NOT NULL,
    EntityId VARCHAR(36),
    OldValue TEXT,
    NewValue TEXT,
    IpAddress VARCHAR(50),
    UserAgent VARCHAR(500),
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (AdminId) REFERENCES Administrators(Id),
    INDEX idx_admin_id (AdminId),
    INDEX idx_action (Action),
    INDEX idx_entity_type (EntityType),
    INDEX idx_created_at (CreatedAt)
);
```

### 数据库关系图

```
Employees (员工)
    ├── ShoppingCarts (购物车)
    ├── Orders (订单)
    ├── Addresses (收货地址)
    ├── PointsTransactions (积分交易)
    └── Notifications (通知)

Products (产品)
    ├── ProductImages (产品图片)
    ├── ProductTags (产品标签)
    ├── ShoppingCarts (购物车)
    └── OrderItems (订单明细)

Orders (订单)
    ├── OrderItems (订单明细)
    └── PointsTransactions (积分交易)

Administrators (管理员)
    └── AuditLogs (操作日志)
```

### 实体类设计 (C#)

```csharp
// Employee.cs
public class Employee
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Name { get; set; }
    public string EmployeeNumber { get; set; }
    public string Department { get; set; }
    public DateTime? Birthday { get; set; }
    public int PointsBalance { get; set; }
    public int MonthlyRedeemCount { get; set; }
    public DateTime? LastRedeemResetDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; }
    
    // Navigation properties
    public ICollection<ShoppingCart> ShoppingCarts { get; set; }
    public ICollection<Order> Orders { get; set; }
    public ICollection<Address> Addresses { get; set; }
    public ICollection<PointsTransaction> PointsTransactions { get; set; }
}

// Product.cs
public class Product
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Points { get; set; }
    public int Stock { get; set; }
    public ProductCategory Category { get; set; }
    public ProductStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CreatedBy { get; set; }
    
    // Navigation properties
    public ICollection<ProductImage> Images { get; set; }
    public ICollection<ProductTag> Tags { get; set; }
}

// Order.cs
public class Order
{
    public string Id { get; set; }
    public string OrderNumber { get; set; }
    public string EmployeeId { get; set; }
    public int TotalPoints { get; set; }
    public OrderStatus Status { get; set; }
    public string AddressId { get; set; }
    public string TrackingNumber { get; set; }
    public string CancelReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public Employee Employee { get; set; }
    public Address Address { get; set; }
    public ICollection<OrderItem> Items { get; set; }
}
```

---

## 正确性属性 (Correctness Properties)

属性是关于系统行为的形式化陈述，应该在所有有效执行中保持为真。属性是人类可读规范和机器可验证正确性保证之间的桥梁。我们将使用属性测试来验证这些属性。

### 核心业务属性

#### 属性 1: 积分余额非负不变量
*对于任何*员工和任何积分操作序列，员工的积分余额必须始终大于或等于0。

**验证**: 需求 3.2  
**测试策略**: 生成随机的积分操作序列（发放、扣除、消费），验证余额始终 >= 0

#### 属性 2: 积分交易记录完整性
*对于任何*积分变动操作，系统必须创建一条包含所有必需字段（时间、数量、原因、操作人、余额变化）的交易记录。

**验证**: 需求 3.10  
**测试策略**: 执行任意积分操作，验证生成的交易记录包含所有必需字段

#### 属性 3: 购物车总积分计算正确性
*对于任何*购物车状态，购物车显示的总积分必须等于所有商品积分的总和。

**验证**: 需求 4.3  
**测试策略**: 生成随机购物车（多个产品和数量），验证 totalPoints = sum(product.points * quantity)

#### 属性 4: 兑换事务原子性
*对于任何*成功的兑换操作，必须同时满足：(1) 积分扣除 (2) 订单创建 (3) 库存减少。如果任一步骤失败，所有操作必须回滚。

**验证**: 需求 5.2  
**测试策略**: 模拟各种失败场景（数据库错误、网络超时），验证要么全部成功，要么全部回滚

#### 属性 5: 兑换幂等性
*对于任何*兑换请求，即使重复提交多次，也只应创建一个订单并扣除一次积分。

**验证**: 需求 5.5  
**测试策略**: 快速提交相同的兑换请求多次，验证只创建一个订单

#### 属性 6: 取消兑换回滚正确性
*对于任何*可取消的订单，取消操作必须同时：(1) 订单状态改为"已取消" (2) 积分退回 (3) 库存恢复。

**验证**: 需求 5.12, 5.13  
**测试策略**: 创建订单后取消，验证积分和库存都正确恢复

#### 属性 7: 每月兑换次数限制
*对于任何*员工，在同一自然月内的兑换次数不能超过5次。

**验证**: 需求 5.9  
**测试策略**: 创建5次兑换记录，尝试第6次，验证系统拒绝

#### 属性 8: 产品重复兑换限制
*对于任何*员工和产品，如果员工已成功兑换过该产品，则不能再次兑换同一产品。

**验证**: 需求 5.10  
**测试策略**: 兑换产品后，再次尝试兑换同一产品，验证系统拒绝

#### 属性 9: 库存一致性
*对于任何*产品，其库存数量必须等于：初始库存 - 已兑换数量 + 已取消数量。

**验证**: 需求 5.2, 5.13  
**测试策略**: 执行一系列兑换和取消操作，验证最终库存计算正确

#### 属性 10: 积分过期正确性
*对于任何*积分记录，如果获得时间距今超过1年，则该积分应被标记为过期并从余额中扣除。

**验证**: 需求 3.4  
**测试策略**: 创建1年前的积分记录，运行过期任务，验证余额减少

#### 属性 11: 搜索结果相关性
*对于任何*搜索关键词，返回的所有产品的名称或描述中必须包含该关键词。

**验证**: 需求 2.8  
**测试策略**: 生成随机关键词，搜索后验证所有结果都包含关键词

#### 属性 12: 分类筛选正确性
*对于任何*产品分类，按该分类筛选后返回的所有产品必须属于该分类。

**验证**: 需求 2.7  
**测试策略**: 按分类筛选，验证所有结果的category字段等于筛选条件

#### 属性 13: 密码加密存储
*对于任何*新创建或更新的用户账号，数据库中存储的密码必须是加密后的值，不等于明文密码。

**验证**: 需求 1.5  
**测试策略**: 创建账号后直接查询数据库，验证passwordHash != plainPassword

#### 属性 14: 会话有效期
*对于任何*登录会话，其有效期必须为2小时，且在30分钟无操作后自动失效。

**验证**: 需求 1.3, 1.4  
**测试策略**: 创建会话后，验证过期时间；模拟30分钟无操作，验证会话失效

#### 属性 15: 权限控制正确性
*对于任何*管理员和操作，如果管理员角色不具备该操作权限，则系统必须拒绝访问。

**验证**: 需求 8.6, 8.7, 8.8  
**测试策略**: 使用不同角色的管理员尝试各种操作，验证权限检查正确

#### 属性 16: 订单状态转换合法性
*对于任何*订单，其状态转换必须遵循：待确认 → 待发货 → 已发货 → 已完成，或任意状态 → 已取消。

**验证**: 需求 5.8  
**测试策略**: 尝试非法的状态转换（如已完成 → 待发货），验证系统拒绝

#### 属性 17: 地址默认唯一性
*对于任何*员工，最多只能有一个默认收货地址。

**验证**: 需求 6.4  
**测试策略**: 设置新的默认地址，验证之前的默认地址被取消

#### 属性 18: 通知触发完整性
*对于任何*积分变动或订单状态变更，系统必须创建相应的通知记录。

**验证**: 需求 14.1, 14.2  
**测试策略**: 执行积分或订单操作，验证生成了对应的通知

#### 属性 19: 操作日志记录完整性
*对于任何*管理员操作，系统必须记录包含操作人、时间、类型、对象、前后值的日志。

**验证**: 需求 13.1  
**测试策略**: 执行任意管理操作，验证生成的日志包含所有必需字段

#### 属性 20: 批量操作原子性
*对于任何*批量积分发放操作，要么所有员工都成功发放，要么全部失败回滚。

**验证**: 需求 10.6  
**测试策略**: 批量发放时模拟部分失败，验证事务回滚

### 数据完整性属性

#### 属性 21: 外键引用完整性
*对于任何*订单，其关联的员工ID和地址ID必须在对应表中存在。

**验证**: 数据库约束  
**测试策略**: 尝试创建引用不存在ID的订单，验证系统拒绝

#### 属性 22: 唯一性约束
*对于任何*两个不同的员工，其邮箱和工号必须不同。

**验证**: 需求 1.7  
**测试策略**: 尝试创建重复邮箱或工号的员工，验证系统拒绝

#### 属性 23: 枚举值约束
*对于任何*产品，其分类必须是"电子产品"、"生活用品"、"礼品卡"、"图书文具"之一。

**验证**: 需求 2.6  
**测试策略**: 尝试创建其他分类的产品，验证系统拒绝

### 性能属性

#### 属性 24: 响应时间上限
*对于任何*API请求，在正常负载下（<1000并发），响应时间必须小于3秒。

**验证**: 需求 16.3, 16.4, 16.5  
**测试策略**: 使用负载测试工具，验证99%的请求响应时间 < 3秒

#### 属性 25: 并发安全性
*对于任何*两个并发的兑换请求，如果都针对同一产品且库存只剩1个，则只有一个请求能成功。

**验证**: 需求 5.2  
**测试策略**: 并发提交多个兑换请求，验证库存不会超卖

---

## 错误处理策略

### 错误分类

1. **客户端错误 (4xx)**
   - 400 Bad Request: 请求参数错误
   - 401 Unauthorized: 未认证
   - 403 Forbidden: 无权限
   - 404 Not Found: 资源不存在
   - 409 Conflict: 资源冲突（如重复提交）
   - 422 Unprocessable Entity: 业务规则验证失败

2. **服务端错误 (5xx)**
   - 500 Internal Server Error: 服务器内部错误
   - 503 Service Unavailable: 服务暂时不可用

### 统一错误响应格式

```json
{
  "success": false,
  "errorCode": "string",
  "message": "string",
  "details": {
    "field": "string",
    "reason": "string"
  },
  "timestamp": "datetime",
  "requestId": "string"
}
```

### 常见错误场景处理

#### 1. 积分不足
```csharp
if (employee.PointsBalance < totalPoints)
{
    throw new BusinessException(
        ErrorCode.InsufficientPoints,
        $"积分不足。当前余额：{employee.PointsBalance}，需要：{totalPoints}"
    );
}
```

#### 2. 库存不足
```csharp
if (product.Stock < quantity)
{
    throw new BusinessException(
        ErrorCode.InsufficientStock,
        $"库存不足。当前库存：{product.Stock}，需要：{quantity}"
    );
}
```

#### 3. 兑换次数超限
```csharp
if (employee.MonthlyRedeemCount >= 5)
{
    throw new BusinessException(
        ErrorCode.MonthlyLimitExceeded,
        "本月兑换次数已达上限（5次）"
    );
}
```

#### 4. 重复兑换
```csharp
if (await _orderRepository.HasRedeemedProduct(employeeId, productId))
{
    throw new BusinessException(
        ErrorCode.DuplicateRedemption,
        "您已兑换过该产品，不能重复兑换"
    );
}
```

#### 5. 并发冲突
```csharp
try
{
    // 使用乐观锁
    await _dbContext.SaveChangesAsync();
}
catch (DbUpdateConcurrencyException)
{
    throw new BusinessException(
        ErrorCode.ConcurrencyConflict,
        "数据已被其他用户修改，请刷新后重试"
    );
}
```

#### 6. 数据库连接失败
```csharp
try
{
    // 数据库操作
}
catch (SqlException ex)
{
    _logger.LogError(ex, "数据库连接失败");
    throw new InfrastructureException(
        ErrorCode.DatabaseError,
        "系统暂时不可用，请稍后重试"
    );
}
```

### 全局异常处理中间件

```csharp
public class ExceptionHandlingMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (BusinessException ex)
        {
            await HandleBusinessException(context, ex);
        }
        catch (ValidationException ex)
        {
            await HandleValidationException(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "未处理的异常");
            await HandleUnexpectedException(context, ex);
        }
    }
    
    private async Task HandleBusinessException(HttpContext context, BusinessException ex)
    {
        context.Response.StatusCode = 422;
        await context.Response.WriteAsJsonAsync(new ErrorResponse
        {
            Success = false,
            ErrorCode = ex.ErrorCode,
            Message = ex.Message,
            Timestamp = DateTime.UtcNow,
            RequestId = context.TraceIdentifier
        });
    }
}
```

---

## 测试策略

### 测试金字塔

```
        /\
       /  \
      / E2E \          10% - 端到端测试
     /______\
    /        \
   / 集成测试  \        20% - 集成测试
  /____________\
 /              \
/   单元测试      \     70% - 单元测试 + 属性测试
/__________________\
```

### 1. 单元测试

**目标**: 测试单个函数或类的行为

**工具**: xUnit + Moq + FluentAssertions

**示例**:
```csharp
[Fact]
public async Task CreateOrder_WithSufficientPoints_ShouldSucceed()
{
    // Arrange
    var employee = new Employee { Id = "emp1", PointsBalance = 1000 };
    var product = new Product { Id = "prod1", Points = 500, Stock = 10 };
    
    // Act
    var result = await _orderService.CreateOrder(employee.Id, product.Id, 1);
    
    // Assert
    result.Should().NotBeNull();
    result.TotalPoints.Should().Be(500);
    employee.PointsBalance.Should().Be(500);
}

[Fact]
public async Task CreateOrder_WithInsufficientPoints_ShouldThrowException()
{
    // Arrange
    var employee = new Employee { Id = "emp1", PointsBalance = 100 };
    var product = new Product { Id = "prod1", Points = 500, Stock = 10 };
    
    // Act & Assert
    await Assert.ThrowsAsync<BusinessException>(
        () => _orderService.CreateOrder(employee.Id, product.Id, 1)
    );
}
```

### 2. 属性测试

**目标**: 验证系统在所有输入下都满足正确性属性

**工具**: FsCheck (F#) 或 CsCheck (C#)

**配置**: 每个属性测试至少运行100次迭代

**标签格式**: `// Feature: awsomeshop, Property {number}: {property_text}`

**示例**:
```csharp
// Feature: awsomeshop, Property 1: 积分余额非负不变量
[Property]
public Property PointsBalance_ShouldNeverBeNegative()
{
    return Prop.ForAll<List<PointsOperation>>(operations =>
    {
        var employee = new Employee { PointsBalance = 1000 };
        
        foreach (var op in operations)
        {
            ApplyOperation(employee, op);
        }
        
        return employee.PointsBalance >= 0;
    });
}

// Feature: awsomeshop, Property 3: 购物车总积分计算正确性
[Property]
public Property ShoppingCart_TotalPoints_ShouldEqualSumOfItems()
{
    return Prop.ForAll<List<CartItem>>(items =>
    {
        var cart = new ShoppingCart { Items = items };
        var expectedTotal = items.Sum(i => i.Product.Points * i.Quantity);
        
        return cart.TotalPoints == expectedTotal;
    });
}

// Feature: awsomeshop, Property 5: 兑换幂等性
[Property]
public async Task<Property> Redemption_ShouldBeIdempotent()
{
    return Prop.ForAll<RedemptionRequest>(request =>
    {
        // 提交相同请求3次
        var task1 = _orderService.CreateOrder(request);
        var task2 = _orderService.CreateOrder(request);
        var task3 = _orderService.CreateOrder(request);
        
        await Task.WhenAll(task1, task2, task3);
        
        // 验证只创建了一个订单
        var orders = await _orderRepository.GetByEmployee(request.EmployeeId);
        return orders.Count(o => o.ProductId == request.ProductId) == 1;
    });
}
```

### 3. 集成测试

**目标**: 测试多个组件协同工作

**工具**: xUnit + TestContainers (Docker容器化测试环境)

**示例**:
```csharp
public class OrderIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task CompleteOrderFlow_ShouldWorkEndToEnd()
    {
        // Arrange
        var client = _factory.CreateClient();
        var loginResponse = await client.PostAsync("/api/auth/login", ...);
        var token = await loginResponse.Content.ReadAsStringAsync();
        
        // Act - 添加到购物车
        await client.PostAsync("/api/cart/items", ...);
        
        // Act - 创建订单
        var orderResponse = await client.PostAsync("/api/orders", ...);
        
        // Assert
        orderResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // 验证积分扣除
        var pointsResponse = await client.GetAsync("/api/points/balance");
        var balance = await pointsResponse.Content.ReadFromJsonAsync<PointsBalance>();
        balance.Balance.Should().BeLessThan(1000);
    }
}
```

### 4. 端到端测试

**目标**: 从用户角度测试完整业务流程

**工具**: Playwright 或 Selenium

**示例场景**:
1. 员工登录 → 浏览产品 → 加入购物车 → 兑换 → 查看订单
2. 管理员登录 → 添加产品 → 发放积分 → 处理订单

### 测试覆盖率目标

- **单元测试**: 代码覆盖率 > 80%
- **属性测试**: 所有核心业务逻辑都有对应属性测试
- **集成测试**: 覆盖所有主要API端点
- **端到端测试**: 覆盖所有关键用户流程

### 持续集成

```yaml
# .github/workflows/ci.yml
name: CI

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: '8.0.x'
      
      - name: Restore dependencies
        run: dotnet restore
      
      - name: Build
        run: dotnet build --no-restore
      
      - name: Run unit tests
        run: dotnet test --no-build --verbosity normal --filter Category=Unit
      
      - name: Run property tests
        run: dotnet test --no-build --verbosity normal --filter Category=Property
      
      - name: Run integration tests
        run: dotnet test --no-build --verbosity normal --filter Category=Integration
      
      - name: Generate coverage report
        run: dotnet test --collect:"XPlat Code Coverage"
```

---

## 安全设计

### 1. 认证与授权

**JWT Token 结构**:
```json
{
  "sub": "employee_id",
  "email": "user@example.com",
  "role": "Employee",
  "exp": 1234567890,
  "iat": 1234567890
}
```

**权限矩阵**:
| 操作 | 员工 | 产品管理员 | 积分管理员 | 超级管理员 |
|------|------|-----------|-----------|-----------|
| 浏览产品 | ✓ | ✓ | ✓ | ✓ |
| 兑换产品 | ✓ | ✗ | ✗ | ✗ |
| 管理产品 | ✗ | ✓ | ✗ | ✓ |
| 管理积分 | ✗ | ✗ | ✓ | ✓ |
| 查看日志 | ✗ | ✗ | ✗ | ✓ |

### 2. 数据加密

- **传输加密**: 所有API使用HTTPS (TLS 1.3)
- **存储加密**: 密码使用BCrypt加密，盐值随机生成
- **敏感数据脱敏**: 
  - 邮箱: `user@example.com` → `u***@example.com`
  - 手机: `13812345678` → `138****5678`

### 3. 防护措施

- **SQL注入**: 使用参数化查询和ORM
- **XSS攻击**: 前端输入验证和输出转义
- **CSRF攻击**: 使用CSRF Token
- **重放攻击**: 请求签名 + 时间戳验证
- **暴力破解**: 登录失败5次后锁定账号15分钟
- **DDoS防护**: API限流（每IP每分钟100请求）

---

## 部署架构

### 生产环境部署

```
┌─────────────────────────────────────────┐
│          负载均衡器 (Nginx)              │
│          SSL终止 / 限流                  │
└─────────────────────────────────────────┘
                    │
        ┌───────────┴───────────┐
        │                       │
┌───────▼────────┐    ┌────────▼────────┐
│  Web Server 1  │    │  Web Server 2   │
│  (.NET API)    │    │  (.NET API)     │
└───────┬────────┘    └────────┬────────┘
        │                       │
        └───────────┬───────────┘
                    │
        ┌───────────┴───────────┐
        │                       │
┌───────▼────────┐    ┌────────▼────────┐
│  MySQL Master  │───▶│  MySQL Slave    │
│  (读写)        │    │  (只读)         │
└────────────────┘    └─────────────────┘
        │
┌───────▼────────┐
│  Redis Cluster │
│  (缓存/会话)   │
└────────────────┘
```

### Docker Compose 配置

```yaml
version: '3.8'

services:
  api:
    image: awsomeshop-api:latest
    ports:
      - "5000:80"
    environment:
      - ConnectionStrings__DefaultConnection=Server=mysql;Database=awsomeshop;
      - Redis__ConnectionString=redis:6379
    depends_on:
      - mysql
      - redis
  
  mysql:
    image: mysql:8.0
    environment:
      - MYSQL_ROOT_PASSWORD=password
      - MYSQL_DATABASE=awsomeshop
    volumes:
      - mysql-data:/var/lib/mysql
  
  redis:
    image: redis:7-alpine
    volumes:
      - redis-data:/data

volumes:
  mysql-data:
  redis-data:
```

---

**文档版本**: 1.0  
**最后更新**: 2026-01-16  
**文档状态**: 待审查

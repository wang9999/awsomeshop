# AWSomeShop 架构图

本文档使用C4模型思想描述AWSomeShop系统的架构，使用标准Mermaid语法绘制。

---

## Level 1: 系统上下文图 (System Context)

展示系统与外部用户和系统的交互关系。

```mermaid
graph TB
    Employee[员工]
    Admin[管理员]
    System[AWSomeShop系统]
    Email[邮件系统]
    Storage[对象存储]
    SMS[短信服务]
    
    Employee --> System
    Admin --> System
    System --> Email
    System --> Storage
    System -.-> SMS
    
    style System fill:#1168bd,stroke:#0b4884,color:#ffffff
    style Employee fill:#08427b,stroke:#052e56,color:#ffffff
    style Admin fill:#08427b,stroke:#052e56,color:#ffffff
    style Email fill:#999999,stroke:#6b6b6b,color:#ffffff
    style Storage fill:#999999,stroke:#6b6b6b,color:#ffffff
    style SMS fill:#999999,stroke:#6b6b6b,color:#ffffff
```

---

## Level 2: 容器图 (Container Diagram)

展示系统内部的主要容器（应用程序、数据存储等）及其交互。

```mermaid
graph TB
    Employee[员工]
    Admin[管理员]
    
    WebEmployee[员工Web应用]
    WebAdmin[管理员Web应用]
    Mobile[移动端H5]
    
    Gateway[API网关]
    
    AuthSvc[认证服务]
    ProductSvc[产品服务]
    OrderSvc[订单服务]
    PointsSvc[积分服务]
    NotifySvc[通知服务]
    AdminSvc[管理服务]
    
    MySQL[(MySQL数据库)]
    Redis[(Redis缓存)]
    
    Storage[对象存储]
    Email[邮件系统]
    
    Employee --> WebEmployee
    Employee --> Mobile
    Admin --> WebAdmin
    
    WebEmployee --> Gateway
    WebAdmin --> Gateway
    Mobile --> Gateway
    
    Gateway --> AuthSvc
    Gateway --> ProductSvc
    Gateway --> OrderSvc
    Gateway --> PointsSvc
    Gateway --> NotifySvc
    Gateway --> AdminSvc
    
    AuthSvc --> MySQL
    AuthSvc --> Redis
    
    ProductSvc --> MySQL
    ProductSvc --> Redis
    ProductSvc --> Storage
    
    OrderSvc --> MySQL
    OrderSvc -.-> PointsSvc
    OrderSvc -.-> ProductSvc
    OrderSvc -.-> NotifySvc
    
    PointsSvc --> MySQL
    PointsSvc -.-> NotifySvc
    
    NotifySvc --> MySQL
    NotifySvc --> Email
    
    AdminSvc --> MySQL
    AdminSvc -.-> ProductSvc
    AdminSvc -.-> PointsSvc
    
    style Gateway fill:#1168bd,stroke:#0b4884,color:#ffffff
    style AuthSvc fill:#1168bd,stroke:#0b4884,color:#ffffff
    style ProductSvc fill:#1168bd,stroke:#0b4884,color:#ffffff
    style OrderSvc fill:#1168bd,stroke:#0b4884,color:#ffffff
    style PointsSvc fill:#1168bd,stroke:#0b4884,color:#ffffff
    style NotifySvc fill:#1168bd,stroke:#0b4884,color:#ffffff
    style AdminSvc fill:#1168bd,stroke:#0b4884,color:#ffffff
    style MySQL fill:#2b7c85,stroke:#1a4d54,color:#ffffff
    style Redis fill:#2b7c85,stroke:#1a4d54,color:#ffffff
```

---

## Level 3: 组件图 - 订单服务 (Component Diagram - Order Service)

展示订单服务内部的组件结构。

```mermaid
graph TB
    subgraph 订单服务
        Controller[订单控制器<br/>OrderController<br/>处理HTTP请求]
        Service[订单业务逻辑<br/>OrderService<br/>实现业务规则]
        Validator[订单验证器<br/>OrderValidator<br/>验证规则]
        Repository[订单仓储<br/>OrderRepository<br/>数据访问]
        TxManager[事务管理器<br/>TransactionManager<br/>管理分布式事务]
    end
    
    MySQL[(MySQL<br/>订单数据)]
    PointsSvc[积分服务]
    ProductSvc[产品服务]
    NotifySvc[通知服务]
    
    Controller --> Service
    Service --> Validator
    Service --> Repository
    Service --> TxManager
    
    TxManager -.->|扣除积分| PointsSvc
    TxManager -.->|减少库存| ProductSvc
    TxManager -.->|发送通知| NotifySvc
    
    Repository -->|EF Core| MySQL
    
    style Controller fill:#1168bd,stroke:#0b4884,color:#ffffff
    style Service fill:#1168bd,stroke:#0b4884,color:#ffffff
    style Validator fill:#1168bd,stroke:#0b4884,color:#ffffff
    style Repository fill:#1168bd,stroke:#0b4884,color:#ffffff
    style TxManager fill:#1168bd,stroke:#0b4884,color:#ffffff
```

---

## Level 3: 组件图 - 积分服务 (Component Diagram - Points Service)

展示积分服务内部的组件结构。

```mermaid
graph TB
    subgraph 积分服务
        Controller[积分控制器<br/>PointsController]
        Service[积分业务逻辑<br/>PointsService]
        Calculator[积分计算器<br/>PointsCalculator<br/>计算余额和过期]
        Validator[积分验证器<br/>PointsValidator]
        Repository[积分仓储<br/>PointsRepository]
        AutoJob[自动发放任务<br/>AutoGrantJob<br/>每月/入职/生日]
        ExpiryJob[过期处理任务<br/>ExpiryJob<br/>处理过期积分]
    end
    
    MySQL[(MySQL<br/>积分数据)]
    NotifySvc[通知服务]
    
    Controller --> Service
    Service --> Calculator
    Service --> Validator
    Service --> Repository
    
    AutoJob -.->|触发| Service
    ExpiryJob -.->|触发| Service
    
    Service -.->|通知| NotifySvc
    Repository -->|EF Core| MySQL
    
    style Controller fill:#1168bd,stroke:#0b4884,color:#ffffff
    style Service fill:#1168bd,stroke:#0b4884,color:#ffffff
    style Calculator fill:#1168bd,stroke:#0b4884,color:#ffffff
    style Validator fill:#1168bd,stroke:#0b4884,color:#ffffff
    style Repository fill:#1168bd,stroke:#0b4884,color:#ffffff
    style AutoJob fill:#85bbf0,stroke:#5d82a8,color:#000000
    style ExpiryJob fill:#85bbf0,stroke:#5d82a8,color:#000000
```

---

## Level 3: 组件图 - 产品服务 (Component Diagram - Product Service)

展示产品服务内部的组件结构。

```mermaid
graph TB
    subgraph 产品服务
        Controller[产品控制器<br/>ProductController]
        Service[产品业务逻辑<br/>ProductService]
        Search[产品搜索引擎<br/>ProductSearch<br/>全文搜索]
        StockMgr[库存管理器<br/>StockManager<br/>并发控制]
        Repository[产品仓储<br/>ProductRepository]
        Uploader[图片上传器<br/>ImageUploader]
    end
    
    MySQL[(MySQL<br/>产品数据)]
    Redis[(Redis<br/>缓存)]
    Storage[对象存储<br/>OSS/S3]
    
    Controller --> Service
    Service --> Search
    Service --> StockMgr
    Service --> Repository
    Service --> Uploader
    
    Repository -->|EF Core| MySQL
    Service -->|缓存| Redis
    Uploader -->|S3 SDK| Storage
    
    style Controller fill:#1168bd,stroke:#0b4884,color:#ffffff
    style Service fill:#1168bd,stroke:#0b4884,color:#ffffff
    style Search fill:#1168bd,stroke:#0b4884,color:#ffffff
    style StockMgr fill:#1168bd,stroke:#0b4884,color:#ffffff
    style Repository fill:#1168bd,stroke:#0b4884,color:#ffffff
    style Uploader fill:#1168bd,stroke:#0b4884,color:#ffffff
```

---

## 数据流图 - 兑换流程 (Data Flow - Redemption Process)

展示员工兑换产品的完整数据流。

```mermaid
sequenceDiagram
    participant E as 员工
    participant W as Web应用
    participant G as API网关
    participant O as 订单服务
    participant P as 积分服务
    participant R as 产品服务
    participant N as 通知服务
    participant D as 数据库
    
    E->>W: 点击兑换
    W->>G: POST请求
    G->>O: 转发请求
    
    O->>O: 验证参数
    O->>D: 查询员工
    D-->>O: 返回数据
    
    O->>P: 检查积分
    P-->>O: 返回余额
    
    O->>R: 检查库存
    R-->>O: 返回库存
    
    O->>O: 验证规则
    
    Note over O,D: 事务开始
    O->>P: 扣除积分
    P->>D: 更新积分
    P->>D: 记录交易
    
    O->>R: 减少库存
    R->>D: 更新库存
    
    O->>D: 创建订单
    O->>D: 创建明细
    Note over O,D: 事务提交
    
    O->>N: 发送通知
    N->>D: 记录通知
    
    O-->>G: 返回订单
    G-->>W: 返回响应
    W-->>E: 显示成功
```

---

## 部署图 (Deployment Diagram)

展示系统在生产环境中的部署架构。

```mermaid
graph TB
    subgraph CDN层
        CDN[CDN<br/>CloudFlare/阿里云<br/>静态资源分发]
    end
    
    subgraph Kubernetes集群
        subgraph Ingress层
            Nginx[Nginx Ingress<br/>负载均衡<br/>SSL终止]
        end
        
        subgraph 应用Pod
            API1[API实例1<br/>.NET Core]
            API2[API实例2<br/>.NET Core]
            API3[API实例3<br/>.NET Core]
        end
        
        subgraph 后台任务Pod
            Job[定时任务<br/>积分发放/过期]
        end
    end
    
    subgraph 数据库集群
        MySQLMaster[(MySQL主库<br/>读写)]
        MySQLSlave[(MySQL从库<br/>只读)]
    end
    
    subgraph 缓存集群
        Redis1[(Redis节点1)]
        Redis2[(Redis节点2)]
        Redis3[(Redis节点3)]
    end
    
    subgraph 对象存储
        OSS[阿里云OSS<br/>AWS S3<br/>图片存储]
    end
    
    CDN -->|回源| Nginx
    Nginx --> API1
    Nginx --> API2
    Nginx --> API3
    
    API1 --> MySQLMaster
    API2 --> MySQLSlave
    API3 --> MySQLSlave
    
    MySQLMaster -.->|主从复制| MySQLSlave
    
    API1 --> Redis1
    API2 --> Redis2
    API3 --> Redis3
    
    API1 --> OSS
    Job --> MySQLMaster
    
    style CDN fill:#85bbf0,stroke:#5d82a8,color:#000000
    style Nginx fill:#1168bd,stroke:#0b4884,color:#ffffff
    style API1 fill:#1168bd,stroke:#0b4884,color:#ffffff
    style API2 fill:#1168bd,stroke:#0b4884,color:#ffffff
    style API3 fill:#1168bd,stroke:#0b4884,color:#ffffff
    style Job fill:#85bbf0,stroke:#5d82a8,color:#000000
    style MySQLMaster fill:#2b7c85,stroke:#1a4d54,color:#ffffff
    style MySQLSlave fill:#2b7c85,stroke:#1a4d54,color:#ffffff
    style Redis1 fill:#2b7c85,stroke:#1a4d54,color:#ffffff
    style Redis2 fill:#2b7c85,stroke:#1a4d54,color:#ffffff
    style Redis3 fill:#2b7c85,stroke:#1a4d54,color:#ffffff
```

---

## 图表说明

### C4模型层次说明

1. **Level 1 - 系统上下文图**: 展示系统与外部用户和系统的关系
2. **Level 2 - 容器图**: 展示系统内部的应用程序和数据存储
3. **Level 3 - 组件图**: 展示容器内部的组件结构（订单服务、积分服务、产品服务）
4. **数据流图**: 展示关键业务流程的数据流动
5. **部署图**: 展示生产环境的部署架构

### 图例说明

- **实线箭头** (→): 直接调用或数据流
- **虚线箭头** (-.->): 异步调用或可选依赖
- **蓝色节点**: 应用服务和组件
- **青色节点**: 数据存储（数据库、缓存）
- **浅蓝色节点**: 后台任务和定时任务
- **灰色节点**: 外部系统

### 架构特点

- **微服务架构**: 按业务领域划分为6个独立服务
- **前后端分离**: React前端 + .NET后端
- **读写分离**: MySQL主从架构，提高查询性能
- **缓存策略**: Redis缓存热点数据
- **容器化部署**: Docker + Kubernetes，支持弹性伸缩
- **高可用**: 多实例部署 + 负载均衡

---

**文档版本**: 1.0  
**最后更新**: 2026-01-16  
**工具**: Mermaid (兼容 8.8.0+)

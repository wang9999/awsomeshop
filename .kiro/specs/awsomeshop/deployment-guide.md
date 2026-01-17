# AWSomeShop 部署指南

## 生产环境配置

### 1. HTTPS配置

#### 1.1 开发环境HTTPS
开发环境已默认启用HTTPS，使用ASP.NET Core开发证书。

```bash
# 信任开发证书
dotnet dev-certs https --trust
```

#### 1.2 生产环境HTTPS配置

**方式一：使用Kestrel直接配置SSL证书**

在 `appsettings.Production.json` 中配置：

```json
{
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://0.0.0.0:5000"
      },
      "Https": {
        "Url": "https://0.0.0.0:5001",
        "Certificate": {
          "Path": "/path/to/certificate.pfx",
          "Password": "certificate-password"
        }
      }
    }
  }
}
```

**方式二：使用Nginx反向代理（推荐）**

```nginx
server {
    listen 80;
    server_name awsomeshop.example.com;
    
    # 强制HTTPS重定向
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name awsomeshop.example.com;

    # SSL证书配置
    ssl_certificate /etc/ssl/certs/awsomeshop.crt;
    ssl_certificate_key /etc/ssl/private/awsomeshop.key;
    
    # SSL安全配置
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers HIGH:!aNULL:!MD5;
    ssl_prefer_server_ciphers on;
    
    # 安全响应头
    add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;

    # 反向代理到后端
    location /api/ {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }

    # 前端静态文件
    location / {
        root /var/www/awsomeshop/frontend;
        try_files $uri $uri/ /index.html;
    }
}
```

**方式三：使用Let's Encrypt免费证书**

```bash
# 安装certbot
sudo apt-get install certbot python3-certbot-nginx

# 获取证书
sudo certbot --nginx -d awsomeshop.example.com

# 自动续期
sudo certbot renew --dry-run
```

### 2. 环境变量配置

创建 `appsettings.Production.json`：

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-mysql-server;Port=3306;Database=awsomeshop;User=awsomeshop_user;Password=${DB_PASSWORD};CharSet=utf8mb4;",
    "Redis": "prod-redis-server:6379,password=${REDIS_PASSWORD}"
  },
  "Jwt": {
    "SecretKey": "${JWT_SECRET_KEY}",
    "Issuer": "AWSomeShop",
    "Audience": "AWSomeShop-Users",
    "ExpirationMinutes": 30
  },
  "AllowedHosts": "awsomeshop.example.com"
}
```

### 3. 数据库配置

#### 3.1 MySQL生产配置

```sql
-- 创建数据库
CREATE DATABASE awsomeshop CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- 创建用户
CREATE USER 'awsomeshop_user'@'%' IDENTIFIED BY 'strong_password';

-- 授权
GRANT SELECT, INSERT, UPDATE, DELETE ON awsomeshop.* TO 'awsomeshop_user'@'%';
FLUSH PRIVILEGES;

-- 性能优化配置
SET GLOBAL innodb_buffer_pool_size = 2G;
SET GLOBAL max_connections = 200;
```

#### 3.2 运行数据库迁移

```bash
cd backend/src/AWSomeShop.API

# 生产环境迁移
dotnet ef database update --environment Production
```

### 4. Redis配置

#### 4.1 Redis生产配置

编辑 `/etc/redis/redis.conf`：

```conf
# 绑定地址
bind 0.0.0.0

# 端口
port 6379

# 密码
requirepass your_strong_redis_password

# 持久化
save 900 1
save 300 10
save 60 10000

# 最大内存
maxmemory 1gb
maxmemory-policy allkeys-lru

# 日志
loglevel notice
logfile /var/log/redis/redis-server.log
```

### 5. Docker部署

#### 5.1 Dockerfile (后端)

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["backend/src/AWSomeShop.API/AWSomeShop.API.csproj", "AWSomeShop.API/"]
RUN dotnet restore "AWSomeShop.API/AWSomeShop.API.csproj"
COPY backend/src/AWSomeShop.API/ AWSomeShop.API/
WORKDIR "/src/AWSomeShop.API"
RUN dotnet build "AWSomeShop.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AWSomeShop.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AWSomeShop.API.dll"]
```

#### 5.2 Dockerfile (前端)

```dockerfile
FROM node:18-alpine AS build
WORKDIR /app
COPY frontend/package*.json ./
RUN npm ci
COPY frontend/ ./
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

#### 5.3 docker-compose.yml

```yaml
version: '3.8'

services:
  mysql:
    image: mysql:8.0
    container_name: awsomeshop-mysql
    environment:
      MYSQL_ROOT_PASSWORD: ${MYSQL_ROOT_PASSWORD}
      MYSQL_DATABASE: awsomeshop
      MYSQL_USER: awsomeshop_user
      MYSQL_PASSWORD: ${MYSQL_PASSWORD}
    volumes:
      - mysql-data:/var/lib/mysql
    ports:
      - "3306:3306"
    networks:
      - awsomeshop-network

  redis:
    image: redis:7-alpine
    container_name: awsomeshop-redis
    command: redis-server --requirepass ${REDIS_PASSWORD}
    volumes:
      - redis-data:/data
    ports:
      - "6379:6379"
    networks:
      - awsomeshop-network

  backend:
    build:
      context: .
      dockerfile: Dockerfile.backend
    container_name: awsomeshop-backend
    environment:
      ASPNETCORE_ENVIRONMENT: Production
      ConnectionStrings__DefaultConnection: "Server=mysql;Port=3306;Database=awsomeshop;User=awsomeshop_user;Password=${MYSQL_PASSWORD};CharSet=utf8mb4;"
      ConnectionStrings__Redis: "redis:6379,password=${REDIS_PASSWORD}"
      Jwt__SecretKey: ${JWT_SECRET_KEY}
    depends_on:
      - mysql
      - redis
    ports:
      - "5000:5000"
    networks:
      - awsomeshop-network

  frontend:
    build:
      context: .
      dockerfile: Dockerfile.frontend
    container_name: awsomeshop-frontend
    ports:
      - "80:80"
      - "443:443"
    depends_on:
      - backend
    networks:
      - awsomeshop-network

volumes:
  mysql-data:
  redis-data:

networks:
  awsomeshop-network:
    driver: bridge
```

#### 5.4 环境变量文件 (.env)

```env
MYSQL_ROOT_PASSWORD=your_root_password
MYSQL_PASSWORD=your_mysql_password
REDIS_PASSWORD=your_redis_password
JWT_SECRET_KEY=your_jwt_secret_key_at_least_32_characters_long
```

### 6. 部署步骤

#### 6.1 使用Docker Compose部署

```bash
# 1. 克隆代码
git clone https://github.com/your-org/awsomeshop.git
cd awsomeshop

# 2. 配置环境变量
cp .env.example .env
# 编辑.env文件，设置生产环境密码

# 3. 构建并启动服务
docker-compose up -d

# 4. 查看日志
docker-compose logs -f

# 5. 运行数据库迁移
docker-compose exec backend dotnet ef database update
```

#### 6.2 手动部署

```bash
# 后端部署
cd backend/src/AWSomeShop.API
dotnet publish -c Release -o /var/www/awsomeshop/backend

# 前端部署
cd frontend
npm install
npm run build
cp -r dist/* /var/www/awsomeshop/frontend/

# 配置systemd服务
sudo systemctl enable awsomeshop-backend
sudo systemctl start awsomeshop-backend
```

### 7. 监控和日志

#### 7.1 应用日志

```bash
# 查看后端日志
docker-compose logs -f backend

# 查看Nginx日志
docker-compose logs -f frontend
```

#### 7.2 健康检查

在 `Program.cs` 中添加健康检查：

```csharp
app.MapHealthChecks("/health");
```

### 8. 性能优化

#### 8.1 数据库索引

确保以下索引已创建：
- Employee: Email, EmployeeNumber
- Product: Category, Status, Points
- Order: Status, CreatedAt, EmployeeId
- PointsTransaction: Type, CreatedAt, ExpiryDate

#### 8.2 Redis缓存策略

- 产品列表：10分钟
- 产品详情：30分钟
- 用户会话：30分钟
- 验证码：5分钟

#### 8.3 CDN配置

将静态资源（图片、CSS、JS）部署到CDN：
- 使用CloudFront、阿里云CDN等
- 配置缓存策略
- 启用Gzip压缩

### 9. 安全检查清单

- [ ] HTTPS已启用
- [ ] 数据库密码已更改
- [ ] Redis密码已设置
- [ ] JWT密钥已更改（至少32字符）
- [ ] CORS配置正确
- [ ] 防火墙规则已配置
- [ ] 定期备份已设置
- [ ] 日志监控已配置
- [ ] 限流中间件已启用
- [ ] 防重复提交已启用

### 10. 备份策略

#### 10.1 数据库备份

```bash
# 每日备份脚本
#!/bin/bash
DATE=$(date +%Y%m%d_%H%M%S)
BACKUP_DIR="/backup/mysql"
mysqldump -u root -p${MYSQL_ROOT_PASSWORD} awsomeshop > ${BACKUP_DIR}/awsomeshop_${DATE}.sql
gzip ${BACKUP_DIR}/awsomeshop_${DATE}.sql

# 保留最近30天的备份
find ${BACKUP_DIR} -name "*.sql.gz" -mtime +30 -delete
```

#### 10.2 Redis备份

```bash
# Redis自动持久化
# RDB快照已在redis.conf中配置
# 定期复制dump.rdb文件到备份位置
```

### 11. 故障排查

#### 11.1 常见问题

**问题1：数据库连接失败**
```bash
# 检查MySQL是否运行
docker-compose ps mysql
# 检查连接字符串
docker-compose logs backend | grep "Connection"
```

**问题2：Redis连接失败**
```bash
# 测试Redis连接
docker-compose exec redis redis-cli -a ${REDIS_PASSWORD} ping
```

**问题3：HTTPS证书问题**
```bash
# 检查证书有效期
openssl x509 -in /path/to/cert.crt -noout -dates
```

### 12. 性能监控

推荐使用以下工具：
- **Application Insights** (Azure)
- **Prometheus + Grafana**
- **ELK Stack** (日志分析)
- **New Relic** (APM)

---

## 快速部署命令

```bash
# 开发环境
docker-compose -f docker-compose.dev.yml up -d

# 生产环境
docker-compose -f docker-compose.prod.yml up -d

# 停止服务
docker-compose down

# 重启服务
docker-compose restart

# 查看状态
docker-compose ps
```

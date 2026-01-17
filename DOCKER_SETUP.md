# Docker 环境设置指南

本指南将帮助你使用Docker快速设置AWSomeShop所需的MySQL和Redis服务。

## 步骤1：安装Docker Desktop

### macOS安装方法

1. **下载Docker Desktop for Mac**
   - 访问：https://www.docker.com/products/docker-desktop/
   - 点击"Download for Mac"
   - 根据你的Mac芯片选择：
     - Apple Silicon (M1/M2/M3): 选择"Mac with Apple chip"
     - Intel芯片: 选择"Mac with Intel chip"

2. **安装Docker Desktop**
   - 打开下载的 `.dmg` 文件
   - 将Docker图标拖到Applications文件夹
   - 从Applications文件夹启动Docker
   - 首次启动需要授权，输入你的Mac密码

3. **验证安装**
   ```bash
   docker --version
   docker-compose --version
   ```
   
   应该看到类似输出：
   ```
   Docker version 24.x.x
   Docker Compose version v2.x.x
   ```

## 步骤2：启动MySQL和Redis

项目根目录已经包含了 `docker-compose.yml` 配置文件。

### 启动服务

在项目根目录执行：

```bash
docker-compose up -d
```

参数说明：
- `-d`: 后台运行（detached mode）

### 查看服务状态

```bash
docker-compose ps
```

应该看到两个服务都在运行：
```
NAME                  IMAGE          STATUS         PORTS
awsomeshop-mysql      mysql:8.0      Up (healthy)   0.0.0.0:3306->3306/tcp
awsomeshop-redis      redis:7-alpine Up (healthy)   0.0.0.0:6379->6379/tcp
```

### 查看服务日志

```bash
# 查看所有服务日志
docker-compose logs

# 查看MySQL日志
docker-compose logs mysql

# 查看Redis日志
docker-compose logs redis

# 实时跟踪日志
docker-compose logs -f
```

## 步骤3：验证服务连接

### 测试MySQL连接

```bash
docker exec -it awsomeshop-mysql mysql -uroot -pyour_password -e "SELECT 1;"
```

### 测试Redis连接

```bash
docker exec -it awsomeshop-redis redis-cli ping
```

应该返回：`PONG`

## 步骤4：启动后端服务

服务启动后，返回项目目录启动后端：

```bash
cd backend/src/AWSomeShop.API
~/.dotnet/dotnet run
```

后端会自动：
1. 连接到MySQL数据库
2. 创建数据库表结构
3. 填充测试数据（开发模式）
4. 连接到Redis缓存

## 常用Docker命令

### 停止服务

```bash
docker-compose stop
```

### 启动已停止的服务

```bash
docker-compose start
```

### 重启服务

```bash
docker-compose restart
```

### 停止并删除容器（保留数据）

```bash
docker-compose down
```

### 停止并删除容器和数据卷（清空所有数据）

```bash
docker-compose down -v
```

### 查看容器资源使用情况

```bash
docker stats awsomeshop-mysql awsomeshop-redis
```

## 数据持久化

Docker Compose配置使用了命名卷来持久化数据：
- `mysql_data`: MySQL数据库文件
- `redis_data`: Redis持久化文件

即使删除容器，数据也会保留。只有使用 `docker-compose down -v` 才会删除数据。

## 连接信息

### MySQL
- **主机**: localhost
- **端口**: 3306
- **用户**: root
- **密码**: your_password
- **数据库**: awsomeshop_dev

### Redis
- **主机**: localhost
- **端口**: 6379
- **密码**: 无

## 故障排除

### 端口已被占用

如果看到端口冲突错误：

```bash
# 查看占用3306端口的进程
lsof -i :3306

# 查看占用6379端口的进程
lsof -i :6379
```

可以修改 `docker-compose.yml` 中的端口映射，例如：
```yaml
ports:
  - "3307:3306"  # 使用3307代替3306
```

### 容器无法启动

```bash
# 查看详细日志
docker-compose logs mysql
docker-compose logs redis

# 重新创建容器
docker-compose down
docker-compose up -d
```

### 清理Docker资源

```bash
# 清理未使用的容器、网络、镜像
docker system prune

# 清理所有未使用的卷（谨慎使用）
docker volume prune
```

## 性能优化

Docker Desktop默认配置通常足够开发使用。如需调整：

1. 打开Docker Desktop
2. 进入 Settings → Resources
3. 调整：
   - CPUs: 建议至少2核
   - Memory: 建议至少4GB
   - Disk: 根据需要调整

## 下一步

服务启动后，你可以：

1. ✅ 启动后端API服务
2. ✅ 使用测试账号登录前端
3. ✅ 体验完整的应用功能

测试账号信息请参考项目README.md文件。

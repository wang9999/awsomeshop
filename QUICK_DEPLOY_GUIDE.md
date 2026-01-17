# 🚀 快速部署指南 - 5分钟让别人访问你的应用

## 方法1：使用ngrok（最简单，推荐）⭐

### 前提条件
- ✅ 后端和前端服务正在运行
- ✅ MySQL和Redis容器正在运行

### 步骤

#### 1. 安装ngrok
```bash
# macOS
brew install ngrok

# 或访问 https://ngrok.com/download 下载
```

#### 2. 注册ngrok账号
1. 访问 https://ngrok.com
2. 注册免费账号
3. 获取你的 authtoken

#### 3. 配置ngrok
```bash
ngrok config add-authtoken 你的token
```

#### 4. 运行部署脚本
```bash
./deploy-ngrok.sh
```

脚本会自动：
- ✅ 检查服务状态
- ✅ 启动后端公网访问
- ✅ 启动前端公网访问
- ✅ 自动配置环境变量
- ✅ 显示公网访问地址

#### 5. 分享链接
脚本运行后会显示类似：
```
🎉 部署完成！

📱 访问地址：
   前端: https://abc123.ngrok.io
   后端: https://xyz789.ngrok.io

🔐 测试账号：
   员工: employee1@awsome.com / Employee@123
   管理员: superadmin / Admin@123
```

把前端地址分享给别人就可以访问了！

#### 6. 停止服务
```bash
./stop-ngrok.sh
```

---

## 方法2：使用Railway + Vercel（专业部署）

详细步骤请查看 `DEPLOYMENT_OPTIONS.md` 文件。

### 优点
- ✅ 免费或低成本（$5/月）
- ✅ 自动HTTPS
- ✅ 稳定可靠
- ✅ 适合作品集展示

### 缺点
- ⚠️ 需要30-60分钟配置
- ⚠️ 需要GitHub账号
- ⚠️ 需要注册多个服务

---

## 方法3：使用云服务器（生产环境）

### 推荐服务商
- 阿里云ECS
- 腾讯云CVM
- AWS EC2
- DigitalOcean

### 成本
- ¥100-300/月（国内）
- $10-50/月（国外）

详细步骤请查看 `DEPLOYMENT_OPTIONS.md` 文件。

---

## 📊 方案对比

| 方案 | 成本 | 时间 | 难度 | 稳定性 | 适合场景 |
|------|------|------|------|--------|----------|
| ngrok | 免费 | 5分钟 | ⭐ | ⭐⭐ | 临时演示 |
| Railway+Vercel | 免费/$5 | 60分钟 | ⭐⭐ | ⭐⭐⭐⭐ | 作品展示 |
| 云服务器 | ¥100+/月 | 4小时 | ⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ | 生产环境 |

---

## 💡 推荐选择

### 如果你想：
- **快速展示给朋友/老师看** → 使用 ngrok ⭐
- **放在简历/作品集** → 使用 Railway + Vercel ⭐⭐
- **实际生产使用** → 租用云服务器 ⭐⭐⭐

---

## ⚠️ ngrok注意事项

1. **免费版限制**
   - 每次重启URL会变化
   - 有连接数限制（40个/分钟）
   - 需要保持电脑运行

2. **付费版优势**（$8/月）
   - 固定域名
   - 无连接数限制
   - 更稳定

3. **使用技巧**
   - 演示前提前启动
   - 保持电脑不休眠
   - 确保网络稳定

---

## 🎯 快速开始

### 最快方式（5分钟）
```bash
# 1. 安装ngrok
brew install ngrok

# 2. 配置token（从 https://ngrok.com 获取）
ngrok config add-authtoken 你的token

# 3. 运行部署脚本
./deploy-ngrok.sh

# 4. 分享前端URL给别人
```

### 专业方式（60分钟）
查看 `DEPLOYMENT_OPTIONS.md` 中的 Railway + Vercel 部署指南

---

## 📚 相关文档

- `DEPLOYMENT_OPTIONS.md` - 详细部署方案对比
- `deploy-ngrok.sh` - ngrok自动部署脚本
- `stop-ngrok.sh` - 停止ngrok服务脚本
- `SERVICES_STATUS.md` - 服务状态说明

---

## 🆘 常见问题

### Q: ngrok URL每次都变化怎么办？
A: 免费版确实会变化。如果需要固定URL，可以：
   1. 升级到付费版（$8/月）
   2. 使用Railway + Vercel部署（免费）

### Q: 别人访问很慢怎么办？
A: ngrok免费版可能有延迟。建议：
   1. 使用Railway + Vercel（有CDN加速）
   2. 租用国内云服务器

### Q: 可以一直运行吗？
A: ngrok需要保持电脑运行。如果需要24/7运行：
   1. 使用Railway + Vercel
   2. 租用云服务器

### Q: 数据会丢失吗？
A: 
   - ngrok：数据在本地，不会丢失
   - Railway：免费版可能休眠，数据会保留
   - 云服务器：数据持久保存

---

**创建时间**: 2026-01-16  
**项目**: AWSomeShop 员工积分兑换平台  
**状态**: ✅ 可用

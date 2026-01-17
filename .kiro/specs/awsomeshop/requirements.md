# AWSomeShop 需求文档 (Requirements Document)

## 简介

AWSomeShop 是一个内部员工福利电商网站，旨在通过最小可行产品（MVP）验证员工积分兑换系统的商业模式。员工可以使用"AWSome积分"浏览和兑换预选产品，管理员可以管理产品信息和员工积分。

## 术语表 (Glossary)

- **System (系统)**: AWSomeShop 员工福利电商平台
- **Employee (员工)**: 使用积分兑换产品的内部员工用户
- **Administrator (管理员)**: 管理产品和积分的后台管理人员
- **AWSome_Points (AWSome积分)**: 员工用于兑换产品的虚拟货币
- **Product (产品)**: 可供员工兑换的商品
- **Order (订单)**: 员工兑换产品后生成的订单记录
- **Points_Transaction (积分交易)**: 积分的发放、扣除、消费等操作记录
- **Shopping_Cart (购物车)**: 员工临时存放待兑换产品的容器
- **Super_Administrator (超级管理员)**: 拥有所有权限的管理员
- **Product_Administrator (产品管理员)**: 负责产品管理的管理员
- **Points_Administrator (积分管理员)**: 负责积分管理的管理员

---

## 需求列表

### 需求 1: 员工认证与账户管理

**用户故事**: 作为员工，我希望能够安全地登录系统并管理我的账户信息，以便使用积分兑换产品。

#### 验收标准

1. WHEN Employee 提供有效的企业邮箱和密码 THEN THE System SHALL 验证凭据并允许登录
2. WHEN Employee 提供无效的企业邮箱或密码 THEN THE System SHALL 拒绝登录并显示错误提示
3. WHEN Employee 登录成功 THEN THE System SHALL 创建会话并设置2小时有效期
4. WHEN Employee 在30分钟内无任何操作 THEN THE System SHALL 自动退出登录
5. WHEN Employee 的密码存储到数据库 THEN THE System SHALL 使用加密算法加密存储
6. THE System SHALL 支持 Employee 在多个设备同时登录
7. WHEN Administrator 批量导入员工信息 THEN THE System SHALL 创建员工账户并包含姓名、工号、部门、邮箱字段
8. WHEN Employee 查看个人信息 THEN THE System SHALL 对敏感信息进行脱敏显示

---

### 需求 2: 产品浏览与搜索

**用户故事**: 作为员工，我希望能够浏览、搜索和筛选产品，以便找到我想要兑换的商品。

#### 验收标准

1. WHEN Employee 访问产品列表页面 THEN THE System SHALL 显示所有上架状态的产品
2. WHEN Product 的库存为0 THEN THE System SHALL 显示"缺货"状态但不隐藏产品
3. THE System SHALL 为每个 Product 显示产品名称、产品描述、产品图片、所需积分、库存数量
4. THE System SHALL 支持 Product 的多张图片轮播展示
5. WHEN Employee 点击产品 THEN THE System SHALL 显示产品详情页面
6. THE System SHALL 将 Product 分类为电子产品、生活用品、礼品卡、图书文具四个类别
7. WHEN Employee 选择产品分类 THEN THE System SHALL 仅显示该分类下的产品
8. WHEN Employee 输入搜索关键词 THEN THE System SHALL 返回产品名称或描述中包含关键词的产品列表
9. WHEN Employee 使用筛选功能 THEN THE System SHALL 支持按分类、积分范围、库存状态进行筛选

---

### 需求 3: 积分系统管理

**用户故事**: 作为员工，我希望能够查看我的积分余额和历史记录，以便了解我的积分情况。

#### 验收标准

1. THE System SHALL 为每个 Employee 维护 AWSome_Points 余额
2. THE System SHALL 确保 AWSome_Points 余额不能为负数
3. WHEN Employee 登录系统 THEN THE System SHALL 在页面显著位置显示当前积分余额
4. WHEN AWSome_Points 获得后满1年 THEN THE System SHALL 将该积分标记为过期并从余额中扣除
5. WHEN 每月1号到来 THEN THE System SHALL 自动为所有 Employee 发放500 AWSome_Points
6. WHEN Employee 入职 THEN THE System SHALL 自动为该 Employee 发放1000 AWSome_Points
7. WHEN Employee 生日到来 THEN THE System SHALL 自动为该 Employee 发放200 AWSome_Points
8. WHEN Points_Administrator 手动发放积分 THEN THE System SHALL 增加指定 Employee 的积分余额
9. WHEN Points_Administrator 因纠错或违规惩罚扣除积分 THEN THE System SHALL 减少指定 Employee 的积分余额
10. THE System SHALL 记录所有 Points_Transaction 包含时间、数量、原因/备注、操作人、余额变化字段
11. WHEN Employee 查看积分历史 THEN THE System SHALL 显示最近2年内的所有 Points_Transaction
12. WHEN Employee 导出积分历史 THEN THE System SHALL 生成包含所有历史记录的文件

---

### 需求 4: 购物车功能

**用户故事**: 作为员工，我希望能够将多个产品加入购物车并一次性兑换，以便提高兑换效率。

#### 验收标准

1. WHEN Employee 点击"加入购物车" THEN THE System SHALL 将选中的 Product 添加到 Shopping_Cart
2. WHEN Product 库存为0 THEN THE System SHALL 禁止将该 Product 添加到 Shopping_Cart
3. WHEN Employee 查看 Shopping_Cart THEN THE System SHALL 显示所有已添加的产品及所需总积分
4. WHEN Employee 修改 Shopping_Cart 中产品数量 THEN THE System SHALL 实时更新所需总积分
5. WHEN Employee 从 Shopping_Cart 中移除产品 THEN THE System SHALL 更新购物车内容和总积分
6. WHEN Shopping_Cart 中产品的库存变为0 THEN THE System SHALL 标记该产品为"缺货"并禁止兑换

---

### 需求 5: 产品兑换流程

**用户故事**: 作为员工，我希望能够使用积分兑换产品并跟踪订单状态，以便获得我想要的商品。

#### 验收标准

1. WHEN Employee 点击"兑换" THEN THE System SHALL 显示兑换确认页面包含产品信息、所需积分、收货地址
2. WHEN Employee 确认兑换且积分余额充足 THEN THE System SHALL 扣除相应积分并创建 Order
3. WHEN Employee 确认兑换但积分余额不足 THEN THE System SHALL 拒绝兑换并提示积分不足
4. WHEN Employee 确认兑换 THEN THE System SHALL 要求填写或选择收货地址
5. WHEN Employee 提交兑换 THEN THE System SHALL 验证防止重复提交
6. WHEN Employee 兑换时 THEN THE System SHALL 要求输入验证码
7. WHEN Order 创建成功 THEN THE System SHALL 将订单状态设置为"待确认"
8. THE System SHALL 支持 Order 状态包括：待确认、待发货、已发货、已完成、已取消
9. WHEN Employee 每月兑换次数达到5次 THEN THE System SHALL 禁止该 Employee 继续兑换
10. WHEN Employee 已兑换过某个 Product THEN THE System SHALL 禁止该 Employee 再次兑换同一产品
11. WHEN Order 状态为"待发货"且兑换时间在24小时内 THEN THE System SHALL 允许 Employee 取消兑换
12. WHEN Employee 取消兑换 THEN THE System SHALL 将订单状态改为"已取消"并退回积分
13. WHEN Employee 取消兑换 THEN THE System SHALL 恢复产品库存数量

---

### 需求 6: 收货地址管理

**用户故事**: 作为员工，我希望能够管理我的收货地址，以便快速完成兑换流程。

#### 验收标准

1. WHEN Employee 添加收货地址 THEN THE System SHALL 保存地址信息包含收货人、电话、详细地址
2. WHEN Employee 编辑收货地址 THEN THE System SHALL 更新地址信息
3. WHEN Employee 删除收货地址 THEN THE System SHALL 从系统中移除该地址
4. WHEN Employee 设置默认地址 THEN THE System SHALL 在兑换时自动选择该地址
5. WHEN Employee 查看收货地址列表 THEN THE System SHALL 显示所有已保存的地址

---

### 需求 7: 兑换历史记录

**用户故事**: 作为员工，我希望能够查看我的兑换历史记录，以便了解我的兑换情况。

#### 验收标准

1. WHEN Employee 查看兑换历史 THEN THE System SHALL 显示最近2年内的所有 Order
2. THE System SHALL 为每个 Order 显示兑换时间、产品名称、消耗积分、订单状态、收货地址
3. WHEN Employee 筛选兑换历史 THEN THE System SHALL 支持按时间范围和订单状态筛选
4. WHEN Employee 导出兑换历史 THEN THE System SHALL 生成包含所有历史记录的文件
5. WHEN Employee 查看订单详情 THEN THE System SHALL 显示完整的订单信息和物流信息

---

### 需求 8: 管理员认证与权限管理

**用户故事**: 作为管理员，我希望能够安全地登录系统并根据权限执行管理操作，以便管理平台运营。

#### 验收标准

1. THE System SHALL 支持独立的管理员账号系统
2. WHEN Administrator 提供有效的管理员账号和密码 THEN THE System SHALL 验证凭据并允许登录
3. THE System SHALL 支持三种管理员角色：Super_Administrator、Product_Administrator、Points_Administrator
4. THE System SHALL 通过RBAC（基于角色的访问控制）授权管理员权限
5. WHEN Super_Administrator 登录 THEN THE System SHALL 授予所有管理功能的访问权限
6. WHEN Product_Administrator 登录 THEN THE System SHALL 仅授予产品管理相关功能的访问权限
7. WHEN Points_Administrator 登录 THEN THE System SHALL 仅授予积分管理相关功能的访问权限
8. WHEN Administrator 尝试访问无权限的功能 THEN THE System SHALL 拒绝访问并显示权限不足提示

---

### 需求 9: 产品管理

**用户故事**: 作为产品管理员，我希望能够管理产品信息，以便为员工提供丰富的兑换选择。

#### 验收标准

1. WHEN Product_Administrator 添加产品 THEN THE System SHALL 创建新产品并包含产品名称、产品描述、产品图片、所需积分、库存数量、产品分类、产品标签字段
2. WHEN Product_Administrator 编辑产品 THEN THE System SHALL 更新产品信息
3. WHEN Product_Administrator 删除产品 THEN THE System SHALL 从系统中移除该产品
4. WHEN Product_Administrator 上架产品 THEN THE System SHALL 将产品状态设置为"上架"并对员工可见
5. WHEN Product_Administrator 下架产品 THEN THE System SHALL 将产品状态设置为"下架"并对员工不可见
6. WHEN Product_Administrator 上传产品图片 THEN THE System SHALL 支持多张图片上传
7. WHEN Product_Administrator 查看产品列表 THEN THE System SHALL 显示所有产品及其状态
8. WHEN Product_Administrator 搜索产品 THEN THE System SHALL 支持按产品名称、分类、状态搜索

---

### 需求 10: 员工积分管理

**用户故事**: 作为积分管理员，我希望能够管理员工积分，以便激励员工或进行纠错。

#### 验收标准

1. WHEN Points_Administrator 查看员工列表 THEN THE System SHALL 显示所有员工及其当前积分余额
2. WHEN Points_Administrator 搜索员工 THEN THE System SHALL 支持按姓名、工号、部门搜索
3. WHEN Points_Administrator 为员工发放积分 THEN THE System SHALL 增加该员工的积分余额并记录 Points_Transaction
4. WHEN Points_Administrator 为员工扣除积分 THEN THE System SHALL 减少该员工的积分余额并记录 Points_Transaction
5. WHEN Points_Administrator 发放或扣除积分 THEN THE System SHALL 要求填写原因/备注
6. WHEN Points_Administrator 批量发放积分 THEN THE System SHALL 支持选择多个员工并统一发放指定积分
7. WHEN Points_Administrator 查看员工积分历史 THEN THE System SHALL 显示该员工的所有 Points_Transaction

---

### 需求 11: 订单管理

**用户故事**: 作为管理员，我希望能够查看和管理员工的兑换订单，以便处理订单发货和售后。

#### 验收标准

1. WHEN Administrator 查看订单列表 THEN THE System SHALL 显示所有 Order 及其状态
2. WHEN Administrator 搜索订单 THEN THE System SHALL 支持按订单号、员工姓名、产品名称、订单状态搜索
3. WHEN Administrator 筛选订单 THEN THE System SHALL 支持按订单状态、时间范围筛选
4. WHEN Administrator 查看订单详情 THEN THE System SHALL 显示完整的订单信息包含员工信息、产品信息、收货地址、积分消耗
5. WHEN Administrator 修改订单状态 THEN THE System SHALL 更新订单状态并记录操作日志
6. WHEN Administrator 将订单状态改为"已发货" THEN THE System SHALL 允许填写物流信息
7. WHEN Administrator 导出订单数据 THEN THE System SHALL 生成包含所有订单信息的文件

---

### 需求 12: 数据统计报表

**用户故事**: 作为管理员，我希望能够查看统计数据和报表，以便了解平台运营情况。

#### 验收标准

1. WHEN Administrator 访问统计页面 THEN THE System SHALL 显示总兑换量、总积分发放量、总员工数、总产品数
2. WHEN Administrator 查看产品统计 THEN THE System SHALL 显示各产品的兑换次数排名
3. WHEN Administrator 查看积分统计 THEN THE System SHALL 显示积分发放和消耗的趋势图
4. WHEN Administrator 查看员工统计 THEN THE System SHALL 显示活跃员工排名
5. WHEN Administrator 选择时间范围 THEN THE System SHALL 显示该时间范围内的统计数据
6. WHEN Administrator 导出报表 THEN THE System SHALL 生成包含统计数据的文件

---

### 需求 13: 操作日志审计

**用户故事**: 作为超级管理员，我希望能够查看所有管理员的操作日志，以便进行审计和问题追溯。

#### 验收标准

1. WHEN Administrator 执行任何管理操作 THEN THE System SHALL 记录操作日志包含操作人、操作时间、操作类型、操作对象、操作前后的值
2. WHEN Super_Administrator 查看操作日志 THEN THE System SHALL 显示所有管理员的操作记录
3. WHEN Super_Administrator 搜索操作日志 THEN THE System SHALL 支持按操作人、操作类型、操作时间搜索
4. WHEN Super_Administrator 筛选操作日志 THEN THE System SHALL 支持按时间范围、操作类型筛选
5. WHEN Super_Administrator 导出操作日志 THEN THE System SHALL 生成包含所有日志记录的文件

---

### 需求 14: 通知系统

**用户故事**: 作为员工，我希望能够收到重要的通知消息，以便及时了解积分变动和订单状态。

#### 验收标准

1. WHEN Employee 的积分余额发生变化 THEN THE System SHALL 发送站内信通知
2. WHEN Employee 的订单状态变更 THEN THE System SHALL 发送站内信通知
3. WHEN 新产品上架 THEN THE System SHALL 向所有 Employee 发送站内信通知
4. WHEN Employee 登录系统 THEN THE System SHALL 显示未读通知数量
5. WHEN Employee 查看通知列表 THEN THE System SHALL 显示所有通知消息及阅读状态
6. WHEN Employee 点击通知 THEN THE System SHALL 标记为已读并跳转到相关页面

---

### 需求 15: 系统安全性

**用户故事**: 作为系统管理员，我希望系统具备基本的安全防护能力，以便保护用户数据和系统稳定性。

#### 验收标准

1. THE System SHALL 使用HTTPS协议进行所有数据传输
2. THE System SHALL 对所有用户密码进行加密存储
3. THE System SHALL 对员工个人信息进行脱敏显示
4. WHEN Employee 快速多次点击兑换按钮 THEN THE System SHALL 防止重复提交
5. WHEN Employee 进行兑换操作 THEN THE System SHALL 要求输入验证码
6. WHEN Employee 或 Administrator 登录成功 THEN THE System SHALL 创建会话并设置2小时有效期
7. WHEN 用户在30分钟内无任何操作 THEN THE System SHALL 自动退出登录
8. THE System SHALL 支持用户在多个设备同时登录

---

### 需求 16: 系统性能

**用户故事**: 作为用户，我希望系统响应迅速，以便获得良好的使用体验。

#### 验收标准

1. WHEN 系统并发用户数不超过1000人 THEN THE System SHALL 保持正常运行
2. WHEN 系统产品数量不超过1000个 THEN THE System SHALL 保持正常运行
3. WHEN Employee 访问任何页面 THEN THE System SHALL 在3秒内完成页面加载
4. WHEN Employee 执行搜索操作 THEN THE System SHALL 在3秒内返回搜索结果
5. WHEN Employee 提交兑换请求 THEN THE System SHALL 在3秒内完成处理并返回结果

---

### 需求 17: 用户界面

**用户故事**: 作为用户，我希望系统界面友好且兼容多种设备，以便随时随地使用系统。

#### 验收标准

1. THE System SHALL 提供Web浏览器访问方式
2. THE System SHALL 支持响应式设计以适配移动设备
3. THE System SHALL 兼容Chrome、Safari、Firefox、Edge浏览器的最新版本
4. THE System SHALL 采用类似亚马逊官网的界面风格
5. WHEN Employee 使用移动设备访问 THEN THE System SHALL 自动调整布局以适配屏幕尺寸

---

## 非功能性需求总结

### 性能需求
- 支持1000并发用户
- 支持1000个产品
- 页面响应时间 < 3秒

### 安全需求
- HTTPS加密传输
- 密码加密存储
- 数据脱敏显示
- 防重复提交
- 验证码保护
- 会话管理（2小时有效期，30分钟无操作自动退出）

### 兼容性需求
- Web浏览器访问
- 响应式设计
- 支持Chrome、Safari、Firefox、Edge最新版本

### 可用性需求
- 类似亚马逊的界面风格
- 移动端友好

---

## 技术约束

- **前端**: React
- **后端**: .NET
- **数据库**: MySQL
- **部署**: 标准云部署环境
- **团队**: 前端2人、后端2人、UI设计2人、测试1人、产品经理1人
- **交付时间**: 3个月一次性交付

---

## 项目范围

### MVP包含功能
**员工端**:
- 员工登录
- 浏览产品列表
- 查看产品详情
- 产品搜索功能
- 产品分类筛选
- 兑换单个产品
- 购物车功能
- 查看积分余额
- 查看兑换历史
- 收货地址管理
- 订单状态跟踪

**管理员端**:
- 管理员登录
- 产品管理（增删改查）
- 产品上下架
- 员工积分发放
- 员工积分扣除
- 查看员工列表
- 查看兑换订单列表
- 修改订单状态
- 数据统计报表
- 操作日志审计

### 后续版本功能
（暂无）

---

**文档版本**: 1.0  
**最后更新**: 2026-01-16  
**文档状态**: 待审查

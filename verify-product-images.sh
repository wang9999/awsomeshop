#!/bin/bash

echo "=========================================="
echo "产品图片配置验证"
echo "=========================================="
echo ""

echo "📊 数据库统计："
echo "----------------------------------------"
docker exec -i awsomeshop-mysql mysql -uroot -pyour_password awsomeshop_dev -e "
SELECT 
    '总产品数' as 项目, 
    COUNT(*) as 数量 
FROM Products
UNION ALL
SELECT 
    '总图片数' as 项目, 
    COUNT(*) as 数量 
FROM ProductImages
UNION ALL
SELECT 
    '有图片的产品数' as 项目, 
    COUNT(DISTINCT ProductId) as 数量 
FROM ProductImages;
" 2>/dev/null

echo ""
echo "📋 每个产品的图片数量："
echo "----------------------------------------"
docker exec -i awsomeshop-mysql mysql -uroot -pyour_password awsomeshop_dev -e "
SELECT 
    p.Name as 产品名称,
    COUNT(pi.Id) as 图片数量,
    CASE 
        WHEN COUNT(pi.Id) > 0 THEN '✅'
        ELSE '❌'
    END as 状态
FROM Products p
LEFT JOIN ProductImages pi ON p.Id = pi.ProductId
GROUP BY p.Id, p.Name
ORDER BY p.Name;
" 2>/dev/null

echo ""
echo "🌐 服务状态："
echo "----------------------------------------"
echo "前端: http://localhost:5173"
echo "后端: http://localhost:5144"
echo "MySQL: localhost:3306"
echo "Redis: localhost:6379"
echo ""
echo "✅ 验证完成！"
echo ""
echo "💡 下一步："
echo "1. 访问 http://localhost:5173/login"
echo "2. 使用账号登录："
echo "   - 员工: employee1@awsome.com / Employee@123"
echo "   - 管理员: superadmin / Admin@123"
echo "3. 查看产品列表，验证图片显示"
echo ""

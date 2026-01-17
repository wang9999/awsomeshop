-- 为剩余5个产品添加图片
-- 使用产品ID直接插入

-- 星巴克咖啡卡 500元
INSERT INTO ProductImages (Id, ProductId, ImageUrl, DisplayOrder, CreatedAt) VALUES
(UUID(), '41a30a69-bf64-455f-8f0d-1cf02fc6d99d', 'https://images.unsplash.com/photo-1559056199-641a0ac8b55e?w=800', 1, NOW()),
(UUID(), '41a30a69-bf64-455f-8f0d-1cf02fc6d99d', 'https://images.unsplash.com/photo-1509042239860-f550ce710b93?w=800', 2, NOW());

-- 飞利浦空气炸锅
INSERT INTO ProductImages (Id, ProductId, ImageUrl, DisplayOrder, CreatedAt) VALUES
(UUID(), '5f4a3e9c-0572-4024-b56c-2bd2228263fd', 'https://images.unsplash.com/photo-1585515320310-259814833e62?w=800', 1, NOW()),
(UUID(), '5f4a3e9c-0572-4024-b56c-2bd2228263fd', 'https://images.unsplash.com/photo-1556909114-f6e7ad7d3136?w=800', 2, NOW());

-- 膳魔师保温杯 500ml
INSERT INTO ProductImages (Id, ProductId, ImageUrl, DisplayOrder, CreatedAt) VALUES
(UUID(), '62cbb2a8-a2a2-4584-b90e-64f422c4c759', 'https://images.unsplash.com/photo-1602143407151-7111542de6e8?w=800', 1, NOW()),
(UUID(), '62cbb2a8-a2a2-4584-b90e-64f422c4c759', 'https://images.unsplash.com/photo-1534056523197-8e2e5d3e0c90?w=800', 2, NOW());

-- 京东E卡 1000元
INSERT INTO ProductImages (Id, ProductId, ImageUrl, DisplayOrder, CreatedAt) VALUES
(UUID(), '7ee21a8c-c5a1-4745-8d7b-b9c6f2eb5c7a', 'https://images.unsplash.com/photo-1563013544-824ae1b704d3?w=800', 1, NOW()),
(UUID(), '7ee21a8c-c5a1-4745-8d7b-b9c6f2eb5c7a', 'https://images.unsplash.com/photo-1556742049-0cfed4f6a45d?w=800', 2, NOW());

-- 罗技MX Master 3S 无线鼠标
INSERT INTO ProductImages (Id, ProductId, ImageUrl, DisplayOrder, CreatedAt) VALUES
(UUID(), 'e5937c74-7b55-4111-b20c-4da4f223bfec', 'https://images.unsplash.com/photo-1527864550417-7fd91fc51a46?w=800', 1, NOW()),
(UUID(), 'e5937c74-7b55-4111-b20c-4da4f223bfec', 'https://images.unsplash.com/photo-1615663245857-ac93bb7c39e7?w=800', 2, NOW());

-- 验证结果
SELECT '✅ 剩余产品图片添加完成！' AS Status;
SELECT p.Name, COUNT(pi.Id) as ImageCount 
FROM Products p 
LEFT JOIN ProductImages pi ON p.Id = pi.ProductId 
GROUP BY p.Id, p.Name 
ORDER BY p.Name;

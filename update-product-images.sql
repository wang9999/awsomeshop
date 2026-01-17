-- 更新产品图片URL
-- 使用Unsplash的产品图片

-- 清空现有图片
DELETE FROM ProductImages;

-- 获取产品ID并插入新图片
-- MacBook Pro
INSERT INTO ProductImages (Id, ProductId, ImageUrl, DisplayOrder, CreatedAt)
SELECT 
    UUID(),
    Id,
    'https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=800',
    1,
    NOW()
FROM Products WHERE Name LIKE '%MacBook%';

INSERT INTO ProductImages (Id, ProductId, ImageUrl, DisplayOrder, CreatedAt)
SELECT 
    UUID(),
    Id,
    'https://images.unsplash.com/photo-1611186871348-b1ce696e52c9?w=800',
    2,
    NOW()
FROM Products WHERE Name LIKE '%MacBook%';

-- Sony耳机
INSERT INTO ProductImages (Id, ProductId, ImageUrl, DisplayOrder, CreatedAt)
SELECT 
    UUID(),
    Id,
    'https://images.unsplash.com/photo-1546435770-a3e426bf472b?w=800',
    1,
    NOW()
FROM Products WHERE Name LIKE '%Sony%';

INSERT INTO ProductImages (Id, ProductId, ImageUrl, DisplayOrder, CreatedAt)
SELECT 
    UUID(),
    Id,
    'https://images.unsplash.com/photo-1484704849700-f032a568e944?w=800',
    2,
    NOW()
FROM Products WHERE Name LIKE '%Sony%';

-- Dyson吸尘器
INSERT INTO ProductImages (Id, ProductId, ImageUrl, DisplayOrder, CreatedAt)
SELECT 
    UUID(),
    Id,
    'https://images.unsplash.com/photo-1558317374-067fb5f30001?w=800',
    1,
    NOW()
FROM Products WHERE Name LIKE '%Dyson%';

-- 星巴克卡
INSERT INTO ProductImages (Id, ProductId, ImageUrl, DisplayOrder, CreatedAt)
SELECT 
    UUID(),
    Id,
    'https://images.unsplash.com/photo-1559056199-641a0ac8b55e?w=800',
    1,
    NOW()
FROM Products WHERE Name LIKE '%星巴克%';

-- Kindle
INSERT INTO ProductImages (Id, ProductId, ImageUrl, DisplayOrder, CreatedAt)
SELECT 
    UUID(),
    Id,
    'https://images.unsplash.com/photo-1592496431122-2349e0fbc666?w=800',
    1,
    NOW()
FROM Products WHERE Name LIKE '%Kindle%';

-- Moleskine笔记本
INSERT INTO ProductImages (Id, ProductId, ImageUrl, DisplayOrder, CreatedAt)
SELECT 
    UUID(),
    Id,
    'https://images.unsplash.com/photo-1531346878377-a5be20888e57?w=800',
    1,
    NOW()
FROM Products WHERE Name LIKE '%Moleskine%';

-- 空气炸锅
INSERT INTO ProductImages (Id, ProductId, ImageUrl, DisplayOrder, CreatedAt)
SELECT 
    UUID(),
    Id,
    'https://images.unsplash.com/photo-1585515320310-259814833e62?w=800',
    1,
    NOW()
FROM Products WHERE Name LIKE '%空气炸锅%';

-- 京东卡
INSERT INTO ProductImages (Id, ProductId, ImageUrl, DisplayOrder, CreatedAt)
SELECT 
    UUID(),
    Id,
    'https://images.unsplash.com/photo-1563013544-824ae1b704d3?w=800',
    1,
    NOW()
FROM Products WHERE Name LIKE '%京东%';

-- 罗技鼠标
INSERT INTO ProductImages (Id, ProductId, ImageUrl, DisplayOrder, CreatedAt)
SELECT 
    UUID(),
    Id,
    'https://images.unsplash.com/photo-1527864550417-7fd91fc51a46?w=800',
    1,
    NOW()
FROM Products WHERE Name LIKE '%罗技%';

-- 保温杯
INSERT INTO ProductImages (Id, ProductId, ImageUrl, DisplayOrder, CreatedAt)
SELECT 
    UUID(),
    Id,
    'https://images.unsplash.com/photo-1602143407151-7111542de6e8?w=800',
    1,
    NOW()
FROM Products WHERE Name LIKE '%保温杯%';

SELECT '✅ 产品图片更新完成！' AS Status;
SELECT COUNT(*) AS TotalImages FROM ProductImages;

#!/bin/bash

echo "📥 开始下载产品图片..."
echo "================================"
echo ""

# 创建产品图片目录
mkdir -p frontend/public/product-images

# 产品图片URL数组
declare -A products

# MacBook Pro
products["macbook-pro-1"]="https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=800&h=600&fit=crop"
products["macbook-pro-2"]="https://images.unsplash.com/photo-1611186871348-b1ce696e52c9?w=800&h=600&fit=crop"

# Sony耳机
products["sony-headphones-1"]="https://images.unsplash.com/photo-1546435770-a3e426bf472b?w=800&h=600&fit=crop"
products["sony-headphones-2"]="https://images.unsplash.com/photo-1484704849700-f032a568e944?w=800&h=600&fit=crop"

# Dyson吸尘器
products["dyson-vacuum-1"]="https://images.unsplash.com/photo-1558317374-067fb5f30001?w=800&h=600&fit=crop"
products["dyson-vacuum-2"]="https://images.unsplash.com/photo-1585659722983-3a675dabf23d?w=800&h=600&fit=crop"

# 星巴克卡
products["starbucks-card-1"]="https://images.unsplash.com/photo-1559056199-641a0ac8b55e?w=800&h=600&fit=crop"
products["starbucks-card-2"]="https://images.unsplash.com/photo-1509042239860-f550ce710b93?w=800&h=600&fit=crop"

# Kindle
products["kindle-1"]="https://images.unsplash.com/photo-1592496431122-2349e0fbc666?w=800&h=600&fit=crop"
products["kindle-2"]="https://images.unsplash.com/photo-1544716278-ca5e3f4abd8c?w=800&h=600&fit=crop"

# Moleskine笔记本
products["moleskine-1"]="https://images.unsplash.com/photo-1531346878377-a5be20888e57?w=800&h=600&fit=crop"
products["moleskine-2"]="https://images.unsplash.com/photo-1517842645767-c639042777db?w=800&h=600&fit=crop"

# 空气炸锅
products["air-fryer-1"]="https://images.unsplash.com/photo-1585515320310-259814833e62?w=800&h=600&fit=crop"
products["air-fryer-2"]="https://images.unsplash.com/photo-1556909114-f6e7ad7d3136?w=800&h=600&fit=crop"

# 京东卡
products["jd-card-1"]="https://images.unsplash.com/photo-1563013544-824ae1b704d3?w=800&h=600&fit=crop"
products["jd-card-2"]="https://images.unsplash.com/photo-1556742049-0cfed4f6a45d?w=800&h=600&fit=crop"

# 罗技鼠标
products["logitech-mouse-1"]="https://images.unsplash.com/photo-1527864550417-7fd91fc51a46?w=800&h=600&fit=crop"
products["logitech-mouse-2"]="https://images.unsplash.com/photo-1615663245857-ac93bb7c39e7?w=800&h=600&fit=crop"

# 保温杯
products["thermos-1"]="https://images.unsplash.com/photo-1602143407151-7111542de6e8?w=800&h=600&fit=crop"
products["thermos-2"]="https://images.unsplash.com/photo-1534056523197-8e2e5d3e0c90?w=800&h=600&fit=crop"

# 下载图片
count=0
total=${#products[@]}

for filename in "${!products[@]}"; do
    url="${products[$filename]}"
    output_file="frontend/public/product-images/${filename}.jpg"
    
    ((count++))
    echo "[$count/$total] 下载: $filename"
    
    curl -s -L "$url" -o "$output_file"
    
    if [ $? -eq 0 ]; then
        echo "  ✅ 成功"
    else
        echo "  ❌ 失败"
    fi
    
    # 避免请求过快
    sleep 0.5
done

echo ""
echo "================================"
echo "✅ 下载完成！"
echo ""
echo "图片保存位置: frontend/public/product-images/"
echo "共下载 $total 张图片"
echo ""
echo "图片文件列表:"
ls -lh frontend/public/product-images/

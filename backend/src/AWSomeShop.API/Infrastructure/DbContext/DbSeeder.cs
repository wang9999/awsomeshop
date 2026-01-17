using AWSomeShop.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AWSomeShop.API.Infrastructure.DbContext;

public static class DbSeeder
{
    public static async Task SeedAsync(AWSomeShopDbContext context)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Seed Administrators
        if (!await context.Administrators.AnyAsync())
        {
            var administrators = new List<Administrator>
            {
                new Administrator
                {
                    Id = Guid.NewGuid().ToString(),
                    Username = "superadmin",
                    // Password: Admin@123 (should be hashed in production)
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Name = "超级管理员",
                    Role = AdminRole.SuperAdmin,
                    IsActive = true
                },
                new Administrator
                {
                    Id = Guid.NewGuid().ToString(),
                    Username = "productadmin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Name = "产品管理员",
                    Role = AdminRole.ProductAdmin,
                    IsActive = true
                },
                new Administrator
                {
                    Id = Guid.NewGuid().ToString(),
                    Username = "pointsadmin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Name = "积分管理员",
                    Role = AdminRole.PointsAdmin,
                    IsActive = true
                }
            };

            await context.Administrators.AddRangeAsync(administrators);
            await context.SaveChangesAsync();
        }

        // Seed Test Employees
        if (!await context.Employees.AnyAsync())
        {
            var employees = new List<Employee>
            {
                new Employee
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "employee1@awsome.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee@123"),
                    Name = "张三",
                    EmployeeNumber = "EMP001",
                    Department = "技术部",
                    Birthday = new DateTime(1990, 5, 15),
                    PointsBalance = 2000,
                    IsActive = true
                },
                new Employee
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "employee2@awsome.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee@123"),
                    Name = "李四",
                    EmployeeNumber = "EMP002",
                    Department = "市场部",
                    Birthday = new DateTime(1992, 8, 20),
                    PointsBalance = 1500,
                    IsActive = true
                },
                new Employee
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "employee3@awsome.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee@123"),
                    Name = "王五",
                    EmployeeNumber = "EMP003",
                    Department = "人力资源部",
                    Birthday = new DateTime(1988, 3, 10),
                    PointsBalance = 3000,
                    IsActive = true
                }
            };

            await context.Employees.AddRangeAsync(employees);
            await context.SaveChangesAsync();
        }

        // Seed Test Products
        if (!await context.Products.AnyAsync())
        {
            var products = new List<Product>
            {
                new Product
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Apple MacBook Pro 14英寸",
                    Description = "M3 Pro芯片，18GB内存，512GB存储，14.2英寸Liquid Retina XDR显示屏",
                    Points = 15000,
                    Stock = 10,
                    Category = ProductCategory.电子产品,
                    Status = ProductStatus.上架
                },
                new Product
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Sony WH-1000XM5 无线降噪耳机",
                    Description = "业界领先的降噪技术，30小时续航，舒适佩戴",
                    Points = 2500,
                    Stock = 25,
                    Category = ProductCategory.电子产品,
                    Status = ProductStatus.上架
                },
                new Product
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Dyson V15 Detect 吸尘器",
                    Description = "激光探测技术，强劲吸力，智能显示屏",
                    Points = 4500,
                    Stock = 15,
                    Category = ProductCategory.生活用品,
                    Status = ProductStatus.上架
                },
                new Product
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "星巴克咖啡卡 500元",
                    Description = "全国星巴克门店通用，有效期一年",
                    Points = 500,
                    Stock = 100,
                    Category = ProductCategory.礼品卡,
                    Status = ProductStatus.上架
                },
                new Product
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Kindle Paperwhite 电子书阅读器",
                    Description = "6.8英寸显示屏，防水设计，长续航",
                    Points = 1200,
                    Stock = 30,
                    Category = ProductCategory.图书文具,
                    Status = ProductStatus.上架
                },
                new Product
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Moleskine 经典笔记本套装",
                    Description = "意大利品牌，硬壳笔记本，含3本",
                    Points = 300,
                    Stock = 50,
                    Category = ProductCategory.图书文具,
                    Status = ProductStatus.上架
                },
                new Product
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "飞利浦空气炸锅",
                    Description = "4.1L大容量，智能控温，健康烹饪",
                    Points = 800,
                    Stock = 20,
                    Category = ProductCategory.生活用品,
                    Status = ProductStatus.上架
                },
                new Product
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "京东E卡 1000元",
                    Description = "京东商城通用，可购买自营商品",
                    Points = 1000,
                    Stock = 200,
                    Category = ProductCategory.礼品卡,
                    Status = ProductStatus.上架
                },
                new Product
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "罗技MX Master 3S 无线鼠标",
                    Description = "人体工学设计，8K DPI传感器，静音按键",
                    Points = 700,
                    Stock = 40,
                    Category = ProductCategory.电子产品,
                    Status = ProductStatus.上架
                },
                new Product
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "膳魔师保温杯 500ml",
                    Description = "真空保温，24小时保温保冷",
                    Points = 250,
                    Stock = 60,
                    Category = ProductCategory.生活用品,
                    Status = ProductStatus.上架
                }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();

            // Add product images for the first few products
            var firstProduct = products[0];
            var productImages = new List<ProductImage>
            {
                new ProductImage
                {
                    ProductId = firstProduct.Id,
                    ImageUrl = "https://via.placeholder.com/800x600/4A90E2/FFFFFF?text=MacBook+Pro+1",
                    DisplayOrder = 1
                },
                new ProductImage
                {
                    ProductId = firstProduct.Id,
                    ImageUrl = "https://via.placeholder.com/800x600/4A90E2/FFFFFF?text=MacBook+Pro+2",
                    DisplayOrder = 2
                }
            };

            await context.ProductImages.AddRangeAsync(productImages);

            // Add product tags
            var productTags = new List<ProductTag>
            {
                new ProductTag { ProductId = firstProduct.Id, TagName = "热门" },
                new ProductTag { ProductId = firstProduct.Id, TagName = "高端" },
                new ProductTag { ProductId = products[1].Id, TagName = "热门" },
                new ProductTag { ProductId = products[3].Id, TagName = "推荐" }
            };

            await context.ProductTags.AddRangeAsync(productTags);
            await context.SaveChangesAsync();
        }
    }
}

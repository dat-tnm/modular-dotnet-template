using CompanyName.ProjectName.Services.Excel.Contracts;
using CompanyName.ProjectName.Services.Excel.Contracts.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CompanyName.ProjectName.Services.Excel.Implements
{
    public abstract class ExampleDescriptorRegistry : IImportDescriptorRegistry
    {
        public ImportDescriptor? Resolve(string entityName)
        {
            if (entityName == null)
            {
                throw new ArgumentNullException(nameof(entityName));
            }

            if (entityName.Equals("User", StringComparison.OrdinalIgnoreCase))
            {
                var userDescriptor = new ImportDescriptor
                (
                    EntityName: "User",
                    Schema: "dbo",
                    TableName: "Users",
                    KeyColumns: new List<string> { "UserId" },
                    MappingColumns: new List<ColumnMapping>
                    {
                        new ColumnMapping("UserId", "UserId", System.Data.SqlDbType.Int),
                        new ColumnMapping("UserName","UserName", System.Data.SqlDbType.VarChar),
                        new ColumnMapping("Email", "Email", System.Data.SqlDbType.NVarChar),
                    },
                    Mode: UpsertMode.Upsert
                );

                return userDescriptor;
            }
            else if (entityName.Equals("Product", StringComparison.OrdinalIgnoreCase))
            {
                var productDescriptor = new ImportDescriptor(
                    EntityName: "Product",
                    Schema: "dbo",
                    TableName: "Products",
                    KeyColumns: new[] { "Sku" },
                    MappingColumns: new[]
                    {
                        new ColumnMapping("SKU", "Sku", SqlDbType.NVarChar, Length: 50, Required: true, Transform: v => (v as string)?.Trim()),
                        new ColumnMapping("Name", "Name", SqlDbType.NVarChar, Length: 100, Required: true, Transform: v => (v as string)?.Trim()),
                        new ColumnMapping("Price", "Price", SqlDbType.Decimal, Precision: 18, Scale: 2, Required: true, IsValid: v => v is decimal d && d >= 0m)
                    },
                    Mode: UpsertMode.Upsert
                );

                return productDescriptor;
            }
            else if (entityName.Equals("Order", StringComparison.OrdinalIgnoreCase))
            {
                var orderItemDescriptor = new ImportDescriptor(
                    EntityName: "OrderItem",
                    Schema: "dbo",
                    TableName: "OrderItems",
                    KeyColumns: new[] { "OrderId", "ProductSku" },
                    MappingColumns: new[]
                    {
                        new ColumnMapping("OrderId", "OrderId", SqlDbType.UniqueIdentifier, Required: true),
                        new ColumnMapping("ProductSku", "ProductSku", SqlDbType.NVarChar, Length: 50, Required: true),
                        new ColumnMapping("Quantity", "Quantity", SqlDbType.Int, Required: true, IsValid: v => v is int i && i > 0),
                        new ColumnMapping("UnitPrice", "UnitPrice", SqlDbType.Decimal, Precision: 18, Scale: 2, Required: true,
                            IsValid: v => v is decimal d && d >= 0m)
                    },
                    Mode: UpsertMode.Upsert
                );

                return orderItemDescriptor;
            }

            return null;
        }
    }
}

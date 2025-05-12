using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Tutorial9.Model.DTO;

namespace Tutorial9.Services;

public class WarehouseService : IWarehouseService
{
    private const string ConnectionString =
        "Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True;Trust Server Certificate=True";

    public async Task<int> AddProduct(ProductDTO dto)
    {
        using (SqlConnection conn = new SqlConnection(ConnectionString))
        {
            await conn.OpenAsync();

            if (!await CheckIfExists(conn, "Product", "IdProduct", dto.IdProduct))
                return -1; //"Product not found";
            if (!await CheckIfExists(conn, "Warehouse", "IdWarehouse", dto.IdWarehouse))
                return -2; //"Warehouse not found";

            int idOrder = await CheckOrderForProduct(conn, dto);
            if (idOrder == -1)
                return -3; //"Bad Request - no order";
            if (await CheckIfExists(conn, "Product_Warehouse", "IdOrder", idOrder))
                return -4; //"Order already fulfilled";

            await UpdateOrderFulfillmentDate(conn, idOrder);

            int priceTotal = (int)(await GetProductPrice(conn, dto.IdProduct) * dto.Amount);

            const string command =
                "insert into Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) output inserted.IdProductWarehouse values (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, getdate())";

            using (SqlCommand cmd = new SqlCommand(command, conn))
            {
                cmd.Parameters.AddWithValue("IdWarehouse", dto.IdWarehouse);
                cmd.Parameters.AddWithValue("IdProduct", dto.IdProduct);
                cmd.Parameters.AddWithValue("IdOrder", idOrder);
                cmd.Parameters.AddWithValue("Amount", dto.Amount);
                cmd.Parameters.AddWithValue("Price", priceTotal);

                var res = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(res);
            }
        }
    }

    private async Task<bool> CheckIfExists(SqlConnection conn, string table, string column, int id)
    {
        string command = $"select count(1) from {table} where {column} = @Id";

        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@Id", id);

            var res = await cmd.ExecuteScalarAsync();
            return Convert.ToBoolean(res);
        }
    }

    private async Task<int> CheckOrderForProduct(SqlConnection conn, ProductDTO product)
    {
        const string command =
            "select IdOrder from [Order] where IdProduct = @IdProduct and Amount = @Amount and CreatedAt < @CreatedAt";

        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("IdProduct", product.IdProduct);
            cmd.Parameters.AddWithValue("Amount", product.Amount);
            cmd.Parameters.AddWithValue("CreatedAt", product.CreatedAt);

            var res = await cmd.ExecuteScalarAsync();
            return res == null ? -1 : Convert.ToInt32(res);
        }
    }

    private async Task UpdateOrderFulfillmentDate(SqlConnection conn, int idOrder)
    {
        const string command = "update [Order] set FulfilledAt = getdate() where IdOrder = @IdOrder";

        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@IdOrder", idOrder);

            await cmd.ExecuteNonQueryAsync();
        }
    }

    private async Task<decimal> GetProductPrice(SqlConnection conn, int idProduct)
    {
        const string command = "select Price from Product where IdProduct = @IdProduct";

        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("IdProduct", idProduct);

            var res = await cmd.ExecuteScalarAsync();
            return Convert.ToDecimal(res);
        }
    }

    public async Task<int> AddProductProcedure(ProductDTO dto)
    {
        const string procName = "AddProductToWarehouse";

        try
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                await conn.OpenAsync();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("IdProduct", dto.IdProduct);
                cmd.Parameters.AddWithValue("IdWarehouse", dto.IdWarehouse);
                cmd.Parameters.AddWithValue("Amount", dto.Amount);
                cmd.Parameters.AddWithValue("CreatedAt", dto.CreatedAt);

                var res = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(res);
            }
        }
        catch (DbException)
        {
            return -1;
        }
    }
}
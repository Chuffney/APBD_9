using Microsoft.Data.SqlClient;
using Tutorial9.Model.DTO;

namespace Tutorial9.Services;

public class WarehouseService : IWarehouseService
{
    private const string ConnectionString = "Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True;Trust Server Certificate=True";
    
    public async Task<string> AddProduct(ProductDTO dto)
    {
        using (SqlConnection conn = new SqlConnection(ConnectionString))
        {
            await conn.OpenAsync();
            
            if (!await CheckIfExists(conn, "Product", "IdProduct", dto.IdProduct))
                return "Product not found";
            if (!await CheckIfExists(conn, "Warehouse", "IdWarehouse", dto.IdWarehouse))
                return "Warehouse not found";

            int idOrder = await CheckOrderForProduct(conn, dto); 
            if (idOrder == -1)
                return "Bad Request - no order";
            if (!await CheckIfExists(conn, "Product_Warehouse", "IdOrder", idOrder))
                return "Order already fulfilled";
            
            UpdateOrderFulfillmentDate(conn, idOrder);

        }

        return "OK";
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
        const string command = "select IdOrder from Order where IdProduct = @IdProduct and Amount = @Amount and CreatedAt < @CreatedAt";

        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("IdProduct", product.IdProduct);
            cmd.Parameters.AddWithValue("Amount", product.Amount);
            cmd.Parameters.AddWithValue("CreatedAt", product.CreatedAt);
            
            var res = await cmd.ExecuteScalarAsync();
            return res == null ? -1 : Convert.ToInt32(res);
        }
    }

    private async void UpdateOrderFulfillmentDate(SqlConnection conn, int idOrder)
    {
        const string command = "update Order set FulfilledAt = getdate() where IdOrder = @IdOrder";

        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@IdOrder", idOrder);

            cmd.ExecuteNonQuery();
        }
    }
}
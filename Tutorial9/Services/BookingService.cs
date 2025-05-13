using Microsoft.Data.SqlClient;
using Tutorial9.Model.DTO;

namespace Tutorial9.Services;

public class BookingService : IBookingService
{
    private const string ConnectionString =
        "Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True;Trust Server Certificate=True";

    public async Task<BookingDTO>? GetBooking(int id)
    {
        using (SqlConnection conn = new SqlConnection(ConnectionString))
        {
            await conn.OpenAsync();

            BookingDTO result = new BookingDTO();

            string dateCommand = "select date from Booking where booking_id = @bookingId";
            string guestCommand =
                "select first_name, last_name, date_of_birth from Guest join Booking B on Guest.guest_id = B.guest_id where booking_id = @bookingId";
            string employeeCommand =
                "select first_name, last_name, employee_number from Employee join Booking B on Employee.employee_id = B.employee_id where booking_id = @bookingId";
            string attractionCommand =
                "select name, price, amount from Attraction join Booking_Attraction BA on Attraction.attraction_id = BA.attraction_id where booking_id = @bookingId";

            using (SqlCommand cmd = new SqlCommand(dateCommand, conn))
            {
                cmd.Parameters.AddWithValue("@bookingId", id);

                var res = await cmd.ExecuteScalarAsync();
                if (res == null)
                    return null;
                result.date = Convert.ToDateTime(res);

                cmd.CommandText = guestCommand;
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    await reader.ReadAsync();

                    result.guest = new GuestDTO()
                    {
                        FirstName = reader["first_name"].ToString(),
                        LastName = reader["last_name"].ToString(),
                        DateOfBirth = Convert.ToDateTime(reader["date_of_birth"])
                    };
                }
                
                cmd.CommandText = employeeCommand;
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    await reader.ReadAsync();

                    result.employee = new EmployeeDTO()
                    {
                        FirstName = reader["first_name"].ToString(),
                        LastName = reader["last_name"].ToString(),
                        EmployeeNumber = reader["employee_number"].ToString(),
                    };
                }

                cmd.CommandText = attractionCommand;
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    List<AttractionDTO> attractions = new List<AttractionDTO>();
                    while (await reader.ReadAsync())
                    {
                        attractions.Add(new AttractionDTO()
                        {
                            amount = Convert.ToInt32(reader["amount"]),
                            name = reader["name"].ToString(),
                            price = Convert.ToInt32(reader["price"])
                        });
                    }
                    result.attractions = attractions;
                }

            }
            return result;
        }
    }

    public async Task<int> AddBooking(PostBookingDTO booking)
    {
        using (SqlConnection conn = new SqlConnection(ConnectionString))
        {
            await conn.OpenAsync();
            
            if (await CheckIfExists(conn, "Booking", "booking_id", booking.BookingId))
                return -1;
            if (!await CheckIfExists(conn, "Guest", "guest_id", booking.GuestId))
                return -2;
            if (!await CheckIfExists(conn, "Employee", "employee_number", booking.EmployeeNumber))
                return -3;

            foreach (var atrakcja in booking.Attractions)
            {
                if (!await CheckIfExists(conn, "Attraction", "name", atrakcja.Name))
                    return -4;
            }

            const string insertBookingCommand =
                "insert into Booking (booking_id, guest_id, employee_id, date) values (@bookingId, @guestId, (select top 1 employee_id from Employee where employee_number = @employeeId), getdate())";
            const string insertAttractionCommand =
                "insert into Booking_Attraction (booking_id, attraction_id, amount) values (@bookingId, (select top 1 attraction_id from Attraction where name = @name), @amount)";

            try
            {
                using (SqlCommand cmd = new SqlCommand(insertBookingCommand, conn))
                {
                    cmd.Parameters.AddWithValue("@bookingId", booking.BookingId);
                    cmd.Parameters.AddWithValue("@guestId", booking.GuestId);
                    cmd.Parameters.AddWithValue("@employeeId", booking.EmployeeNumber);

                    await cmd.ExecuteNonQueryAsync();

                    cmd.CommandText = insertAttractionCommand;

                    foreach (var attraction in booking.Attractions)
                    {
                        cmd.Parameters.AddWithValue("@name", attraction.Name);
                        cmd.Parameters.AddWithValue("@amount", attraction.Amount);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception)
            {
                return -5;
            }
        }

        return 0;
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
    
    private async Task<bool> CheckIfExists(SqlConnection conn, string table, string column, string id)
    {
        string command = $"select count(1) from {table} where {column} = @Id";

        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@Id", id);

            var res = await cmd.ExecuteScalarAsync();
            return Convert.ToBoolean(res);
        }
    }
}
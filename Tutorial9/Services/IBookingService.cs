using Tutorial9.Model.DTO;

namespace Tutorial9.Services;

public interface IBookingService
{
    Task<BookingDTO>? GetBooking(int id);
    Task<int> AddBooking(PostBookingDTO booking);
}
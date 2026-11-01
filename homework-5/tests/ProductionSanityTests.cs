using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace Campus.SanityTests;

public sealed class ProductionSanityTests
{
    [Fact]
    public async Task BookingSmokeTest_CreatesAndDeletesTestBooking()
    {
        // Тест пропускается, если явно не включён оператором релиза.
        if (!string.Equals(Environment.GetEnvironmentVariable("RUN_PRODUCTION_SANITY"),
                "true", StringComparison.OrdinalIgnoreCase))
            throw Xunit.Sdk.SkipException.ForSkip(
                "Set RUN_PRODUCTION_SANITY=true to run the release sanity test.");

        var baseUrl = Environment.GetEnvironmentVariable("SANITY_BASE_URL")
            ?? throw new InvalidOperationException("SANITY_BASE_URL is required.");
        var token = Environment.GetEnvironmentVariable("SANITY_API_TOKEN")
            ?? throw new InvalidOperationException("SANITY_API_TOKEN is required.");
        var auditoryId = Environment.GetEnvironmentVariable("SANITY_AUDITORY_ID")
            ?? throw new InvalidOperationException("SANITY_AUDITORY_ID is required.");

        using var client = new HttpClient { BaseAddress = new Uri(baseUrl) };
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        Guid? bookingId = null;
        try
        {
            var response = await client.PostAsJsonAsync("/api/v1/bookings", new
            {
                auditoryId,
                testMarker = $"release-sanity-{Guid.NewGuid():N}",
                seats = 1
            });
            Assert.True(response.IsSuccessStatusCode,
                $"Booking creation failed: {(int)response.StatusCode}");

            var booking = await response.Content.ReadFromJsonAsync<BookingResponse>();
            Assert.NotNull(booking);
            Assert.NotEqual(Guid.Empty, booking.Id);
            bookingId = booking.Id;

            var get = await client.GetAsync($"/api/v1/bookings/{bookingId}");
            Assert.Equal(HttpStatusCode.OK, get.StatusCode);
        }
        finally
        {
            if (bookingId.HasValue)
            {
                var cleanup = await client.DeleteAsync($"/api/v1/bookings/{bookingId}");
                if (cleanup.StatusCode is not HttpStatusCode.NoContent
                    and not HttpStatusCode.NotFound)
                    throw new InvalidOperationException(
                        $"Cleanup failed for {bookingId}; manual cleanup is required.");
            }
        }
    }

    private sealed record BookingResponse(Guid Id);
}

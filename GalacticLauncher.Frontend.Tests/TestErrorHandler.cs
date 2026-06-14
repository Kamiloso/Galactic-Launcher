using GalacticLauncher.Frontend.Services;

namespace GalacticLauncher.Frontend.Tests.Services
{
    public class ErrorHandlerTests
    {
        private ErrorHandler CreateHandler()
        {
            return new ErrorHandler();
        }

        [Fact]
        public void HandleApiError_ShouldRaiseOnWarning_WhenCodeIsZeroAndShowNoInternetIsTrue()
        {
            var handler = CreateHandler();
            string? receivedTitle = null;
            string? receivedMessage = null;
            int callCount = 0;

            handler.OnWarning += (title, message) =>
            {
                receivedTitle = title;
                receivedMessage = message;
                callCount++;
            };

            handler.HandleApiError(0, showNoInternet: true);

            Assert.Equal(1, callCount);
            Assert.Equal("Offline Mode", receivedTitle);
            Assert.Equal("Failed to reach the server.", receivedMessage);
        }

        [Fact]
        public void HandleApiError_ShouldNotRaiseOnWarning_WhenCodeIsZeroAndShowNoInternetIsFalse()
        {
            var handler = CreateHandler();
            int callCount = 0;

            handler.OnWarning += (title, message) => callCount++;

            handler.HandleApiError(0, showNoInternet: false);

            Assert.Equal(0, callCount);
        }

        [Theory]
        [InlineData(400)]
        [InlineData(401)]
        [InlineData(404)]
        [InlineData(499)]
        public void HandleApiError_ShouldRaiseOnError_WhenCodeIs4xx(int httpCode)
        {
            var handler = CreateHandler();
            string? receivedTitle = null;
            string? receivedMessage = null;
            int callCount = 0;

            handler.OnError += (title, message) =>
            {
                receivedTitle = title;
                receivedMessage = message;
                callCount++;
            };

            handler.HandleApiError(httpCode, showNoInternet: false);

            Assert.Equal(1, callCount);
            Assert.Equal("Client Error", receivedTitle);
            Assert.Equal($"An error occurred on the client side: HTTP {httpCode}", receivedMessage);
        }


        [Theory]
        [InlineData(500)]
        [InlineData(502)]
        [InlineData(503)]
        [InlineData(599)]
        public void HandleApiError_ShouldRaiseOnError_WhenCodeIs5xx(int httpCode)
        {
            var handler = CreateHandler();
            string? receivedTitle = null;
            string? receivedMessage = null;
            int callCount = 0;

            handler.OnError += (title, message) =>
            {
                receivedTitle = title;
                receivedMessage = message;
                callCount++;
            };

            handler.HandleApiError(httpCode, showNoInternet: false);

            Assert.Equal(1, callCount);
            Assert.Equal("Server Error", receivedTitle);
            Assert.Equal($"An error occurred on the server side: HTTP {httpCode}", receivedMessage);
        }


        [Theory]
        [InlineData(200)]
        [InlineData(204)]
        [InlineData(302)]
        public void HandleApiError_ShouldNotRaiseAnyEvent_WhenCodeIsSuccessfulOrRedirect(int httpCode)
        {
            var handler = CreateHandler();
            int anyEventCount = 0;

            handler.OnInfo += (t, m) => anyEventCount++;
            handler.OnWarning += (t, m) => anyEventCount++;
            handler.OnError += (t, m) => anyEventCount++;
            handler.OnSuccess += (t, m) => anyEventCount++;

            handler.HandleApiError(httpCode, showNoInternet: true);

            Assert.Equal(0, anyEventCount);
        }
    }
}
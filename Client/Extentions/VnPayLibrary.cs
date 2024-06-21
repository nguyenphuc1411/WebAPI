using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Client.Extensions
{
    public class VnPayLibrary
    {
        private readonly SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
        private readonly SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayCompare());

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData[key] = value; // Updated to avoid adding duplicate keys
            }
        }

        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _responseData[key] = value; // Updated to avoid adding duplicate keys
            }
        }

        public string GetResponseData(string key)
        {
            return _responseData.TryGetValue(key, out var retValue) ? retValue : string.Empty;
        }

        #region Request
        public string CreateRequestUrl(string baseUrl, string vnpHashSecret)
        {
            var queryStringBuilder = new StringBuilder();
            foreach (var (key, value) in _requestData.Where(kv => !string.IsNullOrEmpty(kv.Value)))
            {
                queryStringBuilder.Append(WebUtility.UrlEncode(key) + "=" + WebUtility.UrlEncode(value) + "&");
            }

            var queryString = queryStringBuilder.ToString().TrimEnd('&');

            var signData = queryString;
            var vnpSecureHash = Utils.HmacSHA512(vnpHashSecret, signData);

            var fullUrl = $"{baseUrl}?{queryString}&vnp_SecureHash={vnpSecureHash}";

            return fullUrl;
        }
        #endregion

        #region Response process
        public bool ValidateSignature(string inputHash, string secretKey)
        {
            var rawData = GetResponseData();
            var computedHash = Utils.HmacSHA512(secretKey, rawData);
            return string.Equals(computedHash, inputHash, StringComparison.OrdinalIgnoreCase);
        }

        private string GetResponseData()
        {
            var responseDataBuilder = new StringBuilder();
            foreach (var (key, value) in _responseData.Where(kv => !string.IsNullOrEmpty(kv.Value) && kv.Key != "vnp_SecureHashType" && kv.Key != "vnp_SecureHash"))
            {
                responseDataBuilder.Append(WebUtility.UrlEncode(key) + "=" + WebUtility.UrlEncode(value) + "&");
            }

            // Remove the last '&' if exists
            return responseDataBuilder.ToString().TrimEnd('&');
        }
        #endregion
    }

    public static class Utils
    {
        public static string HmacSHA512(string key, string inputData)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var inputBytes = Encoding.UTF8.GetBytes(inputData);
            using var hmac = new HMACSHA512(keyBytes);
            var hashValue = hmac.ComputeHash(inputBytes);
            return string.Concat(hashValue.Select(b => b.ToString("x2")));
        }

        public static string GetIpAddress(HttpContext context)
        {
            try
            {
                var remoteIpAddress = context.Connection.RemoteIpAddress;

                if (remoteIpAddress != null)
                {
                    if (remoteIpAddress.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        remoteIpAddress = Dns.GetHostEntry(remoteIpAddress)
                            .AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
                    }

                    return remoteIpAddress?.ToString() ?? "127.0.0.1";
                }
            }
            catch
            {
                // Ignoring the exception and returning a default value is usually not a good idea,
                // logging or proper error handling should be implemented in real-world scenarios
            }

            return "127.0.0.1";
        }
    }

    public class VnPayCompare : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return string.Compare(x, y, StringComparison.Ordinal);
        }
    }
}

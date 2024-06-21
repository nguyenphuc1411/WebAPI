using Client.Extensions;
using Client.Extentions;
using Client.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PFood.Data;
using PFood.Models;
using System;

namespace Client.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _config;

        // Constructor của class VnPayService, kiểm tra cấu hình có null hay không
        public VnPayService(IConfiguration config)
        {
            _config = config;
        }

        public string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (model == null) throw new ArgumentNullException(nameof(model));

            var tick = DateTime.Now.Ticks.ToString();

            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", _config["VnPay:Version"]);
            vnpay.AddRequestData("vnp_Command", _config["VnPay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", ((int)model.Amount* 100000).ToString()); 
            vnpay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _config["VnPay:CurrCode"]);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", _config["VnPay:Locale"]);
            vnpay.AddRequestData("vnp_OrderInfo", "Payment for Order: " + model.OrderID);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", _config["VnPay:ReturnUrl"]);
            vnpay.AddRequestData("vnp_TxnRef", tick);

            // Tạo URL thanh toán
            var paymentUrl = vnpay.CreateRequestUrl(_config["VnPay:BaseUrl"], _config["VnPay:HashSecret"]);
            return paymentUrl;
        }

        public VnPaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var vnpay = new VnPayLibrary();

            // Thêm dữ liệu phản hồi vào VnPayLibrary
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            var vnp_orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
            var vnp_TransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_SecureHash = collections.FirstOrDefault(x => x.Key == "vnp_SecureHash").Value;
            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _config["VnPay:HashSecret"]);
            var responseModel = new VnPaymentResponseModel();
            if (!checkSignature)
            {
                responseModel.Success = false;              
            }
            else
            {
                responseModel.Success = true;
                responseModel.PaymentMethod = "VnPay";
                responseModel.OrderId = vnp_orderId.ToString();
                responseModel.OrderDescription = vnp_OrderInfo;
                responseModel.TransactionId = vnp_TransactionId.ToString();
                responseModel.Token = vnp_SecureHash;
                responseModel.VnPayResponseCode = vnp_ResponseCode;
            }
            return responseModel;
        }
    }
}
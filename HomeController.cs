using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using BitPayDll;
using Microsoft.AspNetCore.Mvc;

namespace BitPayCodeSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        //This is test token replace it with yours
        private string _token = "adxcv-zzadq-polkjsad-opp13opoz-1sdf455aadzmck1244567";
        //Set IsTest to false for live testing
        private bool IsTest = true;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> ProcessPayment()
        {

            var amount = 10000;
            var factorId = 123;
            var name = "Test Payment";
            var Email = "test@test.com";
            var description = "This is for test";
            try
            {
                // Create an instance of the BitPay class from the external DLL
                BitPay bitpay = new BitPay(_token, IsTest);

                var redirectUrl = "https://localhost:7188/Home/PaymentResault";

                // Call the Send method from the BitPay class
                var result = await bitpay.SendAsync(amount, redirectUrl, factorId, name, Email, description);

                if (result.status > 0)
                {
                    // Redirect to a URL based on the result received from the Send method

                    return Redirect(result.GatewayUrl);
                }
                TempData["msg"] = result.message;
                // Handle other scenarios or errors based on the result
                return View();
            }
            catch (Exception ex)
            {
                // Handle exceptions thrown by the external DLL
                return StatusCode(500, "An error occurred while processing payment: " + ex.Message);
            }
        }
        public async Task<IActionResult> PaymentResault(string trans_id, string id_get, string factorId)
        {
            try
            {
                BitPay bitpay = new BitPay(_token, IsTest);

                var result = await bitpay.GetAsync(trans_id, id_get);

                if (result.status == 1)
                {
                    //Success payment
                    TempData["msg"] = string.Format("پرداخت شما با شماره فاکتور {0} با موفقیت انجام شد", factorId);
                    return View();
                }
                //Failed payment
                TempData["msg"] = result.message;

                return View();
            }
            catch (Exception ex)
            {
                // Handle exceptions thrown by the external DLL
                return StatusCode(500, "An error occurred while processing payment: " + ex.Message);
            }
        }

    }
}
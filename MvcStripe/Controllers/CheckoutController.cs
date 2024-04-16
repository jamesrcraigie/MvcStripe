using Microsoft.AspNetCore.Mvc;
using MvcStripe.Models;
using Stripe.Checkout;

namespace MvcStripe.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Index()
        {

             
            return View();
        }

        [HttpGet]
        public ActionResult Checkout(Donatoin donation)
        {
            var domain = "http://localhost:5283/";

            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"Checkout/OrderConfirmation",
                CancelUrl = domain + $"Checkout/Login",
                LineItems = new List<SessionLineItemOptions>
                {
					new SessionLineItemOptions
                    {

					    PriceData = new SessionLineItemPriceDataOptions
					    {
						    UnitAmount=(long)(donation.Amount * 100),
						    Currency="gbp",
						    ProductData = new SessionLineItemPriceDataProductDataOptions
						    {
							    Name = "Donation"
						    }
					    },
					    Quantity = 1
					}

				},
                Mode="payment",
                CustomerEmail="james@test.com"
			};

            var service = new SessionService();
            Session session =service.Create(options);

            TempData["Session"] = session.Id;

            Response.Headers.Add("Location", session.Url);

            return new StatusCodeResult(303);
		}

		public IActionResult OrderConfirmation()
		{
			var service = new SessionService();
			Session session = service.Get(TempData["Session"].ToString());

			if (session.PaymentStatus == "paid")
            {
                var transaction = session.PaymentIntentId.ToString();
                return View("Success");
            }

            return View("Login");

	    }

        public IActionResult Success()
        {
            return View();
        }

		public IActionResult Login()
		{
			return View();

		}
	}
}

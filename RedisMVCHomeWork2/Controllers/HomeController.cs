
using Microsoft.AspNetCore.Mvc;
using RedisMVCHomeWork2.Models;

using StackExchange.Redis;

namespace RedisMVCHomeWork2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ConnectionMultiplexer _muxer;
        private readonly string MyListName = "RPubSub";
        private static string SelectedCName = "";
        private static List<string> OldMessages = new();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;

            var muxer = ConnectionMultiplexer.Connect(
            new ConfigurationOptions
            {
                EndPoints = { { "*", 11518 } },
                User = "*",
                Password = "*"
            }
        );
            _muxer = muxer;


        }

        public IActionResult Index()
        {
            var db = _muxer.GetDatabase();

            RedisValue[]? list = db.ListRange(MyListName, 0, -1);

            RedisViewModel model;

            if (list.Length == 0)
            {
                model = new RedisViewModel();
                return View(model);
            }


            model = new RedisViewModel()
            {
                RedisValues = list,
                Messages = OldMessages
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult AddCName(string newCName)
        {
           

            if (newCName != "" && newCName != null)
            {
                var db = _muxer.GetDatabase();

                db.ListRightPush(MyListName, newCName);

                Console.WriteLine($"Add C Name Ok {newCName}");
            }


            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult SelectCName(string cName)
        {
            if(SelectedCName != cName)
            {
                SelectedCName = cName;
                OldMessages = new();
            }
            
            Console.WriteLine($"Select CName = {cName} Ok");


            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public IActionResult SendMessage(string message)
        {

            if(message != ""       && message != null &&
               SelectedCName != "" && SelectedCName != null
                )
            {

                var sub = _muxer.GetSubscriber();
                sub.Publish(SelectedCName, message);

                var db = _muxer.GetDatabase();

                RedisValue[]? list = db.ListRange(MyListName, 0, -1);

                OldMessages.Add(message);

                RedisViewModel model = new RedisViewModel()
                {
                    RedisValues = list,
                    Messages = OldMessages
                };

                return View("Index", model);
            }


            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

      
    }
}

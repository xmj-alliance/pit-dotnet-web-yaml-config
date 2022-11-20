using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WebYamlConfig.Models;

namespace WebYamlConfig.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class APIController : ControllerBase
    {

        private readonly MySecret appSecret;

        public APIController(MySecret appSecret)
        {
            this.appSecret = appSecret;
        }

        [HttpGet]
        public CommonMessage ReadyState()
        {
            var message = new CommonMessage() {
                OK = true,
                Message = "API works!",
            };

            if (appSecret is null || appSecret.Username is null) {
                message.Message += " But we cannot hear any complete secret.";
            } else {
                var secretIntro = $@"

                    Let's now hear a backdoor on this website,
                    Thanks to this dumb controller,
                    we now have an idea of what the DB connection string looks like:

                    ======
                    Username: {appSecret.Username}
                    Password: {appSecret.Password}
                    Host: {appSecret.Host}
                    DataDB: {appSecret.DataDB}
                    ======

                    And this controller also wishes you have a nice day!

                ";
                message.Message += secretIntro;
            }
            
            return message;
        }
    }

}
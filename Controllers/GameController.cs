using Game.Api.Model.Dto;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Net;
using System.Text;

namespace Game.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly ILogger<GameController> _logger;
        int count = 1;

        public GameController(ILogger<GameController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET api/v1/[controller]/game[?nrOfJokes=6]
        [HttpGet]
        [ProducesResponseType(typeof(GameDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetGameAsync(int nrOfJokes = 6)
        {
            if (nrOfJokes < 2)
                return BadRequest("Minimum amount of jokes must be 2.");

            ConnectionFactory connFactory = new();
            connFactory.Uri = new Uri("amqp://guest:guest@172.22.0.2:5672");
            connFactory.ClientProvidedName = "Game.Api";

            IConnection connection = connFactory.CreateConnection();

            IModel channel = connection.CreateModel();

            string exchangeName = "GameExchange";
            string routingKey = "game-routing-key";
            string queueName = "GameQueue";

            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            channel.QueueDeclare(queueName, false, false, false, null);
            channel.QueueBind(queueName, exchangeName, routingKey, null);

            byte[] message = Encoding.UTF8.GetBytes("Multi mode game " + count);
            count++;
            channel.BasicPublish(exchangeName, routingKey, null, message);

            channel.Close();
            connection.Close();

            return Ok();
        }

        [HttpGet("test")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> TestAsync()
        {
            return Ok("I'm a test endpoint.");
        }
    }
}
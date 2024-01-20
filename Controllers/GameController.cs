using Game.Api.Data;
using Game.Api.Model.Dto;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly GameDbContext _db;
        int count = 1;

        public GameController(
            ILogger<GameController> logger,
            GameDbContext db)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _db = db ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("generate")]
        [ProducesResponseType(typeof(GameDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetGameAsync(int nrOfJokes = 6, string mode = "single")
        {
            if (nrOfJokes < 2)
                return BadRequest("Minimum amount of jokes must be 2.");

            string message = "Multi mode game " + count;

            try
            {
                ConnectionFactory connFactory = new();
                connFactory.HostName = "localhost";
                connFactory.ClientProvidedName = "Game.Api";

                IConnection connection = connFactory.CreateConnection();

                IModel channel = connection.CreateModel();

                string exchangeName = "GameExchange";
                string routingKey = "game-routing-key";
                string queueName = "GameQueue";

                channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
                channel.QueueDeclare(queueName, false, false, false, null);
                channel.QueueBind(queueName, exchangeName, routingKey, null);

                byte[] byteMessage = Encoding.UTF8.GetBytes(message);
                count++;
                channel.BasicPublish(exchangeName, routingKey, null, byteMessage);

                channel.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(message);
        }

        [HttpGet("mode")]
        [ProducesResponseType(typeof(ModeDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetModeAsync()
        {
            var modes = await _db.Modes.ToListAsync();

            if (modes is null)
                return NotFound();

            return Ok(modes);
        }

        [HttpGet("test")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> TestAsync()
        {
            return Ok("I'm a test endpoint for Monika's project.");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using RabbitMQ.Client;

namespace APIEscreverNaFilaRabbit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnviarTarefaController : ControllerBase
    {
        [HttpPost]
        public IActionResult InsertTarefa([FromBody] Tarefa tarefa)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using(var connection = factory.CreateConnection())
                using(var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "minhafila1",
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);
                    var message = JsonSerializer.Serialize(tarefa);
                    var body = Encoding.UTF8.GetBytes(message);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: "",
                        routingKey: "minhafila1",
                        basicProperties: properties,
                        body: body);
                }
                return Ok("Tarefa cadastrada na fila");
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}
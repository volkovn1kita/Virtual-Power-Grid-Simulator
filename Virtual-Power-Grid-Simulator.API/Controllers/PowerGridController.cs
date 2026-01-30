using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Virtual_Power_Grid_Simulator.Application.Contracts;
using Virtual_Power_Grid_Simulator.Application.Interfaces;
using Virtual_Power_Grid_Simulator.Domain.Entities;
using Virtual_Power_Grid_Simulator.Infrastructure.Services;

namespace Virtual_Power_Grid_Simulator.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PowerGridController : ControllerBase
    {
        private readonly IPowerGridService _powerGridService;
        private readonly ILogger<PowerGridController> _logger;

        public PowerGridController(
            IPowerGridService powerGridService,
            ILogger<PowerGridController> logger)
        {
            _powerGridService = powerGridService;
            _logger = logger;
        }

        [HttpGet("status")]
        public ActionResult<GridSnapshot> GetCurrentStatus()
        {
            var snapshot = _powerGridService.CalculateGridState();
            return Ok(snapshot);
        }

        //POWER PLANTS

        [HttpGet("plants")]
        public ActionResult<IEnumerable<PowerPlant>> GetAllPowerPlants()
        {
            var plants = _powerGridService.GetAllPowerPlants();
            return Ok(plants);
        }

        [HttpGet("plants/{id}")]
        public ActionResult<PowerPlant> GetPowerPlantById(Guid id)
        {
            try
            {
                var plant = _powerGridService.GetPowerPlantById(id);
                return Ok(plant);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("plants")]
        public ActionResult<PowerPlant> CreatePowerPlant([FromBody] CreatePowerPlantRequest request)
        {
            var plant = new PowerPlant(
                request.Name,
                request.Type,
                request.MaxCapacity,
                request.MinStableLoad,
                request.RampRate
            );
            _powerGridService.RegisterPowerPlant(plant);
            _logger.LogInformation("Registered new power plant: {PlantName} (ID: {PlantId})", plant.Name, plant.Id);
            return CreatedAtAction(nameof(GetPowerPlantById), new { id = plant.Id }, plant);
        }

        [HttpPost("plants/{id}/turn-on")]
        public IActionResult TurnOnPowerPlant(Guid id)
        {
            try
            {
                _powerGridService.TurnOnPowerPlant(id);
                _logger.LogInformation("Turned on power plant with ID: {PlantId}", id);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("plants/{id}/turn-off")]
        public IActionResult TurnOffPowerPlant(Guid id)
        {
            try
            {
                _powerGridService.TurnOffPowerPlant(id);
                _logger.LogInformation("Turned off power plant with ID: {PlantId}", id);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("plants/{id}/adjust")]
        public IActionResult AdjustPowerPlant(Guid id, [FromBody] UpdatePowerRequest request)
        {
            try
            {
                if (request.TargetPower < 0)
                {
                    return BadRequest("Target power must be non-negative.");
                }
                
                _powerGridService.AdjustPowerPlant(id, request.TargetPower);

                var updatedPlant = _powerGridService.GetPowerPlantById(id);
                _logger.LogInformation("Adjusted power plant with ID: {PlantId} to target power: {TargetPower}", id, request.TargetPower);
                return Ok(updatedPlant);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Failed to adjust power plant with ID: {PlantId}. Error: {ErrorMessage}", id, ex.Message);
                return NotFound(ex.Message);
            }
        }

        //CONSUMERS
        [HttpGet("consumers")]
        public ActionResult<IEnumerable<PowerConsumer>> GetAllConsumers()
        {
            var consumers = _powerGridService.GetAllConsumers();
            return Ok(consumers);
        }

        [HttpGet("consumers/{id}")]
        public ActionResult<PowerConsumer> GetConsumerById(Guid id)
        {
            try
            {
                var consumer = _powerGridService.GetConsumerById(id);
                return Ok(consumer);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("consumers")]
        public ActionResult<PowerConsumer> CreateConsumer([FromBody] CreatePowerConsumerRequest request)
        {
            var consumer = new PowerConsumer(
                request.Name,
                request.Type,
                request.MaxPeakLoad,
                request.Priority
            );

            _powerGridService.RegisterConsumer(consumer);
            _logger.LogInformation("Registered new power consumer: {ConsumerName} (ID: {ConsumerId})", consumer.Name, consumer.Id);
            return CreatedAtAction(nameof(GetConsumerById), new { id = consumer.Id }, consumer);
        }

        [HttpPost("consumers/{id}/connect")]
        public IActionResult ConnectConsumer(Guid id)
        {
            try
            {
                _powerGridService.ConnectConsumer(id);
                _logger.LogInformation("Connected power consumer with ID: {ConsumerId}", id);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Failed to connect power consumer with ID: {ConsumerId}. Error: {ErrorMessage}", id, ex.Message);
                return NotFound(ex.Message);
            }
        }

        [HttpPost("consumers/{id}/disconnect")]
        public IActionResult DisconnectConsumer(Guid id)
        {
            try
            {
                _powerGridService.DisconnectConsumer(id);
                _logger.LogInformation("Disconnected power consumer with ID: {ConsumerId}", id);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Failed to disconnect power consumer with ID: {ConsumerId}. Error: {ErrorMessage}", id, ex.Message);
                return NotFound(ex.Message);
            }
        }

    }
}

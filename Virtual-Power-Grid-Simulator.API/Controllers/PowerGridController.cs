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

        public PowerGridController(IPowerGridService powerGridService)
        {
            _powerGridService = powerGridService;
        }

        [HttpGet("status")]
        public ActionResult<GridSnapshot> GetCurrentStatus()
        {
            var time = DateTime.Now;
            var snapshot = _powerGridService.CalculateGridState(time);
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
            return CreatedAtAction(nameof(GetPowerPlantById), new { id = plant.Id }, plant);
        }

        [HttpPost("plants/{id}/turn-on")]
        public IActionResult TurnOnPowerPlant(Guid id)
        {
            try
            {
                _powerGridService.TurnOnPowerPlant(id);
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
                return Ok(updatedPlant);
            }
            catch (KeyNotFoundException ex)
            {
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
            return CreatedAtAction(nameof(GetConsumerById), new { id = consumer.Id }, consumer);
        }

        [HttpPost("consumers/{id}/connect")]
        public IActionResult ConnectConsumer(Guid id)
        {
            try
            {
                _powerGridService.ConnectConsumer(id);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("consumers/{id}/disconnect")]
        public IActionResult DisconnectConsumer(Guid id)
        {
            try
            {
                _powerGridService.DisconnectConsumer(id);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using FraudReporterAPI.DTOs;
using FraudReporterAPI.Paginations;
using FraudReporterAPI.Interfaces;

namespace FraudReporterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FraudsController : ControllerBase
    {
        private readonly IFraudService fraudService;

        public FraudsController(IFraudService fraudService)
        {
            this.fraudService = fraudService;
        }

        [HttpGet]
        public IActionResult GetFraud([FromQuery] FraudPagination pagination)
        {
            return Ok(fraudService.GetFrauds(pagination));
        }

        [HttpGet("{id}")]
        public IActionResult GetFraud(int id)
        {
            var fraud = fraudService.GetFraudDetail(id);
            if (fraud == null)
            {
                return NotFound();
            }

            return Ok(fraud);
        }

        [HttpPut("{id}")]
        public IActionResult PutFraud(int? id, FraudDTO fraud)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            bool isUpdated = fraudService.UpdateFraud(id.GetValueOrDefault(), fraud);
            return Ok(isUpdated);
        }

        [HttpPost]
        public IActionResult PostFraud(FraudDTO fraud)
        {
            bool isCreated = fraudService.SaveFraud(fraud);
            return Ok(isCreated);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteFraud(int id)
        {
            var isDeleted = fraudService.DeleteFraud(id);
            return Ok(isDeleted);
        }

        [HttpPut("update-fraud-status/{id}")]
        public IActionResult UpdateFraudStatus(int? id, int fraudStatus)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            bool isUpdated = fraudService.UpdateFraudStatus(id.GetValueOrDefault(), fraudStatus);
            return Ok(isUpdated);
        }

        [HttpPut("cancel-report/{id}")]
        public IActionResult CancelReport(int? id, int fraudStatus)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            bool isCancelled = fraudService.CancelReport(id.GetValueOrDefault(), fraudStatus);
            return Ok(isCancelled);
        }
    }
}

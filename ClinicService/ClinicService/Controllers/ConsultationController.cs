using ClinicService.Services.Implementation;
using ClinicService.Services;
using Microsoft.AspNetCore.Mvc;
using ClinicService.Data.Entitys;
using ClinicService.Models.Requests;

namespace ClinicService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultationController : ControllerBase
    {
        private readonly IConsultationRepository _consultationRepository;
        private readonly ILogger<ConsultationController> _logger;

        public ConsultationController(IConsultationRepository consultationRepository, ILogger<ConsultationController> logger)
        {
            _consultationRepository = consultationRepository;
            _logger = logger;
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public IActionResult Create([FromBody] CreateConsultationRequest createRequest)
        {
            _logger.LogInformation("Create consult");

            return Ok(_consultationRepository.Add(new Consultation
            {
                ClientId = createRequest.ClientId,
                PetId = createRequest.PetId,
                ConsultationDate = createRequest.ConsultationDate,
                Description = createRequest.Description
            }));
        }


        [HttpPut("update")]
        public IActionResult Update([FromBody] UpdateConsultationRequest updateRequest)
        {
            _logger.LogInformation("Update consult");

            _consultationRepository.Update(new Consultation
            {
                ConsultationId = updateRequest.ConsultationId,
                ClientId = updateRequest.ClientId,
                PetId = updateRequest.PetId,
                ConsultationDate = updateRequest.ConsultationDate,
                Description = updateRequest.Description
            });

            return Ok();
        }

        [HttpDelete("delete")]
        public IActionResult Delete([FromQuery] int consultationId)
        {
            _logger.LogInformation("Delete consult by id " + consultationId);

            _consultationRepository.Delete(consultationId);

            return Ok();
        }

        [HttpGet("get-all")]
        [ProducesResponseType(typeof(IList<Consultation>), StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            _logger.LogInformation("Get All consults");
            return Ok(_consultationRepository.GetAll());
        }            

        [HttpGet("get/{consultationId}")]
        [ProducesResponseType(typeof(Consultation), StatusCodes.Status200OK)]
        public IActionResult GetById([FromRoute] int consultationId)
        {
            _logger.LogInformation("Get consult by id " + consultationId);
            return Ok(_consultationRepository.GetById(consultationId));
        }            
    }
}

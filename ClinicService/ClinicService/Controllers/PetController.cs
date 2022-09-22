using ClinicService.Data.Entitys;
using ClinicService.Models.Requests;
using ClinicService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PetController : ControllerBase
    {
        private readonly IPetRepository _petRepository;
        private readonly ILogger<PetController> _logger;

        public PetController(IPetRepository petRepository, ILogger<PetController> logger)
        {
            _petRepository = petRepository;
            _logger = logger;
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public IActionResult Create([FromBody] CreatePetRequests createRequest)
        {
            _logger.LogInformation("Create pet");

            return Ok(_petRepository.Add(new Pet
            {
                ClientId = createRequest.ClientId,
                Name = createRequest.Name,
                Birthday = createRequest.Birthday,
            }));
        }

        [HttpPut("update")]
        public IActionResult Update([FromBody] UpdatePetRequest updateRequest)
        {
            _logger.LogInformation("Update pet");

            _petRepository.Update(new Pet
            {
                PetId = updateRequest.PetId,
                ClientId = updateRequest.ClientId,
                Name = updateRequest.Name,
                Birthday = updateRequest.Birthday,
            });

            return Ok();
        }

        [HttpDelete("delete")]
        public IActionResult Delete([FromQuery] int petId)
        {
            _logger.LogInformation("Delete pet by id " + petId);

            _petRepository.Delete(petId);

            return Ok();
        }

        [HttpGet("get-all")]
        [ProducesResponseType(typeof(IList<Pet>), StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            _logger.LogInformation("Get All pets");
            return Ok(_petRepository.GetAll());
        }
            

        [HttpGet("get/{petId}")]
        [ProducesResponseType(typeof(Pet), StatusCodes.Status200OK)]
        public IActionResult GetById([FromRoute] int petId)
        {
            _logger.LogInformation("Get pet by id " + petId);
            return Ok(_petRepository.GetById(petId));
        }
    }
}

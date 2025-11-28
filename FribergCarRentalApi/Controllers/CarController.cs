using AutoMapper;
using FribergCarRental.Core.Classes;
using FribergCarRental.Core.Dtos;
using FribergCarRental.DAL.Data;
using Microsoft.AspNetCore.Mvc;

namespace FribergCarRentalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CarController : ControllerBase
    {
        public CarRepository _carRepository { get; set; }
        public IMapper _mapper { get; }

        public CarController(CarRepository carRepository, IMapper mapper)
        {
            _carRepository = carRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<CarDto>>> GetAll()
        {
            var cars = await _carRepository.GetCarsWithImagesAsync();
            var carDtos = _mapper.Map<List<CarDto>>(cars);
            return carDtos.ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CarDto>> GetById(int id)
        {
            var car = await _carRepository.GetCarWithImagesAsync(id);
            if (car == null)
            {
                return NotFound($"No car with id {id} was found");
            }
            var carDto = _mapper.Map<CarDto>(car);
            return carDto;
        }

        [HttpPost]
        public async Task<ActionResult<CarDto>> AddCar([FromBody] CarDto carDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var car = _mapper.Map<Car>(carDto);
            await _carRepository.AddAsync(car);
            await _carRepository.SaveChangesAsync();

            var createdCarDto = _mapper.Map<CarDto>(car);
            return CreatedAtAction(nameof(GetById), new { id = car.CarId }, createdCarDto);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateCar(int id, [FromBody] CarDto carDto)
        {
            if (id != carDto.CarId)
            {
                return BadRequest("Id mismatch");
            }

            var existingCar = await _carRepository.GetByIdAsync(id);
            if (existingCar == null)
            {
                return NotFound();
            }

            _mapper.Map(carDto, existingCar);
            _carRepository.Update(existingCar);
            await _carRepository.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteCar(int id)
        {
            var car = await _carRepository.GetByIdAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            _carRepository.Delete(car);
            await _carRepository.SaveChangesAsync();
            return Ok();
        }

    }
}

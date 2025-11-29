using AutoMapper;
using FribergCarRental.Core.Classes;
using FribergCarRental.Core.Dtos;
using FribergCarRental.DAL.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FribergCarRentalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CarController : ControllerBase
    {
        private readonly CarRepository _carRepository;
        public readonly ImageRepository _imageRepository;
        public readonly IMapper _mapper;

        public CarController(CarRepository carRepository, IMapper mapper, ImageRepository imageRepository)
        {
            _carRepository = carRepository;
            _mapper = mapper;
            _imageRepository = imageRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<CarDto>>> GetAll()
        {
            var cars = await _carRepository.GetAllAsync();
            var carDtos = _mapper.Map<List<CarDto>>(cars);
            return carDtos.ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CarDto>> GetById(int id)
        {
            var car = await _carRepository.GetByIdAsync(id);
            if (car == null)
            {
                return NotFound($"No car with id {id} was found");
            }
            var carDto = _mapper.Map<CarDto>(car);
            return carDto;
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteImage/{carId:int}/{imageId:int}")]
        public async Task<ActionResult> DeleteImage(int carId, int imageId)
        {
            var image = await _imageRepository.GetByIdAsync(imageId);
            if (image == null || image.CarId != carId)
            {
                return BadRequest("Id mismatch");
            }
            _imageRepository.Delete(image);
            await _imageRepository.SaveChangesAsync();
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddImage")]
        public async Task<ActionResult> AddImage([FromBody] CreateImageDto createImageDto)
        {
            var car = await _carRepository.GetByIdAsync(createImageDto.CarId);
            if (car == null)
            {
                return BadRequest();
            }
            var newImage = new Image { CarId = createImageDto.CarId, Car = car, Path = createImageDto.Path };
            await _imageRepository.AddAsync(newImage);
            await _imageRepository.SaveChangesAsync();
            return Ok();
        }
    }
}

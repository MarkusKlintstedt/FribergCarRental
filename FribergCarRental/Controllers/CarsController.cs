using AutoMapper;
using FribergCarRental.Models;
using FribergCarRental.Services.Authentication;
using FribergCarRental.Services.Base;
using Microsoft.AspNetCore.Mvc;

namespace FribergCarRental.Controllers
{
    public class CarsController : BaseController
    {
        private readonly IMapper _mapper;
        private CarService _carService;


        public CarsController(IMapper mapper, CarService carService, IAuthService authService) : base(authService)
        {
            _mapper = mapper;
            _carService = carService;
        }

        // GET: Cars 
        public async Task<IActionResult> Index()
        {
            var carsResponse = await _carService.GetCars();
            var carViewModels = _mapper.Map<List<CarViewModel>>(carsResponse.Data);
            return View(carViewModels);
        }

        // GET: Cars/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var carResponse = await _carService.GetCar(id);
            var carViewModel = _mapper.Map<CarViewModel>(carResponse.Data);
            if (carViewModel == null)
            {
                return NotFound();
            }

            return View(carViewModel);
        }

        public async Task<IActionResult> Images(int id)
        {
            var carResponse = await _carService.GetCar(id);
            var viewModel = _mapper.Map<CarViewModel>(carResponse.Data);
            return View(viewModel);
        }
    }
}

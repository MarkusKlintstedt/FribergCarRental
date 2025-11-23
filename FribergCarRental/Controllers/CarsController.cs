using AutoMapper;
using FribergCarRental.Client.Services.Base;
using FribergCarRental.Models;
using FribergCarRental.Services.Authentication;
using FribergCarRental.Services.Base;
using Microsoft.AspNetCore.Mvc;

namespace FribergCarRental.Controllers
{
    public class CarsController : BaseController
    {
        private readonly IMapper _mapper;
        private List<CarViewModel> _carViewModels = new List<CarViewModel>();
        private CarService _carService;
        private ImageService _imageService;


        public CarsController(IMapper mapper, CarService carService, ImageService imageService, IAuthService authService) : base(authService)
        {
            _mapper = mapper;
            _carService = carService;
            _imageService = imageService;
        }

        // GET: Cars 
        public async Task<IActionResult> Index()
        {
            var carsResponse = await _carService.GetCars();
            _carViewModels = _mapper.Map<List<CarViewModel>>(carsResponse.Data);
            //foreach (var car in _carViewModels)
            //{
            //    var imageResponse = await _imageService.GetImage(car.CarId);
            //    car.Images = _mapper.Map<List<ImageViewModel>>(imageResponse.Data);
            //}
            return View(_carViewModels);
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
            //var imagesResponse = await _imageService.GetImagesByCarId(id);

            var viewModel = _mapper.Map<CarViewModel>(carResponse.Data);
            //viewModel.Images = _mapper.Map<List<ImageViewModel>>(imagesResponse.Data);
            return View(viewModel);
        }
    }
}

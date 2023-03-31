using Microsoft.AspNetCore.Mvc;
using VitcAuth.Models;
using VitcAuth.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using VitcAuth.DTO.Cars;

namespace VitcAuth.Controllers.Accountant
{
    [Authorize]
    [ApiController]
    [Route("apiv2/[controller]")]
    public class CarController : ControllerBase
    {
        public IUserRepository userRepository;
        IBranchCarRepository branchCar;
        ICarTypeRepository carType;
        IFuelTypeRepository fuelType;
        IUsePurposeRepository usePurpose;
        ICarWeightTypeRepository carWeight;
        IGpsRepository gps;

        IMapper mapper;
        public CarController(IUserRepository userRepository,
         IBranchCarRepository branchCar,
         ICarTypeRepository carType,
         IFuelTypeRepository fuelType,
         IUsePurposeRepository usePurpose,
         ICarWeightTypeRepository carWeight,
         IGpsRepository gps,
         IMapper mapper)
        {
            this.userRepository = userRepository;
            this.branchCar = branchCar;
            this.carType = carType;
            this.fuelType = fuelType;
            this.usePurpose = usePurpose;
            this.carWeight = carWeight;
            this.gps = gps;
            this.mapper = mapper;
        }

        private IQueryable<BranchCar> GetCars()
        {
            var branchId = this.userRepository.currentUser(User).BranchId;

            return this.branchCar.GetAll().Where(e => e.BranchId == branchId);
        }


        [HttpGet("BranchCars")]
        public IActionResult getBranchCars()
        {
            var branchCar = this.mapper.ProjectTo<BranchCarDTO>(this.GetCars().OrderByDescending(e => e.Id)).ToList();
            var carType = this.carType.GetAll().ToList();
            var fuelType = this.fuelType.GetAll().OrderBy(e => e.Id).ToList();
            var usePurpose = this.usePurpose.GetAll().OrderBy(e => e.Id).ToList();
            var gps = this.gps.GetAll().ToList();
            var carWeight = this.carWeight.GetAll().ToList();
            return Ok(new
            {
                data = branchCar,
                gps = gps,
                types = carWeight,
                use_purpose = usePurpose,
                fuel_types = fuelType,
                car_types = carType
            });
        }

        [HttpPost("BranchCars")]
        public IActionResult storeBranchCars(BranchCar branch)
        {
            var branchId = this.userRepository.currentUser(User).BranchId;
            branch.BranchId = (long)branchId;
            this.branchCar.Add(branch);
            this.branchCar.commit();

            return Ok(this.mapper.ProjectTo<BranchCarDTO>(this.GetCars()).First(e => e.Id == branch.Id));
        }

        [HttpPut("BranchCars")]
        public IActionResult editBranchCars(BranchCar branch)
        {
            var branchId = this.userRepository.currentUser(User).BranchId;
            branch.BranchId = (long)branchId;
            this.branchCar.update(branch);
            this.branchCar.commit();
            return Ok(this.mapper.ProjectTo<BranchCarDTO>(this.GetCars()).First(e => e.Id == branch.Id));
        }

        [HttpDelete("BranchCars")]
        public IActionResult deleteBranchCar([FromQuery] int car_id)
        {
            var branch = this.branchCar.GetById(car_id);
            this.branchCar.Remove(branch);
            this.branchCar.commit();
            return Ok();
        }
    }
}
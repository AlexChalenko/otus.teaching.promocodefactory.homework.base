using Microsoft.AspNetCore.Mvc;
using Otus.Teaching.PromoCodeFactory.Core.Abstractions.Repositories;
using Otus.Teaching.PromoCodeFactory.Core.Domain.Administration;
using Otus.Teaching.PromoCodeFactory.Core.Models;
using Otus.Teaching.PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Otus.Teaching.PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeesController
        : ControllerBase
    {
        private readonly IRepository<Employee> _employeeRepository;

        public EmployeesController(IRepository<Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();

            var employeesModelList = employees.Select(x =>
                new EmployeeShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FullName = x.FullName,
                }).ToList();

            return employeesModelList;
        }

        /// <summary>
        /// Получить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            var employeeModel = new EmployeeResponse()
            {
                Id = employee.Id,
                Email = employee.Email,
                Roles = employee.Roles.Select(x => new RoleItemResponse()
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };

            return employeeModel;
        }

        /// <summary>
        /// Создать сотрудника
        /// </summary>
        /// <param name="createEmployeeModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<EmployeeResponse>> CreateEmployeeAsync(CreateEmployeeModel createEmployeeModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {

                var newEmployee = new Employee()
                {
                    Id = Guid.NewGuid(),
                    Email = createEmployeeModel.Email,
                    FirstName = createEmployeeModel.FirstName,
                    LastName = createEmployeeModel.LastName,
                    AppliedPromocodesCount = createEmployeeModel.AppliedPromocodesCount,
                    Roles = createEmployeeModel.Roles.Select(x => new Role()
                    {
                        Name = x.Name,
                        Description = x.Description
                    }).ToList()
                };

                await _employeeRepository.AddAsync(newEmployee);

                var createdEmployee = new EmployeeResponse()
                {
                    Id = newEmployee.Id,
                    Email = newEmployee.Email,
                    FullName = $"{newEmployee.FirstName} {newEmployee.LastName}",
                    AppliedPromocodesCount = newEmployee.AppliedPromocodesCount,
                    Roles = newEmployee.Roles.Select(x => new RoleItemResponse()
                    {
                        Name = x.Name,
                        Description = x.Description
                    }).ToList(),
                };

                return createdEmployee;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Обновить данные сотрудника
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateEmployeeModel"></param>
        /// <returns></returns>
        [HttpPut("{id:guid}")]
        public async Task<ActionResult> UpdateEmployeeAsync(Guid id, UpdateEmployeeModel updateEmployeeModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var employee = await _employeeRepository.GetByIdAsync(id);

                if (employee == null)
                    return NotFound($"Employee with Id {id} not found.");

                employee.FirstName = updateEmployeeModel.FirstName;
                employee.LastName = updateEmployeeModel.LastName;
                employee.Email = updateEmployeeModel.Email;
                employee.AppliedPromocodesCount = updateEmployeeModel.AppliedPromocodesCount;
                await _employeeRepository.UpdateAsync(employee);

                var updatedEmployee = new EmployeeResponse()
                {
                    Id = employee.Id,
                    Email = employee.Email,
                    FullName = $"{employee.FirstName} {employee.LastName}",
                    AppliedPromocodesCount = employee.AppliedPromocodesCount,   
                    Roles = employee.Roles.Select(x => new RoleItemResponse()
                    {
                        Name = x.Name,
                        Description = x.Description
                    }).ToList(),
                };

                return Ok(updatedEmployee);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Удалить сотрудника
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteEmployeeAsync(Guid id)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(id);

                if (employee == null)
                    return NotFound($"Employee with IDd {id} not found.");

                await _employeeRepository.DeleteAsync(employee);
                return Ok(new { message = $"Employee with Id {id} successfully deleted." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
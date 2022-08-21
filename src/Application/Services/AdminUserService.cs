using Application.Contracts;
using Application.DataTransferObjects;
using Application.Helpers;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Application.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AdminUserService(IRepositoryManager repository, IMapper mapper, IConfiguration configuration) : base(repository, userManager, roleManager, mapper, configuration)
        {
            _repository = repository;
            _mapper = mapper;
            _configuration = configuration;
        }

        
    }
}

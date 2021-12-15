using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataService.Http;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo _repository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(IPlatformRepo repository, IMapper mapper, 
            ICommandDataClient commandDataClient, IMessageBusClient messageBusClient)
        {
            _repository = repository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
            _messageBusClient = messageBusClient;
        }
        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            var platformItems = _repository.GetAllPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
        }
        
        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            var platformItem = _repository.GetPlatformById(id);
            if (platformItem != null)
            {
                return Ok(_mapper.Map<PlatformReadDto>(platformItem));
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>>CreatePlatform(PlatformCreateDto platformCreateDto)
        {
             var platformCreate = _mapper.Map<Platform>(platformCreateDto);
             _repository.CreatePlatform(platformCreate);
             _repository.SaveChanges();
             var returnPlatform = _mapper.Map<PlatformReadDto>(platformCreate);
             // Send Sync Message
             try
             {
                await _commandDataClient.SendPlatformToCommand(returnPlatform);
             }
             catch (Exception e)
             {
                 Console.WriteLine($"Could not send sync {e}");
             }
             
             // Send Async Message
             try
             {
                 var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(returnPlatform);
                 platformPublishedDto.Event = "Platform_Published";
                 _messageBusClient.PublishNewPlatform(platformPublishedDto);
             }
             catch (Exception e)
             {
                 Console.WriteLine($"Could not send async {e}");
             }
             return CreatedAtRoute(nameof(GetPlatformById), new { Id = returnPlatform.Id}, returnPlatform);
        }
    }
}
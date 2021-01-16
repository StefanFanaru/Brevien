﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Blogging.API.Asp;
using Blogging.API.Asp.Validators;
using Blogging.API.Dtos;
using Blogging.API.Infrastructure.Data.Entities;
using Blogging.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blogging.API.Controllers
{
  [Route("api/v1/[controller]")]
  [Authorize]
  [ApiController]
  public class BlogController : ControllerBase
  {
    private readonly IBlogService _blogService;
    private readonly IUserInfo _userInfo;

    public BlogController(IBlogService blogService, IUserInfo userInfo)
    {
      _blogService = blogService;
      _userInfo = userInfo;
    }

    [HttpGet("current-user")]
    public async Task<List<Blog>> Get()
    {
      return await _blogService.GetByUserId(_userInfo.Id);
    }

    [HttpGet]
    [Route("all")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<List<Blog>>> GetAll()
    {
      return await _blogService.GetAllAsync();
    }

    [HttpGet]
    [Route("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> Get(string id)
    {
      return await _blogService.GetAsync(id);
    }

    [HttpPost]
    public async Task<IActionResult> Create(BlogCreateDto blog)
    {
      var validator = new BlogControllerValidators.CreateValidator();
      var validationResult = await validator.ValidateAsync(blog);

      if (!validationResult.IsValid)
      {
        return new BadRequestObjectResult(validationResult.Errors);
      }

      var createdBlog = await _blogService.CreateAsync(blog);
      return new OkObjectResult(createdBlog);
    }

    [HttpPatch]
    public async Task<IActionResult> Update(BlogUpdateDto blog)
    {
      var validator = new BlogControllerValidators.UpdateValidator();
      var validationResult = await validator.ValidateAsync(blog);

      if (!validationResult.IsValid)
      {
        return new BadRequestObjectResult(validationResult.Errors);
      }

      return await _blogService.UpdateAsync(blog);
    }

    [HttpPatch]
    [Route("{id}/disable")]
    public async Task<IActionResult> Disable(string id)
    {
      return await _blogService.ToggleEnabledState(id, false);
    }

    [HttpPatch]
    [Route("{id}/enable")]
    public async Task<IActionResult> Enable(string id)
    {
      return await _blogService.ToggleEnabledState(id, true);
    }

    [HttpPatch]
    [Route("{id}/change-owner/{newOwnerId}")]
    public async Task<IActionResult> ChangeOwner(string id, string newOwnerId)
    {
      return await _blogService.ChangeOwnerAsync(id, newOwnerId);
    }

    [HttpPatch]
    [Route("{id}/soft-delete")]
    public async Task<IActionResult> SoftDelete(string id)
    {
      return await _blogService.SoftDeleteAsync(id);
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
      return await _blogService.DeleteAsync(id);
    }

    [HttpGet]
    [Route("{id}/owner/{userId}")]
    public async Task<IActionResult> CheckOwnership(string id, string userId)
    {
      return await _blogService.CheckOwnership(id, userId);
    }
  }
}
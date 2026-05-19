using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TestManager.Application.DTOs;
using TestManager.Application.Services;

namespace TestManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestsController(ITestService testService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<TestListItemDto>>> GetAll(CancellationToken cancellationToken)
    {
        var tests = await testService.GetAllAsync(cancellationToken);

        return Ok(tests);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TestDetailsDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var test = await testService.GetByIdAsync(id, cancellationToken);

        return test is null ? NotFound() : Ok(test);
    }

    [HttpGet("{id:int}/take")]
    public async Task<ActionResult<TakeTestDto>> GetForTaking(int id, CancellationToken cancellationToken)
    {
        var test = await testService.GetForTakingAsync(id, cancellationToken);

        return test is null ? NotFound() : Ok(test);
    }

    [HttpPost]
    public async Task<ActionResult<TestDetailsDto>> Create(
        CreateTestRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var test = await testService.CreateAsync(request, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = test.Id }, test);
        }
        catch (ValidationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TestDetailsDto>> Update(
        int id,
        UpdateTestRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var test = await testService.UpdateAsync(id, request, cancellationToken);

            return test is null ? NotFound() : Ok(test);
        }
        catch (ValidationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await testService.DeleteAsync(id, cancellationToken);

        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("{id:int}/submit")]
    public async Task<ActionResult<TestAttemptResultDto>> Submit(
        int id,
        SubmitTestAttemptRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await testService.SubmitAsync(id, request, cancellationToken);

            return result is null ? NotFound() : Ok(result);
        }
        catch (ValidationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }
}

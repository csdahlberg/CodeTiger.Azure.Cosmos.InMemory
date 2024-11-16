using AspNetCoreWebApiSample.Models;
using AspNetCoreWebApiSample.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreWebApiSample.Controllers;

[ApiController]
[Route("[controller]")]
public class NoteController : ControllerBase
{
    private readonly NoteRepository _repository;

    public NoteController(NoteRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<ActionResult<Note>> CreateAsync(NewNote note, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(note.Text))
        {
            return BadRequest("The note text is required.");
        }

        return await _repository.CreateNoteAsync(note, cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Note>> GetAsync(string id, CancellationToken cancellationToken)
    {
        var note = await _repository.GetNoteAsync(id, cancellationToken);

        return note is not null ? Ok(note) : NotFound();
    }

    [HttpPut]
    public async Task<Note> UpdateAsync(Note note, CancellationToken cancellationToken)
    {
        return await _repository.UpdateNoteAsync(note, cancellationToken);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        await _repository.DeleteNoteAsync(id, cancellationToken);
    }
}

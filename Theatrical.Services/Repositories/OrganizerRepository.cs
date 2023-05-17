﻿using Microsoft.EntityFrameworkCore;
using Theatrical.Data.Context;
using Theatrical.Data.Models;

namespace Theatrical.Services.Repositories;

public interface IOrganizerRepository
{
    Task<Organizer?> Get(int id);
    Task<List<Organizer?>> Get();
    Task Create(Organizer organizer);
    Task Delete(Organizer organizer);
}

public class OrganizerRepository : IOrganizerRepository
{
    private readonly TheatricalPlaysDbContext _context;

    public OrganizerRepository(TheatricalPlaysDbContext context)
    {
        _context = context;
    }
    
    public async Task<Organizer?> Get(int id)
    {
        var organizer = await _context.Organizers.FindAsync(id);
        return organizer;
    }

    public async Task<List<Organizer?>> Get()
    {
        var organizers = await _context.Organizers.ToListAsync();
        return organizers;
    }

    public async Task Create(Organizer organizer)
    {
        await _context.Organizers.AddAsync(organizer);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Organizer organizer)
    {
        _context.Organizers.Remove(organizer);
        await _context.SaveChangesAsync();
    }
}

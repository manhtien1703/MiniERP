using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repositories;

public class ProvinceRepository : IProvinceRepository
{
    private readonly MiniErpDbContext _context;

    public ProvinceRepository(MiniErpDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Province>> GetAllAsync() => await _context.Provinces.ToListAsync();

    public async Task<Province?> GetByIdAsync(int id) => await _context.Provinces.FindAsync(id);
}

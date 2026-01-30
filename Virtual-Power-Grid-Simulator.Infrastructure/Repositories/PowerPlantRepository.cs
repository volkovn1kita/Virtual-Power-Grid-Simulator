using Virtual_Power_Grid_Simulator.Domain.Entities;
using Virtual_Power_Grid_Simulator.Infrastructure.Persistence;

namespace Virtual_Power_Grid_Simulator.Infrastructure.Repositories;

public class PowerPlantRepository
{
    private readonly ApplicationDbContext _context;

    public PowerPlantRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<PowerPlant> GetAll()
    {
        return _context.PowerPlants.ToList();
    }

    public PowerPlant? GetById(Guid id)
    {
        return _context.PowerPlants.FirstOrDefault(p => p.Id == id);
    }

    public void Add(PowerPlant plant)
    {
        _context.PowerPlants.Add(plant);
        _context.SaveChanges();
    }

    public void Update(PowerPlant plant)
    {
        _context.PowerPlants.Update(plant);
        _context.SaveChanges();
    }
}
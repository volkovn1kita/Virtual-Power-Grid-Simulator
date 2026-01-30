using System;
using Virtual_Power_Grid_Simulator.Domain.Entities;
using Virtual_Power_Grid_Simulator.Infrastructure.Persistence;

namespace Virtual_Power_Grid_Simulator.Infrastructure.Repositories;

public class PowerConsumerRepository
{
    private readonly ApplicationDbContext _context;

    public PowerConsumerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(PowerConsumer powerConsumer)
    {
        _context.PowerConsumers.Add(powerConsumer);
        _context.SaveChanges();
    }

    public void Update(PowerConsumer powerConsumer)
    {
        _context.PowerConsumers.Update(powerConsumer);
        _context.SaveChanges();
    }

    public List<PowerConsumer> GetAll()
    {
        return _context.PowerConsumers.ToList();
    }

    public PowerConsumer? GetById(Guid id)
    {
        return _context.PowerConsumers.FirstOrDefault(pc => pc.Id == id);
    }

}

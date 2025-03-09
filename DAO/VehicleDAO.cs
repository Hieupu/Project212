using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project212.Models;

namespace Project212.DAO
{
    class VehicleDAO
    {
        private readonly Prn212AssignmentContext context;

        public VehicleDAO()
        {
            context = new Prn212AssignmentContext();
        }

        public List<Vehicle> GetAllVehicles()
        {
            return context.Vehicles.Include(v => v.Citizen).ToList();
        }

        public List<Vehicle> GetVehiclesByCitizenId(int citizenId)
        {
            return context.Vehicles
                .Include(v => v.Citizen)
                .Where(v => v.CitizenId == citizenId)
                .ToList();
        }
    }
}

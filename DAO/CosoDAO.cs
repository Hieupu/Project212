using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project212.Models;

namespace Project212.DAO
{
    internal class CosoDAO
    {
        public static List<InspectionStation> GetInspectionStations()
        {
            using (Prn212AssignmentContext context = new Prn212AssignmentContext())
            {
                return context.InspectionStations
                              .Select(s => new InspectionStation { Id = s.Id, Name = s.Name })
                              .ToList();
            }
        }
    }
}

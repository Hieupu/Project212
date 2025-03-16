using Microsoft.EntityFrameworkCore;
using Project212.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project212.DAO
{
    class RecordDAO
    {
        private readonly Prn212AssignmentContext context;

        public RecordDAO()
        {
            context = new Prn212AssignmentContext();
        }

        public List<Record> GetAllRecords()
        {
            var records = context.Records.Include(r => r.Vehicle)
                                         .ThenInclude(v => v.Citizen)
                                         .Include(r => r.Time)
                                         .ToList();
            return records;
        }
    }
}

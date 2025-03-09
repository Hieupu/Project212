using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project212.Models;

namespace Project212.DAO
{
    class CitizenDAO
    {
        private readonly Prn212AssignmentContext context;

        public CitizenDAO()
        {
            context = new Prn212AssignmentContext();
        }

        public Citizen GetCitizenByAccountId(int accountId)
        {
            return context.Citizens
                .FirstOrDefault(c => c.AccId == accountId);
        }

        public bool UpdateCitizen(Citizen citizen)
        {
            try
            {
                var existingCitizen = context.Citizens.FirstOrDefault(c => c.Id == citizen.Id);
                if (existingCitizen != null)
                {
                    existingCitizen.Name = citizen.Name;
                    existingCitizen.Dob = citizen.Dob;
                    existingCitizen.Address = citizen.Address;
                    existingCitizen.Phone = citizen.Phone;
                    existingCitizen.Mail = citizen.Mail;

                    context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

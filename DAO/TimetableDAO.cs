using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project212.Models;

namespace Project212.DAO
{
    internal class TimetableDAO
    {
        private readonly Prn212AssignmentContext _context;

        public TimetableDAO(Prn212AssignmentContext context)
        {
            _context = context;
        }

        // đang sửa AddTimetable

        //public bool AddTimetable(int inspectionId, int accId, DateTime inspectTime)
        //{
        //    try
        //    {
        //        var timetable = new Project212.Models.Timetable
        //        {
        //            InspectionId = inspectionId,
        //            AccId = accId,
        //            InspectTime = inspectTime,
        //            Status = "Đang duyệt"
        //        };

        //        _context.Timetables.Add(timetable);
        //        _context.SaveChanges();
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Lỗi khi đặt lịch: {ex.Message}");
        //        return false;
        //    }
        //}

        public bool AddTimetable(int inspectionId, int accId, DateTime inspectTime, int vehicleId)
        {
            try
            {
                // Kiểm tra xem đã có lịch trùng tại cùng một trạm và thời điểm hay chưa
                bool isDuplicate = _context.Timetables      //Timetables đc khai báo trong database prn212assgi...
                    .Any(t => t.InspectionId == inspectionId && t.InspectTime == inspectTime && t.VehicleId == vehicleId);

                if (isDuplicate)
                {
                    Console.WriteLine("Lịch đã tồn tại vào thời điểm này!");
                    return false; // Không cho phép đặt trùng lịch
                }

                var timetable = new Models.Timetable
                {
                    VehicleId = vehicleId,
                    InspectionId = inspectionId,
                    AccId = accId,
                    InspectTime = inspectTime,
                    Status = "Đang duyệt"
                };

                _context.Timetables.Add(timetable);
                _context.SaveChanges();
                return true;
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Lỗi database khi đặt lịch: {dbEx.InnerException?.Message ?? dbEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi đặt lịch: {ex.Message}");
                return false;
            }
        }


        public bool CancelTimetable(int stationId, int accId, DateTime inspectTime)
        {
            try
            {
                var timetable = _context.Timetables
                    .FirstOrDefault(t => t.InspectionId == stationId && t.AccId == accId && t.InspectTime == inspectTime);

                if (timetable == null) return false;

                timetable.Status = "Đã hủy";
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi hủy lịch: {ex.Message}");
                return false;
            }
        }
    }
}

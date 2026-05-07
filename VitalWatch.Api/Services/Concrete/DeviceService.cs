using Microsoft.EntityFrameworkCore;
using VitalWatch.Api.EFConfiguration;
using VitalWatch.Api.Entities;
using VitalWatch.Api.Helpers;
using VitalWatch.Api.Models.Requests;
using VitalWatch.Api.Models.Responses;
using VitalWatch.Api.ResponseManage;
using VitalWatch.Api.Services.Abstract;

namespace VitalWatch.Api.Services.Concrete
{
    public class DeviceService : IDeviceService
    {
        private readonly VitalWatchDbContext _db;

        public DeviceService(VitalWatchDbContext db) { _db = db; }

        public async Task<ResponseModel<List<DeviceDto>>> GetPatientDevices(int patientId)
        {
            var devices = await _db.Devices
                .Include(d => d.DeviceType)
                .Include(d => d.DeviceStatus)
                .Where(d => d.PatientId == patientId && !d.IsDeleted)
                .Select(d => new DeviceDto
                {
                    Id = d.Id,
                    Name = d.DeviceName,
                    DeviceType = d.DeviceType.Name,
                    Status = d.DeviceStatus.Name,
                    IsConnected = d.DeviceStatusId == SeedConstants.DeviceStatuses.Active,
                    BatteryLevel = d.BatteryLevel,
                    LastSeenAt = d.LastSeenAt
                }).ToListAsync();

            return ResponseManager.CreateSuccess(devices);
        }

        public async Task<ResponseModel<int>> AddDevice(AddDeviceRequestModel m)
        {
            if (m.PatientId == null && m.UserId == null)
                return ResponseManager.CreateError<int>("PatientId veya UserId belirtilmeli");

            var device = new Device
            {
                PatientId = m.PatientId,
                UserId = m.UserId,
                DeviceName = m.DeviceName,
                DeviceTypeId = m.DeviceTypeId,
                DeviceStatusId = m.DeviceStatusId,
                BatteryLevel = m.BatteryLevel,
                LastSeenAt = DateTime.UtcNow
            };
            _db.Devices.Add(device);
            await _db.SaveChangesAsync();
            return ResponseManager.CreateSuccess(device.Id);
        }

        public async Task<ResponseModel> UpdateStatus(int deviceId, int statusId, int? batteryLevel)
        {
            var d = await _db.Devices.FindAsync(deviceId);
            if (d == null) return ResponseManager.CreateError("Cihaz bulunamadı");
            d.DeviceStatusId = statusId;
            if (batteryLevel.HasValue) d.BatteryLevel = batteryLevel;
            d.LastSeenAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return ResponseManager.CreateSuccess();
        }

        public async Task<ResponseModel<List<DeviceDto>>> GetUserDevices(int userId)
        {
            var devices = await _db.Devices
                .Include(d => d.DeviceType)
                .Include(d => d.DeviceStatus)
                .Where(d => d.UserId == userId && !d.IsDeleted)
                .Select(d => new DeviceDto
                {
                    Id = d.Id,
                    Name = d.DeviceName,
                    DeviceType = d.DeviceType.Name,
                    Status = d.DeviceStatus.Name,
                    IsConnected = d.DeviceStatusId == SeedConstants.DeviceStatuses.Active,
                    BatteryLevel = d.BatteryLevel,
                    LastSeenAt = d.LastSeenAt
                }).ToListAsync();
            return ResponseManager.CreateSuccess(devices);
        }

        public async Task<ResponseModel> Delete(int deviceId)
        {
            var d = await _db.Devices.FindAsync(deviceId);
            if (d == null) return ResponseManager.CreateError("Cihaz bulunamadı");
            _db.Devices.Remove(d);
            await _db.SaveChangesAsync();
            return ResponseManager.CreateSuccess();
        }
    }
}

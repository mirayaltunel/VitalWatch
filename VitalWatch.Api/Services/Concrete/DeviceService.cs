using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using VitalWatch.Api.EFConfiguration;
using VitalWatch.Api.Models.Responses;
using VitalWatch.Api.ResponseManage;
using VitalWatch.Api.Services.Abstract;

namespace VitalWatch.Api.Services.Concrete
{
    public class DeviceService : IDeviceService
    {
        private readonly VitalWatchDbContext _dbContext;

        public DeviceService(VitalWatchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResponseModel<List<DeviceDto>>> GetPatientDevices(int patientId)
        {
            var devices = await _dbContext.Devices
                .Where(d => d.PatientId == patientId)
                .Select(d => new DeviceDto
                {
                    Name = d.DeviceName,
                    SerialNo = d.DeviceType.ToString() + "-" + d.Id, // dummy serial based on type & id
                    IsConnected = d.IsConnected,
                    BatteryLevel = d.BatteryLevel,
                    LastSync = d.LastSeenAt,
                    FirmwareVersion = "v2.1.4" // mock value as it's missing in DB currently but required in UI
                }).ToListAsync();

            return ResponseManager.CreateSuccess(devices);
        }
    }
}

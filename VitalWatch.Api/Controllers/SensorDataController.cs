using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using VitalWatch.Api.EFConfiguration;
using VitalWatch.Api.Entities;
using VitalWatch.Api.Hubs;
using VitalWatch.Api.Models.Requests;
using VitalWatch.Api.ResponseManage;
using VitalWatch.Api.Services.Concrete;

namespace VitalWatch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous] // IoT cihazlar token taşımaz
    public class SensorDataController : ControllerBase
    {
        private readonly VitalWatchDbContext _db;
        private readonly IHubContext<VitalHub> _hubContext;
        private readonly SimulationService _simulation;

        public SensorDataController(
            VitalWatchDbContext db,
            IHubContext<VitalHub> hubContext,
            SimulationService simulation)
        {
            _db = db;
            _hubContext = hubContext;
            _simulation = simulation;
        }

        /// <summary>
        /// IoT cihazdan veri alır, DB'ye yazar, SignalR ile yayar
        /// </summary>
        [HttpPost("Ingest")]
        public async Task<IActionResult> Ingest([FromBody] IngestDataRequestModel model)
        {
            var measurement = new SensorMeasurement
            {
                PatientId       = model.PatientId,
                MeasurementType = model.MeasurementType,
                DeviceType      = model.DeviceType,
                Value           = model.Value,
                Unit            = model.Unit,
                Timestamp       = model.Timestamp.Kind == DateTimeKind.Utc
                                    ? model.Timestamp
                                    : model.Timestamp.ToUniversalTime()
            };

            _db.SensorMeasurements.Add(measurement);
            await _db.SaveChangesAsync();

            // Aynı anda o hastayı dinleyen clientlara gönder
            await _hubContext.Clients
                .Group($"patient_{model.PatientId}")
                .SendAsync("VitalUpdate", new
                {
                    patientId   = model.PatientId,
                    measurementType = model.MeasurementType.ToString(),
                    value       = model.Value,
                    unit        = model.Unit,
                    timestamp   = measurement.Timestamp
                });

            return Ok(ResponseManager.CreateSuccess("Veri alındı"));
        }

        /// <summary>
        /// Simülasyonu başlatır — 2sn'de bir sahte vital üretir
        /// </summary>
        [HttpPost("Simulation/Start/{patientId}")]
        public IActionResult StartSimulation(int patientId)
        {
            if (_simulation.IsRunning(patientId))
                return Ok(ResponseManager.CreateSuccess("Simülasyon zaten çalışıyor"));

            _simulation.Start(patientId);
            return Ok(ResponseManager.CreateSuccess($"Hasta {patientId} için simülasyon başlatıldı"));
        }

        /// <summary>
        /// Simülasyonu durdurur
        /// </summary>
        [HttpPost("Simulation/Stop/{patientId}")]
        public IActionResult StopSimulation(int patientId)
        {
            _simulation.Stop(patientId);
            return Ok(ResponseManager.CreateSuccess($"Hasta {patientId} için simülasyon durduruldu"));
        }

        [HttpGet("Simulation/Status/{patientId}")]
        public IActionResult SimulationStatus(int patientId)
        {
            return Ok(ResponseManager.CreateSuccess(new { isRunning = _simulation.IsRunning(patientId) }));
        }
    }
}

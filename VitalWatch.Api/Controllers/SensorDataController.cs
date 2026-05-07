using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using VitalWatch.Api.EFConfiguration;
using VitalWatch.Api.Entities;
using VitalWatch.Api.Hubs;
using VitalWatch.Api.Models.Requests;
using VitalWatch.Api.ResponseManage;
using VitalWatch.Api.Services.Abstract;
using VitalWatch.Api.Services.Concrete;

namespace VitalWatch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class SensorDataController : ControllerBase
    {
        private readonly VitalWatchDbContext _db;
        private readonly IHubContext<VitalHub> _hub;
        private readonly SimulationService _sim;
        private readonly IAlertService _alert;

        public SensorDataController(VitalWatchDbContext db, IHubContext<VitalHub> hub, SimulationService sim, IAlertService alert)
        {
            _db = db; _hub = hub; _sim = sim; _alert = alert;
        }

        [HttpPost("Ingest")]
        public async Task<IActionResult> Ingest([FromBody] IngestDataRequestModel m)
        {
            var ts = m.Timestamp.Kind == DateTimeKind.Utc ? m.Timestamp : m.Timestamp.ToUniversalTime();
            var measurement = new SensorMeasurement
            {
                PatientId = m.PatientId,
                DeviceId = m.DeviceId,
                MeasurementTypeId = m.MeasurementTypeId,
                Value = m.Value,
                ValueX = m.ValueX,
                ValueY = m.ValueY,
                ValueZ = m.ValueZ,
                Timestamp = ts
            };
            _db.SensorMeasurements.Add(measurement);
            await _db.SaveChangesAsync();

            await _hub.Clients.Group($"patient_{m.PatientId}").SendAsync("VitalUpdate", new
            {
                patientId = m.PatientId,
                measurementTypeId = m.MeasurementTypeId,
                value = m.Value,
                timestamp = ts
            });

            await _alert.EvaluateMeasurement(m.PatientId, m.DeviceId, m.MeasurementTypeId,
                m.Value, m.ValueX, m.ValueY, m.ValueZ, ts);

            return Ok(ResponseManager.CreateSuccess("Veri alındı"));
        }

        [HttpPost("Simulation/Start/{patientId}")]
        public IActionResult StartSimulation(int patientId)
        {
            if (_sim.IsRunning(patientId)) return Ok(ResponseManager.CreateSuccess("Zaten çalışıyor"));
            _sim.Start(patientId);
            return Ok(ResponseManager.CreateSuccess($"Hasta {patientId} simülasyonu başladı"));
        }

        [HttpPost("Simulation/Stop/{patientId}")]
        public IActionResult StopSimulation(int patientId)
        {
            _sim.Stop(patientId);
            return Ok(ResponseManager.CreateSuccess($"Hasta {patientId} simülasyonu durdu"));
        }

        [HttpGet("Simulation/Status/{patientId}")]
        public IActionResult Status(int patientId)
            => Ok(ResponseManager.CreateSuccess(new { isRunning = _sim.IsRunning(patientId) }));
    }
}

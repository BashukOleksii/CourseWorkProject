using AutoMapper;
using InventorySystem_API.Loging.Models;
using InventorySystem_API.Loging.Repository;
using InventorySystem_Shared.Loging;
using InventorySystem_Shared.User;
using MongoDB.Driver;

namespace InventorySystem_API.Loging.Service
{
    public class LogService : ILogService
    {
        private readonly ILogRepository _logRepository;
        private readonly IMapper _logMapper;

        public LogService(
            ILogRepository logRepository,
            IMapper logMapper)
        {
            _logRepository = logRepository;
            _logMapper = logMapper;
        }

        public async Task<AuditLogResponse> Create(AuditLogCreate dto)
        {
            var model = _logMapper.Map<AuditLogModel>(dto);

            var response = await _logRepository.Create(model);

            return _logMapper.Map<AuditLogResponse>(response);
        }

        public async Task Delete(string id)
        {
            var auditLog = await GetById(id);

            await _logRepository.Delete(id);
        }

        public async Task<List<AuditLogResponse>> Get(AuditLogQuery auditLogQuery, string companyId)
        {
            if (auditLogQuery.Page < 0 || auditLogQuery.PageSize < 0)
                throw new ArgumentException("Значення пагінацій не можуть бути менше нуля");

            if (auditLogQuery.From is not null && auditLogQuery.To is not null
                && auditLogQuery.From > auditLogQuery.To)
                throw new ArgumentException("Ліва межа пошуку по даті не може бути більша за праву");

            var builder = new FilterDefinitionBuilder<AuditLogModel>();
            var filter = builder.Eq(auditLog => auditLog.UserCompanyId, companyId);

            if (auditLogQuery.UserId is not null)
                filter &= builder.Eq(auditLog => auditLog.UserId, auditLogQuery.UserId);

            if (auditLogQuery.UserName is not null)
                filter &= builder.Eq(auditLog => auditLog.UserName, auditLogQuery.UserName);
            
            if (auditLogQuery.Role is not null)
                filter &= builder.Eq(auditLog => auditLog.Role, auditLogQuery.Role);

            if (auditLogQuery.Action is not null)
                filter &= builder.Eq(auditLog => auditLog.Action, auditLogQuery.Action);

            if (auditLogQuery.EntityId is not null)
                filter &= builder.Eq(auditLog => auditLog.EntityId, auditLogQuery.EntityId);

            if (auditLogQuery.EntityType is not null)
                filter &= builder.Eq(auditLog => auditLog.EntityType, auditLogQuery.EntityType);

            if (auditLogQuery.From is not null)
                filter &= builder.Gt(auditLog => auditLog.TimeStamp, auditLogQuery.From);

            if (auditLogQuery.To is not null)
                filter &= builder.Lt(auditLog => auditLog.TimeStamp, auditLogQuery.To);

            var response = await _logRepository.Get(filter, auditLogQuery.PageSize, auditLogQuery.Page);

            return _logMapper.Map<List<AuditLogResponse>>(response);

        }

        public async Task<AuditLogResponse> GetById(string id)
        {
            var model = await _logRepository.GetById(id);

            if (model is null)
                throw new KeyNotFoundException($"Не знайдено запису із id:{id}");

            return _logMapper.Map<AuditLogResponse>(model);
        }
    }
}

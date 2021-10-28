using AutoMapper;
using FraudReporterAPI.Data;
using FraudReporterAPI.DTOs;
using FraudReporterAPI.Enums;
using FraudReporterAPI.Interfaces;
using FraudReporterAPI.Models;
using FraudReporterAPI.Paginations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FraudReporterAPI.Services
{
    public class FraudService : BaseService, IFraudService
    {
        private FraudReporterAPIContext context;
        private readonly IMapper mapper;

        public FraudService(FraudReporterAPIContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public bool DeleteFraud(int id)
        {
            bool isDeleted = false;

            var fraud = context.Fraud.Find(id);
            if (fraud == null)
            {
                return isDeleted;
            }

            using (context)
            {
                try
                {
                    context.Database.BeginTransaction();
                    context.Fraud.Remove(fraud);
                    context.SaveChanges();
                    context.Database.CommitTransaction();

                    isDeleted = true;
                }
                catch (Exception exception)
                {
                    WriteLog(exception);

                    context.Database.RollbackTransaction();
                }
            }

            isDeleted = true;

            return isDeleted;
        }

        public FraudDetailDTO GetFraudDetail(int id)
        {
            FraudDetailDTO mappedData = null;

            try
            {
                var fraud = context.Fraud.Find(id);
                if (fraud == null)
                {
                    return null;
                }

                mappedData = mapper.Map<FraudDetailDTO>(fraud);
            }
            catch (Exception exception)
            {
                WriteLog(exception);
            }

            return mappedData;
        }

        public List<FraudListDTO> GetFrauds(FraudPagination pagination)
        {
            List<FraudListDTO> mappedData = new List<FraudListDTO>(pagination.Item);

            try
            {
                var items = context.Fraud.AsQueryable();

                if (!string.IsNullOrEmpty(pagination.Search))
                {
                    items = items.Where(a => a.Phone.Contains(pagination.Search) || a.Message.Contains(pagination.Search)).AsQueryable();
                }

                var listFraud = items.Skip(pagination.Index * pagination.Item).Take(pagination.Item).ToList();

                mappedData = mapper.Map<List<FraudListDTO>>(listFraud);
            }
            catch (Exception exception)
            {
                WriteLog(exception);
            }

            return mappedData;
        }

        public bool SaveFraud(FraudDTO fraud)
        {
            bool isSaved = false;

            var mappedData = mapper.Map<Fraud>(fraud);

            using (context)
            {
                try
                {
                    context.Database.BeginTransaction();

                    context.ChangeTracker.AutoDetectChangesEnabled = false;

                    List<Fraud> frauds = new List<Fraud>();
                    for (int i = 0; i < 10000; i++)
                    {
                        var newData = new Fraud();
                        newData.Phone = "Phone " + i;
                        newData.Provider = "Provider " + i;
                        newData.Message = "Message " + i;
                        frauds.Add(newData);
                    }

                    context.ChangeTracker.AutoDetectChangesEnabled = true;

                    //context.Fraud.Add(mappedData);
                    context.Fraud.AddRange(frauds);
                    context.SaveChanges();
                    context.Database.CommitTransaction();

                    isSaved = true;
                }
                catch (Exception exception)
                {
                    WriteLog(exception);

                    context.Database.RollbackTransaction();
                }
            }

            return isSaved;
        }

        public bool UpdateFraud(int id, FraudDTO fraud)
        {
            bool isUpdated = false;

            var mappedData = mapper.Map<Fraud>(fraud);
            mappedData.Id = id;

            using (context)
            {
                try
                {
                    context.Database.BeginTransaction();
                    context.Entry(mappedData).State = EntityState.Modified;
                    context.SaveChanges();
                    context.Database.CommitTransaction();

                    isUpdated = true;
                }
                catch (Exception exception)
                {
                    WriteLog(exception);

                    context.Database.RollbackTransaction();
                }
            }

            return isUpdated;
        }

        public bool UpdateFraudStatus(int id, int fraudStatus)
        {
            var fraud = context.Fraud.Find(id);
            if (fraud == null)
            {
                throw new Exception("Not found");
            }

            if (fraud.Status == FraudStatus.Cancel)
            {
                throw new Exception("Unable to update status of cancelled report");
            }

            if (fraud.Status == FraudStatus.NotReported && fraudStatus != 1)
            {
                throw new Exception("Status should be update to pending reported");
            }

            if (fraud.Status == FraudStatus.PendingReported && fraudStatus != 2)
            {
                throw new Exception("Status should be update to reported");
            }

            bool isUpdated = false;

            fraud.Status = fraudStatus;

            using (context)
            {
                try
                {
                    context.Database.BeginTransaction();
                    context.Entry(fraud).State = EntityState.Modified;
                    context.SaveChanges();
                    context.Database.CommitTransaction();

                    isUpdated = true;
                }
                catch (Exception exception)
                {
                    WriteLog(exception);

                    context.Database.RollbackTransaction();
                }
            }

            return isUpdated;
        }

        public bool CancelReport(int id, int fraudStatus)
        {
            if (fraudStatus != 3)
            {
                throw new Exception("Please check status for cancelling report");
            }

            var fraud = context.Fraud.Find(id);
            if (fraud == null)
            {
                throw new Exception("Not found");
            }

            if (fraud.Status == FraudStatus.Cancel)
            {
                return true;
            }

            if (fraud.Status != FraudStatus.Reported && fraudStatus != 3)
            {
                throw new Exception("Unable to cancel report");
            }

            bool isCancelled = false;

            fraud.Status = fraudStatus;

            using (context)
            {
                try
                {
                    context.Database.BeginTransaction();
                    context.Entry(fraud).State = EntityState.Modified;
                    context.SaveChanges();
                    context.Database.CommitTransaction();

                    isCancelled = true;
                }
                catch (Exception exception)
                {
                    WriteLog(exception);

                    context.Database.RollbackTransaction();
                }
            }

            return isCancelled;
        }
    }
}

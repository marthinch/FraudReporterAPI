using FraudReporterAPI.DTOs;
using FraudReporterAPI.Paginations;
using System.Collections.Generic;

namespace FraudReporterAPI.Interfaces
{
    public interface IFraudService
    {
        bool DeleteFraud(int id);
        FraudDetailDTO GetFraudDetail(int id);
        List<FraudListDTO> GetFrauds(FraudPagination pagination);
        bool SaveFraud(FraudDTO fraud);
        bool UpdateFraud(int id, FraudDTO fraud);
        bool UpdateFraudStatus(int id, int fraudStatus);
        bool CancelReport(int id, int fraudStatus);
    }
}

﻿using Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRealizationDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRealizationDocumentExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRealizationDocumentExpedition
{
    public interface IVBRealizationDocumentExpeditionService
    {
        Task<int> InitializeExpedition(int vbRealizationId);
        Task<int> UpdateExpeditionByRealizationId(int vbRealizationId);
        Task<int> SubmitToVerification(List<int> vbRealizationIds);
        Task<int> VerificationDocumentReceipt(List<int> vbRealizationIds);
        Task<int> VerifiedToCashier(List<int> vbRealizationIds);
        Task<int> VerifiedToCashier(int vbRealizationId);
        Task<int> CashierReceipt(List<int> vbRealizationIds);
        Task<int> Reject(int vbRealizationId, string reason);
        ReadResponse<VBRealizationDocumentExpeditionModel> Read(int page, int size, string order, string keyword, VBRealizationPosition position, int vbId, int vbRealizationId, DateTimeOffset? realizationDate, string vbRealizationRequestPerson, int unitId);
        ReadResponse<VBRealizationDocumentModel> ReadRealizationToVerification(int vbId, int vbRealizationId, DateTimeOffset? realizationDate, string vbRealizationRequestPerson, int unitId);
        Task<VBRealizationDocumentExpeditionReportDto> GetReports(int vbId, int vbRealizationId, string vbRequestName, int unitId, int divisionId, DateTimeOffset dateStart, DateTimeOffset dateEnd, string status, int page = 1, int size = 25);
    }
}

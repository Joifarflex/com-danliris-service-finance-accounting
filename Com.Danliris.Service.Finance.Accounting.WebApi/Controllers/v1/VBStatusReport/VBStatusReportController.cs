﻿using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.VBStatusReport;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.ValidateService;
using Com.Danliris.Service.Finance.Accounting.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.VBStatusReport
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/vb-status-report")]
    [Authorize]

    public class VBStatusReportController : Controller
    {
        private IIdentityService IdentityService;
        private readonly IValidateService ValidateService;
        private readonly IVBStatusReportService Service;
        private readonly string ApiVersion;
        private readonly IMapper Mapper;

        public VBStatusReportController(IIdentityService identityService, IValidateService validateService, IMapper mapper, IVBStatusReportService service)
        {
            IdentityService = identityService;
            ValidateService = validateService;
            Service = service;
            Mapper = mapper;
            ApiVersion = "1.0.0";
        }

        [HttpGet("reports")]
        public async Task<IActionResult> GetReportAll(int unitId, int vbRequestId, bool? isRealized, DateTimeOffset? requestDateFrom, DateTimeOffset? requestDateTo, DateTimeOffset? realizeDateFrom, DateTimeOffset? realizeDateTo, [FromHeader(Name = "x-timezone-offset")] string timezone)
        {
            int offset = Convert.ToInt32(timezone);

            try
            {
                var data = await Service.GetReport(unitId, vbRequestId, isRealized, requestDateFrom, requestDateTo, realizeDateFrom, realizeDateTo, offset);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data,
                    message = General.OK_MESSAGE,
                    statusCode = General.OK_STATUS_CODE
                });
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("reports/xls")]
        public async Task<IActionResult> GetXlsAll(int unitId, int vbRequestId, bool? isRealized, DateTimeOffset? requestDateFrom, DateTimeOffset? requestDateTo, DateTimeOffset? realizeDateFrom, DateTimeOffset? realizeDateTo, [FromHeader(Name = "x-timezone-offset")] string timezone)
        {

            try
            {
                byte[] xlsInBytes;
                int offset = Convert.ToInt32(timezone);

                var xls = await Service.GenerateExcel(unitId, vbRequestId, isRealized, requestDateFrom, requestDateTo, realizeDateFrom, realizeDateTo, offset);

                string filename = "Laporan Status VB.xlsx";

                xlsInBytes = xls.ToArray();
                var file = File(xlsInBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                return file;

            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }
    }
}

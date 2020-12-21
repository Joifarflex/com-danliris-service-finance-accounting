﻿using Com.Danliris.Service.Finance.Accounting.Lib.Enums.Expedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentPurchasingExpedition;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentPurchasingExpedition
{
    public class IndexDto
    {
        public IndexDto(int id, string internalNoteNo, DateTimeOffset internalNoteDate, DateTimeOffset internalNoteDueDate, string supplierName, double amount, string currencyCode, string remark, GarmentPurchasingExpeditionPosition position)
        {
            Id = id;
            InternalNoteNo = internalNoteNo;
            InternalNoteDate = internalNoteDate;
            InternalNoteDueDate = internalNoteDueDate;
            SupplierName = supplierName;
            Amount = amount;
            CurrencyCode = currencyCode;
            Remark = remark;
            Status = position.ToDescriptionString();
        }

        public IndexDto(int id, DateTimeOffset? verificationAcceptedDate, string internalNoteNo, DateTimeOffset internalNoteDate, DateTimeOffset internalNoteDueDate, string supplierName, double amount, string currencyCode)
        {
            Id = id;
            InternalNoteNo = internalNoteNo;
            InternalNoteDate = internalNoteDate;
            InternalNoteDueDate = internalNoteDueDate;
            SupplierName = supplierName;
            Amount = amount;
            CurrencyCode = currencyCode;
            VerificationAcceptedDate = verificationAcceptedDate;
        }

        public IndexDto(int id, string internalNoteNo, DateTimeOffset internalNoteDate, DateTimeOffset internalNoteDueDate, string supplierName, double amount, string currencyCode, string remark, GarmentPurchasingExpeditionPosition position, string sendToPurchasingRemark) : this(id, internalNoteNo, internalNoteDate, internalNoteDueDate, supplierName, amount, currencyCode, remark, position)
        {
            SendToPurchasingRemark = sendToPurchasingRemark;
        }

        public IndexDto(GarmentPurchasingExpeditionModel entity)
        {
            Date = entity.Position == GarmentPurchasingExpeditionPosition.SendToAccounting ? entity.SendToAccountingDate : entity.Position == GarmentPurchasingExpeditionPosition.SendToCashier ? entity.SendToCashierDate : entity.SendToPurchasingDate;
            VerifiedBy = entity.Position == GarmentPurchasingExpeditionPosition.SendToAccounting ? entity.SendToAccountingBy : entity.Position == GarmentPurchasingExpeditionPosition.SendToCashier ? entity.SendToCashierBy : entity.SendToPurchasingBy;
        }

        public int Id { get; private set; }
        public string InternalNoteNo { get; private set; }
        public DateTimeOffset InternalNoteDate { get; private set; }
        public DateTimeOffset InternalNoteDueDate { get; private set; }
        public string SupplierName { get; private set; }
        public double Amount { get; private set; }
        public string CurrencyCode { get; private set; }
        public DateTimeOffset? VerificationAcceptedDate { get; private set; }
        public string Remark { get; private set; }
        public string Status { get; private set; }
        public string SendToPurchasingRemark { get; private set; }
        public DateTimeOffset? Date { get; private set; }
        public string VerifiedBy { get; private set; }
    }
}
﻿using System.Collections.Generic;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.MemoDetailGarmentPurchasing
{
    public class MemoDetailGarmentPurchasingDispositionViewModel
    {
        public int DispositionId { get; set; }
        public string DispositionNo { get; set; }
        public List<MemoDetail> MemoDetails { get; set; }
    }
}
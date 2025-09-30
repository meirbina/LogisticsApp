using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace SMS.Models.ViewModels
{
    public class VoucherHeadVM
    {
        // For the "Add Voucher Head" form on the left
        public VoucherHead NewVoucherHead { get; set; } = new VoucherHead();

        // For the "Voucher Head List" on the right
        public IEnumerable<VoucherHead> VoucherHeadList { get; set; } = new List<VoucherHead>();

        // For the dropdowns
        public IEnumerable<SelectListItem> BranchList { get; set; }
        public IEnumerable<SelectListItem> TypeList { get; set; }
    }
}
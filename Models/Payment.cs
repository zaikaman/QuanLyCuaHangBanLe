using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLyCuaHangBanLe.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; } = "cash"; // cash, card, bank_transfer, e-wallet

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        // Navigation property
        public Order? Order { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Eshop.Models
{
    public class Account
    {
        public int Id { get; set; }

        [DisplayName("Tên người dùng")]
        [Required(ErrorMessage = "{0} không được bỏ trống")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [EmailAddress]
        public string Email { get; set; }
               
        public string Phone { get; set; }

        public string Address { get; set; }

        public string FullName { get; set; }

        public bool IsAdmin { get; set; }

        public string Avatar { get; set; }

        public bool Status { get; set; }

        public List<Cart> Carts { get; set; }

        public List<Invoice> Invoices { get; set; }
    }
}

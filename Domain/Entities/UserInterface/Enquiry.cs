using Domain.Entities.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.UserInterface
{
    [Index(nameof(Status), nameof(CreateAt))]
    public class Enquiry : BaseDbModel
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(254)]
        public string Email { get; set; } = string.Empty;

        [Phone, MaxLength(20)]
        public string? Phone { get; set; }

        public string? Qualification { get; set; } = string.Empty;
        public string? Program { get; set; } = string.Empty;
        public string? BestTime { get; set; } = string.Empty;

        [Required, MaxLength(3000)]
        public string Message { get; set; } = string.Empty;

        [Required, MaxLength(30)]
        public string Status { get; set; } = "New";

    }
}

using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.UserInterFace.GetUserInterface
{
    public class GetOranizationDto
    {
        public int Id { get; set; }    
        public  string  Name { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }


        public string? WhatsApp { get; set; }

        public string? LinkedIn { get; set; }

        public string? Facebook { get; set; }

        public string? YouTube { get; set; }

        public string? Twitter { get; set; }


        public string? Timing { get; set; }

 
        public string? Country { get; set; }


        public string? State { get; set; }

        public string? City { get; set; }


        public string? Address { get; set; }

        public string? PinCode { get; set; }
        public string? MapLocation { get; set; }
    }
}

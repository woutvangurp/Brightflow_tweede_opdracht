//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace Brightflow_mvc_opdracht.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class User
    {
        public int UserId { get; set; }
        [StringLength(20), MinLength(2)] //max 20 min 2
        public string Naam { get; set; }

        [StringLength(8)]
        public string Tussenvoegsel { get; set; } // in de toekomst wordt dit alleen achternaam, want het tussenvoegsel kan je daar ook kwijt

        [StringLength(25), MinLength(2)] //max 25 min 2
        public string Achternaam { get; set; }

        [Required] //required omdat ik anders meer moet typen voor validatie
        [RegularExpression(@"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$")] //email validation regex
        public string Mail { get; set; }

        public Nullable<int> Phone { get; set; }

        [Required] //required omdat ik anders meer moet typen voor validatie
        public string Wachtwoord { get; set; }

        public string Role { get; set; }
        public string ProfilePicture { get; set; }
    }
}

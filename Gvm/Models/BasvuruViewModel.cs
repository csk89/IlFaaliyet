using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Turkok.Model;

namespace Gvm.Models
{
    public class BasvuruViewModel
    {
        public int Id { get; set; }
        public bool Completed { get; set; }
        public bool Active { get; set; } 
        public GonulluVerici GonulluVerici { get; set; } 
        public ApplicationUser CreatedBy { get; set; }
        public GonulluVericiMerkezi GonulluVericiMerkezi { get; set; }
        public IEnumerable<SelectListItem> GonulluVericiMerkezleri{ get; set; }
        public string SelectedGonulluVericiMerkezi { get; set; } 
    }
}
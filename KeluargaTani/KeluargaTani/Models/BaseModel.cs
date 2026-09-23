using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeluargaTani.Models
{ 
    public class DataTableRequestDTO
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public string OrderDirection { get; set; }
        public string OrderColumnName { get; set; }
    }
    public class DataTableSearchingDTO
    {
        public string columnName { get; set; }
        public string columnSearch { get; set; }
    }
}
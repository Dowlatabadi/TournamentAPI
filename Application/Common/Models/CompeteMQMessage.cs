using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Application.Common.Models
{
    public class CompeteMQMessage
    {
        public int ContestId { get; set; }
        public string AccountId { get; set; }
         public double Spent { get; set; }
         public DateTime Created { get; set; }
         public List<int> Options { get; set; }
    }
}

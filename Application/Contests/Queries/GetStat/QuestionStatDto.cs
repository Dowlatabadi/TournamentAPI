using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Application.Contests.Queries.GetStat
{
    public class OptionStatDto
    {
        public int OptionId { get; set; }
        public double Rate { get; set; }
        public double RewadsSpent { get; set; }
        public int AnswersCount { get; set; }

    }
    public class QuestionStatDto
	{
		public List<OptionStatDto> OptionsStats {get; set;}
		public int QuestionId {get; set; }
        public int AnswersCount { get; set; }
        public double RewadsSpent { get; set; }

    }
}

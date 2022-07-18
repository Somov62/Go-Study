using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace API_Project.Areas.HelpPage.ModelDescriptions
{
    public class SampleAttribute : Attribute
    {
        public SampleAttribute(string sample)
        {
            Sample = sample;
        }

        public string Sample { get; set; }
    }
}
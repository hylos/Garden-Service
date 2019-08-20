using System;
using System.Collections.Generic;
using System.Text;

namespace Uplift2.Models.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Category> CategoryList { get; set; }

        public IEnumerable<Service> ServiceList { get; set; }

    }
}

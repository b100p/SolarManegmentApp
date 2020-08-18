using System;
using System.Collections.Generic;
using System.Text;

namespace SolarManagement.ViewModels
{
    class Contact
    {
		public string Name { get; set; }

		public bool Admin { get; set; }

		public string email { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}

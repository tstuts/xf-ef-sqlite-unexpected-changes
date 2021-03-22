using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace UnexpectedChanges.Data
{
	[Table("Users")]
	public class User : EntityBase {

		public string Username { get; set; }

		public string EmailAddress { get; set; }

		public string FirstName { get; set; }

		public Guid? OrganizationId { get; set; }
	}
}

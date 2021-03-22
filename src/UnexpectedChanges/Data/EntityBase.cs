using System;

namespace UnexpectedChanges.Data
{
	public abstract class EntityBase {

		public Guid Id { get; set; }

		public Guid? GroupId { get; set; }

		public Guid? SubGroupId { get; set; }
	}
}

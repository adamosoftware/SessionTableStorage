using SessionTableStorage.Library.Enums;
using SessionTableStorage.Library.Interfaces;
using System;
using System.Linq;

namespace Tests.Models
{
	public class SampleType : ICacheable
	{
		public string OrderId { get; set; }
		public string Description { get; set; }
		public bool Flag { get; set; }
		public DateTime DateValue { get; set; }
		public NestedType[] Items { get; set; }

		public override bool Equals(object obj)
		{
			var test = obj as SampleType;
			if (test != null)
			{
				return
					test.OrderId.Equals(OrderId) &&
					test.Description.Equals(Description) &&
					test.Flag.Equals(Flag) &&
					test.DateValue.Equals(DateValue) &&
					test.Items.SequenceEqual(Items);
			}

			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public void Invalidate()
		{
			throw new NotImplementedException();
		}

		public bool IsValid { get; set; }
		public RetrievedFrom RetrievedFrom { get; set; }		
	}

	public class NestedType
	{
		public string ItemName { get; set; }
		public decimal Quantity { get; set; }
		public decimal UnitPrice { get; set; }
		public decimal ExtPrice { get { return Quantity * UnitPrice; } }

		public override bool Equals(object obj)
		{
			var test = obj as NestedType;
			if (test != null)
			{
				return
					test.ItemName.Equals(ItemName) &&
					test.Quantity.Equals(Quantity) &&
					test.UnitPrice.Equals(UnitPrice);
			}

			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Collections;
using System.ComponentModel;

namespace DevelopmentWithADot.AspNetGroupedDropDownList
{
	public class GroupedDropDownList : DropDownList
	{
		[DefaultValue("")]
		public String DataGroupField
		{
			get;
			set;
		}

		protected override void PerformDataBinding(IEnumerable dataSource)
		{
			base.PerformDataBinding(dataSource);

			if ((String.IsNullOrWhiteSpace(this.DataGroupField) == false) && (dataSource != null))
			{
				ListItemCollection items = this.Items;
				IEnumerable<Object> data = dataSource.OfType<Object>();
				Int32 count = data.Count();

				for (Int32 i = 0; i < count; ++i)
				{
					String group = DataBinder.Eval(data.ElementAt(i), this.DataGroupField) as String ?? String.Empty;

					if (String.IsNullOrWhiteSpace(group) == false)
					{
						items[i].Attributes["Group"] = group;
					}
				}
			}
		}

		protected override void RenderContents(HtmlTextWriter writer)
		{
			ListItemCollection items = this.Items;
			Int32 count = items.Count;
			var groupedItems = items.OfType<ListItem>().GroupBy(x => x.Attributes["Group"] ?? String.Empty).Select(x => new { Group = x.Key, Items = x.ToList() });

			if (count > 0)
			{
				Boolean flag = false;

				foreach (var groupedItem in groupedItems)
				{
					if (String.IsNullOrWhiteSpace(groupedItem.Group) == false)
					{
						writer.WriteBeginTag("optgroup");
						writer.WriteAttribute("label", groupedItem.Group);
						writer.Write('>');
					}

					for (Int32 i = 0; i < groupedItem.Items.Count; ++i)
					{
						ListItem item = groupedItem.Items[i];

						if (item.Enabled == true)
						{
							writer.WriteBeginTag("option");

							if (item.Selected == true)
							{
								if (flag == true)
								{
									this.VerifyMultiSelect();
								}

								flag = true;

								writer.WriteAttribute("selected", "selected");
							}

							writer.WriteAttribute("value", item.Value, true);

							if (item.Attributes.Count != 0)
							{
								item.Attributes.Render(writer);
							}

							if (this.Page != null)
							{
								this.Page.ClientScript.RegisterForEventValidation(this.UniqueID, item.Value);
							}

							writer.Write('>');
							HttpUtility.HtmlEncode(item.Text, writer);
							writer.WriteEndTag("option");
							writer.WriteLine();
						}
					}

					if (String.IsNullOrWhiteSpace(groupedItem.Group) == false)
					{
						writer.WriteEndTag("optgroup");
					}
				}
			}
		}
	}
}
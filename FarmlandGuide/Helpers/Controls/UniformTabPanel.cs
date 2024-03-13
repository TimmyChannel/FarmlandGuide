using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows;

namespace FarmlandGuide.Helpers.Controls
{
    public class UniformTabPanel : UniformGrid
    {
        public UniformTabPanel()
        {
            this.IsItemsHost = true;
            this.Rows = 1;

            //Default, so not really needed..
            this.HorizontalAlignment = HorizontalAlignment.Stretch;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var totalMaxWidth = this.Children.OfType<TabItem>().Sum(tab => tab.MaxWidth);
            if (!double.IsInfinity(totalMaxWidth))
            {
                this.HorizontalAlignment = (constraint.Width > totalMaxWidth)
                                                    ? HorizontalAlignment.Left
                                                    : HorizontalAlignment.Stretch;
            }

            return base.MeasureOverride(constraint);
        }
    }
}

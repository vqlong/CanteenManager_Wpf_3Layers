using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace GUI
{
    public class GridViewColumnHeaderSortingAdorner : Adorner
    {
        static Geometry ascendingGeo = Geometry.Parse("M 0 8 L 14 8 L 7 0 Z");
        static Geometry desendingGeo = Geometry.Parse("M 0 0 L 14 0 L 7 8 Z");
        bool _isAscending;
        public GridViewColumnHeaderSortingAdorner(UIElement adornedElement, bool isAscending) : base(adornedElement)
        {
            _isAscending = isAscending;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            TranslateTransform transform = new TranslateTransform(AdornedElement.RenderSize.Width / 2, 0);
            drawingContext.PushTransform(transform);

            if (_isAscending) drawingContext.DrawGeometry(Brushes.Tomato, null, ascendingGeo);
            else drawingContext.DrawGeometry(Brushes.Tomato, null, desendingGeo);
        }
    }
}

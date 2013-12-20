using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace AstoundingApplications.AstoundingDock.Ui
{
    /// <summary>
    /// <para>
    /// Fires a PropertyChanged update event for the 'Value' property.
    /// 
    /// If the thumb is being dragged then it waits until the drag has 
    /// been completed before firing the event.
    /// </para>
    /// <para>
    /// Combine with UpdateSourceTrigger=Explicit
    /// </para>
    /// </summary>
    public class MvvmSlider : Slider
    {
        bool _dragStarted;

        protected override void OnThumbDragStarted(System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            base.OnThumbDragStarted(e);
            _dragStarted = true;
        }

        protected override void OnThumbDragCompleted(System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            base.OnThumbDragCompleted(e);

            _dragStarted = false;
            var be = GetBindingExpression(Slider.ValueProperty);
            if (be != null)
            {
                be.UpdateSource();
            }
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);            

            // Do NOT fire a PropertyChanged if we a dragging the thumb.
            if (!_dragStarted)
            {
                var be = GetBindingExpression(Slider.ValueProperty);
                if (be != null)
                {
                    be.UpdateSource();
                }
            }
        }
    }
}

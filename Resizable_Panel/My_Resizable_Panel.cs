using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace ControlManager
{
    internal class ControlMoverOrResizer
    {
        private static bool _moving;
        private static Point _cursorStartPoint;
        private static bool _moveIsInterNal;
        private static bool _resizing;
        private static Size _currentControlStartSize;
        internal static bool MouseIsInLeftEdge { get; set; }
        internal static bool MouseIsInRightEdge { get; set; }
        internal static bool MouseIsInTopEdge { get; set; }
        internal static bool MouseIsInBottomEdge { get; set; }




        // Body
        internal enum MoveOrResize     // Enum is Reffereing to names with digits like  "Reffering to names with digits" like in a list
        {
            Move,  // Is "0"
            Resize, // Is "1"
            MoveAndResize  // Is "2"
        }



        // Accesor
        internal static MoveOrResize WorkType { get; set; }






        // Main Method
        internal static void Init(Control control) 
        {
            Init(control, control);
        }



         // Control and container control is the one that is dragable anf the container is the parretn of the dragable control so EX: drag container only when you drag the button inside the container
        internal static void Init(Control control, Control container) // Initialize // Settings for the Selected Control
        {
            _moving = false;
            _resizing = false;
            _moveIsInterNal = false;
            _cursorStartPoint = Point.Empty;
            MouseIsInLeftEdge = false;
            MouseIsInLeftEdge = false;
            MouseIsInRightEdge = false;
            MouseIsInTopEdge = false;
            MouseIsInBottomEdge = false;
            WorkType = MoveOrResize.MoveAndResize;
            control.MouseDown += (sender, e) => StartMovingOrResizing(control, e);  //#### Setts Mouse Events to the selected Control  
            control.MouseUp += (sender, e) => StopDragOrResizing(control);
            control.MouseMove += (sender, e) => MoveControl(container, e);
        }






        private static void UpdateMouseEdgeProperties(Control control, Point mouseLocationInControl)
        {
            if (WorkType == MoveOrResize.Move)
            {
                return;
            }

             //## EDGES of the Control

            // Booleans: Mouse is in EDGE 
            MouseIsInLeftEdge = Math.Abs(mouseLocationInControl.X) <= 10;
            MouseIsInRightEdge = Math.Abs(mouseLocationInControl.X - control.Width) <= 10;
            MouseIsInTopEdge = Math.Abs(mouseLocationInControl.Y) <= 2;
            MouseIsInBottomEdge = Math.Abs(mouseLocationInControl.Y - control.Height) <= 10;
        }





        // Change Cursor Types if mouse is in some of the EDGES
        private static void UpdateMouseCursor(Control control)
        {
            if (WorkType == MoveOrResize.Move)   // If is move return and check again if it is moving
            {
                return;
            }


            if (MouseIsInLeftEdge)
            {
                if (MouseIsInTopEdge)
                {
                    control.Cursor = Cursors.SizeNWSE;   // Change Currsor type to Diagonal with two arrows
                }
                else if (MouseIsInBottomEdge)
                {
                    control.Cursor = Cursors.SizeNESW;
                }
                else
                {
                    control.Cursor = Cursors.SizeWE;
                }
            }



            else if (MouseIsInRightEdge)
            {
                if (MouseIsInTopEdge)
                {
                    control.Cursor = Cursors.SizeNESW;
                }
                else if (MouseIsInBottomEdge)
                {
                    control.Cursor = Cursors.SizeNWSE;
                }
                else
                {
                    control.Cursor = Cursors.SizeWE;
                }
            }


            else if (MouseIsInTopEdge || MouseIsInBottomEdge)
            {
                control.Cursor = Cursors.SizeNS;
            }
            else
            {
                control.Cursor = Cursors.Default;
            }
        }





        // Mouse Down::
        private static void StartMovingOrResizing(Control control, MouseEventArgs e)
        {
            if (_moving || _resizing)  // If Moving or Resizing Return and check again
            {
                return;
            }


              // Resizing
            if (WorkType != MoveOrResize.Move && (MouseIsInRightEdge || MouseIsInLeftEdge || MouseIsInTopEdge || MouseIsInBottomEdge))
            {
                _resizing = true;
                _currentControlStartSize = control.Size; // Control Size before Resizing
            }


             // Move
            else if (WorkType != MoveOrResize.Resize)
            {
                _moving = true;
                control.Cursor = Cursors.Hand;
            }
            _cursorStartPoint = new Point(e.X, e.Y);
            control.Capture = true;


        }






        // Mouse Move:: in Control
        private static void MoveControl(Control control, MouseEventArgs e)   // Move EVENT of the Control
        {


            if (!_resizing && !_moving) // If it is not resizing or Moving Update Edge Properties and check if the mouse is in an EDGE Position so you can change the cursor 
            {
                UpdateMouseEdgeProperties(control, new Point(e.X, e.Y)); // Look up for if the mouse is in edge position
                UpdateMouseCursor(control); // Change Cursor if mouse is in an Edge position
            }



            if (_resizing)
            {
                if (MouseIsInLeftEdge)
                {
                    if (MouseIsInTopEdge)
                    {
                        control.Width -= (e.X - _cursorStartPoint.X);
                        control.Left += (e.X - _cursorStartPoint.X);
                        control.Height -= (e.Y - _cursorStartPoint.Y);
                        control.Top += (e.Y - _cursorStartPoint.Y);
                    }
                    else if (MouseIsInBottomEdge)
                    {
                        control.Width -= (e.X - _cursorStartPoint.X);
                        control.Left += (e.X - _cursorStartPoint.X);
                        control.Height = (e.Y - _cursorStartPoint.Y) + _currentControlStartSize.Height;
                    }
                    else
                    {
                        control.Width -= (e.X - _cursorStartPoint.X);
                        control.Left += (e.X - _cursorStartPoint.X);
                    }
                }



                else if (MouseIsInRightEdge)
                {
                    if (MouseIsInTopEdge)
                    {
                        control.Width = (e.X - _cursorStartPoint.X) + _currentControlStartSize.Width;
                        control.Height -= (e.Y - _cursorStartPoint.Y);
                        control.Top += (e.Y - _cursorStartPoint.Y);

                    }
                    else if (MouseIsInBottomEdge)
                    {
                        control.Width = (e.X - _cursorStartPoint.X) + _currentControlStartSize.Width;
                        control.Height = (e.Y - _cursorStartPoint.Y) + _currentControlStartSize.Height;
                    }
                    else
                    {
                        control.Width = (e.X - _cursorStartPoint.X) + _currentControlStartSize.Width;
                    }
                }



                else if (MouseIsInTopEdge)
                {
                    control.Height -= (e.Y - _cursorStartPoint.Y);
                    control.Top += (e.Y - _cursorStartPoint.Y);
                }
                else if (MouseIsInBottomEdge)
                {
                    control.Height = (e.Y - _cursorStartPoint.Y) + _currentControlStartSize.Height;
                }
                else
                {
                    StopDragOrResizing(control);
                }
            }



            else if (_moving)
            {
                _moveIsInterNal = !_moveIsInterNal;
                if (!_moveIsInterNal)
                {
                    int x = (e.X - _cursorStartPoint.X) + control.Left;
                    int y = (e.Y - _cursorStartPoint.Y) + control.Top;
                    control.Location = new Point(x, y);
                }
            }


        }






        private static void StopDragOrResizing(Control control)
        {
            _resizing = false;
            _moving = false;
            control.Capture = false;
            UpdateMouseCursor(control);
        }






        //#region Save And Load

        //private static List<Control> GetAllChildControls(Control control, List<Control> list)
        //{
        //    List<Control> controls = control.Controls.Cast<Control>().ToList();
        //    list.AddRange(controls);
        //    return controls.SelectMany(ctrl => GetAllChildControls(ctrl, list)).ToList();
        //}

        //internal static string GetSizeAndPositionOfControlsToString(Control container)
        //{
        //    List<Control> controls = new List<Control>();
        //    GetAllChildControls(container, controls);
        //    CultureInfo cultureInfo = new CultureInfo("en");
        //    string info = string.Empty;
        //    foreach (Control control in controls)
        //    {
        //        info += control.Name + ":" + control.Left.ToString(cultureInfo) + "," + control.Top.ToString(cultureInfo) + "," +
        //                control.Width.ToString(cultureInfo) + "," + control.Height.ToString(cultureInfo) + "*";
        //    }
        //    return info;
        //}
        //internal static void SetSizeAndPositionOfControlsFromString(Control container, string controlsInfoStr)
        //{
        //    List<Control> controls = new List<Control>();
        //    GetAllChildControls(container, controls);
        //    string[] controlsInfo = controlsInfoStr.Split(new[] { "*" }, StringSplitOptions.RemoveEmptyEntries);
        //    Dictionary<string, string> controlsInfoDictionary = new Dictionary<string, string>();
        //    foreach (string controlInfo in controlsInfo)
        //    {
        //        string[] info = controlInfo.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
        //        controlsInfoDictionary.Add(info[0], info[1]);
        //    }
        //    foreach (Control control in controls)
        //    {
        //        string propertiesStr;
        //        controlsInfoDictionary.TryGetValue(control.Name, out propertiesStr);
        //        string[] properties = propertiesStr.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        //        if (properties.Length == 4)
        //        {
        //            control.Left = int.Parse(properties[0]);
        //            control.Top = int.Parse(properties[1]);
        //            control.Width = int.Parse(properties[2]);
        //            control.Height = int.Parse(properties[3]);
        //        }
        //    }
        //}

        //#endregion
    }
}
using System;
using UnityEngine;
using UnityEngine.AzureSky;

namespace UnityEditor.AzureSky
{
    public sealed class AzureEditorUtilities
    {
        private Vector3Int _mGoToDate = Vector3Int.zero;
        private int _mDaysInMonth;
        private int _mSelectedCalendarDayChecker = 0;
        
        /// <summary>
        /// Draws the date selector above the calendar header when you press the middle button in the sky controller's Inspector.
        /// </summary>
        public void DrawDateSelector(AzureSkyManager target, ref bool showDateSelector)
        {
            // Getting current date
            _mGoToDate.x = DateTime.Today.Month;
            _mGoToDate.y = DateTime.Today.Day;
            _mGoToDate.z = DateTime.Today.Year;
            
            // Getting custom month input
            GUILayout.Space(-3);
            EditorGUILayout.BeginVertical("selectionRect");
            EditorGUILayout.BeginHorizontal("Box");
            GUILayout.Label("Month:", GUILayout.Width(45));
            _mGoToDate.x = EditorGUILayout.DelayedIntField(Mathf.Clamp(_mGoToDate.x, 1, 12), GUILayout.Width(20));
            _mGoToDate.x = Mathf.Clamp(_mGoToDate.x, 1, 12);
            EditorGUILayout.Space();
            
            // Getting custom day input
            GUILayout.Label("Day:", GUILayout.Width(30));
            _mDaysInMonth = DateTime.DaysInMonth(_mGoToDate.z, _mGoToDate.x);
            _mGoToDate.y = EditorGUILayout.DelayedIntField(Mathf.Clamp(_mGoToDate.y, 1, _mDaysInMonth), GUILayout.Width(20));
            _mGoToDate.y = Mathf.Clamp(_mGoToDate.y, 1, _mDaysInMonth);
            EditorGUILayout.Space();
            
            // Getting custom year input
            GUILayout.Label("Year:", GUILayout.Width(35));
            _mGoToDate.z = EditorGUILayout.DelayedIntField(Mathf.Clamp(_mGoToDate.z, 1, 9999), GUILayout.Width(35));
            _mGoToDate.z = Mathf.Clamp(_mGoToDate.z, 1, 9999);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(-3);
            
            // Creating go to date button
            if (GUILayout.Button("Goto", EditorStyles.miniButton))
            {
                target.timeController.month = _mGoToDate.x;
                target.timeController.day = _mGoToDate.y;
                target.timeController.year = _mGoToDate.z;
                showDateSelector = false;
                target.timeController.UpdateCalendar();
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draws the buttons of the calendar header responsible for the months and years navigation in the sky controller's Inspector.
        /// </summary>
        public void DrawCalendarHeaderButtons(AzureSkyManager target, ref bool showDateSelector)
        {
            GUILayout.Space(-3);
            EditorGUILayout.BeginHorizontal("box");
            
            // Decrease year button
            if (GUILayout.Button("<<", EditorStyles.miniButtonLeft, GUILayout.Width(25)))
            {
                target.timeController.year--;
                if (target.timeController.year < 0) { target.timeController.year = 9999; }
                target.timeController.UpdateCalendar();
            }
            
            // Decrease month button
            if (GUILayout.Button("<", EditorStyles.miniButtonMid, GUILayout.Width(25)))
            {
                target.timeController.month--;
                if(target.timeController.month < 1) { target.timeController.month = 12; }
                target.timeController.UpdateCalendar();
            }
            
            // Center Button
            if(GUILayout.Button(target.timeController.CalendarMonthList[target.timeController.month - 1] + " " + target.timeController.day.ToString("00") + ", " + target.timeController.year.ToString("0000"), EditorStyles.miniButtonMid))
            {
                showDateSelector = !showDateSelector;
            }
            
            // Increase month button
            if (GUILayout.Button(">", EditorStyles.miniButtonMid, GUILayout.Width(25)))
            {
                target.timeController.month++;
                if (target.timeController.month > 12) { target.timeController.month = 1; }
                target.timeController.UpdateCalendar();
            }
            
            //Increase year button
            if (GUILayout.Button(">>", EditorStyles.miniButtonRight, GUILayout.Width(25)))
            {
                target.timeController.year++;
                if (target.timeController.year > 9999) { target.timeController.year = 1; }
                target.timeController.UpdateCalendar();
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draws the calendar.
        /// </summary>
        public void DrawCalendar(AzureSkyManager target)
        {
            GUILayout.Space(-5);
            
            // Draws the days of the week strings above the buttons
            EditorGUILayout.BeginHorizontal("box");
            GUILayout.Label("Sun");
            GUILayout.Label("Mon");
            GUILayout.Label("Tue");
            GUILayout.Label("Wed");
            GUILayout.Label("Thu");
            GUILayout.Label("Fri");
            GUILayout.Label("Sat");
            EditorGUILayout.EndHorizontal();
            
            // Stores the current day selected in the calendar to later check if it has changed
            _mSelectedCalendarDayChecker = target.timeController.day;
            
            // Creates the calendar selectable buttons grid
            GUILayout.Space(-5);
            EditorGUILayout.BeginVertical("box");
            target.timeController.selectedCalendarDay = GUILayout.SelectionGrid(target.timeController.selectedCalendarDay, target.timeController.CalendarDayList, 7);
            
            // Do not allow the selection of null calendar buttons
            if (target.timeController.CalendarDayList[target.timeController.selectedCalendarDay] != "")
            {
                target.timeController.day = target.timeController.selectedCalendarDay + 1 - target.timeController.GetDayOfWeek(target.timeController.year, target.timeController.month, 1);
            }
            else
            {
                target.timeController.selectedCalendarDay = target.timeController.day - 1 + target.timeController.GetDayOfWeek(target.timeController.year, target.timeController.month, 1);
            }
            EditorGUILayout.EndVertical();
				
            // Update the calendar if a new day has been selected
            if (_mSelectedCalendarDayChecker != target.timeController.day)
            {
                target.timeController.UpdateCalendar();
            }
        }
    }
}
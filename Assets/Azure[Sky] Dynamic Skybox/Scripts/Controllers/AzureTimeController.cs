using System;
using UnityEngine.Serialization;

namespace UnityEngine.AzureSky
{
    [Serializable]
    public sealed class AzureTimeController
    {
        // Internal use
        private DateTime m_dateTime;
        private int m_daysInMonth = 30;
        private int m_dayOfWeek = 0;
        private Vector2 m_hoursAndMinutes;
        
        // Time controller use
        public int day = 1;
        public int month = 1;
        public int year = 2019;
        public float timeOfDay;
        
        [SerializeField]
        private float m_Timeline = 6.0f;
        public float timeline
        {
            get { return m_Timeline; }
            set
            {
                m_Timeline = value;
                UpdateTimeOfDay();
            }
        }
        
        public float latitude = 0f;
        public float longitude = 0f;
        public float utc = 0f;
        public float dayLength = 24.0f;
        public bool setTimelineByCurve = false;
        public AnimationCurve dayLengthCurve = AnimationCurve.Linear(0, 0, 24, 24);

        public enum TimeMode
        {
            Simple,
            Realistic
        }
        public TimeMode timeMode = TimeMode.Simple;

        public enum RepeatMode
        {
            Off,
            ByDay,
            ByMonth,
            ByYear
        }
        public RepeatMode repeatMode = RepeatMode.Off;
        
        private float m_lst, m_radians, m_radLatitude, m_sinLatitude, m_cosLatitude;
        
        // Calendar use
        public int selectedCalendarDay = 1;
        
        /// <summary>
        /// Array with 7 strings used to store the names of the days of the week.
        /// </summary>
        public readonly string[] CalendarWeekList = new string[]
        {
            "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
        };
        
        /// <summary>
        /// Array with 12 strings used to store the names of the months.
        /// </summary>
        public readonly string[] CalendarMonthList = new string[]
        {
            "January", "February", "March", "April", "May", "June",
            "July", "August", "September", "October", "November", "December"
        };
        
        /// <summary>
        /// Array with 42 numeric strings used to fill a calendar.
        /// </summary>
        public readonly string[] CalendarDayList = new string[]
        {
            "0", "1", "2", "3", "4" , "5" , "6", "7" , "8", "9", "10",
            "11", "12", "13", "14" , "15" , "16", "17" , "18", "19", "20",
            "21", "22", "23", "24" , "25" , "26", "27" , "28", "29", "30",
            "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41"
        };
        
        // Methods
        /// <summary>
        /// Get the day of the week from a custom date and return an integer between 0 and 6.
        /// </summary>
        public int GetDayOfWeek (int year, int month, int day)
        {
            m_dateTime = new DateTime(year, month, day);
            return (int)m_dateTime.DayOfWeek;
        }
        
        /// <summary>
        /// Get the day of the week from the current date and return an integer between 0 and 6.
        /// </summary>
        public int GetCurrentDayOfWeek ()
        {
            m_dateTime = new DateTime(year, month, day);
            return (int)m_dateTime.DayOfWeek;
        }
        
        /// <summary>
        /// Adjust the calendar when there is a change in the date.
        /// </summary>
        public void UpdateCalendar ()
        {
            m_daysInMonth = DateTime.DaysInMonth(year, month);
            if (day > m_daysInMonth) { day = m_daysInMonth; }
            if (day < 1) { day = 1; }

            m_dateTime = new DateTime(year, month, 1);
            m_dayOfWeek = (int)m_dateTime.DayOfWeek;
            selectedCalendarDay = day - 1 + m_dayOfWeek;
            
            m_dateTime = new DateTime(year, month, day);
            for (int i = 0; i < CalendarDayList.Length; i++)
            {
                // Make null all the calendar buttons
                if (i < m_dayOfWeek || i >= (m_dayOfWeek + m_daysInMonth))
                {
                    CalendarDayList[i] = "";
                    continue;
                }
                
                // Sets the day number only on the valid buttons of the current month in use by the calendar.
                m_dateTime = new DateTime(year, month, (i - m_dayOfWeek) + 1);
                CalendarDayList[i] = m_dateTime.Day.ToString();
            }
        }
        
        /// <summary>
        /// Returns the time of day as a Vector2(hours, minutes).
        /// </summary>
        public Vector2 GetTimeOfDay ()
        {
            m_hoursAndMinutes.x = Mathf.Floor(timeOfDay);
            m_hoursAndMinutes.y = 60.0f * (timeOfDay - m_hoursAndMinutes.x);
            m_hoursAndMinutes.y = Mathf.Floor(m_hoursAndMinutes.y);
            return m_hoursAndMinutes;
        }
        
        /// <summary>
        /// Update the time of day when change the timeline.
        /// </summary>
        private void UpdateTimeOfDay ()
        {
            timeOfDay = setTimelineByCurve ? dayLengthCurve.Evaluate(m_Timeline) : m_Timeline;
        }
        
        /// <summary>
        /// Used by the sky manager to apply the time progression.
        /// </summary>
        public float GetTimeProgressionStep ()
        {
            if (dayLength > 0.0f)
                return (24.0f / 60.0f) / dayLength;
            else
                return 0.0f;
        }
        
        /// <summary>
        /// Used by Sky Manager script to convert current system time to (float)timeline.
        /// </summary>
        public void ApplySystemTime ()
        {
            m_Timeline = DateTime.Now.Hour + ((1.0f / 60.0f) * DateTime.Now.Minute);
        }

        /// <summary>
        /// Used by Sky Manager script to apply the current system date.
        /// </summary>
        public void ApplySystemDate ()
        {
            month = DateTime.Now.Month;
            day   = DateTime.Now.Day;
            year  = DateTime.Now.Year;
        }
        
        /// <summary>
        /// Starts the next day based on the repeat mode selected in the Inspector.
        /// </summary>
        public void StartNextDay ()
        {
            m_Timeline = 0;
            if(repeatMode != RepeatMode.ByDay) day++;
            if(day > m_daysInMonth)
            {
                day = 1;
                if (repeatMode != RepeatMode.ByMonth)
                {
                    month++;
                    if (month > 12)
                    {
                        month = 1;
                        if (repeatMode != RepeatMode.ByYear)
                        {
                            year++;
                            if (year > 9999)
                            {
                                year = 1;
                            }
                        }
                    }
                }
            }
            UpdateCalendar();
        }
        
        /// <summary>
        /// Computes the sun simple rotation
        /// </summary>
        public Quaternion GetSunSimpleRotation()
        {
            return Quaternion.Euler(0.0f, longitude, -latitude) * Quaternion.Euler(((timeOfDay + utc) * 360.0f / 24.0f) - 90.0f, 180.0f, 0.0f);
        }

        /// <summary>
        /// Computes celestial rotation.
        /// </summary>
        public Quaternion GetCelestialRotation()
        {
            return Quaternion.Euler(90.0f - latitude, 0.0f, 0.0f) * Quaternion.Euler(0.0f, longitude, 0.0f) * Quaternion.Euler(0.0f, m_lst * Mathf.Rad2Deg, 0.0f);
        }
        
        /// <summary>
        /// Computes the sun realistic rotation based on time, date and location.
        /// </summary>
        //  Based on formulas from Paul Schlyter's web page: http://www.stjarnhimlen.se/comp/ppcomp.html
        public Vector3 GetSunRealisticRotation()
        {
            m_radians = (Mathf.PI * 2.0f) / 360.0f; // Used to convert degress to radians
            m_radLatitude = m_radians * latitude;
            m_sinLatitude = Mathf.Sin(m_radLatitude);
            m_cosLatitude = Mathf.Cos(m_radLatitude);

            float hour = timeline - utc;
            
            // Time Scale
            //--------------------------------------------------
            // d = 367 * y - 7 * (y + (m + 9) / 12) / 4 + 275 * m / 9 + D - 730530
            // d = d + UT / 24.0
            float d = 367 * year - 7 * (year + (month + 9) / 12) / 4 + 275 * month / 9 + day - 730530;
            d = d + hour / 24.0f;

            // Tilt of earth's axis
            //--------------------------------------------------
            // Obliquity of the ecliptic
            float ecliptic = 23.4393f - 3.563E-7f * d;
            // Need convert to radians before apply sine and cosine
            float radEcliptic = m_radians * ecliptic;
            float sinEcliptic = Mathf.Sin(radEcliptic);
            float cosEcliptic = Mathf.Cos(radEcliptic);

            // Orbital elements of the sun
            //--------------------------------------------------
            //float N = 0.0;
            //float i = 0.0;
            float w = 282.9404f + 4.70935E-5f * d;
            //float a = 1.000000f;
            float e = 0.016709f - 1.151E-9f * d;
            float M = 356.0470f + 0.9856002585f * d;

            // Eccentric anomaly
            //--------------------------------------------------
            // E = M + e*(180/pi) * sin(M) * ( 1.0 + e * cos(M)) in degress
            // E = M + e * sin(M) * (1.0 + e * cos(M)) in radians
            // Need convert to radians before apply sine and cosine
            float radM = m_radians * M;
            float sinM = Mathf.Sin(radM);
            float cosM = Mathf.Cos(radM);

            // Need convert to radians before apply sine and cosine
            float radE = radM + e * sinM * (1.0f + e * cosM);
            float sinE = Mathf.Sin(radE);
            float cosE = Mathf.Cos(radE);

            // Sun's distance (r) and its true anomaly (v)
            //--------------------------------------------------
            // Xv = r * cos(v) = cos(E) - e
            // Yv = r * sen(v) = sqrt(1,0 - e * e) * sen(E)
            float xv = cosE - e;
            float yv = Mathf.Sqrt(1.0f - e * e) * sinE;

            // V = atan2(yv, xv)
            // R = sqrt(xv * xv + yv * yv)
            float v = Mathf.Rad2Deg * Mathf.Atan2(yv, xv);
            float r = Mathf.Sqrt(xv * xv + yv * yv);

            // Sun's true longitude
            //--------------------------------------------------
            float radLongitude = m_radians * (v + w);
            float sinLongitude = Mathf.Sin(radLongitude);
            float cosLongitude = Mathf.Cos(radLongitude);

            float xs = r * cosLongitude;
            float ys = r * sinLongitude;

            // Equatorial coordinates
            //--------------------------------------------------
            float xe = xs;
            float ye = ys * cosEcliptic;
            float ze = ys * sinEcliptic;

            // Sun's Right Ascension(RA) and Declination(Dec)
            //--------------------------------------------------
            float RA = Mathf.Atan2(ye, xe);
            float Dec = Mathf.Atan2(ze, Mathf.Sqrt(xe * xe + ye * ye));
            float sinDec = Mathf.Sin(Dec);
            float cosDec = Mathf.Cos(Dec);

            // The Sidereal Time
            //--------------------------------------------------
            float Ls = v + w;
            float GMST0 = Ls + 180.0f;
            float UT = 15.0f * hour;//Universal Time.
            float GMST = GMST0 + UT;
            float LST = m_radians * (GMST + longitude);

            // Store local sideral time
            m_lst = LST;

            // Azimuthal coordinates
            //--------------------------------------------------
            float HA = LST - RA;
            float sinHA = Mathf.Sin(HA);
            float cosHA = Mathf.Cos(HA);

            float x = cosHA * cosDec;
            float y = sinHA * cosDec;
            float z = sinDec;

            float xhor = x * m_sinLatitude - z * m_cosLatitude;
            float yhor = y;
            float zhor = x * m_cosLatitude + z * m_sinLatitude;

            // az  = atan2(yhor, xhor) + 180_degrees
            // alt = asin(zhor) = atan2(zhor, sqrt(xhor*xhor+yhor*yhor))
            float azimuth = Mathf.Atan2(yhor, xhor) + m_radians * 180.0f;
            float altitude = Mathf.Asin(zhor);

            // Zenith angle
            // Zenith=90°−α  Where α is the elevation angle
            float zenith = 90.0f * m_radians - altitude;

            // Converts from Spherical(radius r, zenith-inclination θ, azimuth φ) to Cartesian(x,y,z) coordinates
            // https://en.wikipedia.org/wiki/Spherical_coordinate_system
            //--------------------------------------------------
            // x = r sin(θ)cos(φ)​​
            // y = r sin(θ)sin(φ)
            // z = r cos(θ)
            Vector3 ret;

            // radius = 1
            ret.z = Mathf.Sin(zenith) * Mathf.Cos(azimuth);
            ret.x = Mathf.Sin(zenith) * Mathf.Sin(azimuth);
            ret.y = Mathf.Cos(zenith);

            return ret * -1.0f;
        }
        
        /// <summary>
        /// Computes the moon realistic rotation based on time, date and location.
        /// </summary>
        //  Based on formulas from Paul Schlyter's web page: http://www.stjarnhimlen.se/comp/ppcomp.html
        public Vector3 GetMoonRealisticRotation()
        {
            float hour = timeline - utc;

            // Time Scale
            //--------------------------------------------------
            // d = 367 * y - 7 * (y + (m + 9) / 12) / 4 + 275 * m / 9 + D - 730530
            // d = d + UT / 24.0
            float d = 367 * year - 7 * (year + (month + 9) / 12) / 4 + 275 * month / 9 + day - 730530;
            d = d + hour / 24.0f;

            // Tilt of earth's axis
            //--------------------------------------------------
            // obliquity of the ecliptic
            float ecliptic = 23.4393f - 3.563E-7f * d;
            // Need convert to radians before apply sine and cosine
            float radEcliptic = m_radians * ecliptic;
            float sinEcliptic = Mathf.Sin(radEcliptic);
            float cosEcliptic = Mathf.Cos(radEcliptic);

            // Orbital elements of the Moon
            //--------------------------------------------------
            float N = 125.1228f - 0.0529538083f * d;
            float i = 5.1454f;
            float w = 318.0634f + 0.1643573223f * d;
            float a = 60.2666f;
            float e = 0.054900f;
            float M = 115.3654f + 13.0649929509f * d;

            // Eccentric anomaly
            //--------------------------------------------------
            // E = M + e*(180/pi) * sin(M) * (1.0 + e * cos(M))
            float radM = m_radians * M;
            float E = radM + e * Mathf.Sin(radM) * (1f + e * Mathf.Cos(radM));

            // Planet's distance and true anomaly
            //--------------------------------------------------
            // xv = r * cos(v) = a * (cos(E) - e)
            // yv = r * sin(v) = a * (sqrt(1.0 - e*e) * sin(E))
            float xv = a * (Mathf.Cos(E) - e);
            float yv = a * (Mathf.Sqrt(1f - e * e) * Mathf.Sin(E));
            // V = atan2 (yv, xv)
            // R = sqrt (xv * xv + yv * yv)
            float v = Mathf.Rad2Deg * Mathf.Atan2(yv, xv);
            float r = Mathf.Sqrt(xv * xv + yv * yv);

            // Moon position in 3D space
            //--------------------------------------------------
            float radLongitude = m_radians * (v + w);
            float sinLongitude = Mathf.Sin(radLongitude);
            float cosLongitude = Mathf.Cos(radLongitude);

            // Geocentric (Earth-centered) coordinates
            //--------------------------------------------------
            // xh = r * (cos(N) * cos(v+w) - sin(N) * sin(v+w) * cos(i))
            // yh = r * (sin(N) * cos(v+w) + cos(N) * sin(v+w) * cos(i))
            // zh = r * (sin(v+w) * sin(i))
            float radN = m_radians * N;
            float radI = m_radians * i;

            float xh = r * (Mathf.Cos(radN) * cosLongitude - Mathf.Sin(radN) * sinLongitude * Mathf.Cos(radI));
            float yh = r * (Mathf.Sin(radN) * cosLongitude + Mathf.Cos(radN) * sinLongitude * Mathf.Cos(radI));
            float zh = r * (sinLongitude * Mathf.Sin(radI));

            // No needed to the moon
            // float xg = xh;
            // float yg = yh;
            // float zg = zh;

            // Equatorial coordinates
            //--------------------------------------------------
            float xe = xh;
            float ye = yh * cosEcliptic - zh * sinEcliptic;
            float ze = yh * sinEcliptic + zh * cosEcliptic;

            // Planet's Right Ascension (RA) and Declination (Dec)
            //--------------------------------------------------
            float RA = Mathf.Atan2(ye, xe);
            float Dec = Mathf.Atan2(ze, Mathf.Sqrt(xe * xe + ye * ye));

            // The Sidereal Time
            //--------------------------------------------------
            // It is already calculated for the sun and stored in the lst, it is not necessary to calculate again for the moon
            
            //float Ls = v + w;
            //float GMST0 = Ls + 180.0f;
            //float UT    = 15.0f * hour;
            //float GMST  = GMST0 + UT;
            //float LST   = radians * (GMST + Azure_Longitude);

            // Azimuthal coordinates
            //--------------------------------------------------
            float HA = m_lst - RA;

            float x = Mathf.Cos(HA) * Mathf.Cos(Dec);
            float y = Mathf.Sin(HA) * Mathf.Cos(Dec);
            float z = Mathf.Sin(Dec);

            float xhor = x * m_sinLatitude - z * m_cosLatitude;
            float yhor = y;
            float zhor = x * m_cosLatitude + z * m_sinLatitude;

            // az  = atan2(yhor, xhor) + 180_degrees
            // alt = asin(zhor) = atan2(zhor, sqrt(xhor*xhor+yhor*yhor))
            float azimuth = Mathf.Atan2(yhor, xhor) + m_radians * 180.0f;
            float altitude = Mathf.Asin(zhor);

            // Zenith angle
            // Zenith = 90°−α  where α is the elevation angle
            float zenith = 90.0f * m_radians - altitude;

            // Converts from Spherical(radius r, zenith-inclination θ, azimuth φ) to Cartesian(x,y,z) coordinates
            // https://en.wikipedia.org/wiki/Spherical_coordinate_system
            //--------------------------------------------------
            // x = r sin(θ)cos(φ)​​
            // y = r sin(θ)sin(φ)
            // z = r cos(θ)
            Vector3 ret;

            //radius = 1
            ret.z = Mathf.Sin(zenith) * Mathf.Cos(azimuth);
            ret.x = Mathf.Sin(zenith) * Mathf.Sin(azimuth);
            ret.y = Mathf.Cos(zenith);

            return ret * -1.0f;
        }
    }
}
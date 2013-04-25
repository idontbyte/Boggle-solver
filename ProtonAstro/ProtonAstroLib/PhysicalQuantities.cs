﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProtonAstroLib
{

    /// <summary>
    /// A length or distance; default unit is in meters.
    /// </summary>
    public struct Distance
    {
        private double value;

        public static explicit operator double(Distance d)
        {
            return d.value;
        }

        public static explicit operator Distance(double d)
        {
            return new Distance { value = d };
        }

        public double LightYears { get { return value / (double)Constants.LightYear; } }
        public double Parsecs { get { return value / (double)Constants.Parsec; } }
        public double AUs { get { return value / (double)Constants.AU; } }

        public static Distance FromParsecs(double parsecs) { return parsecs * Constants.Parsec; }
        public static Distance FromLightyears(double lightyears) { return lightyears * Constants.LightYear; }
        public static Distance FromAU(double AUs) { return Constants.AU * AUs; }


        public static Distance operator -(Distance a) { return (Distance)(-a.value); }
        public static Distance operator +(Distance a, Distance b) { return (Distance)(a.value + b.value); }
        public static Distance operator -(Distance a, Distance b) { return (Distance)(a.value - b.value); }
        public static Distance operator *(Distance a, double b) { return (Distance)(a.value * b); }
        public static Distance operator *(double b, Distance a) { return (Distance)(a.value * b); }
        public static Distance operator /(Distance a, double b) { return (Distance)(a.value / b); }

    }

    /// <summary>
    /// A geometric angle, default representation in radians
    /// </summary>
    public struct Angle
    {
        private double value;

        public static explicit operator double(Angle d)
        {
            return d.value;
        }

        public static explicit operator Angle(double d)
        {
            return new Angle { value = d };
        }

        /// <summary>
        /// Returns the angle as between 0, inclusive, and 2Pi, exclusive.
        /// Throws InvalidOperationException on degenerate doubles (NaN, inf)
        /// </summary>
        public Angle Normalized
        {
            get
            {
                var normalizedValue = value;
                var circle = 2 * Math.PI;
                if (double.IsInfinity(normalizedValue)) throw new InvalidOperationException();
                if (double.IsNaN(normalizedValue)) throw new InvalidOperationException();
                while (normalizedValue < 0)
                    normalizedValue += circle;
                while (normalizedValue >= circle)
                    normalizedValue -= circle;
                return new Angle() { value = normalizedValue };
            }
        }

        public double Degrees { get { return (360 * value) / (2 * Math.PI); } }
        public static Angle FromDegrees(double value) { return (Angle)((2 * Math.PI) * (value / 360)); }

        /// <summary>
        /// The angle as hours, minutes, seconds in a 24-hour clock
        /// </summary>
        public TimeSpan Time { get { return TimeSpan.FromHours(Degrees / (360 / 24)); } }
        /// <summary>
        /// The angle of the time using a 24-hour clock; 1h = 15 degrees
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static Angle FromTime(TimeSpan time) { return FromDegrees(time.TotalHours * (360 / 24)); }

        public static Angle operator -(Angle a) { return (Angle)(-a.value); }
        public static Angle operator +(Angle a, Angle b) { return (Angle)(a.value + b.value); }
        public static Angle operator -(Angle a, Angle b) { return (Angle)(a.value - b.value); }
        public static Angle operator *(Angle a, double b) { return (Angle)(a.value * b); }
        public static Angle operator *(double b, Angle a) { return (Angle)(a.value * b); }
        public static Angle operator /(Angle a, double b) { return (Angle)(a.value / b); }

        public static Angle FromDegrees(int degrees, int minutes, double seconds)
        {
            return FromDegrees(degrees + minutes / 60.0 + seconds / (3600.0));
        }

        public override string ToString()
        {
            return ToDMSString();
            // return Degrees.ToString() + "deg";
        }

        public string ToDMSString()
        {
            var deg = Normalized.Degrees;
            var D = Math.Floor(deg);            //degrees
            var M = Math.Floor(60 * (deg - D)); //minutes
            var S = 60 * 60 * (deg - D - (M / 60));    //seconds
            return string.Format("{0}d{1:00}m{2:0.0}s", D, M, S);
        }
    }


    /// <summary>
    /// A mass; default unit is in kg.
    /// </summary>
    public struct Mass
    {
        private double value;

        public static implicit operator double(Mass d)
        {
            return d.value;
        }

        public static implicit operator Mass(double d)
        {
            return new Mass { value = d };
        }
    }

    public static class Constants
    {

        //General public static doubleants
        public static Distance AU = (Distance)149597900000;                //AU in m
        public static Distance Parsec = 3.261633 * LightYear;                //Parsecs in light year
        public static double cLightSpeed = 299792500;           //Light speed in m
        public static Distance LightYear = (Distance)9.46053E+15;          //Light year (m)
        public static double cG = 0.0000000000667;              //Gravitational constant

        public static double cGalNPoleRA = 192.25;              //Galactic North Pole RA Epoch 1950.0
        public static double cGalNPoleDecl = 27.4;              //Galactic North Pole Decl Epoch 1950.0
        public static double cGalPlaneAscNode = 33;             //Galactic Plane Asc Node (l)Epoch 1950.0

        //Elemental public static doubleants
        public static double cProtonMass = 1.67352E-27;         //Mass of proton (kg)


        //The Sun
        public static Mass cSolarMass = 1.989E+30;            //Solar mass (kg)
        public static Distance cSolarRadius = (Distance)695990000.0;        //Solar radius (m)
        public static double cSolarLuminosity = 3.826E+26;      //Solar luminosity (J/s)

        //The Moon
        public static Mass cLunarMass = 7.35E+22;             //Lunar mass (kg)
        public static Distance cLunarRadius = (Distance)1738000;            //Lunar radius (m)
        public static Distance cLunarDistance = (Distance)384400000;        //Lunar distance to Earth (m)
        public static TimeSpan cLunarOrbitalPeriod = TimeSpan.FromDays(27.322);      //Orbital Period (days)
        public static double cLunarOrbitalEcc = 0.055;          //Orbital Eccentricity 
        public static Angle cLunarOrbitalIncl = Angle.FromDegrees(5.1452778);     //Orbital Inclination

        //The Earth

        /// <summary>
        /// According to (the sources of) Wikipedia, 
        /// the J2000 epoch is measured from
        /// January 1, 2000, 11:58:55.816 UTC
        /// </summary>
        public static DateTimeOffset J2000Epoch = new DateTimeOffset(2000, 1, 1, 11, 58, 55, 816, TimeSpan.Zero);

        public static Mass EarthMass = 5.976E+24;            //Mass (kg)
        public static Distance EarthEquRadius = (Distance)6378164;         //Equatorial Radius (m)
        public static double EarthAvgDensity = 5.497;          //Average density (g/cm^3)
        public static double EarthSurfaceGravity = 1.0;        //Surface Gravity (Earth = 1)
        public static double EarthEscapeVelocity = 11.2;       //Escape Velocity (km/s)
        public static TimeSpan EarthSiderealPeriod = TimeSpan.FromHours(23.9344722); //Sidereal Period (hours)
        public static Angle EarthInclination = Angle.FromDegrees(23.45);         //Orbital Inclination (Degrees)
        public static double EarthSMA = 1;                     //Semimajor Axis (AU)
        public static TimeSpan EarthOrbitalPeriod = TimeSpan.FromDays(365.26);      //Orbital Period (days)
        public static double EarthAvgOrbitalVel = 29.79;       //Average orbital velocity (km/s)
        public static double EarthOrbitalEcc = 0.0167;         //Orbital Eccentricity
        public static double EarthInclEcl = 0;                 //Inclination to Ecliptic (Degrees)
        public static double EarthAvgAlbedo = 0.39;            //Albedo
        public static double EarthOblate = 0.0034;             //Oblateness
        public static Distance EarthMaxOrbit = Distance.FromAU(1.0167);           //Maximum distance to Sun (AU)
        public static Distance EarthMinOrbit = Distance.FromAU(0.9833);           //Minimum distance to Sun (AU)
        public static TimeSpan EarthRotationPeriod = TimeSpan.FromHours(24.0);       //Period of rotation to Sun (hours)

    }
}
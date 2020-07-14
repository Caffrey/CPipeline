#include "Color.h"

inline Color Color::Color::operator=(const Color& rsh)
{
	_r = rsh._r;
	_g = rsh._g;
	_b = rsh._b;
}

inline Color Color::operator+=(const Color& rsh)
{
	*this = *this + rsh;
	return *this;
}

inline Color Color::operator*=(const Color& rsh)
{
	*this = *this * rsh;
	return *this;
}

inline Color Color::operator/=(const Color& rsh)
{
	*this = *this / rsh;
	return *this;
}

inline Color Color::operator*=(float rsh) 
{
	*this = *this * rsh;
	return *this;
}

inline Color Color::operator/=(float rsh)
{
	*this = *this / rsh;
	return *this;
}

void Color::Clamp()
{
	if (_r > 1.0) _r = 1.0f;
	if (_g > 1.0) _g = 1.0f;
	if (_b > 1.0) _b = 1.0f;

	if (_r < 0.0f) _r = 0.0f;
	if (_g < 0.0f) _g = 0.0f;
	if (_b < 0.0f) _b = 0.0f;
}

inline Color operator*(const Color& c, float f) 
{
	return Color(c._r * f, c._g * f, c._b * f);
}

inline Color operator*(float f, const Color& c)
{
	return Color(c._r * f, c._g * f, c._b * f);
}

inline Color operator/(const Color& c, float f) 
{
	return Color(c._r / f, c._g / f, c._b / f);
}

inline Color operator*(const Color& c1, const Color& c2)
{
	return Color(c1._r * c2._r, c1._g * c2._g, c1._b * c2._b);
}

inline Color operator/(const Color& c1, const Color& c2)
{
	return Color(c1._r / c2._r, c1._g / c2._g, c1._b / c2._b);
}

inline Color operator+(const Color& c1, const Color& c2)
{
	return  Color(c1._r + c2._r, c1._g + c2._g, c1._b + c2._b);
}


inline std::ostream& operator<<(std::ostream& out, const Color& the_rgb)
{
	out << the_rgb._r << ' '
		<< the_rgb._g << ' '
		<< the_rgb._b << ' ';
	return out;
}
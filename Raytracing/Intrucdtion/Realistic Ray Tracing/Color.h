#pragma once
#include "Common.h"
#include <iostream>

class Color
{
public:
	Color()
	{
	}

	Color(RFLOAT r, RFLOAT b, RFLOAT a)
	{
		data[0] = r;
		data[1] = b;
		data[2] = a;
	}

	Color(const Color& c)
	{
		data[0] = c.data[0];
		data[1] = c.data[1];
		data[2] = c.data[2];
	}

	inline void set(RFLOAT r, RFLOAT g, RFLOAT b)
	{
		data[0] = r;
		data[1] = g;
		data[2] = b;
	}

	inline RFLOAT r() const
	{
		return data[0];
	}

	inline RFLOAT g() const
	{
		return data[1];
	}

	inline RFLOAT b() const
	{
		return data[2];
	}

	inline void set_u8(t_u8 r, t_u8 g, t_u8 b)
	{
		data[0] = ((RFLOAT(r) + 0.5f) / 256.0f);
		data[1] = ((RFLOAT(g) + 0.5f) / 256.0f);
		data[2] = ((RFLOAT(b) + 0.5f) / 256.0f);
	}

	inline t_u8 r_u8() const
	{
		t_u32 i = (t_u32)(256.0 * data[0]);
		if (i > 255) i = 255;
		return (t_u8)i;
	}

	inline t_u8 g_u8() const
	{
		t_u32 i = (t_u32)(256.0 * data[1]);
		if (i > 255) i = 255;
		return (t_u8)i;
	}

	inline t_u8 b_u8() const
	{
		t_u32 i = (t_u32)(256.0 * data[2]);
		if (i > 255) i = 255;
		return (t_u8)i;
	}

	inline void clamp()
	{
		data[0] = fclamp01(data[0]);
		data[1] = fclamp01(data[1]);
		data[2] = fclamp01(data[2]);
	}

	inline Color gamma(RFLOAT gamma = 1.0f / 2.2f) const
	{
		return Color(pow(data[0], gamma), pow(data[1], gamma), pow(data[2], gamma));
	}

	Color operator+() const
	{
		return Color(data[0], data[1], data[2]);
	}

	Color operator-() const
	{
		return Color(-data[0], -data[1], -data[2]);
	}

	RFLOAT operator[](int i) const
	{
		return data[i];
	}

	RFLOAT& operator[](int i)
	{
		return data[i];
	}

	Color& operator+=(const Color& c)
	{
		data[0] += c.data[0];
		data[1] += c.data[1];
		data[2] += c.data[2];
		return *this;
	}

	Color& operator-=(const Color& c)
	{
		data[0] -= c.data[0];
		data[1] -= c.data[1];
		data[2] -= c.data[2];
		return *this;
	}

	Color& operator*=(const Color& c)
	{
		data[0] *= c.data[0];
		data[1] *= c.data[1];
		data[2] *= c.data[2];
		return *this;
	}

	Color& operator/=(const Color& c)
	{
		data[0] /= c.data[0];
		data[1] /= c.data[1];
		data[2] /= c.data[2];
		return *this;
	}

	Color& operator*=(RFLOAT f)
	{
		data[0] *= f;
		data[1] *= f;
		data[2] *= f;
		return *this;
	}

	Color& operator/=(RFLOAT f)
	{
		data[0] /= f;
		data[1] /= f;
		data[2] /= f;
		return *this;
	}

public:
	RFLOAT data[3];
};

inline bool operator==(const Color& c1, const Color& c2)
{
	return (fequal(c1[0], c2[0]) && fequal(c1[1], c2[1]) && fequal(c1[2], c2[2]));
}

inline bool operator!=(const Color& c1, const Color& c2)
{
	return (!fequal(c1[0], c2[0]) || !fequal(c1[1], c2[1]) || !fequal(c1[2], c2[2]));
}

inline std::istream& operator>>(std::istream& is, Color& c)
{
	return (is >> c[0] >> c[1] >> c[2]);
}

inline std::ostream& operator<<(std::ostream& os, const Color& c)
{
	return (os << c[0] << c[1] << c[2]);
}

inline Color operator+(const Color& c1, const Color& c2)
{
	return Color(c1.r() + c2.r(), c1.g() + c2.g(), c1.b() + c2.b());
}

inline Color operator-(const Color& c1, const Color& c2)
{
	return Color(c1.r() - c2.r(), c1.g() - c2.g(), c1.b() - c2.b());
}

inline Color operator*(const Color& c1, const Color& c2)
{
	return Color(c1.r() * c2.r(), c1.g() * c2.g(), c1.b() * c2.b());
}

inline Color operator/(const Color& c1, const Color& c2)
{
	return Color(c1.r() / c2.r(), c1.g() / c2.g(), c1.b() / c2.b());
}

inline Color operator*(const Color& c, RFLOAT f)
{
	return Color(c.r() * f, c.g() * f, c.b() * f);
}

inline Color operator*(RFLOAT f, const Color& c)
{
	return Color(c.r() * f, c.g() * f, c.b() * f);
}

inline Color operator/(const Color& c, RFLOAT f)
{
	return Color(c.r() / f, c.g() / f, c.b() / f);
}


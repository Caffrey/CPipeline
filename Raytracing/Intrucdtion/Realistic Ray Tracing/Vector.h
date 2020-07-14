#pragma once
#include "Common.h"
#include <cmath>
#include <iostream>
class Vector
{
public:
	Vector()
	{
		data[0] = data[1] = data[2] = 0;
	}

	Vector(RFLOAT f0, RFLOAT f1, RFLOAT f2)
	{
		data[0] = f0;
		data[1] = f1;
		data[2] = f2;
	}

	Vector(const Vector& v)
	{
		data[0] = v.data[0];
		data[1] = v.data[1];
		data[2] = v.data[2];
	}

	RFLOAT x() const
	{
		return data[0];
	}

	RFLOAT y() const
	{
		return data[1];
	}

	RFLOAT z() const
	{
		return data[2];
	}

	void set_x(RFLOAT f)
	{
		data[0] = f;
	}

	void set_y(RFLOAT f)
	{
		data[1] = f;
	}

	void set_z(RFLOAT f)
	{
		data[2] = f;
	}

	RFLOAT length() const
	{
		return sqrt(squared_length());
	}

	RFLOAT squared_length() const
	{
		return data[0] * data[0] + data[1] * data[1] + data[2] * data[2];
	}

	void normalize();

	RFLOAT min_component() const
	{
		return data[index_Of_min_component()];
	}

	RFLOAT max_component() const
	{
		return data[index_of_max_component()];
	}

	RFLOAT min_abs_component() const
	{
		return data[index_of_min_abs_component()];
	}

	RFLOAT max_abs_component() const
	{
		return data[index_of_max_abs_component()];
	}

	int index_Of_min_component() const
	{
		return (data[0] < data[1] && data[0] < data[2]) ? 0 : (data[1] < data[2] ? 1 : 2);
	}

	int index_of_min_abs_component() const
	{
		if (fabs(data[0]) < fabs(data[1]) && fabs(data[0]) < fabs(data[2]))
			return 0;
		else if (fabs(data[1]) < fabs(data[2]))
			return 1;
		else
			return 2;
	}

	int index_of_max_component() const
	{
		return (data[0] > data[1] && data[0] > data[2]) ? 0 : (data[1] > data[2] ? 1 : 2);
	}

	int index_of_max_abs_component() const
	{
		if (fabs(data[0]) > fabs(data[1]) && fabs(data[0]) > fabs(data[2]))
			return 0;
		else if (fabs(data[1]) > fabs(data[2]))
			return 1;
		else
			return 2;
	}

	Vector operator+() const
	{
		return *this;
	}

	Vector operator-() const
	{
		return Vector(-data[0], -data[1], -data[2]);
	}

	RFLOAT operator[](int i) const
	{
		return data[i];
	}

	RFLOAT& operator[](int i)
	{
		return data[i];
	}

	Vector& operator+=(const Vector& v)
	{
		data[0] += v.data[0];
		data[1] += v.data[1];
		data[2] += v.data[2];
		return *this;
	}

	Vector& operator-=(const Vector& v)
	{
		data[0] -= v.data[0];
		data[1] -= v.data[1];
		data[2] -= v.data[2];
		return *this;
	}

	Vector& operator+=(RFLOAT f)
	{
		data[0] += f;
		data[1] += f;
		data[2] += f;
		return *this;
	}

	Vector& operator-=(RFLOAT f)
	{
		data[0] -= f;
		data[1] -= f;
		data[2] -= f;
		return *this;
	}

	Vector& operator*=(const RFLOAT f)
	{
		data[0] *= f;
		data[1] *= f;
		data[2] *= f;
		return *this;
	}

	Vector& operator/=(const RFLOAT f)
	{
		data[0] /= f;
		data[1] /= f;
		data[2] /= f;
		return *this;
	}
public:
	RFLOAT data[3];
};

inline void Vector::normalize()
{
	RFLOAT len = length();
	if (len > 0)
	{
		RFLOAT k = 1.0f / len;
		data[0] *= k;
		data[1] *= k;
		data[2] *= k;
	}
}

inline bool operator==(const Vector& v1, const Vector& v2)
{
	return (fequal(v1[0], v2[0]) && fequal(v1[1], v2[1]) && fequal(v1[2], v2[2]));
}

inline bool operator!=(const Vector& v1, const Vector& v2)
{
	return (!fequal(v1[0], v2[0]) || !fequal(v1[1], v2[1]) || !fequal(v1[2], v2[2]));
}

inline std::istream& operator>>(std::istream& is, Vector& v)
{
	return (is >> v[0] >> v[1] >> v[2]);
}

inline std::ostream& operator<<(std::ostream& os, const Vector& v)
{
	return (os << v[0] << v[1] << v[2]);
}

inline Vector operator+(const Vector& v1, const Vector& v2)
{
	return Vector(v1.data[0] + v2.data[0], v1.data[1] + v2.data[1], v1.data[2] + v2.data[2]);
}

inline Vector operator-(const Vector& v1, const Vector& v2)
{
	return Vector(v1.data[0] - v2.data[0], v1.data[1] - v2.data[1], v1.data[2] - v2.data[2]);
}

inline Vector operator+(const Vector& v, RFLOAT f)
{
	return Vector(v.data[0] + f, v.data[1] + f, v.data[2] + f);
}

inline Vector operator-(const Vector& v, RFLOAT f)
{
	return Vector(v.data[0] - f, v.data[1] - f, v.data[2] - f);
}

inline Vector operator*(RFLOAT f, const Vector& v)
{
	return Vector(v.data[0] * f, v.data[1] * f, v.data[2] * f);
}

inline Vector operator*(const Vector& v, RFLOAT f)
{
	return Vector(v.data[0] * f, v.data[1] * f, v.data[2] * f);
}

inline Vector operator/(const Vector& v, RFLOAT f)
{
	return Vector(v.data[0] / f, v.data[1] / f, v.data[2] / f);
}

inline Vector min(const Vector& v1, const Vector& v2)
{
	return Vector(min(v1.x(), v2.x()), min(v1.y(), v2.y()), min(v1.z(), v2.z()));
}

inline Vector max(const Vector& v1, const Vector& v2)
{
	return Vector(max(v1.x(), v2.x()), max(v1.y(), v2.y()), max(v1.z(), v2.z()));
}

inline Vector normalize(const Vector& v)
{
	RFLOAT k = 1.0f / sqrt(v.data[0] * v.data[0] + v.data[1] * v.data[1] + v.data[2] * v.data[2]);
	return Vector(v.data[0] * k, v.data[1] * k, v.data[2] * k);
}

inline RFLOAT dot(const Vector& v1, const Vector& v2)
{
	return v1.data[0] * v2.data[0] + v1.data[1] * v2.data[1] + v1.data[2] * v2.data[2];
}

inline Vector cross(const Vector& v1, const Vector& v2)
{
	return Vector(
		(v1.data[1] * v2.data[2] - v1.data[2] * v2.data[1]),
		(v1.data[2] * v2.data[0] - v1.data[0] * v2.data[2]),
		(v1.data[0] * v2.data[1] - v1.data[1] * v2.data[0]));
}

inline Vector reflect(const Vector& in, const Vector& normal)
{
	// assumes unit length normal
	return in - normal * (2 * dot(in, normal));
}

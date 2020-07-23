#pragma once
#include "Common.h"
#include <cmath>
#include <iostream>
class Vector3
{
public:
	Vector3()
	{
		data[0] = data[1] = data[2] = 0;
	}

	Vector3(RFLOAT f0, RFLOAT f1, RFLOAT f2)
	{
		data[0] = f0;
		data[1] = f1;
		data[2] = f2;
	}

	Vector3(const Vector3& v)
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

	Vector3 operator+() const
	{
		return *this;
	}

	Vector3 operator-() const
	{
		return Vector3(-data[0], -data[1], -data[2]);
	}

	RFLOAT operator[](int i) const
	{
		return data[i];
	}

	RFLOAT& operator[](int i)
	{
		return data[i];
	}

	Vector3& operator+=(const Vector3& v)
	{
		data[0] += v.data[0];
		data[1] += v.data[1];
		data[2] += v.data[2];
		return *this;
	}

	Vector3& operator-=(const Vector3& v)
	{
		data[0] -= v.data[0];
		data[1] -= v.data[1];
		data[2] -= v.data[2];
		return *this;
	}

	Vector3& operator+=(RFLOAT f)
	{
		data[0] += f;
		data[1] += f;
		data[2] += f;
		return *this;
	}

	Vector3& operator-=(RFLOAT f)
	{
		data[0] -= f;
		data[1] -= f;
		data[2] -= f;
		return *this;
	}

	Vector3& operator*=(const RFLOAT f)
	{
		data[0] *= f;
		data[1] *= f;
		data[2] *= f;
		return *this;
	}

	Vector3& operator/=(const RFLOAT f)
	{
		data[0] /= f;
		data[1] /= f;
		data[2] /= f;
		return *this;
	}
public:
	RFLOAT data[3];
};

inline void Vector3::normalize()
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

inline bool operator==(const Vector3& v1, const Vector3& v2)
{
	return (fequal(v1[0], v2[0]) && fequal(v1[1], v2[1]) && fequal(v1[2], v2[2]));
}

inline bool operator!=(const Vector3& v1, const Vector3& v2)
{
	return (!fequal(v1[0], v2[0]) || !fequal(v1[1], v2[1]) || !fequal(v1[2], v2[2]));
}

inline std::istream& operator>>(std::istream& is, Vector3& v)
{
	return (is >> v[0] >> v[1] >> v[2]);
}

inline std::ostream& operator<<(std::ostream& os, const Vector3& v)
{
	return (os << v[0] << v[1] << v[2]);
}

inline Vector3 operator+(const Vector3& v1, const Vector3& v2)
{
	return Vector3(v1.data[0] + v2.data[0], v1.data[1] + v2.data[1], v1.data[2] + v2.data[2]);
}

inline Vector3 operator-(const Vector3& v1, const Vector3& v2)
{
	return Vector3(v1.data[0] - v2.data[0], v1.data[1] - v2.data[1], v1.data[2] - v2.data[2]);
}

inline Vector3 operator+(const Vector3& v, RFLOAT f)
{
	return Vector3(v.data[0] + f, v.data[1] + f, v.data[2] + f);
}

inline Vector3 operator-(const Vector3& v, RFLOAT f)
{
	return Vector3(v.data[0] - f, v.data[1] - f, v.data[2] - f);
}

inline Vector3 operator*(RFLOAT f, const Vector3& v)
{
	return Vector3(v.data[0] * f, v.data[1] * f, v.data[2] * f);
}

inline Vector3 operator*(const Vector3& v, RFLOAT f)
{
	return Vector3(v.data[0] * f, v.data[1] * f, v.data[2] * f);
}

inline Vector3 operator/(const Vector3& v, RFLOAT f)
{
	return Vector3(v.data[0] / f, v.data[1] / f, v.data[2] / f);
}

inline Vector3 min(const Vector3& v1, const Vector3& v2)
{
	return Vector3(min(v1.x(), v2.x()), min(v1.y(), v2.y()), min(v1.z(), v2.z()));
}

inline Vector3 max(const Vector3& v1, const Vector3& v2)
{
	return Vector3(max(v1.x(), v2.x()), max(v1.y(), v2.y()), max(v1.z(), v2.z()));
}

inline Vector3 normalize(const Vector3& v)
{
	RFLOAT k = 1.0f / sqrt(v.data[0] * v.data[0] + v.data[1] * v.data[1] + v.data[2] * v.data[2]);
	return Vector3(v.data[0] * k, v.data[1] * k, v.data[2] * k);
}

inline RFLOAT dot(const Vector3& v1, const Vector3& v2)
{
	return v1.data[0] * v2.data[0] + v1.data[1] * v2.data[1] + v1.data[2] * v2.data[2];
}

inline Vector3 cross(const Vector3& v1, const Vector3& v2)
{
	return Vector3(
		(v1.data[1] * v2.data[2] - v1.data[2] * v2.data[1]),
		(v1.data[2] * v2.data[0] - v1.data[0] * v2.data[2]),
		(v1.data[0] * v2.data[1] - v1.data[1] * v2.data[0]));
}

inline Vector3 reflect(const Vector3& in, const Vector3& normal)
{
	// assumes unit length normal
	return in - normal * (2 * dot(in, normal));
}


class Vector2
{
public:
	Vector2()
	{
		data[0] = data[1] = 0;
	}

	Vector2(RFLOAT f0, RFLOAT f1)
	{
		data[0] = f0;
		data[1] = f1;
	}

	Vector2(const Vector2& v)
	{
		data[0] = v.data[0];
		data[1] = v.data[1];
	}

	RFLOAT x() const
	{
		return data[0];
	}

	RFLOAT y() const
	{
		return data[1];
	}

	void set_x(RFLOAT f)
	{
		data[0] = f;
	}

	void set_y(RFLOAT f)
	{
		data[1] = f;
	}

	RFLOAT length() const
	{
		return sqrt(squraed_length());
	}

	RFLOAT squraed_length() const
	{
		return data[0] * data[0] + data[1] * data[1];
	}

	void normalize();

	void scramble();

	Vector2 operator+() const
	{
		return *this;
	}

	Vector2 operator-() const
	{
		return Vector2(-data[0], -data[1]);
	}

	RFLOAT operator[](int i) const
	{
		return data[i];
	}

	RFLOAT& operator[](int i)
	{
		return data[i];
	}

	Vector2& operator+=(const Vector2& v)
	{
		data[0] += v.data[0];
		data[1] += v.data[1];
		return *this;
	}

	Vector2& operator-=(const Vector2& v)
	{
		data[0] -= v.data[0];
		data[1] -= v.data[1];
		return *this;
	}

	Vector2& operator+=(const RFLOAT f)
	{
		data[0] += f;
		data[1] += f;
		return *this;
	}

	Vector2& operator-=(const RFLOAT f)
	{
		data[0] -= f;
		data[1] -= f;
		return *this;
	}

	Vector2& operator*=(const RFLOAT f)
	{
		data[0] *= f;
		data[1] *= f;
		return *this;
	}

	Vector2& operator/=(const RFLOAT f)
	{
		data[0] /= f;
		data[1] /= f;
		return *this;
	}

public:
	RFLOAT data[2];
};

inline void Vector2::scramble()
{
	RFLOAT _x;
	RFLOAT _y = data[0];

	_x = data[1] * 1234.12345054321f;
	data[0] = _x - (int)_x;
	_y = _y * 7654.54321012345f;
	data[1] = _y - (int)_y;
}

inline void Vector2::normalize()
{
	RFLOAT len = length();
	if (len > 0)
	{
		RFLOAT k = 1.0f / len;
		data[0] *= k;
		data[1] *= k;
	}
}

inline bool operator==(const Vector2& v1, const Vector2& v2)
{
	return (fequal(v1[0], v2[0]) && fequal(v1[1], v2[1]));
}

inline bool operator!=(const Vector2& v1, const Vector2& v2)
{
	return (!fequal(v1[0], v2[0]) || !fequal(v1[1], v2[1]));
}

inline std::istream& operator>>(std::istream& is, Vector2& v)
{
	return (is >> v[0] >> v[1]);
}

inline std::ostream& operator<<(std::ostream& os, const Vector2& v)
{
	return (os << v[0] << v[1]);
}

inline Vector2 operator+(const Vector2& v1, const Vector2& v2)
{
	return Vector2(v1.data[0] + v2.data[0], v1.data[1] + v2.data[1]);
}

inline Vector2 operator-(const Vector2& v1, const Vector2& v2)
{
	return Vector2(v1.data[0] - v2.data[0], v1.data[1] - v2.data[1]);
}

inline Vector2 operator+(const Vector2& v, RFLOAT f)
{
	return Vector2(v.data[0] + f, v.data[1] + f);
}

inline Vector2 operator-(const Vector2& v, RFLOAT f)
{
	return Vector2(v.data[0] - f, v.data[1] - f);
}

inline Vector2 operator*(RFLOAT f, const Vector2& v)
{
	return Vector2(v.data[0] * f, v.data[1] * f);
}

inline Vector2 operator*(const Vector2& v, RFLOAT f)
{
	return Vector2(v.data[0] * f, v.data[1] * f);
}

inline Vector2 operator/(const Vector2& v, RFLOAT f)
{
	return Vector2(v.data[0] / f, v.data[1] / f);
}

inline Vector2 min(const Vector2& v1, const Vector2& v2)
{
	return Vector2(min(v1.x(), v2.x()), min(v1.y(), v2.y()));
}

inline Vector2 max(const Vector2& v1, const Vector2& v2)
{
	return Vector2(max(v1.x(), v2.x()), max(v1.y(), v2.y()));
}

inline Vector2 unit(const Vector2& v)
{
	RFLOAT k = 1.0f / sqrt(v.data[0] * v.data[0] + v.data[1] * v.data[1]);
	return Vector2(v.data[0] * k, v.data[1] * k);
}

inline RFLOAT dot(const Vector2& v1, const Vector2& v2)
{
	return v1.data[0] * v2.data[0] + v1.data[1] * v2.data[1];
}

